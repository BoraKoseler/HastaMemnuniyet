using HastaMemnuniyet.Application;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HastaMemnuniyet.Infrastructure.Data.Seed;

/// <summary>
/// Uygulamanın ilk çalıştırılmasında gerekli başlangıç verilerini (rol, kullanıcı, hastane,
/// birim, doktor, örnek anket ve sistem ayarları) oluşturan yardımcı sınıf.
/// </summary>
public static class SeedData
{
    /// <summary>Örnek yönetici kullanıcının e-postası.</summary>
    public const string AdminEposta = "admin@hastamemnuniyet.com";

    /// <summary>Örnek yönetici kullanıcının parolası.</summary>
    private const string AdminParola = "Admin123!";

    /// <summary>Örnek kalite birimi kullanıcısının e-postası.</summary>
    public const string KaliteEposta = "kalite@hastamemnuniyet.com";

    /// <summary>Örnek kalite birimi kullanıcısının parolası.</summary>
    private const string KaliteParola = "Kalite123!";

    /// <summary>Örnek birim yöneticisi kullanıcısının e-postası.</summary>
    public const string BirimYoneticisiEposta = "birimyoneticisi@hastamemnuniyet.com";

    /// <summary>Örnek birim yöneticisi kullanıcısının parolası.</summary>
    private const string BirimYoneticisiParola = "Birim123!";

    /// <summary>Örnek üst yönetim kullanıcısının e-postası.</summary>
    public const string UstYonetimEposta = "ustyonetim@hastamemnuniyet.com";

    /// <summary>Örnek üst yönetim kullanıcısının parolası.</summary>
    private const string UstYonetimParola = "Yonetim123!";

    /// <summary>
    /// Başlangıç verilerini oluşturur. Veriler zaten mevcutsa yeniden eklemez (idempotent).
    /// </summary>
    /// <param name="serviceProvider">Bağımlılık çözümleme için servis sağlayıcı.</param>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var kullaniciYoneticisi = serviceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();
        var rolYoneticisi = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await RolleriOlusturAsync(rolYoneticisi);
        await KullanicilariOlusturAsync(kullaniciYoneticisi);
        await KurumsalVerileriOlusturAsync(context);
        await OrnekAnketiOlusturAsync(context);
        await SistemAyarlariniOlusturAsync(context);
        await BirimYoneticisiKapsaminiOlusturAsync(context, kullaniciYoneticisi);
        await OrnekKritikKuraliOlusturAsync(context);
    }

    private static async Task RolleriOlusturAsync(RoleManager<IdentityRole> rolYoneticisi)
    {
        foreach (var rol in RolSabitleri.TumRoller)
        {
            if (!await rolYoneticisi.RoleExistsAsync(rol))
            {
                await rolYoneticisi.CreateAsync(new IdentityRole(rol));
            }
        }
    }

    private static async Task KullanicilariOlusturAsync(UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        await KullaniciEkleAsync(kullaniciYoneticisi, AdminEposta, AdminParola, "Sistem", "Yöneticisi", RolSabitleri.Admin);
        await KullaniciEkleAsync(kullaniciYoneticisi, KaliteEposta, KaliteParola, "Kalite", "Sorumlusu", RolSabitleri.KaliteBirimi);
        await KullaniciEkleAsync(kullaniciYoneticisi, BirimYoneticisiEposta, BirimYoneticisiParola, "Birim", "Yöneticisi", RolSabitleri.BirimYoneticisi);
        await KullaniciEkleAsync(kullaniciYoneticisi, UstYonetimEposta, UstYonetimParola, "Üst", "Yönetim", RolSabitleri.UstYonetim);
    }

    private static async Task KullaniciEkleAsync(
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi,
        string eposta,
        string parola,
        string ad,
        string soyad,
        string rol)
    {
        if (await kullaniciYoneticisi.FindByEmailAsync(eposta) is not null)
            return;

        var kullanici = new UygulamaKullanicisi
        {
            UserName = eposta,
            Email = eposta,
            Ad = ad,
            Soyad = soyad,
            AktifMi = true,
            EmailConfirmed = true
        };

        var sonuc = await kullaniciYoneticisi.CreateAsync(kullanici, parola);
        if (sonuc.Succeeded)
        {
            await kullaniciYoneticisi.AddToRoleAsync(kullanici, rol);
        }
    }

    private static async Task KurumsalVerileriOlusturAsync(AppDbContext context)
    {
        if (await context.Hastaneler.AnyAsync())
            return;

        var birimAdlari = new[] { "Dahiliye", "Kardiyoloji", "Ortopedi" };
        var hastaneTanimlari = new[]
        {
            ("Ankara Şehir Hastanesi", "ANK-01"),
            ("İstanbul Üniversite Hastanesi", "IST-01")
        };

        var doktorAdlari = new[]
        {
            ("Ahmet", "Yılmaz"), ("Ayşe", "Demir"),
            ("Mehmet", "Kaya"), ("Fatma", "Şahin"),
            ("Ali", "Çelik"), ("Zeynep", "Aydın")
        };

        foreach (var (hastaneAdi, hastaneKodu) in hastaneTanimlari)
        {
            var hastane = new Hastane
            {
                Ad = hastaneAdi,
                Kod = hastaneKodu,
                Adres = $"{hastaneAdi} Kampüsü",
                Telefon = "0312 000 00 00",
                AktifMi = true
            };

            var doktorSayaci = 0;
            for (var i = 0; i < birimAdlari.Length; i++)
            {
                var birim = new Birim
                {
                    Ad = birimAdlari[i],
                    Kod = $"{hastaneKodu}-{birimAdlari[i][..3].ToUpperInvariant()}",
                    AktifMi = true
                };

                for (var j = 0; j < 2; j++)
                {
                    var (dAd, dSoyad) = doktorAdlari[doktorSayaci % doktorAdlari.Length];
                    doktorSayaci++;
                    var doktor = new Doktor
                    {
                        Ad = dAd,
                        Soyad = dSoyad,
                        Unvan = "Uzm. Dr.",
                        AktifMi = true
                    };
                    // İlişkiyi her iki uçtan da bağlayarak EF'in doktoru grafikte keşfetmesini sağlarız.
                    var doktorBirim = new DoktorBirim { Doktor = doktor, Birim = birim, AktifMi = true };
                    doktor.DoktorBirimleri.Add(doktorBirim);
                    birim.DoktorBirimleri.Add(doktorBirim);
                }

                hastane.Birimler.Add(birim);
            }

            await context.Hastaneler.AddAsync(hastane);
        }

        await context.SaveChangesAsync();
    }

    private static async Task OrnekAnketiOlusturAsync(AppDbContext context)
    {
        if (await context.Anketler.AnyAsync())
            return;

        var anket = new Anket
        {
            Ad = "Ayaktan Hasta Memnuniyet Anketi",
            Aciklama = "Ayaktan (poliklinik) hizmeti alan hastaların memnuniyetini ölçen anket.",
            AnketTuru = AnketTuru.Ayaktan,
            AktifMi = true,
            SurumNo = 1,
            TahminiSureDakika = 3,
            GizlilikMetni = "Verdiğiniz yanıtlar gizli tutulur ve yalnızca hizmet kalitesini " +
                            "iyileştirmek amacıyla anonim olarak değerlendirilir."
        };

        var sorular = new List<AnketSorusu>
        {
            YeniPuanlamaSorusu("Hastaneye giriş ve kayıt işlemlerinden ne kadar memnun kaldınız?", 1, "Kayıt"),
            YeniPuanlamaSorusu("Doktorunuzun ilgi ve bilgilendirmesinden ne kadar memnun kaldınız?", 2, "Doktor"),
            YeniPuanlamaSorusu("Hemşire ve sağlık personelinin ilgisinden ne kadar memnun kaldınız?", 3, "Personel"),
            YeniPuanlamaSorusu("Hastanenin temizlik ve hijyeninden ne kadar memnun kaldınız?", 4, "Ortam"),
            YeniPuanlamaSorusu("Bekleme sürenizden ne kadar memnun kaldınız?", 5, "Süreç"),
            YeniEvetHayirSorusu("Hastanemizi yakınlarınıza tavsiye eder misiniz?", 6, "Genel"),
            YeniNpsSorusu("Hastanemizi bir arkadaşınıza ya da yakınınıza tavsiye etme olasılığınız nedir? (0-10)", 7, "Genel"),
            YeniAcikUcluSorusu("Hizmetlerimizi geliştirmemiz için önerileriniz nelerdir?", 8, "Genel")
        };

        foreach (var soru in sorular)
        {
            anket.Sorular.Add(soru);
        }

        await context.Anketler.AddAsync(anket);
        await context.SaveChangesAsync();
    }

    private static AnketSorusu YeniPuanlamaSorusu(string metin, int sira, string kategori) => new()
    {
        SoruMetni = metin,
        SoruTipi = SoruTipi.Puanlama,
        SiraNo = sira,
        ZorunluMu = true,
        AktifMi = true,
        Kategori = kategori,
        PuanlamaAltSinir = 1,
        PuanlamaUstSinir = 5
    };

    private static AnketSorusu YeniEvetHayirSorusu(string metin, int sira, string kategori) => new()
    {
        SoruMetni = metin,
        SoruTipi = SoruTipi.EvetHayir,
        SiraNo = sira,
        ZorunluMu = true,
        AktifMi = true,
        Kategori = kategori
    };

    private static AnketSorusu YeniNpsSorusu(string metin, int sira, string kategori) => new()
    {
        SoruMetni = metin,
        SoruTipi = SoruTipi.Nps,
        SiraNo = sira,
        ZorunluMu = true,
        AktifMi = true,
        Kategori = kategori,
        PuanlamaAltSinir = 0,
        PuanlamaUstSinir = 10
    };

    private static AnketSorusu YeniAcikUcluSorusu(string metin, int sira, string kategori) => new()
    {
        SoruMetni = metin,
        SoruTipi = SoruTipi.AcikUclu,
        SiraNo = sira,
        ZorunluMu = false,
        AktifMi = true,
        Kategori = kategori,
        MaksimumKarakterSayisi = 1000
    };

    private static async Task SistemAyarlariniOlusturAsync(AppDbContext context)
    {
        if (await context.SistemAyarlari.AnyAsync())
            return;

        var ayarlar = new List<SistemAyari>
        {
            YeniAyar(AyarAnahtarlari.DavetGecerlilikSaati, "72", "Anket bağlantısının geçerlilik süresi (saat).", "Davet"),
            YeniAyar(AyarAnahtarlari.MinimumGonderimAraligi, "30", "Aynı telefona minimum gönderim aralığı (gün).", "Davet"),
            YeniAyar(AyarAnahtarlari.DusukOrneklemEsigi, "5", "Düşük örneklem uyarı eşiği.", "Raporlama"),
            YeniAyar(AyarAnahtarlari.AcikUcluKarakterLimiti, "1000", "Açık uçlu cevaplar için karakter limiti.", "Anket"),
            YeniAyar(AyarAnahtarlari.DashboardVarsayilanGunSayisi, "30", "Dashboard varsayılan gösterim gün sayısı.", "Dashboard"),
            YeniAyar(AyarAnahtarlari.MaksimumHatirlatmaSayisi, "2", "Bir davet için gönderilebilecek en fazla hatırlatma sayısı.", "Davet"),
            YeniAyar(AyarAnahtarlari.KisiselVeriSaklamaSuresiGun, "730", "Kişisel verilerin (ör. telefon) saklanma süresi (gün).", "KVKK"),
            YeniAyar(AyarAnahtarlari.AnketYanitiSaklamaSuresiGun, "1825", "Anket yanıtlarının saklanma süresi (gün).", "KVKK"),
            YeniAyar(AyarAnahtarlari.OtomatikAnonimlestir, "false", "Süresi dolan kayıtların otomatik anonimleştirilmesi.", "KVKK")
        };

        await context.SistemAyarlari.AddRangeAsync(ayarlar);
        await context.SaveChangesAsync();
    }

    private static SistemAyari YeniAyar(string anahtar, string deger, string aciklama, string kategori) => new()
    {
        Anahtar = anahtar,
        Deger = deger,
        Aciklama = aciklama,
        Kategori = kategori,
        GuncellenmeTarihi = DateTime.UtcNow
    };

    /// <summary>
    /// Örnek birim yöneticisi kullanıcısı için hastane ve birim kapsamı oluşturur (idempotent).
    /// Ankara Şehir Hastanesi ve onun Dahiliye birimi kapsam olarak atanır.
    /// </summary>
    private static async Task BirimYoneticisiKapsaminiOlusturAsync(
        AppDbContext context,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        var kullanici = await kullaniciYoneticisi.FindByEmailAsync(BirimYoneticisiEposta);
        if (kullanici is null)
            return;

        // Kapsam zaten atanmışsa tekrar ekleme
        if (await context.Set<KullaniciHastaneKapsami>().AnyAsync(k => k.KullaniciId == kullanici.Id))
            return;

        var hastane = await context.Hastaneler
            .Include(h => h.Birimler)
            .FirstOrDefaultAsync(h => h.Ad == "Ankara Şehir Hastanesi");
        if (hastane is null)
            return;

        var birim = hastane.Birimler.FirstOrDefault(b => b.Ad == "Dahiliye");

        await context.Set<KullaniciHastaneKapsami>().AddAsync(new KullaniciHastaneKapsami
        {
            KullaniciId = kullanici.Id,
            HastaneId = hastane.Id
        });

        if (birim is not null)
        {
            await context.Set<KullaniciBirimKapsami>().AddAsync(new KullaniciBirimKapsami
            {
                KullaniciId = kullanici.Id,
                BirimId = birim.Id
            });
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Örnek bir kritik geri bildirim kuralı oluşturur (idempotent).
    /// Genel puanı 2 ve altında olan yanıtlar kritik olarak işaretlenir.
    /// </summary>
    private static async Task OrnekKritikKuraliOlusturAsync(AppDbContext context)
    {
        if (await context.KritikGeriBildirimKurallari.AnyAsync())
            return;

        var kural = new KritikGeriBildirimKurali
        {
            AnketId = null,
            SoruId = null,
            KuralTipi = KuralTipi.PuanAlti,
            EsikDegeri = 2,
            AktifMi = true
        };

        await context.KritikGeriBildirimKurallari.AddAsync(kural);
        await context.SaveChangesAsync();
    }
}
