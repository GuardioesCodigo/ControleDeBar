using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

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
}
