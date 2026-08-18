using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ProdutoTests
{
    [TestMethod]
    public void Validar_DeveRetornarSemErros_QuandoProdutoEstaValido()
    {
        // CT-PRD-001 / CT-PRD-006 (preço decimal)
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28.90m };

        List<string> erros = produto.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoNomeEstaVazio()
    {
        // CT-PRD-002
        Produto produto = new() { Nome = "", Preco = 10m };

        List<string> erros = produto.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Nome")));
    }

    [DataTestMethod]
    [DataRow(0.0)]
    [DataRow(-5.0)]
    public void Validar_DeveRetornarErro_QuandoPrecoNaoForMaiorQueZero(double precoDouble)
    {
        // CT-PRD-004 / CT-PRD-005
        Produto produto = new() { Nome = "Refrigerante", Preco = (decimal)precoDouble };

        List<string> erros = produto.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Preço")));
    }
}
