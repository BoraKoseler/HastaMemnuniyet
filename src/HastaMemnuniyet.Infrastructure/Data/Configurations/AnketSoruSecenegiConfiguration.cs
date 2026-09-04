using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket soru seçeneği varlığının EF Core eşleme yapılandırması.</summary>
public class AnketSoruSecenegiConfiguration : IEntityTypeConfiguration<AnketSoruSecenegi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AnketSoruSecenegi> builder)
    {
        builder.ToTable("AnketSoruSecenekleri");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.MetinDegeri).IsRequired().HasMaxLength(300);
    }
}
