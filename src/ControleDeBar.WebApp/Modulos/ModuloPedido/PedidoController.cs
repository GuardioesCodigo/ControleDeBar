using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloPedido;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using ControleDeBar.WebApp.Modulos.ModuloConta;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloPedido;

public class PedidoController(
    ServicoPedido servicoPedido,
    IMapper mapeador
) : Controller
{
    [HttpPost]
    public ActionResult Adicionar(AdicionarPedidoViewModel adicionarVm)
    {
        if (!ModelState.IsValid)
        {
            TempData.AddErrorMessage(Result.Fail("Informe o produto e uma quantidade válida."));

            return RedirectToAction(
                nameof(ContaController.Visualizar),
                "Conta",
                new { id = adicionarVm.ContaId }
            );
        }

        CadastrarPedidoDto dto = mapeador.Map<CadastrarPedidoDto>(adicionarVm);

        Result resultado = servicoPedido.Registrar(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(
            nameof(ContaController.Visualizar),
            "Conta",
            new { id = adicionarVm.ContaId }
        );
    }

    [HttpPost]
    public ActionResult Remover(Guid id, Guid contaId)
    {
        Result resultado = servicoPedido.Remover(id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(
            nameof(ContaController.Visualizar),
            "Conta",
            new { id = contaId }
        );
    }
}