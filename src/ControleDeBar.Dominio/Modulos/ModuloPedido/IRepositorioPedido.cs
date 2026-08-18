using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloPedido;

public interface IRepositorioPedido : IRepositorio<Pedido>
{
    List<Pedido> SelecionarPorContaId(Guid contaId);
}
