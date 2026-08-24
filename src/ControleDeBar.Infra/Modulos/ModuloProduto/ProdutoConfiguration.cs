using ControleDeBar.Dominio.Modulos.ModuloProduto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloProduto;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("TBProduto");

        builder.HasKey(p => p.Id)
            .HasName("PK_TBProduto");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Preco)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
    }
}
