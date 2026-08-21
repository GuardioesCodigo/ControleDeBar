using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class ServicoGarcomTests
{
    private Mock<IRepositorioGarcom> repositorioGarcomMock = null!;
    private ServicoGarcom servicoGarcom = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioGarcomMock = new Mock<IRepositorioGarcom>();
        servicoGarcom = new ServicoGarcom(repositorioGarcomMock.Object);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarTodosOsGarconsOrdenadosPorNome()
    {
        // CT-GAR-004 (a isolação por estabelecimento em si é garantida pelo Query
        // Filter do EF - CT-GAR-005 - validada nos testes de integração)
        repositorioGarcomMock.Setup(r => r.SelecionarTodos()).Returns(
        [
            new Garcom { Nome = "Pedro Lima" },
            new Garcom { Nome = "Ana Souza" }
        ]);

        List<ListarGarcomDto> garcons = servicoGarcom.SelecionarTodos();

        Assert.AreEqual(2, garcons.Count);
        Assert.AreEqual("Ana Souza", garcons[0].Nome);
    }

    [TestMethod]
    public void Editar_DeveAtualizarNome_QuandoDadosSaoValidos()
    {
        // CT-GAR-006
        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcom.Id)).Returns(garcom);

        Result resultado = servicoGarcom.Editar(new EditarGarcomDto(garcom.Id, "João Pereira"));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioGarcomMock.Verify(r => r.Editar(garcom.Id, It.Is<Garcom>(g => g.Nome == "João Pereira")), Times.Once);
    }

    [TestMethod]
    public void Editar_DeveFalhar_QuandoNomeEstaEmBranco()
    {
        // CT-GAR-007
        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcom.Id)).Returns(garcom);

        Result resultado = servicoGarcom.Editar(new EditarGarcomDto(garcom.Id, ""));

        Assert.IsTrue(resultado.IsFailed);
        repositorioGarcomMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarDadosDoGarcom_QuandoExiste()
    {
        // CT-GAR-010
        Garcom garcom = new() { Nome = "Maria Souza" };
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcom.Id)).Returns(garcom);

        Result<DetalhesGarcomDto> resultado = servicoGarcom.SelecionarPorId(garcom.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Maria Souza", resultado.Value.Nome);
    }
}
