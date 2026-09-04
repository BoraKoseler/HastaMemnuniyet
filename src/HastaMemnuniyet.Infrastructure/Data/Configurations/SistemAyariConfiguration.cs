using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Sistem ayarı varlığının EF Core eşleme yapılandırması.</summary>
public class SistemAyariConfiguration : IEntityTypeConfiguration<SistemAyari>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SistemAyari> builder)
    {
        builder.ToTable("SistemAyarlari");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Anahtar).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Deger).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.Aciklama).HasMaxLength(500);
        builder.Property(a => a.Kategori).HasMaxLength(100);
        builder.HasIndex(a => a.Anahtar).IsUnique();
    }
}
