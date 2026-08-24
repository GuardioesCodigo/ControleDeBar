using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloEstabelecimento;

public class EstabelecimentoController(
    ServicoEstabelecimento servicoEstabelecimento,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Visualizar()
    {
        Result<DetalhesEstabelecimentoDto> resultado = servicoEstabelecimento.SelecionarAtual();

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction("Index", "Home");
        }

        VisualizarEstabelecimentoViewModel visualizarVm =
            mapeador.Map<VisualizarEstabelecimentoViewModel>(resultado.Value);

        return View(visualizarVm);
    }

    [HttpGet]
    public ActionResult Editar()
    {
        Result<DetalhesEstabelecimentoDto> resultado = servicoEstabelecimento.SelecionarAtual();

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction("Index", "Home");
        }

        EditarEstabelecimentoViewModel editarVm =
            mapeador.Map<EditarEstabelecimentoViewModel>(resultado.Value);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarEstabelecimentoViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        EditarEstabelecimentoDto dto = mapeador.Map<EditarEstabelecimentoDto>(editarVm);

        Result resultado = servicoEstabelecimento.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm);
        }

        return RedirectToAction(nameof(Visualizar));
    }
}
