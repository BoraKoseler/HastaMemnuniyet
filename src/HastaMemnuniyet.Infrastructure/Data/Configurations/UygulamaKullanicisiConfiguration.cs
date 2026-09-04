using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Uygulama kullanıcısı varlığının EF Core eşleme yapılandırması.</summary>
public class UygulamaKullanicisiConfiguration : IEntityTypeConfiguration<UygulamaKullanicisi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UygulamaKullanicisi> builder)
    {
        builder.Property(k => k.Ad).IsRequired().HasMaxLength(100);
        builder.Property(k => k.Soyad).IsRequired().HasMaxLength(100);

        builder.HasMany(k => k.HastaneKapsamlari)
            .WithOne(h => h.Kullanici)
            .HasForeignKey(h => h.KullaniciId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(k => k.BirimKapsamlari)
            .WithOne(b => b.Kullanici)
            .HasForeignKey(b => b.KullaniciId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
