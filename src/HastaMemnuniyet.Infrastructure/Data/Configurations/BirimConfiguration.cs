using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Birim varlığının EF Core eşleme yapılandırması.</summary>
public class BirimConfiguration : IEntityTypeConfiguration<Birim>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Birim> builder)
    {
        builder.ToTable("Birimler");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Ad).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Kod).HasMaxLength(50);
        builder.HasIndex(b => new { b.HastaneId, b.Ad });
    }
}
