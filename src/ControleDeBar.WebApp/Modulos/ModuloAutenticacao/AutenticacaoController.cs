using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;
using FluentResults;

namespace ControleDeBar.WebApp.Modulos.ModuloAutenticacao;

[AllowAnonymous]
public sealed class AutenticacaoController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    ServicoEstabelecimento servicoEstabelecimento
) : Controller
{
    [HttpGet]
    public ActionResult Registrar()
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Registrar(RegistrarViewModel viewModel)
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return View(viewModel);

        IdentityUser<Guid> user = new IdentityUser<Guid>()
        {
            Id = Guid.CreateVersion7(),
            UserName = viewModel.Email,
            Email = viewModel.Email
        };

        IdentityResult resultado = await userManager.CreateAsync(user, viewModel.Senha);

        if (!resultado.Succeeded)
        {
            foreach (IdentityError erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(viewModel);
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        // Cria o estabelecimento vinculado ao usuário recém-autenticado.
        // Precisa ocorrer após o SignInAsync: o ServicoEstabelecimento depende do
        // IProvedorDeUsuario, que só reconhece o usuário como autenticado a partir daqui.
        Result resultadoEstabelecimento = servicoEstabelecimento.Cadastrar(viewModel.NomeEstabelecimento);

        if (resultadoEstabelecimento.IsFailed)
        {
            await signInManager.SignOutAsync();
            await userManager.DeleteAsync(user);

            foreach (var erro in resultadoEstabelecimento.Errors)
                ModelState.AddModelError(string.Empty, erro.Message);

            return View(viewModel);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public ActionResult Entrar(string? returnUrl = null)
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Entrar(EntrarViewModel viewModel)
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return View(viewModel);

        Microsoft.AspNetCore.Identity.SignInResult resultado = await signInManager.PasswordSignInAsync(
            viewModel.Email,
            viewModel.Senha,
            viewModel.LembrarMe,
            lockoutOnFailure: true
        );

        if (resultado.Succeeded)
        {
            if (Url.IsLocalUrl(viewModel.ReturnUrl))
                return Redirect(viewModel.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Conta bloqueada temporariamente. Tente novamente mais tarde.");
        }
        else
        {
            ModelState.AddModelError(string.Empty,
                "E-mail ou senha inválidos.");
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<ActionResult> Sair()
    {
        await signInManager.SignOutAsync();

        return RedirectToAction(nameof(Entrar));
    }
}
