using HastaMemnuniyet.Application.Services;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.ValueObjects;
using HastaMemnuniyet.Infrastructure.Data;
using HastaMemnuniyet.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Tests;

/// <summary>
/// Faz 1 çekirdek altyapısının temel doğrulama (smoke) testleri.
/// Bu testler; DbContext modelinin kurulabildiğini, kayıtların eklenip
/// okunabildiğini ve yardımcı servislerin beklenen davranışı sergilediğini doğrular.
/// </summary>
public class AltyapiDoganlamaTestleri
{
    /// <summary>
    /// Bellek içi sağlayıcı kullanarak yeni bir <see cref="AppDbContext"/> örneği üretir.
    /// </summary>
    private static AppDbContext BellekIciBaglamOlustur()
    {
        var secenekler = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(secenekler);
    }

    /// <summary>
    /// DbContext modelinin sorunsuz kurulabildiğini ve bir hastanenin
    /// kaydedilip geri okunabildiğini doğrular.
    /// </summary>
    [Fact]
    public async Task DbContext_HastaneKaydedipOkuyabilmeli()
    {
        await using var baglam = BellekIciBaglamOlustur();

        var hastane = new Hastane
        {
            Ad = "Test Şehir Hastanesi",
            Kod = "TST-01",
            AktifMi = true
        };

        await baglam.Hastaneler.AddAsync(hastane);
        await baglam.SaveChangesAsync();

        var okunan = await baglam.Hastaneler.SingleAsync(h => h.Kod == "TST-01");

        Assert.Equal("Test Şehir Hastanesi", okunan.Ad);
        Assert.True(okunan.Id > 0);
    }

    /// <summary>
    /// Telefon hash'leyicinin aynı numara için tutarlı, farklı numaralar için
    /// farklı ve boş girdi için boş değer ürettiğini doğrular.
    /// </summary>
    [Fact]
    public void TelefonHashleyici_TutarliVeGeriDondurulemezOlmali()
    {
        var hash1 = TelefonHashleyici.Hashle("0555 111 22 33");
        var hash2 = TelefonHashleyici.Hashle("05551112233");
        var hashFarkli = TelefonHashleyici.Hashle("0555 000 00 00");

        Assert.Equal(hash1, hash2);
        Assert.NotEqual(hash1, hashFarkli);
        Assert.NotEqual("05551112233", hash1);
        Assert.Equal(string.Empty, TelefonHashleyici.Hashle(null));
    }

    /// <summary>
    /// Token üreticinin boş olmayan ve birbirinden farklı (benzersiz) token'lar
    /// ürettiğini doğrular.
    /// </summary>
    [Fact]
    public void TokenUretici_BenzersizTokenUretmeli()
    {
        var uretici = new GuidTokenUretici();

        var token1 = uretici.BenzersizTokenUret();
        var token2 = uretici.BenzersizTokenUret();

        Assert.False(string.IsNullOrWhiteSpace(token1));
        Assert.NotEqual(token1, token2);
    }

    /// <summary>
    /// SMS gönderim sonucu değer nesnesinin fabrika metotlarının doğru
    /// durum bilgisini ürettiğini doğrular.
    /// </summary>
    [Fact]
    public void SmsGonderimSonucu_FabrikaMetotlariDogruDurumUretmeli()
    {
        var basarili = SmsGonderimSonucu.BasariliOlustur("Gönderildi", "REF-1");
        var basarisiz = SmsGonderimSonucu.BasarisizOlustur("Sağlayıcı hatası");

        Assert.True(basarili.Basarili);
        Assert.Equal("REF-1", basarili.SaglayiciReferansi);
        Assert.False(basarisiz.Basarili);
        Assert.Equal("Sağlayıcı hatası", basarisiz.Mesaj);
    }

    /// <summary>
    /// Enum değerlerinin (int) sözleşmeye uygun biçimde 1'den başladığını doğrular;
    /// bu, veritabanında saklanacak sayısal değerlerin kararlılığını güvence altına alır.
    /// </summary>
    [Fact]
    public void Enumlar_BeklenenSayisalDegerlereSahipOlmali()
    {
        Assert.Equal(1, (int)AnketTuru.Ayaktan);
        Assert.Equal(1, (int)SoruTipi.Puanlama);
        Assert.Equal(1, (int)AksiyonDurumu.Acik);
    }
}
