using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloPedido;

public sealed class RepositorioPedidoEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Pedido>(dbContext), IRepositorioPedido
{
    public override Pedido? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(p => p.Produto)
            .SingleOrDefault(p => p.Id == idSelecionado);
    }

    public List<Pedido> SelecionarPorContaId(Guid contaId)
    {
        return registros
            .Include(p => p.Produto)
            .Where(p => p.ContaId == contaId)
            .ToList();
    }
}
