using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Doktor varlığının EF Core eşleme yapılandırması.</summary>
public class DoktorConfiguration : IEntityTypeConfiguration<Doktor>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Doktor> builder)
    {
        builder.ToTable("Doktorlar");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Ad).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Soyad).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Unvan).HasMaxLength(50);
    }
}
