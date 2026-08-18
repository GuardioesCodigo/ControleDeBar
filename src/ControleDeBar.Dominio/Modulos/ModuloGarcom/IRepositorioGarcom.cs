using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloGarcom;

public interface IRepositorioGarcom : IRepositorio<Garcom>
{
    bool PossuiContaAbertaVinculada(Guid idGarcom);
}
