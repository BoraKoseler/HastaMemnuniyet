using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Aksiyon geçmişi varlığının EF Core eşleme yapılandırması.</summary>
public class AksiyonGecmisiConfiguration : IEntityTypeConfiguration<AksiyonGecmisi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AksiyonGecmisi> builder)
    {
        builder.ToTable("AksiyonGecmisleri");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.EskiDurum).HasConversion<int>();
        builder.Property(g => g.YeniDurum).HasConversion<int>();
        builder.Property(g => g.Aciklama).HasMaxLength(1000);
        builder.Property(g => g.KullaniciId).HasMaxLength(450).IsRequired();
        builder.HasIndex(g => g.AksiyonId);
    }
}
