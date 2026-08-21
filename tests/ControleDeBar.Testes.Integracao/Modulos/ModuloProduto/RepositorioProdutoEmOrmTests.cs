using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloProduto;

[TestClass]
public sealed class RepositorioProdutoEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void PossuiPedidoVinculado_DeveRetornarVerdadeiro_QuandoProdutoFoiPedido()
    {
        // CT-PRD-013: bloqueio de exclusão de produto com pedido vinculado
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcom.Cadastrar(garcom);

        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        repositorioProduto.Cadastrar(produto);

        Conta conta = new() { NomeCliente = "Cliente X", MesaId = mesa.Id, GarcomId = garcom.Id };
        repositorioConta.Cadastrar(conta);

        repositorioPedido.Cadastrar(new Pedido { ContaId = conta.Id, ProdutoId = produto.Id, Quantidade = 1 });

        bool possuiVinculo = repositorioProduto.PossuiPedidoVinculado(produto.Id);

        Assert.IsTrue(possuiVinculo);
    }

    [TestMethod]
    public void PossuiPedidoVinculado_DeveRetornarFalso_QuandoProdutoNuncaFoiPedido()
    {
        // CT-PRD-012: exclusão permitida sem vínculo
        Produto produto = new() { Nome = "Porção de Batata", Preco = 22m };
        repositorioProduto.Cadastrar(produto);

        bool possuiVinculo = repositorioProduto.PossuiPedidoVinculado(produto.Id);

        Assert.IsFalse(possuiVinculo);
    }

    [TestMethod]
    public void SelecionarTodos_NaoDeveRetornarProdutosDeOutroEstabelecimento()
    {
        // CT-PRD-009
        repositorioProduto.Cadastrar(new Produto { Nome = "Hambúrguer", Preco = 28m });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloProduto.RepositorioProdutoEmOrm repositorioProdutoOutro = new(contextoOutroEstabelecimento);
        repositorioProdutoOutro.Cadastrar(new Produto { Nome = "Refrigerante", Preco = 6m });

        List<Produto> produtosDoEstabelecimentoOriginal = repositorioProduto.SelecionarTodos();
        List<Produto> produtosDoOutroEstabelecimento = repositorioProdutoOutro.SelecionarTodos();

        Assert.AreEqual(1, produtosDoEstabelecimentoOriginal.Count);
        Assert.AreEqual(1, produtosDoOutroEstabelecimento.Count);
        Assert.AreEqual("Hambúrguer", produtosDoEstabelecimentoOriginal[0].Nome);
        Assert.AreEqual("Refrigerante", produtosDoOutroEstabelecimento[0].Nome);
    }
}
