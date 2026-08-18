using ControleDeBar.Dominio.Modulos.ModuloPedido;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloPedido;

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("TBPedido");

        builder.HasKey(p => p.Id)
            .HasName("PK_TBPedido");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Quantidade)
            .IsRequired();

        builder.HasOne(p => p.Produto)
            .WithMany()
            .HasForeignKey(p => p.ProdutoId)
            .HasConstraintName("FK_TBPedido_TBProduto")
            .OnDelete(DeleteBehavior.Restrict);

        // Não mapeado: calculado em memória a partir do preço do produto.
        builder.Ignore(p => p.Subtotal);
    }
}
