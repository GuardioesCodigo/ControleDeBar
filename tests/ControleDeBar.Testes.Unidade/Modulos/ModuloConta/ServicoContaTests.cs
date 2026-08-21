using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ServicoContaTests
{
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private Mock<IRepositorioMesa> repositorioMesaMock = null!;
    private Mock<IRepositorioGarcom> repositorioGarcomMock = null!;
    private ServicoConta servicoConta = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioContaMock = new Mock<IRepositorioConta>();
        repositorioMesaMock = new Mock<IRepositorioMesa>();
        repositorioGarcomMock = new Mock<IRepositorioGarcom>();

        servicoConta = new ServicoConta(
            repositorioContaMock.Object,
            repositorioMesaMock.Object,
            repositorioGarcomMock.Object
        );
    }

    [TestMethod]
    public void Abrir_DeveFalhar_QuandoMesaNaoExiste()
    {
        // CT-CTA-005
        repositorioMesaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns((Mesa?)null);

        Result resultado = servicoConta.Abrir(new CadastrarContaDto("Cliente X", Guid.NewGuid(), Guid.NewGuid()));

        Assert.IsTrue(resultado.IsFailed);
        repositorioContaMock.Verify(r => r.Cadastrar(It.IsAny<Conta>()), Times.Never);
    }

    [TestMethod]
    public void Abrir_DeveFalhar_QuandoGarcomNaoExiste()
    {
        // CT-CTA-006
        repositorioMesaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(new Mesa { Numero = 1, QuantidadeLugares = 4 });
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns((Garcom?)null);

        Result resultado = servicoConta.Abrir(new CadastrarContaDto("Cliente X", Guid.NewGuid(), Guid.NewGuid()));

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Abrir_DeveFalhar_QuandoMesaJaPossuiContaAberta()
    {
        // CT-CTA-004
        repositorioMesaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(new Mesa { Numero = 3, QuantidadeLugares = 4 });
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(new Garcom { Nome = "João" });
        repositorioContaMock.Setup(r => r.MesaPossuiContaAberta(It.IsAny<Guid>(), null)).Returns(true);

        Result resultado = servicoConta.Abrir(new CadastrarContaDto("Cliente X", Guid.NewGuid(), Guid.NewGuid()));

        Assert.IsTrue(resultado.IsFailed);
        repositorioContaMock.Verify(r => r.Cadastrar(It.IsAny<Conta>()), Times.Never);
    }

    [TestMethod]
    public void Abrir_DeveCadastrarComSituacaoAberta_QuandoDadosSaoValidos()
    {
        // CT-CTA-001 / CT-CTA-003
        repositorioMesaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(new Mesa { Numero = 3, QuantidadeLugares = 4 });
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(new Garcom { Nome = "João" });
        repositorioContaMock.Setup(r => r.MesaPossuiContaAberta(It.IsAny<Guid>(), null)).Returns(false);

        Conta? contaCadastrada = null;
        repositorioContaMock.Setup(r => r.Cadastrar(It.IsAny<Conta>()))
            .Callback<Conta>(c => contaCadastrada = c);

        Result resultado = servicoConta.Abrir(new CadastrarContaDto("Carlos Andrade", Guid.NewGuid(), Guid.NewGuid()));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(contaCadastrada);
        Assert.AreEqual(SituacaoConta.Aberta, contaCadastrada.Situacao);
    }

    [TestMethod]
    public void Fechar_DeveFalhar_QuandoContaJaEstaFechada()
    {
        // CT-CTA-013
        Conta contaFechada = new()
        {
            NomeCliente = "Cliente X",
            MesaId = Guid.NewGuid(),
            GarcomId = Guid.NewGuid(),
            Situacao = SituacaoConta.Fechada
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(contaFechada);

        Result resultado = servicoConta.Fechar(contaFechada.Id);

        Assert.IsTrue(resultado.IsFailed);
        repositorioContaMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Conta>()), Times.Never);
    }

    [TestMethod]
    public void Fechar_DeveAlterarSituacaoParaFechada_QuandoContaEstaAberta()
    {
        // CT-CTA-012
        Conta contaAberta = new()
        {
            NomeCliente = "Cliente X",
            MesaId = Guid.NewGuid(),
            GarcomId = Guid.NewGuid(),
            Situacao = SituacaoConta.Aberta
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(It.IsAny<Guid>())).Returns(contaAberta);

        Result resultado = servicoConta.Fechar(contaAberta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(SituacaoConta.Fechada, contaAberta.Situacao);
        repositorioContaMock.Verify(r => r.Editar(contaAberta.Id, It.IsAny<Conta>()), Times.Once);
    }

    [TestMethod]
    public void Editar_DeveAtualizarNomeDoCliente_QuandoContaEstaAberta()
    {
        // CT-CTA-010
        Guid mesaId = Guid.NewGuid();
        Guid garcomId = Guid.NewGuid();

        Conta contaAberta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = mesaId,
            GarcomId = garcomId,
            Situacao = SituacaoConta.Aberta
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(contaAberta.Id)).Returns(contaAberta);
        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesaId)).Returns(new Mesa { Numero = 3, QuantidadeLugares = 4 });
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcomId)).Returns(new Garcom { Nome = "João" });
        repositorioContaMock.Setup(r => r.MesaPossuiContaAberta(mesaId, contaAberta.Id)).Returns(false);

        Result resultado = servicoConta.Editar(new EditarContaDto(contaAberta.Id, "Carlos A. Andrade", mesaId, garcomId));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContaMock.Verify(r => r.Editar(
            contaAberta.Id, It.Is<Conta>(c => c.NomeCliente == "Carlos A. Andrade")
        ), Times.Once);
    }

    [TestMethod]
    public void Editar_DeveTrocarAMesaVinculada_QuandoNovaMesaEstaLivre()
    {
        // CT-CTA-011
        Guid mesaOriginalId = Guid.NewGuid();
        Guid novaMesaId = Guid.NewGuid();
        Guid garcomId = Guid.NewGuid();

        Conta contaAberta = new()
        {
            NomeCliente = "Carlos Andrade",
            MesaId = mesaOriginalId,
            GarcomId = garcomId,
            Situacao = SituacaoConta.Aberta
        };

        repositorioContaMock.Setup(r => r.SelecionarPorId(contaAberta.Id)).Returns(contaAberta);
        repositorioMesaMock.Setup(r => r.SelecionarPorId(novaMesaId)).Returns(new Mesa { Numero = 8, QuantidadeLugares = 2 });
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcomId)).Returns(new Garcom { Nome = "João" });
        repositorioContaMock.Setup(r => r.MesaPossuiContaAberta(novaMesaId, contaAberta.Id)).Returns(false);

        Result resultado = servicoConta.Editar(new EditarContaDto(contaAberta.Id, "Carlos Andrade", novaMesaId, garcomId));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContaMock.Verify(r => r.Editar(
            contaAberta.Id, It.Is<Conta>(c => c.MesaId == novaMesaId)
        ), Times.Once);
    }
}
