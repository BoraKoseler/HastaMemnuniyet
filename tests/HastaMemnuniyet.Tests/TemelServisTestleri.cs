using System.Linq.Expressions;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Application.Services;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Services;
using Moq;

namespace HastaMemnuniyet.Tests;

/// <summary>
/// Faz 3 kapsamında eklenen temel birim (unit) testler.
/// Servislerin çekirdek iş kurallarını mock/InMemory bağımlılıklarla doğrular.
/// </summary>
public class TemelServisTestleri
{
    /// <summary>
    /// DavetServisi: Geçerli bir token verildiğinde davetin bulunması,
    /// bilinmeyen token verildiğinde ise null dönmesi (token geçerlilik kontrolü).
    /// </summary>
    [Fact]
    public async Task DavetServisi_TokenGecerlilikKontrolu_DogruSonucVermeli()
    {
        // Arrange
        const string gecerliToken = "gecerli-token-123";
        var davet = new AnketDaveti { Id = 1, AnketId = 1, HastaneId = 1, Token = gecerliToken };

        var davetRepo = new Mock<IAnketDavetiRepository>();
        davetRepo.Setup(r => r.GetByTokenAsync(gecerliToken)).ReturnsAsync(davet);
        davetRepo.Setup(r => r.GetByTokenAsync(It.Is<string>(t => t != gecerliToken)))
                 .ReturnsAsync((AnketDaveti?)null);

        var servis = new DavetServisi(
            davetRepo.Object,
            Mock.Of<IAnketRepository>(),
            Mock.Of<ITokenUretici>(),
            Mock.Of<ISmsSender>(),
            Mock.Of<ISistemAyariRepository>(),
            Mock.Of<IGenericRepository<SmsGonderimKaydi>>());

        // Act
        var gecerliSonuc = await servis.TokenIleGetirAsync(gecerliToken);
        var gecersizSonuc = await servis.TokenIleGetirAsync("bilinmeyen-token");

        // Assert
        Assert.NotNull(gecerliSonuc);
        Assert.Equal(gecerliToken, gecerliSonuc!.Token);
        Assert.Null(gecersizSonuc);
    }

    /// <summary>
    /// DashboardServisi: Yanıt oranının (geçerli yanıt / davet * 100) doğru hesaplanması.
    /// 10 davet ve 5 geçerli yanıt için oranın %50 olması beklenir.
    /// </summary>
    [Fact]
    public async Task DashboardServisi_YanitOrani_DogruHesaplanmali()
    {
        // Arrange
        var simdi = DateTime.UtcNow;
        var tarih = simdi.AddDays(-5);

        var davetler = Enumerable.Range(1, 10)
            .Select(i => new AnketDaveti { Id = i, AnketId = 1, HastaneId = 1, OlusturulmaTarihi = tarih })
            .ToList();

        var yanitlar = Enumerable.Range(1, 5)
            .Select(i => new AnketYaniti
            {
                Id = i,
                AnketId = 1,
                HastaneId = 1,
                GecerliMi = true,
                TamamlanmaTarihi = tarih
            })
            .ToList();

        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(new List<Anket> { new() { Id = 1, Ad = "Test", AktifMi = true } });

        var yanitRepo = new Mock<IAnketYanitiRepository>();
        yanitRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(yanitlar);

        var hastaneRepo = new Mock<IHastaneRepository>();
        hastaneRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Hastane>());

        var birimRepo = new Mock<IBirimRepository>();
        birimRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Birim>());

        var doktorRepo = new Mock<IDoktorRepository>();
        doktorRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Doktor>());

        var davetRepo = new Mock<IGenericRepository<AnketDaveti>>();
        davetRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(davetler);

        var cevapRepo = new Mock<IGenericRepository<AnketCevabi>>();
        cevapRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnketCevabi>());

        var ayarRepo = new Mock<ISistemAyariRepository>();
        ayarRepo.Setup(r => r.DegerGetirAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("30");

        var kritikServisi = new Mock<IKritikGeriBildirimServisi>();
        kritikServisi.Setup(s => s.SayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);

        var aksiyonServisi = new Mock<IAksiyonServisi>();
        aksiyonServisi.Setup(s => s.AcikSayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);
        aksiyonServisi.Setup(s => s.GecikmisSayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);

        var servis = new DashboardServisi(
            anketRepo.Object, yanitRepo.Object, hastaneRepo.Object, birimRepo.Object,
            doktorRepo.Object, davetRepo.Object, cevapRepo.Object, ayarRepo.Object,
            kritikServisi.Object, aksiyonServisi.Object);

        // Act
        var sonuc = await servis.IstatistikleriGetirAsync(simdi.AddDays(-10), simdi.AddDays(1));

        // Assert
        Assert.Equal(10, sonuc.ToplamDavetSayisi);
        Assert.Equal(5, sonuc.ToplamYanitSayisi);
        Assert.Equal(50, sonuc.YanitOrani);
    }

    /// <summary>
    /// TelefonHashleyici: Aynı girdinin her zaman aynı hash değerini üretmesi (tutarlılık).
    /// </summary>
    [Fact]
    public void TelefonHashleyici_AyniGirdi_AyniHashUretmeli()
    {
        // Arrange
        const string numara = "0555 123 45 67";

        // Act
        var hash1 = TelefonHashleyici.Hashle(numara);
        var hash2 = TelefonHashleyici.Hashle(numara);

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.False(string.IsNullOrWhiteSpace(hash1));
    }

    /// <summary>
    /// GuidTokenUretici: Üst üste üretilen çok sayıda token'ın tamamının benzersiz olması.
    /// </summary>
    [Fact]
    public void GuidTokenUretici_UretilenTokenler_BenzersizOlmali()
    {
        // Arrange
        var uretici = new GuidTokenUretici();
        const int adet = 1000;

        // Act
        var tokenler = Enumerable.Range(0, adet)
            .Select(_ => uretici.BenzersizTokenUret())
            .ToList();

        // Assert
        Assert.Equal(adet, tokenler.Distinct().Count());
    }

    /// <summary>
    /// KritikGeriBildirimMotoru: Puan altı (PuanAlti) kuralı için, eşik değerine eşit ya da
    /// altındaki puanların kritik geri bildirim üretmesi; eşiğin üzerindeki puanların üretmemesi.
    /// </summary>
    [Fact]
    public async Task KritikGeriBildirimMotoru_PuanAltiKurali_EsikAltiPuaniKritikYapmali()
    {
        // Arrange
        var kural = new KritikGeriBildirimKurali
        {
            Id = 1,
            KuralTipi = KuralTipi.PuanAlti,
            EsikDegeri = 2,
            AktifMi = true,
            AnketId = null,
            SoruId = null
        };

        var kuralRepo = new Mock<IGenericRepository<KritikGeriBildirimKurali>>();
        kuralRepo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(new List<KritikGeriBildirimKurali> { kural });

        var motor = new KritikGeriBildirimMotoru(kuralRepo.Object);

        var yanit = new AnketYaniti { Id = 10, AnketId = 1, HastaneId = 1, BirimId = 3 };
        var cevaplar = new List<AnketCevabi>
        {
            new() { Id = 1, SoruId = 1, PuanDegeri = 1 }, // eşik (2) altında -> kritik
            new() { Id = 2, SoruId = 2, PuanDegeri = 2 }, // eşiğe eşit -> kritik
            new() { Id = 3, SoruId = 3, PuanDegeri = 5 }  // eşik üstü -> kritik değil
        };

        // Act
        var sonuclar = await motor.DegerlendiAsync(yanit, cevaplar);

        // Assert
        Assert.Equal(2, sonuclar.Count);
        Assert.All(sonuclar, k => Assert.Equal(yanit.Id, k.YanitId));
        Assert.All(sonuclar, k => Assert.Equal(1, k.KuralId));
    }

    /// <summary>
    /// KapsamFiltresi: Birim bazlı kısıtlama tanımlandığında, yalnızca kapsamdaki hastane ve
    /// birime ait kayıtların erişilebilir sayılması (BirimYoneticisi veri kapsamı davranışı).
    /// </summary>
    [Fact]
    public void KapsamFiltresi_BirimKisiti_YalnizcaKapsamdakiKayitlariKapsamali()
    {
        // Arrange
        var kapsam = new KapsamFiltresi
        {
            HastaneIdleri = new List<int> { 1 },
            BirimIdleri = new List<int> { 3 }
        };

        // Act & Assert
        Assert.True(kapsam.BirimKisitiVar);
        Assert.True(kapsam.Kapsiyor(1, 3));      // kapsamdaki hastane + birim
        Assert.False(kapsam.Kapsiyor(1, 4));     // kapsamdaki hastane, kapsam dışı birim
        Assert.False(kapsam.Kapsiyor(2, 3));     // kapsam dışı hastane
        Assert.False(kapsam.Kapsiyor(1, null));  // birim kısıtı varken birimsiz kayıt
    }

    /// <summary>
    /// DavetServisi: Tekrar gönderim engeli. Aynı numaraya, minimum gönderim aralığı içinde
    /// zaten bir davet gönderilmişse yeni davet oluşturma girişiminin hata fırlatması beklenir.
    /// </summary>
    [Fact]
    public async Task DavetServisi_TekrarGonderimEngeli_AralikIcindeHataFirlatmali()
    {
        // Arrange
        var dto = new AnketDavetiOlusturDto
        {
            AnketId = 1,
            HastaneId = 1,
            TelefonNumarasi = "0555 123 45 67",
            GonderimKanali = GonderimKanali.Sms
        };

        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(new Anket { Id = 1, Ad = "Test Anketi", AktifMi = true });

        var davetRepo = new Mock<IAnketDavetiRepository>();
        // Minimum gönderim aralığı içinde önceki bir davet bulunuyor.
        davetRepo.Setup(r => r.BulAsync(It.IsAny<Expression<Func<AnketDaveti, bool>>>()))
                 .ReturnsAsync(new List<AnketDaveti> { new() { Id = 99, AnketId = 1 } });

        var ayarRepo = new Mock<ISistemAyariRepository>();
        ayarRepo.Setup(r => r.DegerGetirAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("30");

        var servis = new DavetServisi(
            davetRepo.Object,
            anketRepo.Object,
            Mock.Of<ITokenUretici>(),
            Mock.Of<ISmsSender>(),
            ayarRepo.Object,
            Mock.Of<IGenericRepository<SmsGonderimKaydi>>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => servis.OlusturAsync(dto, "kullanici-1"));

        // Tekrar gönderim engellendiği için yeni davet kaydedilmemelidir.
        davetRepo.Verify(r => r.AddAsync(It.IsAny<AnketDaveti>()), Times.Never);
    }

    /// <summary>
    /// AnketServisi: Sürüm oluşturma. Yeni sürümün sürüm numarasının bir artması, aktif olması ve
    /// kaynak (eski) sürümün pasifleştirilmesi (AktifMi = false) beklenir.
    /// </summary>
    [Fact]
    public async Task AnketServisi_SurumOlustur_YeniSurumArtmaliVeEskisiPasiflesmeli()
    {
        // Arrange
        var kaynak = new Anket
        {
            Id = 5,
            Ad = "Poliklinik Anketi",
            AktifMi = true,
            SurumNo = 1
        };
        kaynak.Sorular.Add(new AnketSorusu { Id = 51, SoruMetni = "Soru 1", SiraNo = 1, AktifMi = true });
        kaynak.Sorular.Add(new AnketSorusu { Id = 52, SoruMetni = "Soru 2", SiraNo = 2, AktifMi = true });

        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.SorulariylaGetirAsync(5)).ReturnsAsync(kaynak);

        var servis = new AnketServisi(
            anketRepo.Object,
            Mock.Of<IGenericRepository<AnketSorusu>>(),
            Mock.Of<IGenericRepository<AnketSoruSecenegi>>());

        // Act
        var yeniSurum = await servis.SurumOlusturAsync(5);

        // Assert
        Assert.Equal(2, yeniSurum.SurumNo);
        Assert.True(yeniSurum.AktifMi);
        Assert.False(kaynak.AktifMi); // eski sürüm pasifleştirildi
        anketRepo.Verify(r => r.AddAsync(It.IsAny<Anket>()), Times.Once);
    }

    /// <summary>
    /// Faz 6 - DashboardServisi.NpsHesaplaAsync: NPS skorunun doğru hesaplanması.
    /// 5 destekçi (9-10), 2 pasif (7-8) ve 3 kötüleyen (0-6) için
    /// NPS = %50 destekçi - %30 kötüleyen = 20 beklenir. Ayrıca NPS puanları (0-10)
    /// genel ortalama puanı (1-5) etkilememelidir.
    /// </summary>
    [Fact]
    public async Task DashboardServisi_NpsHesapla_DogruSkorVermeliVeOrtalamayiEtkilememeli()
    {
        // Arrange
        var simdi = DateTime.UtcNow;
        var tarih = simdi.AddDays(-3);

        // Soru 1: Puanlama (1-5), Soru 2: NPS (0-10)
        var anket = new Anket { Id = 1, Ad = "Test", AktifMi = true };
        anket.Sorular.Add(new AnketSorusu { Id = 1, SoruMetni = "Memnuniyet", SoruTipi = SoruTipi.Puanlama, AktifMi = true });
        anket.Sorular.Add(new AnketSorusu { Id = 2, SoruMetni = "Tavsiye", SoruTipi = SoruTipi.Nps, AktifMi = true });

        var npsPuanlari = new[] { 10, 10, 9, 9, 9, 8, 7, 6, 3, 0 }; // 5 destekçi, 2 pasif, 3 kötüleyen
        var yanitlar = new List<AnketYaniti>();
        var cevaplar = new List<AnketCevabi>();
        var cevapId = 1;
        for (var i = 0; i < npsPuanlari.Length; i++)
        {
            var yanitId = i + 1;
            yanitlar.Add(new AnketYaniti
            {
                Id = yanitId,
                AnketId = 1,
                HastaneId = 1,
                GecerliMi = true,
                TamamlanmaTarihi = tarih
            });
            // Puanlama cevabı (sabit 4 puan -> genel ortalama 4.00 olmalı)
            cevaplar.Add(new AnketCevabi { Id = cevapId++, YanitId = yanitId, SoruId = 1, PuanDegeri = 4 });
            // NPS cevabı
            cevaplar.Add(new AnketCevabi { Id = cevapId++, YanitId = yanitId, SoruId = 2, PuanDegeri = npsPuanlari[i] });
        }

        var servis = DashboardServisiOlustur(anket, yanitlar, cevaplar, new List<AnketDaveti>());

        // Act
        var nps = await servis.NpsHesaplaAsync(simdi.AddDays(-10), simdi.AddDays(1));
        var istatistik = await servis.IstatistikleriGetirAsync(simdi.AddDays(-10), simdi.AddDays(1));

        // Assert
        Assert.Equal(10, nps.ToplamYanit);
        Assert.Equal(5, nps.DestekciSayisi);
        Assert.Equal(2, nps.PasifSayisi);
        Assert.Equal(3, nps.KotuleyenSayisi);
        Assert.Equal(20, nps.NpsSkoru); // %50 - %30 = 20
        Assert.True(nps.VeriVarMi);
        // NPS puanları (0-10) genel ortalamayı (yalnızca puanlama, 4.00) bozmamalı
        Assert.Equal(4.00, istatistik.OrtalamaPuan);
    }

    /// <summary>
    /// Faz 6 - MetinAnaliziServisi: Açık uçlu yanıtlardan en sık geçen anahtar kelimelerin
    /// çıkarılması. Türkçe etkisiz kelimeler (ve, çok, için) ve çok kısa kelimeler elenmelidir.
    /// </summary>
    [Fact]
    public async Task MetinAnaliziServisi_AnahtarKelimeler_EnSikKelimeyiBulmaliVeEtkisizleriElemeli()
    {
        // Arrange
        var simdi = DateTime.UtcNow;
        var tarih = simdi.AddDays(-2);

        var anket = new Anket { Id = 1, Ad = "Test", AktifMi = true };
        anket.Sorular.Add(new AnketSorusu { Id = 5, SoruMetni = "Görüş", SoruTipi = SoruTipi.AcikUclu, AktifMi = true });

        var yorumlar = new[]
        {
            "Doktor çok güler yüzlü",
            "Doktor ve hemşire nazik",
            "Doktor gerçekten başarılı"
        };

        var yanitlar = new List<AnketYaniti>();
        var cevaplar = new List<AnketCevabi>();
        for (var i = 0; i < yorumlar.Length; i++)
        {
            var yanitId = i + 1;
            yanitlar.Add(new AnketYaniti { Id = yanitId, AnketId = 1, HastaneId = 1, GecerliMi = true, TamamlanmaTarihi = tarih });
            cevaplar.Add(new AnketCevabi { Id = yanitId, YanitId = yanitId, SoruId = 5, MetinDegeri = yorumlar[i] });
        }

        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Anket> { anket });
        var yanitRepo = new Mock<IAnketYanitiRepository>();
        yanitRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(yanitlar);
        var cevapRepo = new Mock<IGenericRepository<AnketCevabi>>();
        cevapRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cevaplar);
        var ayarRepo = new Mock<ISistemAyariRepository>();
        ayarRepo.Setup(r => r.DegerGetirAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("30");

        var servis = new MetinAnaliziServisi(anketRepo.Object, yanitRepo.Object, cevapRepo.Object, ayarRepo.Object);

        // Act
        var sonuc = await servis.AnahtarKelimeleriGetirAsync(simdi.AddDays(-10), simdi.AddDays(1));

        // Assert
        Assert.True(sonuc.VeriVarMi);
        Assert.Equal(3, sonuc.ToplamYorum);
        // "doktor" 3 kez geçtiği için en sık kelime olmalı
        Assert.Equal("doktor", sonuc.EnSikKelimeler[0].Kelime);
        Assert.Equal(3, sonuc.EnSikKelimeler[0].Sayi);
        // Etkisiz kelimeler elenmiş olmalı
        Assert.DoesNotContain(sonuc.EnSikKelimeler, k => k.Kelime == "çok");
        Assert.DoesNotContain(sonuc.EnSikKelimeler, k => k.Kelime == "ve");
    }

    /// <summary>
    /// Faz 6 - DashboardServisi.DonemselKarsilastirAsync: İki dönem arasındaki ortalama puan
    /// yüzdesel değişiminin doğru hesaplanması. Önceki 4.00, güncel 4.40 için %10 artış beklenir.
    /// </summary>
    [Fact]
    public async Task DashboardServisi_DonemselKarsilastir_YuzdeselDegisimiDogruHesaplamali()
    {
        // Arrange
        var anket = new Anket { Id = 1, Ad = "Test", AktifMi = true };
        anket.Sorular.Add(new AnketSorusu { Id = 1, SoruMetni = "Memnuniyet", SoruTipi = SoruTipi.Puanlama, AktifMi = true });

        var oncekiTarih = new DateTime(2026, 1, 15);
        var guncelTarih = new DateTime(2026, 2, 15);

        var yanitlar = new List<AnketYaniti>
        {
            new() { Id = 1, AnketId = 1, HastaneId = 1, GecerliMi = true, TamamlanmaTarihi = oncekiTarih },
            new() { Id = 2, AnketId = 1, HastaneId = 1, GecerliMi = true, TamamlanmaTarihi = guncelTarih }
        };
        var cevaplar = new List<AnketCevabi>
        {
            new() { Id = 1, YanitId = 1, SoruId = 1, PuanDegeri = 4 }, // önceki ortalama 4.00
            new() { Id = 2, YanitId = 2, SoruId = 1, PuanDegeri = 4 },
            new() { Id = 3, YanitId = 2, SoruId = 1, PuanDegeri = 5 }  // güncel ortalama (4+5)/2 = 4.50
        };

        var servis = DashboardServisiOlustur(anket, yanitlar, cevaplar, new List<AnketDaveti>());

        // Act
        var sonuc = await servis.DonemselKarsilastirAsync(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31),
            new DateTime(2026, 2, 1), new DateTime(2026, 2, 28));

        // Assert
        Assert.Equal(4.00, sonuc.OncekiOrtalamaPuan);
        Assert.Equal(4.50, sonuc.GuncelOrtalamaPuan);
        Assert.Equal(12.5, sonuc.OrtalamaPuanDegisimYuzdesi); // (4.5-4.0)/4.0*100 = 12.5
        Assert.Equal(1, sonuc.OncekiYanitSayisi);
        Assert.Equal(1, sonuc.GuncelYanitSayisi);
    }

    /// <summary>
    /// Faz 6 - DoktorKarnesiServisi: Bir doktorun soru bazlı ortalamalarından güçlü ve zayıf
    /// alanların doğru belirlenmesi. En yüksek ortalamalı soru güçlü, en düşük ortalamalı soru
    /// zayıf alan olmalıdır.
    /// </summary>
    [Fact]
    public async Task DoktorKarnesiServisi_GucluZayifAlanlar_DogruBelirlenmeli()
    {
        // Arrange
        var simdi = DateTime.UtcNow;
        var tarih = simdi.AddDays(-3);
        const int doktorId = 7;

        var anket = new Anket { Id = 1, Ad = "Test", AktifMi = true };
        anket.Sorular.Add(new AnketSorusu { Id = 1, SoruMetni = "İletişim", SoruTipi = SoruTipi.Puanlama, AktifMi = true });
        anket.Sorular.Add(new AnketSorusu { Id = 2, SoruMetni = "Bekleme Süresi", SoruTipi = SoruTipi.Puanlama, AktifMi = true });

        var yanitlar = new List<AnketYaniti>();
        var cevaplar = new List<AnketCevabi>();
        var cevapId = 1;
        for (var i = 0; i < 5; i++)
        {
            var yanitId = i + 1;
            yanitlar.Add(new AnketYaniti { Id = yanitId, AnketId = 1, HastaneId = 1, DoktorId = doktorId, GecerliMi = true, TamamlanmaTarihi = tarih });
            cevaplar.Add(new AnketCevabi { Id = cevapId++, YanitId = yanitId, SoruId = 1, PuanDegeri = 5 }); // İletişim güçlü
            cevaplar.Add(new AnketCevabi { Id = cevapId++, YanitId = yanitId, SoruId = 2, PuanDegeri = 2 }); // Bekleme zayıf
        }

        var doktor = new Doktor { Id = doktorId, Ad = "Ayşe", Soyad = "Yılmaz", Unvan = "Dr.", AktifMi = true };

        var doktorRepo = new Mock<IDoktorRepository>();
        doktorRepo.Setup(r => r.BirimleriyleGetirAsync(doktorId)).ReturnsAsync(doktor);
        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Anket> { anket });
        var yanitRepo = new Mock<IAnketYanitiRepository>();
        yanitRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(yanitlar);
        var cevapRepo = new Mock<IGenericRepository<AnketCevabi>>();
        cevapRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cevaplar);
        var birimRepo = new Mock<IBirimRepository>();
        birimRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Birim>());
        var hastaneRepo = new Mock<IHastaneRepository>();
        hastaneRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Hastane>());
        var ayarRepo = new Mock<ISistemAyariRepository>();
        ayarRepo.Setup(r => r.DegerGetirAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("5");

        var servis = new DoktorKarnesiServisi(
            doktorRepo.Object, anketRepo.Object, yanitRepo.Object, cevapRepo.Object,
            birimRepo.Object, hastaneRepo.Object, ayarRepo.Object);

        // Act
        var karne = await servis.KarneGetirAsync(doktorId, simdi.AddDays(-10), simdi.AddDays(1));

        // Assert
        Assert.NotNull(karne);
        Assert.True(karne!.VeriVarMi);
        Assert.Equal(5, karne.ToplamYanit);
        Assert.Equal(3.50, karne.OrtalamaPuan); // (5+2)/2 = 3.50
        Assert.False(karne.DusukOrneklemUyarisi); // 5 >= eşik(5)
        Assert.Equal("İletişim", karne.GucluAlanlar[0].SoruMetni);
        Assert.Equal(5.00, karne.GucluAlanlar[0].Ortalama);
        Assert.Equal("Bekleme Süresi", karne.ZayifAlanlar[0].SoruMetni);
        Assert.Equal(2.00, karne.ZayifAlanlar[0].Ortalama);
    }

    /// <summary>
    /// Testlerde tekrarı azaltmak için standart mock bağımlılıklarıyla bir <see cref="DashboardServisi"/> örneği kurar.
    /// </summary>
    /// <param name="anket">Soruları içeren anket.</param>
    /// <param name="yanitlar">Anket yanıtları.</param>
    /// <param name="cevaplar">Anket cevapları.</param>
    /// <param name="davetler">Anket davetleri.</param>
    /// <returns>Yapılandırılmış DashboardServisi örneği.</returns>
    private static DashboardServisi DashboardServisiOlustur(
        Anket anket,
        List<AnketYaniti> yanitlar,
        List<AnketCevabi> cevaplar,
        List<AnketDaveti> davetler)
    {
        var anketRepo = new Mock<IAnketRepository>();
        anketRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Anket> { anket });

        var yanitRepo = new Mock<IAnketYanitiRepository>();
        yanitRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(yanitlar);

        var hastaneRepo = new Mock<IHastaneRepository>();
        hastaneRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Hastane>());

        var birimRepo = new Mock<IBirimRepository>();
        birimRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Birim>());

        var doktorRepo = new Mock<IDoktorRepository>();
        doktorRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Doktor>());

        var davetGenericRepo = new Mock<IGenericRepository<AnketDaveti>>();
        davetGenericRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(davetler);

        var cevapRepo = new Mock<IGenericRepository<AnketCevabi>>();
        cevapRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cevaplar);

        var ayarRepo = new Mock<ISistemAyariRepository>();
        ayarRepo.Setup(r => r.DegerGetirAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("30");

        var kritikServisi = new Mock<IKritikGeriBildirimServisi>();
        kritikServisi.Setup(s => s.SayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);

        var aksiyonServisi = new Mock<IAksiyonServisi>();
        aksiyonServisi.Setup(s => s.AcikSayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);
        aksiyonServisi.Setup(s => s.GecikmisSayiGetirAsync(It.IsAny<KapsamFiltresi?>())).ReturnsAsync(0);

        return new DashboardServisi(
            anketRepo.Object, yanitRepo.Object, hastaneRepo.Object, birimRepo.Object,
            doktorRepo.Object, davetGenericRepo.Object, cevapRepo.Object, ayarRepo.Object,
            kritikServisi.Object, aksiyonServisi.Object);
    }
}
