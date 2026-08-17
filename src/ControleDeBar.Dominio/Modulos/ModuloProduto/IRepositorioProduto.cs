using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloProduto;

public interface IRepositorioProduto : IRepositorio<Produto>
{
    bool PossuiPedidoVinculado(Guid idProduto);
}
