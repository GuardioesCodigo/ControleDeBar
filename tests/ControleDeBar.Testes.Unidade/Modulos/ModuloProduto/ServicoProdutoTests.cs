using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ServicoProdutoTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private ServicoProduto servicoProduto = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        servicoProduto = new ServicoProduto(repositorioProdutoMock.Object);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarTodosOsProdutosOrdenadosPorNome()
    {
        // CT-PRD-008 (a isolação por estabelecimento em si - CT-PRD-009 - é
        // garantida pelo Query Filter do EF, validada nos testes de integração)
        repositorioProdutoMock.Setup(r => r.SelecionarTodos()).Returns(
        [
            new Produto { Nome = "Refrigerante", Preco = 6m },
            new Produto { Nome = "Hambúrguer", Preco = 28m }
        ]);

        List<ListarProdutoDto> produtos = servicoProduto.SelecionarTodos();

        Assert.AreEqual(2, produtos.Count);
        Assert.AreEqual("Hambúrguer", produtos[0].Nome);
    }

    [TestMethod]
    public void Editar_DeveAtualizarNomeEPreco_QuandoDadosSaoValidos()
    {
        // CT-PRD-010
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(produto.Id)).Returns(produto);

        Result resultado = servicoProduto.Editar(new EditarProdutoDto(produto.Id, "Hambúrguer Duplo", 34m));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioProdutoMock.Verify(r => r.Editar(
            produto.Id,
            It.Is<Produto>(p => p.Nome == "Hambúrguer Duplo" && p.Preco == 34m)
        ), Times.Once);
    }

    [TestMethod]
    public void Editar_DeveFalhar_QuandoPrecoInformadoEZero()
    {
        // CT-PRD-011
        Produto produto = new() { Nome = "Hambúrguer", Preco = 28m };
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(produto.Id)).Returns(produto);

        Result resultado = servicoProduto.Editar(new EditarProdutoDto(produto.Id, "Hambúrguer", 0m));

        Assert.IsTrue(resultado.IsFailed);
        repositorioProdutoMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Produto>()), Times.Never);
    }
}
