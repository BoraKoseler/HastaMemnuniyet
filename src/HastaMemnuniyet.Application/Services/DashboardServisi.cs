using System.Globalization;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Dashboard istatistiklerini hesaplayan servis.
/// </summary>
public class DashboardServisi : IDashboardServisi
{
    /// <summary>Ayar okunamazsa kullanılacak varsayılan gün sayısı.</summary>
    private const int VarsayilanGunSayisi = 30;

    private readonly IAnketRepository _anketRepository;
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IDoktorRepository _doktorRepository;
    private readonly IGenericRepository<AnketDaveti> _davetRepository;
    private readonly IGenericRepository<AnketCevabi> _cevapRepository;
    private readonly ISistemAyariRepository _sistemAyariRepository;
    private readonly IKritikGeriBildirimServisi _kritikServisi;
    private readonly IAksiyonServisi _aksiyonServisi;

    /// <summary>Yeni bir <see cref="DashboardServisi"/> örneği oluşturur.</summary>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    /// <param name="davetRepository">Davet veri erişim bileşeni.</param>
    /// <param name="cevapRepository">Cevap veri erişim bileşeni.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı veri erişim bileşeni.</param>
    /// <param name="kritikServisi">Kritik geri bildirim servisi.</param>
    /// <param name="aksiyonServisi">İyileştirme aksiyonu servisi.</param>
    public DashboardServisi(
        IAnketRepository anketRepository,
        IAnketYanitiRepository yanitRepository,
        IHastaneRepository hastaneRepository,
        IBirimRepository birimRepository,
        IDoktorRepository doktorRepository,
        IGenericRepository<AnketDaveti> davetRepository,
        IGenericRepository<AnketCevabi> cevapRepository,
        ISistemAyariRepository sistemAyariRepository,
        IKritikGeriBildirimServisi kritikServisi,
        IAksiyonServisi aksiyonServisi)
    {
        _anketRepository = anketRepository;
        _yanitRepository = yanitRepository;
        _hastaneRepository = hastaneRepository;
        _birimRepository = birimRepository;
        _doktorRepository = doktorRepository;
        _davetRepository = davetRepository;
        _cevapRepository = cevapRepository;
        _sistemAyariRepository = sistemAyariRepository;
        _kritikServisi = kritikServisi;
        _aksiyonServisi = aksiyonServisi;
    }

    /// <inheritdoc />
    public async Task<DashboardDto> IstatistikleriGetirAsync(DateTime? baslangic = null, DateTime? bitis = null, KapsamFiltresi? kapsam = null)
    {
        var gunSayisi = await GunSayisiGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var anketler = await _anketRepository.GetAllAsync();
        var davetler = await _davetRepository.GetAllAsync();
        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var hastaneler = await _hastaneRepository.GetAllAsync();
        var birimler = await _birimRepository.GetAllAsync();

        var araliktakiDavetler = davetler
            .Where(d => d.OlusturulmaTarihi >= baslangicTarihi && d.OlusturulmaTarihi <= bitisTarihi
                && (kapsam is null || kapsam.Kapsiyor(d.HastaneId, d.BirimId)))
            .ToList();

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi
                && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi
                && (kapsam is null || kapsam.Kapsiyor(y.HastaneId, y.BirimId)))
            .ToList();

        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);
        var gecerliYanitIdleri = gecerliYanitlar.Select(y => y.Id).ToHashSet();
        // Genel memnuniyet ortalaması yalnızca puanlama (1-5) sorularından hesaplanır;
        // NPS (0-10) cevapları ölçek farkı nedeniyle ortalamaya dâhil edilmez.
        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && !npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .ToList();

        var hastaneAdlari = hastaneler.ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = birimler.ToDictionary(b => b.Id, b => b.Ad);
        var birimHastaneAdlari = birimler.ToDictionary(
            b => b.Id,
            b => hastaneAdlari.TryGetValue(b.HastaneId, out var ad) ? ad : null);
        var yanitBilgisi = gecerliYanitlar.ToDictionary(y => y.Id, y => y);

        var toplamDavet = araliktakiDavetler.Count;
        var toplamYanit = gecerliYanitlar.Count;

        var hastanePuanlari = puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId))
            .GroupBy(c => yanitBilgisi[c.YanitId].HastaneId)
            .Select(g => new HastanePuanDto
            {
                HastaneId = g.Key,
                HastaneAdi = hastaneAdlari.TryGetValue(g.Key, out var ad) ? ad : "Bilinmiyor",
                OrtalamaPuan = Math.Round(g.Average(c => c.PuanDegeri!.Value), 2),
                YanitSayisi = g.Select(c => c.YanitId).Distinct().Count()
            })
            .OrderByDescending(h => h.OrtalamaPuan)
            .ToList();

        var birimPuanlari = puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId) && yanitBilgisi[c.YanitId].BirimId.HasValue)
            .GroupBy(c => yanitBilgisi[c.YanitId].BirimId!.Value)
            .Select(g => new BirimPuanDto
            {
                BirimId = g.Key,
                BirimAdi = birimAdlari.TryGetValue(g.Key, out var ad) ? ad : "Bilinmiyor",
                HastaneAdi = birimHastaneAdlari.TryGetValue(g.Key, out var hAd) ? hAd : null,
                OrtalamaPuan = Math.Round(g.Average(c => c.PuanDegeri!.Value), 2),
                YanitSayisi = g.Select(c => c.YanitId).Distinct().Count()
            })
            .OrderByDescending(b => b.OrtalamaPuan)
            .ToList();

        var kritikSayisi = await _kritikServisi.SayiGetirAsync(kapsam);
        var acikAksiyonSayisi = await _aksiyonServisi.AcikSayiGetirAsync(kapsam);
        var gecikmisAksiyonSayisi = await _aksiyonServisi.GecikmisSayiGetirAsync(kapsam);

        return new DashboardDto
        {
            ToplamAnketSayisi = anketler.Count(a => a.AktifMi),
            ToplamDavetSayisi = toplamDavet,
            ToplamYanitSayisi = toplamYanit,
            YanitOrani = toplamDavet == 0 ? 0 : Math.Round((double)toplamYanit / toplamDavet * 100, 2),
            OrtalamaPuan = puanliCevaplar.Count == 0
                ? 0
                : Math.Round(puanliCevaplar.Average(c => c.PuanDegeri!.Value), 2),
            HastanePuanlari = hastanePuanlari,
            BirimPuanlari = birimPuanlari,
            KritikGeriBildirimSayisi = kritikSayisi,
            AcikAksiyonSayisi = acikAksiyonSayisi,
            GecikmisAksiyonSayisi = gecikmisAksiyonSayisi,
            BaslangicTarihi = baslangicTarihi,
            BitisTarihi = bitisTarihi
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<HastaneKarsilastirmaDto>> HastaneKarsilastirmaGetirAsync(
        DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null)
    {
        var gunSayisi = await GunSayisiGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var hastaneler = await _hastaneRepository.GetAllAsync();
        var davetler = await _davetRepository.GetAllAsync();
        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var anketler = await _anketRepository.GetAllAsync();
        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);

        var araliktakiDavetler = davetler
            .Where(d => d.OlusturulmaTarihi >= baslangicTarihi && d.OlusturulmaTarihi <= bitisTarihi
                && (hastaneId is null || d.HastaneId == hastaneId))
            .ToList();

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi
                && (hastaneId is null || y.HastaneId == hastaneId))
            .ToList();

        var yanitBilgisi = gecerliYanitlar.ToDictionary(y => y.Id, y => y);
        var gecerliYanitIdleri = yanitBilgisi.Keys.ToHashSet();
        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && !npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .ToList();

        var davetSayilari = araliktakiDavetler
            .GroupBy(d => d.HastaneId)
            .ToDictionary(g => g.Key, g => g.Count());

        var yanitSayilari = gecerliYanitlar
            .GroupBy(y => y.HastaneId)
            .ToDictionary(g => g.Key, g => g.Count());

        var puanlar = puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId))
            .GroupBy(c => yanitBilgisi[c.YanitId].HastaneId)
            .ToDictionary(g => g.Key, g => Math.Round(g.Average(c => c.PuanDegeri!.Value), 2));

        var kapsamdakiHastaneler = hastaneId is null
            ? hastaneler
            : hastaneler.Where(h => h.Id == hastaneId);

        return kapsamdakiHastaneler
            .Select(h =>
            {
                var davetSayisi = davetSayilari.TryGetValue(h.Id, out var d) ? d : 0;
                var yanitSayisi = yanitSayilari.TryGetValue(h.Id, out var y) ? y : 0;
                return new HastaneKarsilastirmaDto
                {
                    HastaneId = h.Id,
                    HastaneAdi = h.Ad,
                    OrtalamaPuan = puanlar.TryGetValue(h.Id, out var p) ? p : 0,
                    YanitSayisi = yanitSayisi,
                    DavetSayisi = davetSayisi,
                    YanitOrani = davetSayisi == 0 ? 0 : Math.Round((double)yanitSayisi / davetSayisi * 100, 2)
                };
            })
            .OrderByDescending(h => h.OrtalamaPuan)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DoktorPuanDto>> DoktorPuanlariGetirAsync(
        int minimumOrneklem, DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null)
    {
        var gunSayisi = await GunSayisiGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var doktorlar = await _doktorRepository.GetAllAsync();
        var anketler = await _anketRepository.GetAllAsync();
        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi && y.DoktorId.HasValue && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi
                && (hastaneId is null || y.HastaneId == hastaneId))
            .ToList();

        var yanitBilgisi = gecerliYanitlar.ToDictionary(y => y.Id, y => y);
        var gecerliYanitIdleri = yanitBilgisi.Keys.ToHashSet();
        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && !npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .ToList();

        var doktorAdlari = doktorlar.ToDictionary(d => d.Id, DoktorGorunenAd);

        return puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId))
            .GroupBy(c => yanitBilgisi[c.YanitId].DoktorId!.Value)
            .Select(g => new DoktorPuanDto
            {
                DoktorId = g.Key,
                DoktorAdi = doktorAdlari.TryGetValue(g.Key, out var ad) ? ad : "Bilinmiyor",
                OrtalamaPuan = Math.Round(g.Average(c => c.PuanDegeri!.Value), 2),
                YanitSayisi = g.Select(c => c.YanitId).Distinct().Count()
            })
            .Where(d => d.YanitSayisi >= minimumOrneklem)
            .OrderByDescending(d => d.OrtalamaPuan)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AylikTrendDto>> AylikTrendGetirAsync(int aySayisi, int? hastaneId = null)
    {
        if (aySayisi < 1)
            aySayisi = 1;

        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var anketler = await _anketRepository.GetAllAsync();
        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);

        var simdi = DateTime.UtcNow;
        // Aralığın başlangıcı: (aySayisi-1) ay öncesinin ayın ilk günü.
        var ilkAy = new DateTime(simdi.Year, simdi.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(aySayisi - 1));

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= ilkAy
                && (hastaneId is null || y.HastaneId == hastaneId))
            .ToList();

        var yanitBilgisi = gecerliYanitlar.ToDictionary(y => y.Id, y => y);
        var gecerliYanitIdleri = yanitBilgisi.Keys.ToHashSet();
        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && !npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .ToList();

        var aylikPuanlar = puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId))
            .Select(c => new { Tarih = yanitBilgisi[c.YanitId].TamamlanmaTarihi!.Value, c.PuanDegeri, c.YanitId })
            .GroupBy(x => new { x.Tarih.Year, x.Tarih.Month })
            .ToDictionary(
                g => (g.Key.Year, g.Key.Month),
                g => new { Ortalama = Math.Round(g.Average(x => x.PuanDegeri!.Value), 2), Yanit = g.Select(x => x.YanitId).Distinct().Count() });

        var sonuc = new List<AylikTrendDto>();
        for (var i = 0; i < aySayisi; i++)
        {
            var ay = ilkAy.AddMonths(i);
            var anahtar = (ay.Year, ay.Month);
            var veri = aylikPuanlar.TryGetValue(anahtar, out var v) ? v : null;
            sonuc.Add(new AylikTrendDto
            {
                Yil = ay.Year,
                Ay = ay.Month,
                Etiket = ay.ToString("MM.yyyy", CultureInfo.InvariantCulture),
                OrtalamaPuan = veri?.Ortalama ?? 0,
                YanitSayisi = veri?.Yanit ?? 0
            });
        }

        return sonuc;
    }

    /// <inheritdoc />
    public async Task<NpsSonucuDto> NpsHesaplaAsync(
        DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null, int? birimId = null)
    {
        var gunSayisi = await GunSayisiGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var anketler = await _anketRepository.GetAllAsync();
        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);

        var gecerliYanitIdleri = yanitlar
            .Where(y => y.GecerliMi && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi
                && (hastaneId is null || y.HastaneId == hastaneId)
                && (birimId is null || y.BirimId == birimId))
            .Select(y => y.Id)
            .ToHashSet();

        var npsPuanlari = cevaplar
            .Where(c => c.PuanDegeri.HasValue && npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .Select(c => c.PuanDegeri!.Value)
            .ToList();

        return NpsSonucuOlustur(npsPuanlari, baslangicTarihi, bitisTarihi);
    }

    /// <inheritdoc />
    public async Task<DonemselKarsilastirmaDto> DonemselKarsilastirAsync(
        DateTime oncekiBaslangic, DateTime oncekiBitis,
        DateTime guncelBaslangic, DateTime guncelBitis,
        int? hastaneId = null)
    {
        var davetler = await _davetRepository.GetAllAsync();
        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();
        var anketler = await _anketRepository.GetAllAsync();
        var hastaneler = await _hastaneRepository.GetAllAsync();
        var npsSoruIdleri = NpsSoruIdleriToparla(anketler);

        var onceki = DonemMetrikleriHesapla(oncekiBaslangic, oncekiBitis, hastaneId, davetler, yanitlar, cevaplar, npsSoruIdleri);
        var guncel = DonemMetrikleriHesapla(guncelBaslangic, guncelBitis, hastaneId, davetler, yanitlar, cevaplar, npsSoruIdleri);

        var hastaneAdlari = hastaneler.ToDictionary(h => h.Id, h => h.Ad);
        var hastaneIdleri = onceki.HastaneBazli.Keys.Union(guncel.HastaneBazli.Keys).Distinct();

        var hastaneKarsilastirmalari = hastaneIdleri
            .Select(hid =>
            {
                var oncekiVar = onceki.HastaneBazli.TryGetValue(hid, out var o);
                var guncelVar = guncel.HastaneBazli.TryGetValue(hid, out var g);
                var oncekiOrt = oncekiVar ? o.Ort : 0;
                var guncelOrt = guncelVar ? g.Ort : 0;
                return new HastaneDonemKarsilastirmaDto
                {
                    HastaneId = hid,
                    HastaneAdi = hastaneAdlari.TryGetValue(hid, out var ad) ? ad : "Bilinmiyor",
                    OncekiOrtalamaPuan = oncekiOrt,
                    GuncelOrtalamaPuan = guncelOrt,
                    Fark = Math.Round(guncelOrt - oncekiOrt, 2),
                    OncekiYanitSayisi = oncekiVar ? o.Yanit : 0,
                    GuncelYanitSayisi = guncelVar ? g.Yanit : 0
                };
            })
            .OrderByDescending(h => h.GuncelOrtalamaPuan)
            .ToList();

        return new DonemselKarsilastirmaDto
        {
            OncekiBaslangic = oncekiBaslangic,
            OncekiBitis = oncekiBitis,
            GuncelBaslangic = guncelBaslangic,
            GuncelBitis = guncelBitis,
            OncekiOrtalamaPuan = onceki.OrtalamaPuan,
            GuncelOrtalamaPuan = guncel.OrtalamaPuan,
            OrtalamaPuanDegisimYuzdesi = YuzdeDegisim(onceki.OrtalamaPuan, guncel.OrtalamaPuan),
            OncekiYanitSayisi = onceki.YanitSayisi,
            GuncelYanitSayisi = guncel.YanitSayisi,
            YanitSayisiDegisimYuzdesi = YuzdeDegisim(onceki.YanitSayisi, guncel.YanitSayisi),
            OncekiYanitOrani = onceki.YanitOrani,
            GuncelYanitOrani = guncel.YanitOrani,
            YanitOraniFarki = Math.Round(guncel.YanitOrani - onceki.YanitOrani, 2),
            OncekiNps = onceki.Nps,
            GuncelNps = guncel.Nps,
            NpsFarki = guncel.Nps - onceki.Nps,
            HastaneKarsilastirmalari = hastaneKarsilastirmalari
        };
    }

    /// <summary>Belirli bir dönem için toplu memnuniyet metriklerini (bellek içi) hesaplar.</summary>
    private static (double OrtalamaPuan, int YanitSayisi, int DavetSayisi, double YanitOrani, int Nps,
        Dictionary<int, (double Ort, int Yanit)> HastaneBazli) DonemMetrikleriHesapla(
        DateTime baslangic, DateTime bitis, int? hastaneId,
        IReadOnlyList<AnketDaveti> davetler,
        IReadOnlyList<AnketYaniti> yanitlar,
        IReadOnlyList<AnketCevabi> cevaplar,
        HashSet<int> npsSoruIdleri)
    {
        var davetSayisi = davetler.Count(d => d.OlusturulmaTarihi >= baslangic && d.OlusturulmaTarihi <= bitis
            && (hastaneId is null || d.HastaneId == hastaneId));

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangic
                && y.TamamlanmaTarihi.Value <= bitis
                && (hastaneId is null || y.HastaneId == hastaneId))
            .ToList();

        var yanitBilgisi = gecerliYanitlar.ToDictionary(y => y.Id, y => y);
        var gecerliYanitIdleri = yanitBilgisi.Keys.ToHashSet();

        var puanliCevaplar = cevaplar
            .Where(c => c.PuanDegeri.HasValue && !npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .ToList();

        var npsPuanlari = cevaplar
            .Where(c => c.PuanDegeri.HasValue && npsSoruIdleri.Contains(c.SoruId) && gecerliYanitIdleri.Contains(c.YanitId))
            .Select(c => c.PuanDegeri!.Value)
            .ToList();

        var ortalama = puanliCevaplar.Count == 0 ? 0 : Math.Round(puanliCevaplar.Average(c => c.PuanDegeri!.Value), 2);
        var yanitSayisi = gecerliYanitlar.Count;
        var yanitOrani = davetSayisi == 0 ? 0 : Math.Round((double)yanitSayisi / davetSayisi * 100, 2);
        var nps = NpsSkoruHesapla(npsPuanlari);

        var hastaneBazli = puanliCevaplar
            .Where(c => yanitBilgisi.ContainsKey(c.YanitId))
            .GroupBy(c => yanitBilgisi[c.YanitId].HastaneId)
            .ToDictionary(
                g => g.Key,
                g => (Ort: Math.Round(g.Average(c => c.PuanDegeri!.Value), 2), Yanit: g.Select(c => c.YanitId).Distinct().Count()));

        return (ortalama, yanitSayisi, davetSayisi, yanitOrani, nps, hastaneBazli);
    }

    /// <summary>NPS sorularına ait tüm soru kimliklerini toplar.</summary>
    private static HashSet<int> NpsSoruIdleriToparla(IEnumerable<Anket> anketler)
        => anketler
            .SelectMany(a => a.Sorular)
            .Where(s => s.SoruTipi == SoruTipi.Nps)
            .Select(s => s.Id)
            .ToHashSet();

    /// <summary>Verilen 0-10 puan listesinden NPS skorunu (-100..+100) hesaplar.</summary>
    private static int NpsSkoruHesapla(IReadOnlyCollection<int> puanlar)
    {
        if (puanlar.Count == 0)
            return 0;
        var destekci = puanlar.Count(p => p >= 9);
        var kotuleyen = puanlar.Count(p => p <= 6);
        return (int)Math.Round((double)(destekci - kotuleyen) / puanlar.Count * 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>Verilen 0-10 puan listesinden ayrıntılı NPS sonucu üretir.</summary>
    private static NpsSonucuDto NpsSonucuOlustur(IReadOnlyCollection<int> puanlar, DateTime baslangic, DateTime bitis)
    {
        var toplam = puanlar.Count;
        var destekci = puanlar.Count(p => p >= 9);
        var kotuleyen = puanlar.Count(p => p <= 6);
        var pasif = toplam - destekci - kotuleyen;
        var nps = NpsSkoruHesapla(puanlar);

        return new NpsSonucuDto
        {
            ToplamYanit = toplam,
            DestekciSayisi = destekci,
            PasifSayisi = pasif,
            KotuleyenSayisi = kotuleyen,
            DestekciYuzdesi = toplam == 0 ? 0 : Math.Round((double)destekci / toplam * 100, 2),
            PasifYuzdesi = toplam == 0 ? 0 : Math.Round((double)pasif / toplam * 100, 2),
            KotuleyenYuzdesi = toplam == 0 ? 0 : Math.Round((double)kotuleyen / toplam * 100, 2),
            NpsSkoru = nps,
            Kategori = NpsKategori(nps, toplam),
            BaslangicTarihi = baslangic,
            BitisTarihi = bitis
        };
    }

    /// <summary>NPS skoru için sözel kategori döndürür.</summary>
    private static string NpsKategori(int nps, int toplam)
    {
        if (toplam == 0)
            return "Veri yok";
        if (nps >= 70)
            return "Mükemmel";
        if (nps >= 30)
            return "İyi";
        if (nps >= 0)
            return "Geliştirilmeli";
        return "Kritik";
    }

    /// <summary>İki değer arasındaki yüzdesel değişimi hesaplar.</summary>
    private static double YuzdeDegisim(double onceki, double guncel)
    {
        if (onceki == 0)
            return guncel == 0 ? 0 : 100;
        return Math.Round((guncel - onceki) / onceki * 100, 2);
    }

    /// <summary>Doktorun unvan, ad ve soyadını birleştirerek görüntülenecek adı üretir.</summary>
    /// <param name="doktor">Görünen adı üretilecek doktor.</param>
    /// <returns>Boşlukları temizlenmiş görüntüleme adı.</returns>
    private static string DoktorGorunenAd(Domain.Entities.Doktor doktor)
        => string.Join(" ", new[] { doktor.Unvan, doktor.Ad, doktor.Soyad }
            .Where(p => !string.IsNullOrWhiteSpace(p))).Trim();

    private async Task<int> GunSayisiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.DashboardVarsayilanGunSayisi,
            VarsayilanGunSayisi.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gun)
            ? gun
            : VarsayilanGunSayisi;
    }
}
