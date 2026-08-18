using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;

namespace ControleDeBar.Infra.Modulos.ModuloEstabelecimento;

public sealed class RepositorioEstabelecimentoEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Estabelecimento>(dbContext), IRepositorioEstabelecimento
{
    public Estabelecimento? SelecionarDoUsuarioAtual()
    {
        return registros.SingleOrDefault();
    }
}
