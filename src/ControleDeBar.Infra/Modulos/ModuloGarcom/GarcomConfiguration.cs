using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloGarcom;

public sealed class GarcomConfiguration : IEntityTypeConfiguration<Garcom>
{
    public void Configure(EntityTypeBuilder<Garcom> builder)
    {
        builder.ToTable("TBGarcom");

        builder.HasKey(g => g.Id)
            .HasName("PK_TBGarcom");

        builder.Property(g => g.Id)
            .ValueGeneratedNever();

        builder.Property(g => g.Nome)
            .HasMaxLength(100)
            .IsRequired();
    }
}
