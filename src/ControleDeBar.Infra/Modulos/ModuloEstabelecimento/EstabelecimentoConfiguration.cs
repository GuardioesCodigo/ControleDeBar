using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloEstabelecimento;

public sealed class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
{
    public void Configure(EntityTypeBuilder<Estabelecimento> builder)
    {
        builder.ToTable("TBEstabelecimento");

        builder.HasKey(e => e.Id)
            .HasName("PK_TBEstabelecimento");

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Endereco)
            .HasMaxLength(200);
    }
}
