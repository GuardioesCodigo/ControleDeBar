using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloFaturamento;

public class FaturamentoController(
    ServicoFaturamento servicoFaturamento,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Visualizar(DateOnly? data)
    {
        DateOnly dataConsultada = data ?? DateOnly.FromDateTime(DateTime.Now);

        FaturamentoDiarioDto dto = servicoFaturamento.SelecionarFaturamentoDoDia(dataConsultada);

        VisualizarFaturamentoViewModel visualizarVm = mapeador.Map<VisualizarFaturamentoViewModel>(dto);

        return View(visualizarVm);
    }
}
