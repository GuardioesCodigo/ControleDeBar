using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloEstabelecimento;

public sealed class RepositorioEstabelecimentoEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Estabelecimento>(dbContext), IRepositorioEstabelecimento
{
    public Estabelecimento? SelecionarDoUsuarioAtual()
    {
        if (!provedorDeUsuario.EstaAutenticado || provedorDeUsuario.Id is null)
            return null;

        return registros.SingleOrDefault(e => e.UserId == provedorDeUsuario.Id);
    }
}