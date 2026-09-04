using System.Globalization;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Bir doktorun soru bazlı ortalamalarını, genel ortalamasını, NPS'ini, güçlü/zayıf alanlarını
/// ve önceki döneme göre trendini hesaplayarak performans karnesi üreten servis.
/// </summary>
public class DoktorKarnesiServisi : IDoktorKarnesiServisi
{
    /// <summary>Ayar okunamazsa kullanılacak varsayılan gün sayısı.</summary>
    private const int VarsayilanGunSayisi = 30;

    /// <summary>Ayar okunamazsa kullanılacak varsayılan düşük örneklem eşiği.</summary>
    private const int VarsayilanMinimumOrneklem = 5;

    /// <summary>Güçlü/zayıf alanlarda listelenecek en fazla soru sayısı.</summary>
    private const int OneCikanAlanSayisi = 3;

    private readonly IDoktorRepository _doktorRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IGenericRepository<AnketCevabi> _cevapRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly ISistemAyariRepository _sistemAyariRepository;

    /// <summary>Yeni bir <see cref="DoktorKarnesiServisi"/> örneği oluşturur.</summary>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="cevapRepository">Cevap veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı veri erişim bileşeni.</param>
    public DoktorKarnesiServisi(
        IDoktorRepository doktorRepository,
        IAnketRepository anketRepository,
        IAnketYanitiRepository yanitRepository,
        IGenericRepository<AnketCevabi> cevapRepository,
        IBirimRepository birimRepository,
        IHastaneRepository hastaneRepository,
        ISistemAyariRepository sistemAyariRepository)
    {
        _doktorRepository = doktorRepository;
        _anketRepository = anketRepository;
        _yanitRepository = yanitRepository;
        _cevapRepository = cevapRepository;
        _birimRepository = birimRepository;
        _hastaneRepository = hastaneRepository;
        _sistemAyariRepository = sistemAyariRepository;
    }

    /// <inheritdoc />
    public async Task<DoktorKarnesiDto?> KarneGetirAsync(int doktorId, DateTime? baslangic = null, DateTime? bitis = null)
    {
        var doktor = await _doktorRepository.BirimleriyleGetirAsync(doktorId);
        if (doktor is null)
            return null;

        var gunSayisi = await GunSayisiGetirAsync();
        var minimumOrneklem = await MinimumOrneklemGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var anketler = await _anketRepository.GetAllAsync();
        var sorular = anketler.SelectMany(a => a.Sorular).ToList();
        var puanlamaSoruIdleri = sorular.Where(s => s.SoruTipi == SoruTipi.Puanlama).Select(s => s.Id).ToHashSet();
        var npsSoruIdleri = sorular.Where(s => s.SoruTipi == SoruTipi.Nps).Select(s => s.Id).ToHashSet();
        var soruBilgileri = sorular
            .GroupBy(s => s.Id)
            .ToDictionary(g => g.Key, g => g.First());

        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();

        var karne = new DoktorKarnesiDto
        {
            DoktorId = doktor.Id,
            DoktorAdSoyad = DoktorGorunenAd(doktor),
            Unvan = doktor.Unvan,
            Birimler = await BirimAdlariGetirAsync(doktor),
            BaslangicTarihi = baslangicTarihi,
            BitisTarihi = bitisTarihi,
            MinimumOrneklem = minimumOrneklem
        };

        // Güncel dönem yanıtları
        var guncelYanitIdleri = yanitlar
            .Where(y => y.GecerliMi && y.DoktorId == doktorId && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi)
            .Select(y => y.Id)
            .ToHashSet();

        karne.ToplamYanit = guncelYanitIdleri.Count;
        if (karne.ToplamYanit == 0)
            return karne;

        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && puanlamaSoruIdleri.Contains(c.SoruId) && guncelYanitIdleri.Contains(c.YanitId))
            .ToList();

        karne.OrtalamaPuan = puanliCevaplar.Count == 0
            ? 0
            : Math.Round(puanliCevaplar.Average(c => c.PuanDegeri!.Value), 2);

        // Soru bazlı ortalamalar
        karne.SoruBazliOrtalamalar = puanliCevaplar
            .GroupBy(c => c.SoruId)
            .Select(g => new SoruOrtalamaDto
            {
                SoruId = g.Key,
                SoruMetni = soruBilgileri.TryGetValue(g.Key, out var soru) ? soru.SoruMetni : "Bilinmeyen soru",
                Kategori = soruBilgileri.TryGetValue(g.Key, out var s2) ? s2.Kategori : null,
                Ortalama = Math.Round(g.Average(c => c.PuanDegeri!.Value), 2),
                YanitSayisi = g.Count()
            })
            .OrderByDescending(s => s.Ortalama)
            .ThenBy(s => s.SoruMetni, StringComparer.Ordinal)
            .ToList();

        // Güçlü ve zayıf alanlar: az sayıda soruda güçlü listesi tüm soruları kapıp
        // zayıf listesini boş bırakmasın diye her iki tarafın boyutu soru sayısının
        // yarısıyla sınırlanır (en az 1). Böylece iki soruda biri güçlü, diğeri zayıf olur.
        var alanSayisi = Math.Max(1, Math.Min(OneCikanAlanSayisi, karne.SoruBazliOrtalamalar.Count / 2));
        karne.GucluAlanlar = karne.SoruBazliOrtalamalar.Take(alanSayisi).ToList();
        karne.ZayifAlanlar = karne.SoruBazliOrtalamalar
            .OrderBy(s => s.Ortalama)
            .ThenBy(s => s.SoruMetni, StringComparer.Ordinal)
            .Where(z => !karne.GucluAlanlar.Any(g => g.SoruId == z.SoruId))
            .Take(alanSayisi)
            .ToList();

        // NPS
        var npsPuanlari = cevaplar
            .Where(c => c.PuanDegeri.HasValue && npsSoruIdleri.Contains(c.SoruId) && guncelYanitIdleri.Contains(c.YanitId))
            .Select(c => c.PuanDegeri!.Value)
            .ToList();
        if (npsPuanlari.Count > 0)
            karne.NpsSkoru = NpsSkoruHesapla(npsPuanlari);

        // Önceki dönem (eşit uzunlukta, hemen öncesinde) trend hesabı
        var donemUzunlugu = bitisTarihi - baslangicTarihi;
        var oncekiBitis = baslangicTarihi;
        var oncekiBaslangic = baslangicTarihi - donemUzunlugu;

        var oncekiYanitIdleri = yanitlar
            .Where(y => y.GecerliMi && y.DoktorId == doktorId && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= oncekiBaslangic
                && y.TamamlanmaTarihi.Value < oncekiBitis)
            .Select(y => y.Id)
            .ToHashSet();

        if (oncekiYanitIdleri.Count > 0)
        {
            var oncekiPuanlar = cevaplar
                .Where(c => c.PuanDegeri.HasValue && puanlamaSoruIdleri.Contains(c.SoruId) && oncekiYanitIdleri.Contains(c.YanitId))
                .Select(c => c.PuanDegeri!.Value)
                .ToList();

            if (oncekiPuanlar.Count > 0)
            {
                var oncekiOrtalama = Math.Round(oncekiPuanlar.Average(), 2);
                karne.OncekiDonemOrtalamaPuan = oncekiOrtalama;
                karne.TrendYuzdesi = oncekiOrtalama == 0
                    ? null
                    : Math.Round((karne.OrtalamaPuan - oncekiOrtalama) / oncekiOrtalama * 100, 2);
            }
        }

        return karne;
    }

    /// <summary>Doktorun görev yaptığı birimlerin "Hastane - Birim" biçiminde adlarını getirir.</summary>
    /// <param name="doktor">Birim adları üretilecek doktor.</param>
    /// <returns>Birim adları listesi.</returns>
    private async Task<List<string>> BirimAdlariGetirAsync(Doktor doktor)
    {
        var birimIdleri = doktor.DoktorBirimleri
            .Where(db => db.AktifMi)
            .Select(db => db.BirimId)
            .ToHashSet();
        if (birimIdleri.Count == 0)
            return new List<string>();

        var birimler = await _birimRepository.GetAllAsync();
        var hastaneler = await _hastaneRepository.GetAllAsync();
        var hastaneAdlari = hastaneler.ToDictionary(h => h.Id, h => h.Ad);

        return birimler
            .Where(b => birimIdleri.Contains(b.Id))
            .Select(b => hastaneAdlari.TryGetValue(b.HastaneId, out var hAd) ? $"{hAd} - {b.Ad}" : b.Ad)
            .OrderBy(ad => ad, StringComparer.CurrentCulture)
            .ToList();
    }

    /// <summary>Verilen 0-10 puan listesinden NPS skorunu (-100..+100) hesaplar.</summary>
    private static int NpsSkoruHesapla(IReadOnlyCollection<int> puanlar)
    {
        var destekci = puanlar.Count(p => p >= 9);
        var kotuleyen = puanlar.Count(p => p <= 6);
        return (int)Math.Round((double)(destekci - kotuleyen) / puanlar.Count * 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>Doktorun unvan, ad ve soyadını birleştirerek görüntülenecek adı üretir.</summary>
    private static string DoktorGorunenAd(Doktor doktor)
        => string.Join(" ", new[] { doktor.Unvan, doktor.Ad, doktor.Soyad }
            .Where(p => !string.IsNullOrWhiteSpace(p))).Trim();

    private async Task<int> GunSayisiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.DashboardVarsayilanGunSayisi,
            VarsayilanGunSayisi.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gun) && gun > 0
            ? gun
            : VarsayilanGunSayisi;
    }

    private async Task<int> MinimumOrneklemGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.DusukOrneklemEsigi,
            VarsayilanMinimumOrneklem.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var esik) && esik > 0
            ? esik
            : VarsayilanMinimumOrneklem;
    }
}
