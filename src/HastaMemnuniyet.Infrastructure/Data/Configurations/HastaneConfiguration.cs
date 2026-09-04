using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Hastane varlığının EF Core eşleme yapılandırması.</summary>
public class HastaneConfiguration : IEntityTypeConfiguration<Hastane>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Hastane> builder)
    {
        builder.ToTable("Hastaneler");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Ad).IsRequired().HasMaxLength(200);
        builder.Property(h => h.Kod).IsRequired().HasMaxLength(50);
        builder.Property(h => h.Adres).HasMaxLength(500);
        builder.Property(h => h.Telefon).HasMaxLength(30);
        builder.HasIndex(h => h.Kod).IsUnique();

        builder.HasMany(h => h.Birimler)
            .WithOne(b => b.Hastane)
            .HasForeignKey(b => b.HastaneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
