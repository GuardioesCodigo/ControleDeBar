using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloMesa;

[TestClass]
public sealed class RepositorioMesaEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_DevePreencherUserIdAutomaticamente()
    {
        // CT-MSA-001 / CT-GAR-003 / CT-PRD-007 (preenchimento automático do dono)
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4, Status = StatusMesa.Livre };

        repositorioMesa.Cadastrar(mesa);

        Mesa? salva = repositorioMesa.SelecionarPorId(mesa.Id);

        Assert.IsNotNull(salva);
        Assert.AreEqual(provedorDeUsuario.Id, salva.UserId);
    }

    [TestMethod]
    public void SelecionarTodos_NaoDeveRetornarMesasDeOutroEstabelecimento()
    {
        // CT-MSA-008: isolamento por estabelecimento via Query Filter global
        repositorioMesa.Cadastrar(new Mesa { Numero = 1, QuantidadeLugares = 4 });
        repositorioMesa.Cadastrar(new Mesa { Numero = 2, QuantidadeLugares = 2 });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out Guid outroUserId);

        Infra.Modulos.ModuloMesa.RepositorioMesaEmOrm repositorioMesaOutro = new(contextoOutroEstabelecimento);

        repositorioMesaOutro.Cadastrar(new Mesa { Numero = 1, QuantidadeLugares = 6 });

        List<Mesa> mesasDoEstabelecimentoOriginal = repositorioMesa.SelecionarTodos();
        List<Mesa> mesasDoOutroEstabelecimento = repositorioMesaOutro.SelecionarTodos();

        Assert.AreEqual(2, mesasDoEstabelecimentoOriginal.Count);
        Assert.AreEqual(1, mesasDoOutroEstabelecimento.Count);
        Assert.IsTrue(mesasDoEstabelecimentoOriginal.All(m => m.UserId == provedorDeUsuario.Id));
        Assert.IsTrue(mesasDoOutroEstabelecimento.All(m => m.UserId == outroUserId));
    }

    [TestMethod]
    public void NumeroJaExiste_DeveConsiderarApenasMesasDoProprioEstabelecimento()
    {
        // CT-MSA-004: mesmo número em estabelecimentos diferentes é permitido
        repositorioMesa.Cadastrar(new Mesa { Numero = 5, QuantidadeLugares = 4 });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloMesa.RepositorioMesaEmOrm repositorioMesaOutro = new(contextoOutroEstabelecimento);

        bool existeNoOutroEstabelecimento = repositorioMesaOutro.NumeroJaExiste(5);

        Assert.IsFalse(existeNoOutroEstabelecimento);
    }

    [TestMethod]
    public void SelecionarPorId_NaoDeveEncontrarRegistroDeOutroEstabelecimento()
    {
        // CT-USR-013 / CT-USR-014: um usuário não deve acessar dados de outro estabelecimento
        Mesa mesa = new() { Numero = 3, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloMesa.RepositorioMesaEmOrm repositorioMesaOutro = new(contextoOutroEstabelecimento);

        Mesa? resultado = repositorioMesaOutro.SelecionarPorId(mesa.Id);

        Assert.IsNull(resultado);
    }
}
