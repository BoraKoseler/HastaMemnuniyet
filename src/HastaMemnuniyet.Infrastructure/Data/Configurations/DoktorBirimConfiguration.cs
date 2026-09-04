using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Doktor-birim ilişki varlığının EF Core eşleme yapılandırması.</summary>
public class DoktorBirimConfiguration : IEntityTypeConfiguration<DoktorBirim>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DoktorBirim> builder)
    {
        builder.ToTable("DoktorBirimleri");
        builder.HasKey(db => new { db.DoktorId, db.BirimId });

        builder.HasOne(db => db.Doktor)
            .WithMany(d => d.DoktorBirimleri)
            .HasForeignKey(db => db.DoktorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(db => db.Birim)
            .WithMany(b => b.DoktorBirimleri)
            .HasForeignKey(db => db.BirimId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
