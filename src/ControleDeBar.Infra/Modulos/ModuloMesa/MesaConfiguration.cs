using ControleDeBar.Dominio.Modulos.ModuloMesa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MesaConfiguration : IEntityTypeConfiguration<Mesa>
{
    public void Configure(EntityTypeBuilder<Mesa> builder)
    {
        builder.ToTable("TBMesa");

        builder.HasKey(m => m.Id)
            .HasName("PK_TBMesa");

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(m => m.Estabelecimento)
            .WithMany()
            .HasForeignKey(m => m.EstabelecimentoId)
            .HasConstraintName("FK_TBMesa_TBEstabelecimento")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.EstabelecimentoId, m.Numero })
            .IsUnique()
            .HasDatabaseName("UQ_TBMesa_EstabelecimentoId_Numero");
    }
}