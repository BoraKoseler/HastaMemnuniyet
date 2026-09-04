using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>QR anket kampanyası varlığının EF Core eşleme yapılandırması.</summary>
public class QrAnketKampanyasiConfiguration : IEntityTypeConfiguration<QrAnketKampanyasi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QrAnketKampanyasi> builder)
    {
        builder.ToTable("QrAnketKampanyalari");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Ad).HasMaxLength(200).IsRequired();
        builder.HasIndex(k => k.HastaneId);
        builder.HasIndex(k => k.AktifMi);

        builder.HasOne(k => k.Anket)
            .WithMany()
            .HasForeignKey(k => k.AnketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Hastane)
            .WithMany()
            .HasForeignKey(k => k.HastaneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Birim)
            .WithMany()
            .HasForeignKey(k => k.BirimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Doktor)
            .WithMany()
            .HasForeignKey(k => k.DoktorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
