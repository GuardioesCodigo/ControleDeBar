using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos;

[TestClass]
public sealed class IsolamentoEntreEstabelecimentosTests : E2ETestsBase
{
    [TestMethod]
    public async Task Mesas_NaoDevemSerVisiveis_ParaUsuarioDeOutroEstabelecimento()
    {
        string emailBarDoZe = $"garcom-{Guid.NewGuid():N}@barteste.com";
        string emailBoteco = $"garcom-{Guid.NewGuid():N}@barteste.com";

        // Estabelecimento 1
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");
        await Page.FillAsync("#Email", emailBarDoZe);
        await Page.FillAsync("#Senha", "SenhaForte@123");
        await Page.FillAsync("#ConfirmarSenha", "SenhaForte@123");
        await Page.ClickAsync("button:has-text('Criar Conta')");
        await Page.WaitForURLAsync(url => !url.Contains("Registrar"));

        await Page.GotoAsync($"{UrlBase}/Mesa/Cadastrar");
        await Page.FillAsync("#Numero", "10");
        await Page.FillAsync("#QuantidadeLugares", "4");
        await Page.ClickAsync("button:has-text('Cadastrar')");
        await Page.WaitForURLAsync(url => url.Contains("/Mesa/Listar"));

        // Encerra a sessão do primeiro estabelecimento.
        await Context.ClearCookiesAsync();

        // Estabelecimento 2
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");
        await Page.FillAsync("#Email", emailBoteco);
        await Page.FillAsync("#Senha", "SenhaForte@123");
        await Page.FillAsync("#ConfirmarSenha", "SenhaForte@123");
        await Page.ClickAsync("button:has-text('Criar Conta')");
        await Page.WaitForURLAsync(url => !url.Contains("Registrar"));

        await Page.GotoAsync($"{UrlBase}/Mesa/Listar");

        string conteudoPagina = await Page.ContentAsync();

        Assert.IsFalse(
            conteudoPagina.Contains(">10<"),
            "A Mesa 10 do outro estabelecimento não deveria aparecer aqui.");

        Assert.IsTrue(
            conteudoPagina.Contains("Nenhuma mesa cadastrada"));
    }
}