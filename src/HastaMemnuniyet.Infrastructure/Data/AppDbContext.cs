using System.Reflection;
using HastaMemnuniyet.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Data;

/// <summary>
/// Uygulamanın EF Core veritabanı bağlamı. ASP.NET Core Identity tablolarını da içerir.
/// </summary>
public class AppDbContext : IdentityDbContext<UygulamaKullanicisi>
{
    /// <summary>Yeni bir <see cref="AppDbContext"/> örneği oluşturur.</summary>
    /// <param name="options">Bağlam yapılandırma seçenekleri.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Hastane kayıtları.</summary>
    public DbSet<Hastane> Hastaneler => Set<Hastane>();

    /// <summary>Birim kayıtları.</summary>
    public DbSet<Birim> Birimler => Set<Birim>();

    /// <summary>Doktor kayıtları.</summary>
    public DbSet<Doktor> Doktorlar => Set<Doktor>();

    /// <summary>Doktor-birim ilişkileri.</summary>
    public DbSet<DoktorBirim> DoktorBirimleri => Set<DoktorBirim>();

    /// <summary>Kullanıcı-hastane kapsamları.</summary>
    public DbSet<KullaniciHastaneKapsami> KullaniciHastaneKapsamlari => Set<KullaniciHastaneKapsami>();

    /// <summary>Kullanıcı-birim kapsamları.</summary>
    public DbSet<KullaniciBirimKapsami> KullaniciBirimKapsamlari => Set<KullaniciBirimKapsami>();

    /// <summary>Anket kayıtları.</summary>
    public DbSet<Anket> Anketler => Set<Anket>();

    /// <summary>Anket soruları.</summary>
    public DbSet<AnketSorusu> AnketSorulari => Set<AnketSorusu>();

    /// <summary>Anket soru seçenekleri.</summary>
    public DbSet<AnketSoruSecenegi> AnketSoruSecenekleri => Set<AnketSoruSecenegi>();

    /// <summary>Anket davetleri.</summary>
    public DbSet<AnketDaveti> AnketDavetleri => Set<AnketDaveti>();

    /// <summary>Anket yanıtları.</summary>
    public DbSet<AnketYaniti> AnketYanitlari => Set<AnketYaniti>();

    /// <summary>Anket cevapları.</summary>
    public DbSet<AnketCevabi> AnketCevaplari => Set<AnketCevabi>();

    /// <summary>SMS gönderim kayıtları.</summary>
    public DbSet<SmsGonderimKaydi> SmsGonderimKayitlari => Set<SmsGonderimKaydi>();

    /// <summary>Sistem ayarları.</summary>
    public DbSet<SistemAyari> SistemAyarlari => Set<SistemAyari>();

    /// <summary>Denetim kayıtları.</summary>
    public DbSet<DenetimKaydi> DenetimKayitlari => Set<DenetimKaydi>();

    /// <summary>Kritik geri bildirim kuralları.</summary>
    public DbSet<KritikGeriBildirimKurali> KritikGeriBildirimKurallari => Set<KritikGeriBildirimKurali>();

    /// <summary>Kritik geri bildirim kayıtları.</summary>
    public DbSet<KritikGeriBildirim> KritikGeriBildirimler => Set<KritikGeriBildirim>();

    /// <summary>İyileştirme aksiyonları.</summary>
    public DbSet<IyilestirmeAksiyonu> IyilestirmeAksiyonlari => Set<IyilestirmeAksiyonu>();

    /// <summary>İyileştirme aksiyonu geçmiş kayıtları.</summary>
    public DbSet<AksiyonGecmisi> AksiyonGecmisleri => Set<AksiyonGecmisi>();

    /// <summary>QR anket kampanyaları.</summary>
    public DbSet<QrAnketKampanyasi> QrAnketKampanyalari => Set<QrAnketKampanyasi>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
