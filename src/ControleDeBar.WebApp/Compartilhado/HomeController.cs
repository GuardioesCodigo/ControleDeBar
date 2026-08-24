using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Compartilhado;

public class HomeController(
    ServicoConta servicoConta,
    ServicoFaturamento servicoFaturamento,
    ServicoMesa servicoMesa,
    ServicoEstabelecimento servicoEstabelecimento
) : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        DateOnly dataAtual = DateOnly.FromDateTime(DateTime.Now);

        List<ListarContaDto> contas = servicoConta.SelecionarTodos();
        FaturamentoDiarioDto faturamento =
            servicoFaturamento.SelecionarFaturamentoDoDia(dataAtual);
        List<ListarMesaDto> mesas = servicoMesa.SelecionarTodos();

        Result<DetalhesEstabelecimentoDto> resultadoEstabelecimento =
            servicoEstabelecimento.SelecionarAtual();

        string nomeEstabelecimento = resultadoEstabelecimento.IsSuccess
            ? resultadoEstabelecimento.Value.Nome
            : string.Empty;

        int contasAbertas =
            contas.Count(c => c.Situacao == SituacaoConta.Aberta);

        int mesasOcupadas =
            mesas.Count(m => m.Status == StatusMesa.Ocupada);

        IndexViewModel vm = new(
            nomeEstabelecimento,
            contasAbertas,
            faturamento.ValorTotal,
            mesasOcupadas,
            mesas.Count
        );

        return View(vm);
    }
}