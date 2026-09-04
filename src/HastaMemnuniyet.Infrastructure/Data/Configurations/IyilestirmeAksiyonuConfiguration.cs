using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>İyileştirme aksiyonu varlığının EF Core eşleme yapılandırması.</summary>
public class IyilestirmeAksiyonuConfiguration : IEntityTypeConfiguration<IyilestirmeAksiyonu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IyilestirmeAksiyonu> builder)
    {
        builder.ToTable("IyilestirmeAksiyonlari");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Baslik).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Aciklama).HasMaxLength(2000).IsRequired();
        builder.Property(a => a.KapanisNotu).HasMaxLength(2000);
        builder.Property(a => a.SorumluKullaniciId).HasMaxLength(450).IsRequired();
        builder.Property(a => a.OlusturanKullaniciId).HasMaxLength(450).IsRequired();
        builder.Property(a => a.Oncelik).HasConversion<int>();
        builder.Property(a => a.Durum).HasConversion<int>();
        builder.HasIndex(a => a.HastaneId);
        builder.HasIndex(a => a.Durum);
        builder.HasIndex(a => a.SorumluKullaniciId);

        builder.HasOne(a => a.SorumluKullanici)
            .WithMany()
            .HasForeignKey(a => a.SorumluKullaniciId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Gecmis)
            .WithOne(g => g.Aksiyon)
            .HasForeignKey(g => g.AksiyonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
