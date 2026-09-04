using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket varlığının EF Core eşleme yapılandırması.</summary>
public class AnketConfiguration : IEntityTypeConfiguration<Anket>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Anket> builder)
    {
        builder.ToTable("Anketler");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Ad).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Aciklama).HasMaxLength(1000);
        builder.Property(a => a.GizlilikMetni).HasMaxLength(4000);
        builder.Property(a => a.AnketTuru).HasConversion<int>();

        builder.HasMany(a => a.Sorular)
            .WithOne(s => s.Anket)
            .HasForeignKey(s => s.AnketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Davetler)
            .WithOne(d => d.Anket)
            .HasForeignKey(d => d.AnketId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
