using ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloEstabelecimento;

[TestClass]
public sealed class ServicoEstabelecimentoTests
{
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimentoMock = null!;
    private ServicoEstabelecimento servicoEstabelecimento = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioEstabelecimentoMock = new Mock<IRepositorioEstabelecimento>();
        servicoEstabelecimento = new ServicoEstabelecimento(repositorioEstabelecimentoMock.Object);
    }

    [TestMethod]
    public void Cadastrar_DeveCriarEstabelecimentoComNomeInformado()
    {
        // CT-USR-006: vínculo automático de um novo estabelecimento ao usuário
        Estabelecimento? estabelecimentoCadastrado = null;
        repositorioEstabelecimentoMock.Setup(r => r.Cadastrar(It.IsAny<Estabelecimento>()))
            .Callback<Estabelecimento>(e => estabelecimentoCadastrado = e);

        Result resultado = servicoEstabelecimento.Cadastrar("Bar do Zé");

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(estabelecimentoCadastrado);
        Assert.AreEqual("Bar do Zé", estabelecimentoCadastrado.Nome);
    }

    [TestMethod]
    public void SelecionarAtual_DeveRetornarDadosDoEstabelecimentoDoUsuarioAutenticado()
    {
        // CT-USR-011
        Estabelecimento estabelecimento = new() { Nome = "Bar do Zé", Endereco = "Rua A, 123" };
        repositorioEstabelecimentoMock.Setup(r => r.SelecionarDoUsuarioAtual()).Returns(estabelecimento);

        Result<DetalhesEstabelecimentoDto> resultado = servicoEstabelecimento.SelecionarAtual();

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Bar do Zé", resultado.Value.Nome);
        Assert.AreEqual("Rua A, 123", resultado.Value.Endereco);
    }

    [TestMethod]
    public void Editar_DeveAtualizarNomeEEndereco_QuandoDadosSaoValidos()
    {
        // CT-USR-012
        Estabelecimento estabelecimento = new() { Nome = "Bar do Zé" };
        repositorioEstabelecimentoMock.Setup(r => r.SelecionarDoUsuarioAtual()).Returns(estabelecimento);

        Result resultado = servicoEstabelecimento.Editar(new EditarEstabelecimentoDto("Bar do Zé Ltda", "Rua Nova, 45"));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioEstabelecimentoMock.Verify(r => r.Editar(
            estabelecimento.Id,
            It.Is<Estabelecimento>(e => e.Nome == "Bar do Zé Ltda" && e.Endereco == "Rua Nova, 45")
        ), Times.Once);
    }
}
