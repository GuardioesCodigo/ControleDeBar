using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class RepositorioContaEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Conta>(dbContext), IRepositorioConta
{
    public override Conta? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Include(c => c.Pedidos)
                .ThenInclude(p => p.Produto)
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public override List<Conta> SelecionarTodos()
    {
        return registros
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Include(c => c.Pedidos)
            .ToList();
    }

    public List<Conta> SelecionarAbertas()
    {
        return registros
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Include(c => c.Pedidos)
            .Where(c => c.Situacao == SituacaoConta.Aberta)
            .ToList();
    }

    public List<Conta> SelecionarFechadasPorData(DateOnly data)
    {
        DateTime inicio = data.ToDateTime(TimeOnly.MinValue);
        DateTime fim = data.ToDateTime(TimeOnly.MaxValue);

        return registros
            .Include(c => c.Pedidos)
                .ThenInclude(p => p.Produto)
            .Where(c =>
                c.Situacao == SituacaoConta.Fechada &&
                c.DataAbertura >= inicio &&
                c.DataAbertura <= fim)
            .ToList();
    }

    public bool MesaPossuiContaAberta(Guid idMesa, Guid? idContaIgnorada = null)
    {
        return registros.Any(c =>
            c.MesaId == idMesa &&
            c.Situacao == SituacaoConta.Aberta &&
            c.Id != idContaIgnorada);
    }

    public bool GarcomPossuiContaAberta(Guid idGarcom)
    {
        return registros.Any(c =>
            c.GarcomId == idGarcom &&
            c.Situacao == SituacaoConta.Aberta);
    }
}
