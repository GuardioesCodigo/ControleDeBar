using ControleDeBar.Dominio.Modulos.ModuloConta;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable("TBConta");

        builder.HasKey(c => c.Id)
            .HasName("PK_TBConta");

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.NomeCliente)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Situacao)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(c => c.Mesa)
            .WithMany()
            .HasForeignKey(c => c.MesaId)
            .HasConstraintName("FK_TBConta_TBMesa")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Garcom)
            .WithMany()
            .HasForeignKey(c => c.GarcomId)
            .HasConstraintName("FK_TBConta_TBGarcom")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Pedidos)
            .WithOne()
            .HasForeignKey(p => p.ContaId)
            .HasConstraintName("FK_TBPedido_TBConta")
            .OnDelete(DeleteBehavior.Cascade);

        // Não mapeado: calculado em memória a partir dos Pedidos.
        builder.Ignore(c => c.ValorTotal);
    }
}
