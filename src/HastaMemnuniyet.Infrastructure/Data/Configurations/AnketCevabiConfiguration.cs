using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket cevabı varlığının EF Core eşleme yapılandırması.</summary>
public class AnketCevabiConfiguration : IEntityTypeConfiguration<AnketCevabi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AnketCevabi> builder)
    {
        builder.ToTable("AnketCevaplari");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.MetinDegeri).HasMaxLength(4000);
        builder.Property(c => c.SeciliSecenekIdleri).HasMaxLength(500);
        builder.HasIndex(c => c.SoruId);

        builder.HasOne(c => c.Soru)
            .WithMany()
            .HasForeignKey(c => c.SoruId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Secenek)
            .WithMany()
            .HasForeignKey(c => c.SecenekId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
