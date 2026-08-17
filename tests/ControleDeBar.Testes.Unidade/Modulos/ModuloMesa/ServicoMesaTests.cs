using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class ServicoMesaTests
{
    private Mock<IRepositorioMesa> repositorioMesaMock = null!;
    private ServicoMesa servicoMesa = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioMesaMock = new Mock<IRepositorioMesa>();
        servicoMesa = new ServicoMesa(repositorioMesaMock.Object);
    }

    [TestMethod]
    public void Cadastrar_DeveFalhar_QuandoNumeroJaExiste()
    {
        // CT-MSA-003
        repositorioMesaMock.Setup(r => r.NumeroJaExiste(5, null)).Returns(true);

        Result resultado = servicoMesa.Cadastrar(new CadastrarMesaDto(5, 4, StatusMesa.Livre));

        Assert.IsTrue(resultado.IsFailed);
        repositorioMesaMock.Verify(r => r.Cadastrar(It.IsAny<Mesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_DeveSalvar_QuandoNumeroNaoExiste()
    {
        // CT-MSA-001
        repositorioMesaMock.Setup(r => r.NumeroJaExiste(5, null)).Returns(false);

        Result resultado = servicoMesa.Cadastrar(new CadastrarMesaDto(5, 4, StatusMesa.Livre));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioMesaMock.Verify(r => r.Cadastrar(It.IsAny<Mesa>()), Times.Once);
    }

    [TestMethod]
    public void Excluir_DeveFalhar_QuandoMesaPossuiContaAbertaVinculada()
    {
        // CT-MSA-013
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4 };

        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesa.Id)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.PossuiContaAbertaVinculada(mesa.Id)).Returns(true);

        Result resultado = servicoMesa.Excluir(mesa.Id);

        Assert.IsTrue(resultado.IsFailed);
        repositorioMesaMock.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Excluir_DeveExcluir_QuandoMesaNaoPossuiVinculo()
    {
        // CT-MSA-012
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4 };

        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesa.Id)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.PossuiContaAbertaVinculada(mesa.Id)).Returns(false);

        Result resultado = servicoMesa.Excluir(mesa.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioMesaMock.Verify(r => r.Excluir(mesa.Id), Times.Once);
    }
}
