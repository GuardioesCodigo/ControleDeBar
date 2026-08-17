using System.Reflection;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Compartilhado.Orm;

public sealed class ControleDeBarDbContext(
    DbContextOptions<ControleDeBarDbContext> options,
    IProvedorDeUsuario? provedorDeUsuario = null
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Estabelecimento> Estabelecimentos => Set<Estabelecimento>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Garcom> Garcons => Set<Garcom>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Assembly assembly = typeof(ControleDeBarDbContext).Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        // Query Filters globais: cada estabelecimento só enxerga seus próprios dados.
        // Devem usar a dependência do provedorDeUsuario diretamente — o EF faz cache
        // do OnModelCreating e variáveis locais não seriam reavaliadas por requisição.
        if (provedorDeUsuario != null)
        {
            modelBuilder.Entity<Estabelecimento>()
                .HasQueryFilter(e => e.UserId == provedorDeUsuario.Id);

            modelBuilder.Entity<Mesa>()
                .HasQueryFilter(m => m.UserId == provedorDeUsuario.Id);

            modelBuilder.Entity<Garcom>()
                .HasQueryFilter(g => g.UserId == provedorDeUsuario.Id);

            modelBuilder.Entity<Produto>()
                .HasQueryFilter(p => p.UserId == provedorDeUsuario.Id);

            modelBuilder.Entity<Conta>()
                .HasQueryFilter(c => c.UserId == provedorDeUsuario.Id);

            modelBuilder.Entity<Pedido>()
                .HasQueryFilter(p => p.UserId == provedorDeUsuario.Id);
        }
    }

    public override int SaveChanges()
    {
        Guid? userId = provedorDeUsuario?.Id;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Não é possível salvar dados do estabelecimento sem estar autenticado."
            );
        }

        foreach (var entry in ChangeTracker.Entries<IEntidadeDoUsuario>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.UserId == Guid.Empty)
                    {
                        entry.Property(nameof(IEntidadeDoUsuario.UserId)).CurrentValue = userId.Value;
                    }
                    else if (entry.Entity.UserId != userId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de criar um registro para outro estabelecimento."
                        );
                    }

                    break;

                case EntityState.Modified:
                    Guid userIdOriginal = entry
                        .Property(nameof(IEntidadeDoUsuario.UserId))
                        .OriginalValue is Guid original
                        ? original
                        : Guid.Empty;

                    if (userIdOriginal != userId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de modificar um registro de outro estabelecimento."
                        );
                    }

                    break;

                case EntityState.Deleted:
                    Guid userIdOriginalExclusao = entry
                        .Property(nameof(IEntidadeDoUsuario.UserId))
                        .OriginalValue is Guid originalExclusao
                        ? originalExclusao
                        : Guid.Empty;

                    if (userIdOriginalExclusao != userId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de excluir um registro de outro estabelecimento."
                        );
                    }

                    break;
            }
        }

        return base.SaveChanges();
    }
}
