using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket sorusu varlığının EF Core eşleme yapılandırması.</summary>
public class AnketSorusuConfiguration : IEntityTypeConfiguration<AnketSorusu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AnketSorusu> builder)
    {
        builder.ToTable("AnketSorulari");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SoruMetni).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Kategori).HasMaxLength(100);
        builder.Property(s => s.SoruTipi).HasConversion<int>();
        builder.Property(s => s.KosulDegeri).HasMaxLength(200);
        builder.HasIndex(s => new { s.AnketId, s.SiraNo });

        builder.HasMany(s => s.Secenekler)
            .WithOne(o => o.Soru)
            .HasForeignKey(o => o.SoruId)
            .OnDelete(DeleteBehavior.Cascade);

        // Koşullu görünürlük için soru kendi içinde başka bir soruya bağlanabilir (öz-referans).
        // Silme davranışı, döngü/çoklu yol hatalarını önlemek için kısıtlanır.
        builder.HasOne(s => s.KosulBagliSoru)
            .WithMany()
            .HasForeignKey(s => s.KosulBagliSoruId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
