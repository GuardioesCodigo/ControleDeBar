using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloMesa;

public sealed class RepositorioMesaEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Mesa>(dbContext), IRepositorioMesa
{
    public bool NumeroJaExiste(
        int numero,
        Guid estabelecimentoId,
        Guid? idIgnorado = null
    )
    {
        return registros.Any(m =>
            m.EstabelecimentoId == estabelecimentoId &&
            m.Numero == numero &&
            m.Id != idIgnorado);
    }

    public bool PossuiContaAbertaVinculada(Guid idMesa)
    {
        return dbContext.Set<Conta>()
            .Any(c => c.MesaId == idMesa &&
                      c.Situacao == SituacaoConta.Aberta);
    }
}