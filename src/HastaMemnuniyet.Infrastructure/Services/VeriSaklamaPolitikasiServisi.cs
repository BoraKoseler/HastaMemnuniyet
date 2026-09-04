using System.Globalization;
using HastaMemnuniyet.Application;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// KVKK veri saklama politikası iş kurallarını uygulayan servis. Süresi dolan kişisel verileri
/// tespit eder ve geri alınamaz biçimde anonimleştirir.
/// </summary>
public class VeriSaklamaPolitikasiServisi : IVeriSaklamaPolitikasiServisi
{
    /// <summary>Kişisel veri saklama süresi ayarı okunamazsa kullanılacak varsayılan gün (2 yıl).</summary>
    private const int VarsayilanKisiselVeriSaklamaGun = 730;

    /// <summary>Anket yanıtı saklama süresi ayarı okunamazsa kullanılacak varsayılan gün (5 yıl).</summary>
    private const int VarsayilanAnketYanitiSaklamaGun = 1825;

    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IAnketDavetiRepository _davetRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly ISistemAyariRepository _sistemAyariRepository;
    private readonly IAuditLogger _denetimKaydedici;

    /// <summary>Yeni bir <see cref="VeriSaklamaPolitikasiServisi"/> örneği oluşturur.</summary>
    /// <param name="yanitRepository">Anket yanıtı veri erişim bileşeni.</param>
    /// <param name="davetRepository">Anket daveti veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı veri erişim bileşeni.</param>
    /// <param name="denetimKaydedici">Denetim kaydı bileşeni.</param>
    public VeriSaklamaPolitikasiServisi(
        IAnketYanitiRepository yanitRepository,
        IAnketDavetiRepository davetRepository,
        IAnketRepository anketRepository,
        ISistemAyariRepository sistemAyariRepository,
        IAuditLogger denetimKaydedici)
    {
        _yanitRepository = yanitRepository;
        _davetRepository = davetRepository;
        _anketRepository = anketRepository;
        _sistemAyariRepository = sistemAyariRepository;
        _denetimKaydedici = denetimKaydedici;
    }

    /// <inheritdoc />
    public async Task<VeriSaklamaOzetiDto> OzetGetirAsync()
    {
        var kisiselVeriGun = await AyarSayiGetirAsync(
            AyarAnahtarlari.KisiselVeriSaklamaSuresiGun, VarsayilanKisiselVeriSaklamaGun);
        var yanitGun = await AyarSayiGetirAsync(
            AyarAnahtarlari.AnketYanitiSaklamaSuresiGun, VarsayilanAnketYanitiSaklamaGun);
        var otomatik = await AyarBoolGetirAsync(AyarAnahtarlari.OtomatikAnonimlestir, false);

        var esikTarih = DateTime.UtcNow.AddDays(-kisiselVeriGun);
        var yanitlar = await _yanitRepository.GetAllAsync();

        var anonimlestirilmis = yanitlar.Count(KisiselVerisiTemiz);
        var anonimlestirilecek = yanitlar.Count(y => SaklamaSuresiDoldu(y, esikTarih) && !KisiselVerisiTemiz(y));

        return new VeriSaklamaOzetiDto
        {
            KisiselVeriSaklamaSuresiGun = kisiselVeriGun,
            AnketYanitiSaklamaSuresiGun = yanitGun,
            KisiselVeriEsikTarihi = esikTarih,
            AnonimlestirilecekYanitSayisi = anonimlestirilecek,
            AnonimlestirilmisYanitSayisi = anonimlestirilmis,
            ToplamYanitSayisi = yanitlar.Count,
            OtomatikAnonimlestirmeEtkin = otomatik
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SuresiDolanKayitDto>> SuresiDolanKayitlariGetirAsync()
    {
        var kisiselVeriGun = await AyarSayiGetirAsync(
            AyarAnahtarlari.KisiselVeriSaklamaSuresiGun, VarsayilanKisiselVeriSaklamaGun);
        var esikTarih = DateTime.UtcNow.AddDays(-kisiselVeriGun);

        var yanitlar = await _yanitRepository.GetAllAsync();
        var suresiDolanlar = yanitlar
            .Where(y => SaklamaSuresiDoldu(y, esikTarih) && !KisiselVerisiTemiz(y))
            .OrderBy(y => y.BaslamaTarihi)
            .ToList();

        if (suresiDolanlar.Count == 0)
        {
            return new List<SuresiDolanKayitDto>();
        }

        var anketler = await _anketRepository.GetAllAsync();
        var anketAdlari = anketler.ToDictionary(a => a.Id, a => a.Ad);

        return suresiDolanlar.Select(y => new SuresiDolanKayitDto
        {
            YanitId = y.Id,
            AnketAdi = anketAdlari.TryGetValue(y.AnketId, out var ad) ? ad : null,
            BaslamaTarihi = y.BaslamaTarihi,
            TamamlanmaTarihi = y.TamamlanmaTarihi,
            KisiselVeriIceriyor = true
        }).ToList();
    }

    /// <inheritdoc />
    public async Task AnonimlestirAsync(int yanitId)
    {
        var yanit = await _yanitRepository.GetByIdAsync(yanitId)
            ?? throw new InvalidOperationException($"{yanitId} kimlikli yanıt bulunamadı.");

        await YanitiAnonimlestirAsync(yanit);
        await _yanitRepository.SaveChangesAsync();

        await _denetimKaydedici.LoglaAsync(
            islem: "KvkkAnonimlestir",
            tablo: nameof(AnketYaniti),
            kayitId: yanit.Id.ToString(CultureInfo.InvariantCulture),
            detay: "Süresi dolan kişisel veriler KVKK saklama politikası gereği anonimleştirildi.");
    }

    /// <inheritdoc />
    public async Task<ImhaRaporuDto> SuresiDolanlariAnonimlestirAsync()
    {
        var kisiselVeriGun = await AyarSayiGetirAsync(
            AyarAnahtarlari.KisiselVeriSaklamaSuresiGun, VarsayilanKisiselVeriSaklamaGun);
        var esikTarih = DateTime.UtcNow.AddDays(-kisiselVeriGun);

        var yanitlar = await _yanitRepository.GetAllAsync();
        var suresiDolanlar = yanitlar
            .Where(y => SaklamaSuresiDoldu(y, esikTarih) && !KisiselVerisiTemiz(y))
            .ToList();

        foreach (var yanit in suresiDolanlar)
        {
            await YanitiAnonimlestirAsync(yanit);
        }

        if (suresiDolanlar.Count > 0)
        {
            await _yanitRepository.SaveChangesAsync();
            await _denetimKaydedici.LoglaAsync(
                islem: "KvkkTopluAnonimlestir",
                tablo: nameof(AnketYaniti),
                detay: $"{suresiDolanlar.Count} adet süresi dolan yanıt toplu olarak anonimleştirildi.");
        }

        return new ImhaRaporuDto
        {
            AnonimlestirilenYanitSayisi = suresiDolanlar.Count,
            IslemTarihi = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Bir yanıttaki kişisel verileri temizler ve ilişkili davetin telefon hash'ini siler.
    /// Kaydetme işlemi çağıran tarafından yapılır.
    /// </summary>
    /// <param name="yanit">Anonimleştirilecek yanıt.</param>
    private async Task YanitiAnonimlestirAsync(AnketYaniti yanit)
    {
        yanit.IpAdresi = null;
        yanit.KullaniciAjan = null;
        _yanitRepository.Update(yanit);

        var davet = await _davetRepository.GetByIdAsync(yanit.DavetId);
        if (davet is not null && !string.IsNullOrEmpty(davet.TelefonHash))
        {
            davet.TelefonHash = null;
            _davetRepository.Update(davet);
            await _davetRepository.SaveChangesAsync();
        }
    }

    /// <summary>Bir yanıtın kişisel veriden arındırılmış olup olmadığını belirtir.</summary>
    /// <param name="yanit">Denetlenecek yanıt.</param>
    /// <returns>Kişisel veri yoksa true.</returns>
    private static bool KisiselVerisiTemiz(AnketYaniti yanit) =>
        string.IsNullOrEmpty(yanit.IpAdresi) && string.IsNullOrEmpty(yanit.KullaniciAjan);

    /// <summary>Bir yanıtın kişisel veri saklama süresinin dolup dolmadığını belirtir.</summary>
    /// <param name="yanit">Denetlenecek yanıt.</param>
    /// <param name="esikTarih">Süre dolumu eşik tarihi.</param>
    /// <returns>Süre dolmuşsa true.</returns>
    private static bool SaklamaSuresiDoldu(AnketYaniti yanit, DateTime esikTarih) =>
        (yanit.TamamlanmaTarihi ?? yanit.BaslamaTarihi) <= esikTarih;

    private async Task<int> AyarSayiGetirAsync(string anahtar, int varsayilan)
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            anahtar, varsayilan.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sayi)
            ? sayi
            : varsayilan;
    }

    private async Task<bool> AyarBoolGetirAsync(string anahtar, bool varsayilan)
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            anahtar, varsayilan.ToString(CultureInfo.InvariantCulture));
        return bool.TryParse(deger, out var sonuc) ? sonuc : varsayilan;
    }
}
