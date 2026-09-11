using System.Globalization;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Anket daveti oluşturma, sorgulama ve SMS tetikleme iş kurallarını uygulayan servis.
/// </summary>
public class DavetServisi : IDavetServisi
{
    /// <summary>Davet geçerlilik süresi ayarı okunamazsa kullanılacak varsayılan saat.</summary>
    private const int VarsayilanGecerlilikSaati = 72;

    /// <summary>Aynı telefona minimum gönderim aralığı ayarı okunamazsa kullanılacak varsayılan gün.</summary>
    private const int VarsayilanMinimumGonderimAraligi = 30;

    /// <summary>Maksimum hatırlatma sayısı ayarı okunamazsa kullanılacak varsayılan değer.</summary>
    private const int VarsayilanMaksimumHatirlatmaSayisi = 2;

    private readonly IAnketDavetiRepository _davetRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly ITokenUretici _tokenUretici;
    private readonly ISmsSender _smsSender;
    private readonly ISistemAyariRepository _sistemAyariRepository;
    private readonly IGenericRepository<SmsGonderimKaydi> _smsKayitRepository;

    /// <summary>Yeni bir <see cref="DavetServisi"/> örneği oluşturur.</summary>
    /// <param name="davetRepository">Davet veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="tokenUretici">Token üretim bileşeni.</param>
    /// <param name="smsSender">SMS gönderim bileşeni.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı veri erişim bileşeni.</param>
    /// <param name="smsKayitRepository">SMS gönderim kaydı veri erişim bileşeni.</param>
    public DavetServisi(
        IAnketDavetiRepository davetRepository,
        IAnketRepository anketRepository,
        ITokenUretici tokenUretici,
        ISmsSender smsSender,
        ISistemAyariRepository sistemAyariRepository,
        IGenericRepository<SmsGonderimKaydi> smsKayitRepository)
    {
        _davetRepository = davetRepository;
        _anketRepository = anketRepository;
        _tokenUretici = tokenUretici;
        _smsSender = smsSender;
        _sistemAyariRepository = sistemAyariRepository;
        _smsKayitRepository = smsKayitRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnketDavetiDto>> TumunuGetirAsync()
    {
        var davetler = await _davetRepository.GetAllAsync();
        return davetler.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<AnketDavetiDto?> GetirAsync(int id)
    {
        var davet = await _davetRepository.GetByIdAsync(id);
        return davet is null ? null : HaritalaDto(davet);
    }

    /// <inheritdoc />
    public async Task<AnketDavetiDto?> TokenIleGetirAsync(string token)
    {
        var davet = await _davetRepository.GetByTokenAsync(token);
        return davet is null ? null : HaritalaDto(davet);
    }

    /// <inheritdoc />
    public async Task<AnketDavetiDto> OlusturAsync(AnketDavetiOlusturDto dto, string? olusturanKullaniciId)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var anket = await _anketRepository.GetByIdAsync(dto.AnketId)
            ?? throw new InvalidOperationException($"{dto.AnketId} kimlikli anket bulunamadı.");

        var gecerlilikSaati = await GecerlilikSaatiGetirAsync();
        var telefonHash = TelefonHashleyici.Hashle(dto.TelefonNumarasi);

        if (dto.GonderimKanali != GonderimKanali.Qr && !string.IsNullOrWhiteSpace(dto.TelefonNumarasi))
        {
            await TekrarGonderimKontrolEtAsync(dto.AnketId, telefonHash);
        }

        var davet = new AnketDaveti
        {
            AnketId = anket.Id,
            HastaneId = dto.HastaneId,
            BirimId = dto.BirimId,
            DoktorId = dto.DoktorId,
            Token = _tokenUretici.BenzersizTokenUret(),
            TelefonHash = telefonHash,
            GonderimKanali = dto.GonderimKanali,
            Durum = DavetDurumu.Olusturuldu,
            HizmetTarihi = dto.HizmetTarihi,
            SonGecerlilikTarihi = DateTime.UtcNow.AddHours(gecerlilikSaati),
            OlusturanKullaniciId = olusturanKullaniciId
        };

        await _davetRepository.AddAsync(davet);
        await _davetRepository.SaveChangesAsync();

        if (dto.GonderimKanali == GonderimKanali.Sms && !string.IsNullOrWhiteSpace(dto.TelefonNumarasi))
        {
            await SmsGonderAsync(davet, dto.TelefonNumarasi, anket.Ad);
        }

        return HaritalaDto(davet);
    }

    /// <inheritdoc />
    public async Task DurumGuncelleAsync(int davetId, DavetDurumu yeniDurum)
    {
        var davet = await _davetRepository.GetByIdAsync(davetId)
            ?? throw new InvalidOperationException($"{davetId} kimlikli davet bulunamadı.");
        davet.Durum = yeniDurum;
        davet.GuncellenmeTarihi = DateTime.UtcNow;
        _davetRepository.Update(davet);
        await _davetRepository.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task HatirlatmaGonderAsync(int davetId)
    {
        var davet = await _davetRepository.GetByIdAsync(davetId)
            ?? throw new InvalidOperationException($"{davetId} kimlikli davet bulunamadı.");

        if (davet.Durum != DavetDurumu.Gonderildi && davet.Durum != DavetDurumu.Acildi
            && davet.Durum != DavetDurumu.KismiTamamlandi)
        {
            throw new InvalidOperationException(
                "Yalnızca gönderilmiş ancak henüz tamamlanmamış davetlere hatırlatma gönderilebilir.");
        }

        var maksimum = await MaksimumHatirlatmaSayisiGetirAsync();
        if (davet.HatirlatmaSayisi >= maksimum)
        {
            throw new InvalidOperationException(
                $"Bu davet için en fazla {maksimum} hatırlatma gönderilebilir.");
        }

        // Gerçek telefon numarası yalnızca hash olarak saklandığından, hatırlatma gönderimi
        // SMS kaydı üzerinden izlenir. Gönderim sağlayıcısı hash ile tetiklenir.
        var kayit = new SmsGonderimKaydi
        {
            DavetId = davet.Id,
            TelefonHash = davet.TelefonHash,
            SmsDurumu = SmsDurumu.Kuyrukta
        };
        await _smsKayitRepository.AddAsync(kayit);
        await _smsKayitRepository.SaveChangesAsync();

        var mesaj = $"Hatırlatma: Anketinizi doldurmak için bağlantı: /Anket/{davet.Token}";
        var sonuc = await _smsSender.GonderAsync(davet.TelefonHash ?? string.Empty, mesaj);

        kayit.SmsDurumu = sonuc.Basarili ? SmsDurumu.Gonderildi : SmsDurumu.Basarisiz;
        kayit.SmsYaniti = sonuc.Mesaj;
        kayit.GonderimTarihi = DateTime.UtcNow;
        _smsKayitRepository.Update(kayit);
        await _smsKayitRepository.SaveChangesAsync();

        davet.HatirlatmaSayisi += 1;
        davet.GuncellenmeTarihi = DateTime.UtcNow;
        _davetRepository.Update(davet);
        await _davetRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Aynı ankete, aynı telefon numarasına, izin verilen minimum aralıktan daha yakın bir tarihte
    /// gönderilmiş bir davet olup olmadığını denetler. Varsa hata fırlatır.
    /// </summary>
    /// <param name="anketId">Kontrol edilecek anket kimliği.</param>
    /// <param name="telefonHash">Kontrol edilecek telefon numarasının hash değeri.</param>
    private async Task TekrarGonderimKontrolEtAsync(int anketId, string telefonHash)
    {
        var aralik = await MinimumGonderimAraligiGetirAsync();
        if (aralik <= 0)
        {
            return;
        }

        var esikTarih = DateTime.UtcNow.AddDays(-aralik);
        var oncekiDavetler = await _davetRepository.BulAsync(d =>
            d.AnketId == anketId
            && d.TelefonHash == telefonHash
            && d.OlusturulmaTarihi >= esikTarih);

        if (oncekiDavetler.Any())
        {
            throw new InvalidOperationException(
                $"Bu numaraya son {aralik} gün içinde zaten bir anket daveti gönderilmiş. " +
                "Tekrar gönderim engellendi.");
        }
    }

    private async Task<int> MinimumGonderimAraligiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.MinimumGonderimAraligi,
            VarsayilanMinimumGonderimAraligi.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gun)
            ? gun
            : VarsayilanMinimumGonderimAraligi;
    }

    private async Task<int> MaksimumHatirlatmaSayisiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.MaksimumHatirlatmaSayisi,
            VarsayilanMaksimumHatirlatmaSayisi.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sayi)
            ? sayi
            : VarsayilanMaksimumHatirlatmaSayisi;
    }

    private async Task SmsGonderAsync(AnketDaveti davet, string telefonNumarasi, string anketAdi)
    {
        var kayit = new SmsGonderimKaydi
        {
            DavetId = davet.Id,
            TelefonHash = davet.TelefonHash,
            SmsDurumu = SmsDurumu.Kuyrukta
        };
        await _smsKayitRepository.AddAsync(kayit);
        await _smsKayitRepository.SaveChangesAsync();

        var mesaj = $"Değerli hastamız, '{anketAdi}' anketini doldurmak için bağlantı: /Anket/{davet.Token}";
        var sonuc = await _smsSender.GonderAsync(telefonNumarasi, mesaj);

        kayit.SmsDurumu = sonuc.Basarili ? SmsDurumu.Gonderildi : SmsDurumu.Basarisiz;
        kayit.SmsYaniti = sonuc.Mesaj;
        kayit.GonderimTarihi = DateTime.UtcNow;
        _smsKayitRepository.Update(kayit);
        await _smsKayitRepository.SaveChangesAsync();

        if (sonuc.Basarili)
        {
            davet.Durum = DavetDurumu.Gonderildi;
            _davetRepository.Update(davet);
            await _davetRepository.SaveChangesAsync();
        }
    }

    private async Task<int> GecerlilikSaatiGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(
            AyarAnahtarlari.DavetGecerlilikSaati,
            VarsayilanGecerlilikSaati.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var saat)
            ? saat
            : VarsayilanGecerlilikSaati;
    }

    private static AnketDavetiDto HaritalaDto(AnketDaveti davet) => new()
    {
        Id = davet.Id,
        AnketId = davet.AnketId,
        AnketAdi = davet.Anket?.Ad,
        HastaneId = davet.HastaneId,
        HastaneAdi = davet.Hastane?.Ad,
        BirimId = davet.BirimId,
        BirimAdi = davet.Birim?.Ad,
        DoktorId = davet.DoktorId,
        Token = davet.Token,
        GonderimKanali = davet.GonderimKanali,
        Durum = davet.Durum,
        HizmetTarihi = davet.HizmetTarihi,
        SonGecerlilikTarihi = davet.SonGecerlilikTarihi,
        OlusturulmaTarihi = davet.OlusturulmaTarihi,
        HatirlatmaSayisi = davet.HatirlatmaSayisi
    };
}
