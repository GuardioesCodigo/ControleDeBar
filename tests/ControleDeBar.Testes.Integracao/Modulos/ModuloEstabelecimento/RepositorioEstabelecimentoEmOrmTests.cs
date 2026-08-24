using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloEstabelecimento;

[TestClass]
public sealed class RepositorioEstabelecimentoEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void SelecionarDoUsuarioAtual_NaoDeveRetornarEstabelecimentoDeOutroUsuario()
    {
        // CT-USR-013: um usuário não deve conseguir visualizar dados de outro estabelecimento
        repositorioEstabelecimento.Cadastrar(new Estabelecimento { Nome = "Bar do Zé" });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloEstabelecimento.RepositorioEstabelecimentoEmOrm repositorioEstabelecimentoOutro = new(contextoOutroEstabelecimento);

        Estabelecimento? resultado = repositorioEstabelecimentoOutro.SelecionarDoUsuarioAtual();

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void SaveChanges_DeveBloquearEdicao_DeEstabelecimentoDeOutroUsuario()
    {
        // CT-USR-014: um usuário não deve conseguir editar dados de outro estabelecimento,
        // mesmo contornando o Query Filter e anexando a entidade manualmente.
        Estabelecimento estabelecimento = new() { Nome = "Bar do Zé" };
        repositorioEstabelecimento.Cadastrar(estabelecimento);

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Estabelecimento estabelecimentoForjado = new()
        {
            Id = estabelecimento.Id,
            UserId = estabelecimento.UserId,
            Nome = "Nome Adulterado"
        };
        contextoOutroEstabelecimento.Attach(estabelecimentoForjado);
        contextoOutroEstabelecimento.Entry(estabelecimentoForjado).State = EntityState.Modified;

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => contextoOutroEstabelecimento.SaveChanges());
    }
}
