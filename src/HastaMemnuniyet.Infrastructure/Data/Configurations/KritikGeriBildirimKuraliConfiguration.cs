using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Kritik geri bildirim kuralı varlığının EF Core eşleme yapılandırması.</summary>
public class KritikGeriBildirimKuraliConfiguration : IEntityTypeConfiguration<KritikGeriBildirimKurali>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<KritikGeriBildirimKurali> builder)
    {
        builder.ToTable("KritikGeriBildirimKurallari");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.KuralTipi).HasConversion<int>();
        builder.Property(k => k.AnahtarKelimeler).HasMaxLength(500);
        builder.HasIndex(k => k.AnketId);
        builder.HasIndex(k => k.AktifMi);

        builder.HasOne(k => k.Anket)
            .WithMany()
            .HasForeignKey(k => k.AnketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Soru)
            .WithMany()
            .HasForeignKey(k => k.SoruId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
