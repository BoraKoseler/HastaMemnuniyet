using System.Globalization;
using System.Text;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Açık uçlu yanıtlar üzerinde harici bir NLP kütüphanesi kullanmadan (saf C# ile)
/// Türkçe etkisiz kelimeleri ayıklayarak anahtar kelime frekans analizi yapan servis.
/// </summary>
public class MetinAnaliziServisi : IMetinAnaliziServisi
{
    /// <summary>Ayar okunamazsa kullanılacak varsayılan gün sayısı.</summary>
    private const int VarsayilanGunSayisi = 30;

    /// <summary>Analize dâhil edilecek en kısa kelime uzunluğu.</summary>
    private const int MinimumKelimeUzunlugu = 3;

    /// <summary>Örnek olarak gösterilecek en fazla yorum sayısı.</summary>
    private const int OrnekYorumSayisi = 10;

    /// <summary>Türkçe metinlerde doğru büyük/küçük harf dönüşümü için kültür bilgisi.</summary>
    private static readonly CultureInfo TurkceKultur = new("tr-TR");

    /// <summary>
    /// Analiz dışında bırakılacak Türkçe etkisiz kelimeler (stop-word) kümesi.
    /// </summary>
    private static readonly HashSet<string> EtkisizKelimeler = new(StringComparer.Ordinal)
    {
        "acaba", "ama", "ancak", "artık", "asla", "aslında", "az", "bana", "bazen", "bazı",
        "belki", "ben", "benden", "beni", "benim", "beri", "bile", "bir", "birçok", "biri",
        "birkaç", "birşey", "biz", "bize", "bizden", "bizi", "bizim", "bu", "buna", "bunda",
        "bundan", "bunlar", "bunları", "bunların", "bunu", "bunun", "burada", "çok", "çünkü",
        "da", "daha", "de", "değil", "diğer", "diye", "dolayı", "eğer", "en", "gibi", "göre",
        "hala", "hangi", "hani", "hatta", "hem", "henüz", "hep", "hepsi", "her", "herhangi",
        "herkes", "hiç", "hiçbir", "için", "içinde", "ile", "ilgili", "ise", "işte", "kadar",
        "karşı", "kendi", "ki", "kim", "kimi", "madem", "mı", "mi", "mu", "mü", "nasıl", "ne",
        "neden", "nedenle", "nerede", "nereye", "niçin", "niye", "o", "olan", "olarak", "oldu",
        "olduğu", "olup", "olur", "on", "ona", "ondan", "onlar", "onları", "onların", "onu",
        "onun", "orada", "öyle", "pek", "rağmen", "sana", "sanki", "sen", "senden", "seni",
        "senin", "siz", "sizden", "sizi", "sizin", "şey", "şeyi", "şeyler", "şöyle", "şu",
        "şuna", "şunda", "şundan", "şunu", "tüm", "üzere", "var", "ve", "veya", "ya", "yani",
        "yani", "yine", "yok", "zaten", "çünkü", "ayrıca", "böyle", "çünki", "gene", "hiçbiri",
        "kendisi", "mızrak", "nerde", "oysa", "üzerine"
    };

    private readonly IAnketRepository _anketRepository;
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IGenericRepository<Domain.Entities.AnketCevabi> _cevapRepository;
    private readonly ISistemAyariRepository _sistemAyariRepository;

    /// <summary>Yeni bir <see cref="MetinAnaliziServisi"/> örneği oluşturur.</summary>
    /// <param name="anketRepository">Anket veri erişim bileşeni (açık uçlu soruları bulmak için).</param>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="cevapRepository">Cevap veri erişim bileşeni.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı veri erişim bileşeni.</param>
    public MetinAnaliziServisi(
        IAnketRepository anketRepository,
        IAnketYanitiRepository yanitRepository,
        IGenericRepository<Domain.Entities.AnketCevabi> cevapRepository,
        ISistemAyariRepository sistemAyariRepository)
    {
        _anketRepository = anketRepository;
        _yanitRepository = yanitRepository;
        _cevapRepository = cevapRepository;
        _sistemAyariRepository = sistemAyariRepository;
    }

    /// <inheritdoc />
    public async Task<MetinAnaliziSonucuDto> AnahtarKelimeleriGetirAsync(
        DateTime? baslangic = null,
        DateTime? bitis = null,
        int? hastaneId = null,
        int? birimId = null,
        int enFazlaKelime = 30)
    {
        if (enFazlaKelime < 1)
            enFazlaKelime = 1;

        var gunSayisi = await GunSayisiGetirAsync();
        var bitisTarihi = bitis ?? DateTime.UtcNow;
        var baslangicTarihi = baslangic ?? bitisTarihi.AddDays(-gunSayisi);

        var anketler = await _anketRepository.GetAllAsync();
        var acikUcluSoruIdleri = anketler
            .SelectMany(a => a.Sorular)
            .Where(s => s.SoruTipi == SoruTipi.AcikUclu)
            .Select(s => s.Id)
            .ToHashSet();

        var sonuc = new MetinAnaliziSonucuDto
        {
            BaslangicTarihi = baslangicTarihi,
            BitisTarihi = bitisTarihi
        };

        if (acikUcluSoruIdleri.Count == 0)
            return sonuc;

        var yanitlar = await _yanitRepository.GetAllAsync();
        var cevaplar = await _cevapRepository.GetAllAsync();

        var gecerliYanitlar = yanitlar
            .Where(y => y.GecerliMi && y.TamamlanmaTarihi.HasValue
                && y.TamamlanmaTarihi.Value >= baslangicTarihi
                && y.TamamlanmaTarihi.Value <= bitisTarihi
                && (hastaneId is null || y.HastaneId == hastaneId)
                && (birimId is null || y.BirimId == birimId))
            .ToList();

        var yanitTarihleri = gecerliYanitlar.ToDictionary(y => y.Id, y => y.TamamlanmaTarihi!.Value);
        var gecerliYanitIdleri = yanitTarihleri.Keys.ToHashSet();

        var metinCevaplar = cevaplar
            .Where(c => acikUcluSoruIdleri.Contains(c.SoruId)
                && gecerliYanitIdleri.Contains(c.YanitId)
                && !string.IsNullOrWhiteSpace(c.MetinDegeri))
            .Select(c => new { Metin = c.MetinDegeri!.Trim(), Tarih = yanitTarihleri[c.YanitId] })
            .ToList();

        sonuc.ToplamYorum = metinCevaplar.Count;
        sonuc.AnlamliYorumSayisi = metinCevaplar.Count;

        if (metinCevaplar.Count == 0)
            return sonuc;

        var frekanslar = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var yorum in metinCevaplar)
        {
            foreach (var kelime in KelimeleriAyikla(yorum.Metin))
            {
                frekanslar[kelime] = frekanslar.TryGetValue(kelime, out var mevcut) ? mevcut + 1 : 1;
            }
        }

        sonuc.EnSikKelimeler = frekanslar
            .OrderByDescending(k => k.Value)
            .ThenBy(k => k.Key, StringComparer.Ordinal)
            .Take(enFazlaKelime)
            .Select(k => new AnahtarKelimeDto { Kelime = k.Key, Sayi = k.Value })
            .ToList();

        sonuc.OrnekYorumlar = metinCevaplar
            .OrderByDescending(y => y.Tarih)
            .Select(y => y.Metin)
            .Take(OrnekYorumSayisi)
            .ToList();

        return sonuc;
    }

    /// <summary>
    /// Verilen metni küçük harfe indirger, noktalama ve rakamları ayıklar, kelimelere böler
    /// ve etkisiz kelimeleri / çok kısa kelimeleri filtreler.
    /// </summary>
    /// <param name="metin">Çözümlenecek serbest metin.</param>
    /// <returns>Anlamlı kelimelerin listesi.</returns>
    private static IEnumerable<string> KelimeleriAyikla(string metin)
    {
        var kucuk = metin.ToLower(TurkceKultur);
        var tampon = new StringBuilder(kucuk.Length);
        foreach (var karakter in kucuk)
        {
            // Harfleri koru; diğer tüm karakterleri (rakam, noktalama, sembol) boşluğa çevir.
            tampon.Append(char.IsLetter(karakter) ? karakter : ' ');
        }

        var parcalar = tampon.ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var parca in parcalar)
        {
            if (parca.Length < MinimumKelimeUzunlugu)
                continue;
            if (EtkisizKelimeler.Contains(parca))
                continue;
            yield return parca;
        }
    }

    private async Task<int> GunSayisiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.DashboardVarsayilanGunSayisi,
            VarsayilanGunSayisi.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gun) && gun > 0
            ? gun
            : VarsayilanGunSayisi;
    }
}
