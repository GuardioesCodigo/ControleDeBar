using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloPedido;

[TestClass]
public sealed class RepositorioPedidoEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    private Conta CriarContaComMesaEGarcom(int numeroMesa = 1)
    {
        Mesa mesa = new() { Numero = numeroMesa, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new() { NomeCliente = "Carlos Andrade", MesaId = mesa.Id, GarcomId = garcom.Id };
        repositorioConta.Cadastrar(conta);

        return conta;
    }

    [TestMethod]
    public void SelecionarPorContaId_DeveRetornarTodosOsPedidosDaConta()
    {
        // CT-PED-011
        Conta conta = CriarContaComMesaEGarcom();

        Produto hamburguer = new() { Nome = "Hambúrguer", Preco = 28m };
        Produto refrigerante = new() { Nome = "Refrigerante", Preco = 6m };
        repositorioProduto.Cadastrar(hamburguer);
        repositorioProduto.Cadastrar(refrigerante);

        repositorioPedido.Cadastrar(new Pedido { ContaId = conta.Id, ProdutoId = hamburguer.Id, Quantidade = 1 });
        repositorioPedido.Cadastrar(new Pedido { ContaId = conta.Id, ProdutoId = refrigerante.Id, Quantidade = 2 });

        List<Pedido> pedidos = repositorioPedido.SelecionarPorContaId(conta.Id);

        Assert.AreEqual(2, pedidos.Count);
        Assert.IsTrue(pedidos.All(p => p.Produto is not null));
    }

    [TestMethod]
    public void SelecionarPorContaId_NaoDeveRetornarPedidosDeContaDeOutroEstabelecimento()
    {
        // CT-PED-012
        Conta conta = CriarContaComMesaEGarcom();
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        repositorioProduto.Cadastrar(produto);
        repositorioPedido.Cadastrar(new Pedido { ContaId = conta.Id, ProdutoId = produto.Id, Quantidade = 1 });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloPedido.RepositorioPedidoEmOrm repositorioPedidoOutro = new(contextoOutroEstabelecimento);

        // O Query Filter global impede inclusive a leitura direta pelo Id da conta
        // de outro estabelecimento, então a lista retornada é sempre vazia.
        List<Pedido> pedidosDoOutroEstabelecimento = repositorioPedidoOutro.SelecionarPorContaId(conta.Id);

        Assert.AreEqual(0, pedidosDoOutroEstabelecimento.Count);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarPedidoComProdutoCarregado()
    {
        // CT-PED-013
        Conta conta = CriarContaComMesaEGarcom();
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        repositorioProduto.Cadastrar(produto);

        Pedido pedido = new() { ContaId = conta.Id, ProdutoId = produto.Id, Quantidade = 3 };
        repositorioPedido.Cadastrar(pedido);

        Pedido? pedidoCarregado = repositorioPedido.SelecionarPorId(pedido.Id);

        Assert.IsNotNull(pedidoCarregado);
        Assert.AreEqual("Hambúrguer", pedidoCarregado.Produto.Nome);
        Assert.AreEqual(3, pedidoCarregado.Quantidade);
        Assert.AreEqual(84m, pedidoCarregado.Subtotal);
    }
}
