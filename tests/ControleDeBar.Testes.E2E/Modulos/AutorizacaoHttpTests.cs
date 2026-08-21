using System.Net;
using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos;

[TestClass]
public sealed class AutorizacaoHttpTests
{
    [TestMethod]
    public async Task Faturamento_DeveRedirecionarParaLogin_QuandoNaoAutenticado()
    {
        // CT-FAT-007: a FallbackPolicy global (RequireAuthenticatedUser) em
        // InjecaoDeDependencia.cs bloqueia qualquer controller sem [AllowAnonymous].
        using WebAppHttpFactory fabrica = new();
        using HttpClient cliente = fabrica.CriarClienteSemRedirecionamento();

        HttpResponseMessage resposta = await cliente.GetAsync("/Faturamento/Visualizar");

        Assert.AreEqual(HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.IsTrue(resposta.Headers.Location!.OriginalString.Contains("/Autenticacao/Entrar"));
    }
}
