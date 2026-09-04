using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Kullanıcı-birim kapsamı varlığının EF Core eşleme yapılandırması.</summary>
public class KullaniciBirimKapsamiConfiguration : IEntityTypeConfiguration<KullaniciBirimKapsami>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<KullaniciBirimKapsami> builder)
    {
        builder.ToTable("KullaniciBirimKapsamlari");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.KullaniciId).IsRequired();
        builder.HasIndex(k => new { k.KullaniciId, k.BirimId }).IsUnique();

        builder.HasOne(k => k.Birim)
            .WithMany(b => b.KullaniciKapsamlari)
            .HasForeignKey(k => k.BirimId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
