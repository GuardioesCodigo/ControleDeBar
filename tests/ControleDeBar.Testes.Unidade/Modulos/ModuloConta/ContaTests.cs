using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ContaTests
{
    [TestMethod]
    public void Validar_DeveRetornarSemErros_QuandoContaEstaValida()
    {
        // CT-CTA-001
        Conta conta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = Guid.CreateVersion7(),
            GarcomId = Guid.CreateVersion7()
        };

        List<string> erros = conta.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoMesaNaoForInformada()
    {
        // CT-CTA-002
        Conta conta = new() { NomeCliente = "Cliente X", GarcomId = Guid.CreateVersion7() };

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Mesa")));
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoGarcomNaoForInformado()
    {
        // CT-CTA-002
        Conta conta = new() { NomeCliente = "Cliente X", MesaId = Guid.CreateVersion7() };

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Garçom")));
    }

    [TestMethod]
    public void Fechar_DeveAlterarSituacaoParaFechada()
    {
        // CT-CTA-012
        Conta conta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = Guid.CreateVersion7(),
            GarcomId = Guid.CreateVersion7(),
            Situacao = SituacaoConta.Aberta
        };

        conta.Fechar();

        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
    }

    [TestMethod]
    public void ValorTotal_DeveSomarOsSubtotaisDeTodosOsPedidos()
    {
        // CT-PED-009 / CT-PED-010
        Produto hamburguer = new() { Nome = "Hambúrguer", Preco = 28m };
        Produto refrigerante = new() { Nome = "Refrigerante", Preco = 6m };

        Conta conta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = Guid.CreateVersion7(),
            GarcomId = Guid.CreateVersion7(),
            Pedidos =
            [
                new Pedido { Produto = hamburguer, Quantidade = 2 },
                new Pedido { Produto = refrigerante, Quantidade = 3 }
            ]
        };

        Assert.AreEqual(74m, conta.ValorTotal); // 2*28 + 3*6
    }

    [TestMethod]
    public void ValorTotal_DeveSerZero_QuandoNaoHaPedidos()
    {
        // CT-FAT-004 (base para faturamento zerado)
        Conta conta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = Guid.CreateVersion7(),
            GarcomId = Guid.CreateVersion7()
        };

        Assert.AreEqual(0m, conta.ValorTotal);
    }
}
