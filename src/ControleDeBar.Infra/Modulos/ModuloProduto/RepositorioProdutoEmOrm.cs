using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloProduto;

public sealed class RepositorioProdutoEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Produto>(dbContext), IRepositorioProduto
{
    public bool PossuiPedidoVinculado(Guid idProduto)
    {
        return dbContext.Set<Pedido>().Any(p => p.ProdutoId == idProduto);
    }
}
