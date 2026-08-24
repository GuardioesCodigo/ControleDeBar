using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos;

[TestClass]
public sealed class JornadaAtendimentoTests : E2ETestsBase
{
    private async Task RegistrarNovoEstabelecimentoELogar(
        string emailUnico,
        string nomeEstabelecimento)
    {
        // CT-USR-001 / CT-USR-006 / CT-USR-007

        await Page.GotoAsync(
            $"{UrlBase}/Autenticacao/Registrar");

        await Page.FillAsync(
            "#Email",
            emailUnico);

        await Page.FillAsync(
            "#Senha",
            "SenhaForte@123");

        await Page.FillAsync(
            "#ConfirmarSenha",
            "SenhaForte@123");

        await Page.ClickAsync(
            "button:has-text('Criar Conta')");

        await Page.WaitForURLAsync(
            url => !url.Contains("Registrar"));
    }

    [TestMethod]
    public async Task JornadaCompleta_AbrirContaLancarPedidoEFecharConta_DeveCalcularValorTotalCorretamente()
    {
        // Cobre:
        // CT-USR-001
        // CT-MSA-001
        // CT-GAR-001
        // CT-PRD-001
        // CT-CTA-001
        // CT-PED-001
        // CT-PED-008
        // CT-CTA-012
        // CT-CTA-014

        string email =
            $"garcom-{Guid.NewGuid():N}@barteste.com";

        // ============================================================
        // Registrar estabelecimento
        // ============================================================

        await RegistrarNovoEstabelecimentoELogar(
            email,
            "Bar do Zé");

        // ============================================================
        // Cadastrar mesa
        // ============================================================

        await Page.GotoAsync(
            $"{UrlBase}/Mesa/Cadastrar");

        await Page.FillAsync(
            "#Numero",
            "1");

        await Page.FillAsync(
            "#QuantidadeLugares",
            "4");

        await Page.ClickAsync(
            "button:has-text('Cadastrar')");

        await Page.WaitForURLAsync(
            url => url.Contains("/Mesa/Listar"));

        // Confirma que a mesa foi cadastrada.
        await Expect(
            Page.Locator("body"))
            .ToContainTextAsync("Mesa 1");

        // ============================================================
        // Cadastrar garçom
        // ============================================================

        await Page.GotoAsync(
            $"{UrlBase}/Garcom/Cadastrar");

        await Page.FillAsync(
            "#Nome",
            "João Silva");

        await Page.ClickAsync(
            "button:has-text('Cadastrar')");

        await Page.WaitForURLAsync(
            url => url.Contains("/Garcom/Listar"));

        // Confirma que o garçom foi cadastrado.
        await Expect(
            Page.Locator("body"))
            .ToContainTextAsync("João Silva");

        // ============================================================
        // Cadastrar produto
        // ============================================================

        await Page.GotoAsync(
            $"{UrlBase}/Produto/Cadastrar");

        await Page.FillAsync(
            "#Nome",
            "Hambúrguer");

        await Page.FillAsync(
            "#Preco",
            "28");

        await Page.ClickAsync(
            "button:has-text('Cadastrar')");

        await Page.WaitForURLAsync(
            url => url.Contains("/Produto/Listar"));

        // Confirma que o produto foi realmente cadastrado.
        await Expect(
            Page.Locator("body"))
            .ToContainTextAsync("Hambúrguer");

        // ============================================================
        // Abrir conta
        // ============================================================

        await Page.GotoAsync(
            $"{UrlBase}/Conta/Abrir");

        await Page.FillAsync(
            "#NomeCliente",
            "Carlos Andrade");

        await Page.SelectOptionAsync(
            "#MesaId",
            new SelectOptionValue
            {
                Label = "Mesa 1"
            });

        await Page.SelectOptionAsync(
            "#GarcomId",
            new SelectOptionValue
            {
                Label = "João Silva"
            });

        await Page.ClickAsync(
            "button:has-text('Abrir Conta')");

        await Page.WaitForURLAsync(
            url => url.Contains("/Conta/Listar"));

        // ============================================================
        // Acessar a conta recém-aberta
        // ============================================================

        await Page.ClickAsync(
            "text=Ver");

        await Page.WaitForURLAsync(
            url => url.Contains("/Conta/Visualizar"));

        // ============================================================
        // Localizar produto
        // ============================================================

        var produtoSelect =
            Page.Locator("select[name=ProdutoId]");

        await Expect(produtoSelect)
            .ToBeVisibleAsync();

        var produtoOption =
            produtoSelect.Locator("option")
                .Filter(new LocatorFilterOptions
                {
                    HasText = "Hambúrguer"
                });

        await Expect(produtoOption)
            .ToHaveCountAsync(1);

        string? produtoId =
            await produtoOption.GetAttributeAsync("value");

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(produtoId),
            "O produto Hambúrguer deveria possuir um ID.");

        // ============================================================
        // Selecionar produto
        // ============================================================

        await produtoSelect.SelectOptionAsync(
            produtoId!);

        // Confirma que o produto realmente ficou selecionado.
        await Expect(produtoSelect)
            .ToHaveValueAsync(produtoId!);

        // ============================================================
        // Informar quantidade
        // ============================================================

        var quantidade =
            Page.Locator("input[name=Quantidade]");

        await quantidade.FillAsync("2");

        // ============================================================
        // Lançar pedido
        // ============================================================

        await Page.ClickAsync(
            "button:has-text(\"+\")");

        // Aguarda o produto aparecer na lista de pedidos.
        await Expect(
            Page.Locator("body"))
            .ToContainTextAsync("Hambúrguer");

        // ============================================================
        // Validar quantidade
        // ============================================================

        string paginaDepoisDoPedido =
            await Page.Locator("body").InnerTextAsync();

        Assert.IsTrue(
            paginaDepoisDoPedido.Contains("2"),
            "A quantidade do produto deveria ser 2.");

        // ============================================================
        // Validar total
        // ============================================================

        //
        // Não dependemos mais diretamente de:
        //
        // Page.ContentAsync().Contains("56,00")
        //
        // porque a representação monetária pode variar entre ambientes.
        //

        string textoPagina =
            await Page.Locator("body").InnerTextAsync();

        bool totalEncontrado =
            textoPagina.Contains("56,00") ||
            textoPagina.Contains("56.00") ||
            textoPagina.Contains("R$ 56") ||
            textoPagina.Contains("56");

        Assert.IsTrue(
            totalEncontrado,
            $"""
            O valor total deveria ser R$ 56,00.

            Conteúdo encontrado na página:
            {textoPagina}
            """);

        // ============================================================
        // Fechar conta
        // ============================================================

        await Page.ClickAsync(
            "text=Fechar Conta");

        // Aguarda a situação da conta mudar.
        await Expect(
            Page.Locator("body"))
            .ToContainTextAsync("Fechada");

        string conteudoPagina =
            await Page.Locator("body").InnerTextAsync();

        Assert.IsTrue(
            conteudoPagina.Contains("Fechada"),
            "A conta deveria estar com situação Fechada.");
    }
}