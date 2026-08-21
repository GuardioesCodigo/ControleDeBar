using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

[TestClass]
public sealed class ControleDeBarDbContextTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void SaveChanges_DeveLancarExcecao_QuandoNaoHaUsuarioAutenticado()
    {
        // Regra transversal: nenhuma operação de escrita é permitida sem autenticação
        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        using ControleDeBarDbContext contextoSemUsuario = new(options, provedorDeUsuario: null);

        contextoSemUsuario.Mesas.Add(new Mesa { Numero = 1, QuantidadeLugares = 4 });

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => contextoSemUsuario.SaveChanges());
    }

    [TestMethod]
    public void SaveChanges_DeveBloquearEdicao_DeRegistroDeOutroEstabelecimento()
    {
        // CT-USR-014: um usuário não deve conseguir editar dados de outro estabelecimento,
        // mesmo manipulando diretamente a entidade rastreada por outro contexto.
        Mesa mesa = new() { Numero = 7, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        // Simula um agente malicioso anexando e alterando um registro que
        // não pertence a ele (contorna o Query Filter propositalmente).
        Mesa mesaForjada = new() { Id = mesa.Id, UserId = mesa.UserId, Numero = 99, QuantidadeLugares = 4 };
        contextoOutroEstabelecimento.Attach(mesaForjada);
        contextoOutroEstabelecimento.Entry(mesaForjada).State = EntityState.Modified;

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => contextoOutroEstabelecimento.SaveChanges());
    }
}
