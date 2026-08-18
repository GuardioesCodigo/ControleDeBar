using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloMesa;

public interface IRepositorioMesa : IRepositorio<Mesa>
{
    bool NumeroJaExiste(
        int numero,
        Guid estabelecimentoId,
        Guid? idIgnorado = null
    );
    bool PossuiContaAbertaVinculada(Guid idMesa);
}
