using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos;

[TestClass]
public sealed class JornadaAtendimentoTests : E2ETestsBase
{
    private async Task RegistrarNovoEstabelecimentoELogar(string emailUnico, string nomeEstabelecimento)
    {
        // CT-USR-001 / CT-USR-006 / CT-USR-007
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");

        await Page.FillAsync("#Email", emailUnico);
        await Page.FillAsync("#Senha", "SenhaForte@123");
        await Page.FillAsync("#ConfirmarSenha", "SenhaForte@123");
        await Page.FillAsync("#NomeEstabelecimento", nomeEstabelecimento);

        await Page.ClickAsync("button[type=submit]");

        await Page.WaitForURLAsync(url => !url.Contains("Registrar"));
    }

    [TestMethod]
    public async Task JornadaCompleta_AbrirContaLancarPedidoEFecharConta_DeveCalcularValorTotalCorretamente()
    {
        // Cobre: CT-USR-001, CT-MSA-001, CT-GAR-001, CT-PRD-001, CT-CTA-001,
        // CT-PED-001, CT-PED-008, CT-CTA-012, CT-CTA-014
        string email = $"garcom-{Guid.NewGuid():N}@barteste.com";

        await RegistrarNovoEstabelecimentoELogar(email, "Bar do Zé");

        // Cadastrar mesa
        await Page.GotoAsync($"{UrlBase}/Mesa/Cadastrar");
        await Page.FillAsync("#Numero", "1");
        await Page.FillAsync("#QuantidadeLugares", "4");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => url.Contains("/Mesa/Listar"));

        // Cadastrar garçom
        await Page.GotoAsync($"{UrlBase}/Garcom/Cadastrar");
        await Page.FillAsync("#Nome", "João Silva");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => url.Contains("/Garcom/Listar"));

        // Cadastrar produto
        await Page.GotoAsync($"{UrlBase}/Produto/Cadastrar");
        await Page.FillAsync("#Nome", "Hambúrguer");
        await Page.FillAsync("#Preco", "28");
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => url.Contains("/Produto/Listar"));

        // Abrir conta
        await Page.GotoAsync($"{UrlBase}/Conta/Abrir");
        await Page.FillAsync("#NomeCliente", "Carlos Andrade");
        await Page.SelectOptionAsync("#MesaId", new SelectOptionValue { Label = "Mesa 1" });
        await Page.SelectOptionAsync("#GarcomId", new SelectOptionValue { Label = "João Silva" });
        await Page.ClickAsync("button[type=submit]");
        await Page.WaitForURLAsync(url => url.Contains("/Conta/Listar"));

        // Acessar a conta recém-aberta
        await Page.ClickAsync("text=Ver");
        await Page.WaitForURLAsync(url => url.Contains("/Conta/Visualizar"));

        // Lançar pedido: 2 hambúrgueres
        await Page.SelectOptionAsync("select[name=ProdutoId]", new SelectOptionValue { Label = "Hambúrguer (R$ 28,00)" });
        await Page.FillAsync("input[name=Quantidade]", "2");
        await Page.ClickAsync("button:has-text(\"+\")");

        await Page.WaitForSelectorAsync("text=Hambúrguer");

        // O total deve refletir 2 x R$ 28,00 = R$ 56,00
        string conteudoPagina = await Page.ContentAsync();
        Assert.IsTrue(conteudoPagina.Contains("56,00"), "O valor total exibido deveria ser R$ 56,00.");

        // Fechar conta
        await Page.ClickAsync("text=Fechar Conta");
        await Page.WaitForSelectorAsync("text=Fechada");

        conteudoPagina = await Page.ContentAsync();
        Assert.IsTrue(conteudoPagina.Contains("Fechada"), "A conta deveria estar com situação Fechada.");
    }
}
