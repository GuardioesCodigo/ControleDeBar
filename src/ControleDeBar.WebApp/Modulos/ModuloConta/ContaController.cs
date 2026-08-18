using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public class ContaController(
    ServicoConta servicoConta,
    ServicoMesa servicoMesa,
    ServicoGarcom servicoGarcom,
    ServicoProduto servicoProduto,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarContaDto> dtos = servicoConta.SelecionarTodos();

        List<ListarContaViewModel> listarVms = mapeador.Map<List<ListarContaViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Abrir()
    {
        AbrirContaViewModel abrirVm = new(string.Empty, Guid.Empty, Guid.Empty);

        PreencherListasSelecao(abrirVm.Mesas, abrirVm.Garcons);

        return View(abrirVm);
    }

    [HttpPost]
    public ActionResult Abrir(AbrirContaViewModel abrirVm)
    {
        if (!ModelState.IsValid)
        {
            PreencherListasSelecao(abrirVm.Mesas, abrirVm.Garcons);

            return View(abrirVm);
        }

        CadastrarContaDto dto = mapeador.Map<CadastrarContaDto>(abrirVm);

        Result resultado = servicoConta.Abrir(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            PreencherListasSelecao(abrirVm.Mesas, abrirVm.Garcons);

            return View(abrirVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarContaViewModel editarVm = mapeador.Map<EditarContaViewModel>(resultado.Value);

        PreencherListasSelecao(editarVm.Mesas, editarVm.Garcons);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarContaViewModel editarVm)
    {
        if (!ModelState.IsValid)
        {
            PreencherListasSelecao(editarVm.Mesas, editarVm.Garcons);

            return View(editarVm);
        }

        EditarContaDto dto = mapeador.Map<EditarContaDto>(editarVm);

        Result resultado = servicoConta.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            PreencherListasSelecao(editarVm.Mesas, editarVm.Garcons);

            return View(editarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Visualizar(Guid id)
    {
        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        VisualizarContaViewModel visualizarVm = mapeador.Map<VisualizarContaViewModel>(resultado.Value);

        ViewBag.Produtos = servicoProduto.SelecionarTodos()
            .Select(p => new SelectListItem($"{p.Nome} ({p.Preco:C})", p.Id.ToString()))
            .ToList();

        return View(visualizarVm);
    }

    [HttpPost]
    public ActionResult Fechar(Guid id)
    {
        Result resultado = servicoConta.Fechar(id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Visualizar), new { id });
    }

    private void PreencherListasSelecao(List<SelectListItem> mesas, List<SelectListItem> garcons)
    {
        mesas.AddRange(servicoMesa.SelecionarTodos()
            .Select(m => new SelectListItem($"Mesa {m.Numero}", m.Id.ToString())));

        garcons.AddRange(servicoGarcom.SelecionarTodos()
            .Select(g => new SelectListItem(g.Nome, g.Id.ToString())));
    }
}
