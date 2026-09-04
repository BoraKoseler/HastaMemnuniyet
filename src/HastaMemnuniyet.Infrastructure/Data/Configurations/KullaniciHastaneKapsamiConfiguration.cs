using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Kullanıcı-hastane kapsamı varlığının EF Core eşleme yapılandırması.</summary>
public class KullaniciHastaneKapsamiConfiguration : IEntityTypeConfiguration<KullaniciHastaneKapsami>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<KullaniciHastaneKapsami> builder)
    {
        builder.ToTable("KullaniciHastaneKapsamlari");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.KullaniciId).IsRequired();
        builder.HasIndex(k => new { k.KullaniciId, k.HastaneId }).IsUnique();

        builder.HasOne(k => k.Hastane)
            .WithMany(h => h.KullaniciKapsamlari)
            .HasForeignKey(k => k.HastaneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
