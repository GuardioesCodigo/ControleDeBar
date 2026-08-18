using ControleDeBar.Aplicacao.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloPedido;

[TestClass]
public sealed class ServicoPedidoTests
{
    private Mock<IRepositorioPedido> repositorioPedidoMock = null!;
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private ServicoPedido servicoPedido = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioPedidoMock = new Mock<IRepositorioPedido>();
        repositorioContaMock = new Mock<IRepositorioConta>();
        repositorioProdutoMock = new Mock<IRepositorioProduto>();

        servicoPedido = new ServicoPedido(
            repositorioPedidoMock.Object,
            repositorioContaMock.Object,
            repositorioProdutoMock.Object
        );
    }

    [TestMethod]
    public void Registrar_DeveFalhar_QuandoContaNaoExiste()
    {
        // CT-PED-006
        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns((Conta?)null);

        Result resultado = servicoPedido.Registrar(new CadastrarPedidoDto(Guid.NewGuid(), Guid.NewGuid(), 1));

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Registrar_DeveFalhar_QuandoContaJaEstaFechada()
    {
        // CT-PED-007
        Conta contaFechada = new()
        {
            NomeCliente = "Cliente X",
            MesaId = Guid.NewGuid(),
            GarcomId = Guid.NewGuid(),
            Situacao = SituacaoConta.Fechada
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(contaFechada);

        Result resultado = servicoPedido.Registrar(new CadastrarPedidoDto(contaFechada.Id, Guid.NewGuid(), 1));

        Assert.IsTrue(resultado.IsFailed);
        repositorioPedidoMock.Verify(r => r.Cadastrar(It.IsAny<Pedido>()), Times.Never);
    }

    [TestMethod]
    public void Registrar_DeveFalhar_QuandoProdutoNaoExiste()
    {
        // CT-PED-005
        Conta contaAberta = new()
        {
            NomeCliente = "Cliente X",
            MesaId = Guid.NewGuid(),
            GarcomId = Guid.NewGuid(),
            Situacao = SituacaoConta.Aberta
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(contaAberta);
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns((Produto?)null);

        Result resultado = servicoPedido.Registrar(new CadastrarPedidoDto(contaAberta.Id, Guid.NewGuid(), 1));

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Registrar_DeveCadastrarPedido_QuandoContaEProdutoSaoValidos()
    {
        // CT-PED-001 / CT-PED-008
        Conta contaAberta = new()
        {
            NomeCliente = "Cliente X",
            MesaId = Guid.NewGuid(),
            GarcomId = Guid.NewGuid(),
            Situacao = SituacaoConta.Aberta
        };
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };

        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(contaAberta);
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(produto);

        Result resultado = servicoPedido.Registrar(new CadastrarPedidoDto(contaAberta.Id, produto.Id, 2));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioPedidoMock.Verify(r => r.Cadastrar(It.Is<Pedido>(p => p.Quantidade == 2)), Times.Once);
    }
}
