using HastaMemnuniyet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaMemnuniyet.Infrastructure.Data.Configurations;

/// <summary>SMS gönderim kaydı varlığının EF Core eşleme yapılandırması.</summary>
public class SmsGonderimKaydiConfiguration : IEntityTypeConfiguration<SmsGonderimKaydi>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SmsGonderimKaydi> builder)
    {
        builder.ToTable("SmsGonderimKayitlari");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.TelefonHash).HasMaxLength(128);
        builder.Property(s => s.SmsYaniti).HasMaxLength(1000);
        builder.Property(s => s.SmsDurumu).HasConversion<int>();
    }
}
