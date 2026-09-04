using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Denetim kaydı varlığının EF Core eşleme yapılandırması.</summary>
public class DenetimKaydiConfiguration : IEntityTypeConfiguration<DenetimKaydi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DenetimKaydi> builder)
    {
        builder.ToTable("DenetimKayitlari");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Islem).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Tablo).HasMaxLength(100);
        builder.Property(d => d.KayitId).HasMaxLength(100);
        builder.Property(d => d.KullaniciId).HasMaxLength(450);
        builder.Property(d => d.KullaniciAdi).HasMaxLength(256);
        builder.Property(d => d.IpAdresi).HasMaxLength(64);
        builder.HasIndex(d => d.Tarih);
    }
}
