using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Kritik geri bildirim varlığının EF Core eşleme yapılandırması.</summary>
public class KritikGeriBildirimConfiguration : IEntityTypeConfiguration<KritikGeriBildirim>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<KritikGeriBildirim> builder)
    {
        builder.ToTable("KritikGeriBildirimler");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Aciklama).HasMaxLength(1000).IsRequired();
        builder.HasIndex(k => k.HastaneId);
        builder.HasIndex(k => k.YanitId);
        builder.HasIndex(k => k.OlusturulmaTarihi);

        builder.HasOne(k => k.Yanit)
            .WithMany()
            .HasForeignKey(k => k.YanitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Cevap)
            .WithMany()
            .HasForeignKey(k => k.CevapId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Kural)
            .WithMany()
            .HasForeignKey(k => k.KuralId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(k => k.Aksiyonlar)
            .WithOne(a => a.KritikGeriBildirim)
            .HasForeignKey(a => a.KritikGeriBildirimId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
