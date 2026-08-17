using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloGarcom;

public sealed class RepositorioGarcomEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Garcom>(dbContext), IRepositorioGarcom
{
    public bool PossuiContaAbertaVinculada(Guid idGarcom)
    {
        return dbContext.Set<Conta>()
            .Any(c => c.GarcomId == idGarcom && c.Situacao == SituacaoConta.Aberta);
    }
}
