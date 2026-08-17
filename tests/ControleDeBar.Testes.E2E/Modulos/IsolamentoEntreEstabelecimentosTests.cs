using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos;

[TestClass]
public sealed class IsolamentoEntreEstabelecimentosTests : E2ETestsBase
{
    [TestMethod]
    public async Task Mesas_NaoDevemSerVisiveis_ParaUsuarioDeOutroEstabelecimento()
    {
        // CT-MSA-008 / CT-USR-013: isolamento de dados por estabelecimento, validado
        // de ponta a ponta pela interface (não apenas na camada de repositório).
        string emailBarDoZe = $"garcom-{Guid.NewGuid():N}@barteste.com";
        string emailBoteco = $"garcom-{Guid.NewGuid():N}@barteste.com";

        // Estabelecimento 1: cadastra a Mesa 10
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");
        await Page.FillAsync("#Email", emailBarDoZe);
        await Page.FillAsync("#Senha", "SenhaForte@123");
        await Page.FillAsync("#ConfirmarSenha", "SenhaForte@123");
        await Page.FillAsync("#NomeEstabelecimento", "Bar do Zé");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => !url.Contains("Registrar"));

        await Page.GotoAsync($"{UrlBase}/Mesa/Cadastrar");
        await Page.FillAsync("#Numero", "10");
        await Page.FillAsync("#QuantidadeLugares", "4");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => url.Contains("/Mesa/Listar"));

        await Page.ClickAsync("text=Sair");

        // Estabelecimento 2: nunca cadastrou mesas
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");
        await Page.FillAsync("#Email", emailBoteco);
        await Page.FillAsync("#Senha", "SenhaForte@123");
        await Page.FillAsync("#ConfirmarSenha", "SenhaForte@123");
        await Page.FillAsync("#NomeEstabelecimento", "Boteco do Chico");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => !url.Contains("Registrar"));

        await Page.GotoAsync($"{UrlBase}/Mesa/Listar");

        string conteudoPagina = await Page.ContentAsync();

        Assert.IsFalse(conteudoPagina.Contains(">10<"), "A Mesa 10 do outro estabelecimento não deveria aparecer aqui.");
        Assert.IsTrue(conteudoPagina.Contains("Nenhuma mesa cadastrada"));
    }
}
