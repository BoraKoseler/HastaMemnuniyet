using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket daveti varlığının EF Core eşleme yapılandırması.</summary>
public class AnketDavetiConfiguration : IEntityTypeConfiguration<AnketDaveti>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AnketDaveti> builder)
    {
        builder.ToTable("AnketDavetleri");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Token).IsRequired().HasMaxLength(128);
        builder.Property(d => d.TelefonHash).HasMaxLength(128);
        builder.Property(d => d.OlusturanKullaniciId).HasMaxLength(450);
        builder.Property(d => d.GonderimKanali).HasConversion<int>();
        builder.Property(d => d.Durum).HasConversion<int>();
        builder.HasIndex(d => d.Token).IsUnique();
        builder.HasIndex(d => d.TelefonHash);
        builder.HasIndex(d => d.Durum);

        builder.HasOne(d => d.Hastane)
            .WithMany()
            .HasForeignKey(d => d.HastaneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Birim)
            .WithMany()
            .HasForeignKey(d => d.BirimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Doktor)
            .WithMany()
            .HasForeignKey(d => d.DoktorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Yanitlar)
            .WithOne(y => y.Davet)
            .HasForeignKey(y => y.DavetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.SmsGonderimKayitlari)
            .WithOne(s => s.Davet)
            .HasForeignKey(s => s.DavetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
