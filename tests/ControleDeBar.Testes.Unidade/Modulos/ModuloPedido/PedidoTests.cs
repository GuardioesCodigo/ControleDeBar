using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloPedido;

[TestClass]
public sealed class PedidoTests
{
    [TestMethod]
    public void Validar_DeveRetornarSemErros_QuandoPedidoEstaValido()
    {
        // CT-PED-001
        Pedido pedido = new() { ContaId = Guid.CreateVersion7(), ProdutoId = Guid.CreateVersion7(), Quantidade = 2 };

        List<string> erros = pedido.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void Validar_DeveRetornarErro_QuandoQuantidadeNaoForMaiorQueZero(int quantidade)
    {
        // CT-PED-003 / CT-PED-004
        Pedido pedido = new() { ContaId = Guid.CreateVersion7(), ProdutoId = Guid.CreateVersion7(), Quantidade = quantidade };

        List<string> erros = pedido.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Quantidade")));
    }

    [TestMethod]
    public void Subtotal_DeveSerPrecoDoProdutoMultiplicadoPelaQuantidade()
    {
        // CT-PED-010
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        Pedido pedido = new() { Produto = produto, Quantidade = 3 };

        Assert.AreEqual(84m, pedido.Subtotal);
    }
}
