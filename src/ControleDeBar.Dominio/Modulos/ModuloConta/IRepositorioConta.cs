using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public interface IRepositorioConta : IRepositorio<Conta>
{
    List<Conta> SelecionarAbertas();
    List<Conta> SelecionarFechadasPorData(DateOnly data);
    bool MesaPossuiContaAberta(Guid idMesa, Guid? idContaIgnorada = null);
    bool GarcomPossuiContaAberta(Guid idGarcom);
}
