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

    [TestMethod]
    public void SelecionarTodos_DeveRetornarTodasAsMesasOrdenadasPorNumero()
    {
        // CT-MSA-007
        repositorioMesaMock.Setup(r => r.SelecionarTodos()).Returns(
        [
            new Mesa { Numero = 3, QuantidadeLugares = 2 },
            new Mesa { Numero = 1, QuantidadeLugares = 4 }
        ]);

        List<ListarMesaDto> mesas = servicoMesa.SelecionarTodos();

        Assert.AreEqual(2, mesas.Count);
        Assert.AreEqual(1, mesas[0].Numero);
        Assert.AreEqual(3, mesas[1].Numero);
    }

    [TestMethod]
    public void Editar_DeveFalhar_QuandoNovoNumeroJaPertenceAOutraMesa()
    {
        // CT-MSA-011
        Mesa mesa = new() { Numero = 5, QuantidadeLugares = 4 };
        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesa.Id)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.NumeroJaExiste(6, mesa.Id)).Returns(true);

        Result resultado = servicoMesa.Editar(new EditarMesaDto(mesa.Id, 6, 4, StatusMesa.Livre));

        Assert.IsTrue(resultado.IsFailed);
        repositorioMesaMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Mesa>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarDadosDaMesa_QuandoExiste()
    {
        // CT-MSA-014
        Mesa mesa = new() { Numero = 7, QuantidadeLugares = 6, Status = StatusMesa.Ocupada };
        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesa.Id)).Returns(mesa);

        Result<DetalhesMesaDto> resultado = servicoMesa.SelecionarPorId(mesa.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(7, resultado.Value.Numero);
        Assert.AreEqual(StatusMesa.Ocupada, resultado.Value.Status);
    }
}
