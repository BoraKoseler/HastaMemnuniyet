using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>Anket yanıtı varlığının EF Core eşleme yapılandırması.</summary>
public class AnketYanitiConfiguration : IEntityTypeConfiguration<AnketYaniti>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AnketYaniti> builder)
    {
        builder.ToTable("AnketYanitlari");
        builder.HasKey(y => y.Id);
        builder.Property(y => y.IpAdresi).HasMaxLength(64);
        builder.Property(y => y.KullaniciAjan).HasMaxLength(512);
        builder.HasIndex(y => y.AnketId);
        builder.HasIndex(y => y.HastaneId);
        builder.HasIndex(y => y.TamamlanmaTarihi);

        builder.HasMany(y => y.Cevaplar)
            .WithOne(c => c.Yanit)
            .HasForeignKey(c => c.YanitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
