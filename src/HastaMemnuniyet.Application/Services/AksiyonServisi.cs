using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// İyileştirme aksiyonlarının oluşturulması, güncellenmesi ve listelenmesini uygulayan servis.
/// </summary>
public class AksiyonServisi : IAksiyonServisi
{
    private readonly IIyilestirmeAksiyonuRepository _aksiyonRepository;
    private readonly IGenericRepository<AksiyonGecmisi> _gecmisRepository;
    private readonly IGenericRepository<UygulamaKullanicisi> _kullaniciRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IDoktorRepository _doktorRepository;

    /// <summary>Yeni bir <see cref="AksiyonServisi"/> örneği oluşturur.</summary>
    /// <param name="aksiyonRepository">Aksiyon veri erişim bileşeni.</param>
    /// <param name="gecmisRepository">Aksiyon geçmişi veri erişim bileşeni.</param>
    /// <param name="kullaniciRepository">Kullanıcı veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    public AksiyonServisi(
        IIyilestirmeAksiyonuRepository aksiyonRepository,
        IGenericRepository<AksiyonGecmisi> gecmisRepository,
        IGenericRepository<UygulamaKullanicisi> kullaniciRepository,
        IHastaneRepository hastaneRepository,
        IBirimRepository birimRepository,
        IDoktorRepository doktorRepository)
    {
        _aksiyonRepository = aksiyonRepository;
        _gecmisRepository = gecmisRepository;
        _kullaniciRepository = kullaniciRepository;
        _hastaneRepository = hastaneRepository;
        _birimRepository = birimRepository;
        _doktorRepository = doktorRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IyilestirmeAksiyonuDto>> TumunuGetirAsync(
        AksiyonDurumu? durum = null,
        bool sadeceGecikmis = false,
        int? hastaneId = null,
        KapsamFiltresi? kapsam = null)
    {
        var kayitlar = await _aksiyonRepository.TumunuIliskileriyleGetirAsync();
        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);
        var kullanicilar = (await _kullaniciRepository.GetAllAsync()).ToDictionary(k => k.Id, k => k);

        var sorgu = kayitlar.AsEnumerable();

        if (durum.HasValue)
            sorgu = sorgu.Where(a => a.Durum == durum.Value);
        if (hastaneId.HasValue)
            sorgu = sorgu.Where(a => a.HastaneId == hastaneId.Value);
        if (sadeceGecikmis)
            sorgu = sorgu.Where(GecikmisMi);
        if (kapsam is not null)
            sorgu = sorgu.Where(a => kapsam.Kapsiyor(a.HastaneId, a.BirimId));

        return sorgu
            .OrderByDescending(a => a.OlusturulmaTarihi)
            .Select(a => HaritalaDto(a, hastaneAdlari, birimAdlari, doktorlar, kullanicilar))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IyilestirmeAksiyonuDto?> DetayGetirAsync(int id)
    {
        var kayit = await _aksiyonRepository.DetayGetirAsync(id);
        if (kayit is null)
            return null;

        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);
        var kullanicilar = (await _kullaniciRepository.GetAllAsync()).ToDictionary(k => k.Id, k => k);

        var dto = HaritalaDto(kayit, hastaneAdlari, birimAdlari, doktorlar, kullanicilar);
        dto.Gecmis = kayit.Gecmis
            .OrderByDescending(g => g.Tarih)
            .Select(g => new AksiyonGecmisiDto
            {
                Id = g.Id,
                AksiyonId = g.AksiyonId,
                EskiDurum = g.EskiDurum,
                YeniDurum = g.YeniDurum,
                Aciklama = g.Aciklama,
                KullaniciId = g.KullaniciId,
                KullaniciAdi = KullaniciAdiCoz(g.KullaniciId, kullanicilar),
                Tarih = g.Tarih
            })
            .ToList();

        return dto;
    }

    /// <inheritdoc />
    public async Task<int> OlusturAsync(AksiyonOlusturDto dto, string olusturanKullaniciId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var aksiyon = new IyilestirmeAksiyonu
        {
            KritikGeriBildirimId = dto.KritikGeriBildirimId,
            Baslik = dto.Baslik,
            Aciklama = dto.Aciklama,
            HastaneId = dto.HastaneId,
            BirimId = dto.BirimId,
            DoktorId = dto.DoktorId,
            SorumluKullaniciId = dto.SorumluKullaniciId,
            Oncelik = dto.Oncelik,
            Durum = AksiyonDurumu.Acik,
            HedefTarih = dto.HedefTarih,
            OlusturanKullaniciId = olusturanKullaniciId
        };

        await _aksiyonRepository.AddAsync(aksiyon);
        await _aksiyonRepository.SaveChangesAsync();

        await _gecmisRepository.AddAsync(new AksiyonGecmisi
        {
            AksiyonId = aksiyon.Id,
            EskiDurum = AksiyonDurumu.Acik,
            YeniDurum = AksiyonDurumu.Acik,
            Aciklama = "Aksiyon oluşturuldu.",
            KullaniciId = olusturanKullaniciId
        });
        await _gecmisRepository.SaveChangesAsync();

        return aksiyon.Id;
    }

    /// <inheritdoc />
    public async Task GuncelleAsync(AksiyonGuncelleDto dto, string guncelleyenKullaniciId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var aksiyon = await _aksiyonRepository.GetByIdAsync(dto.Id);
        if (aksiyon is null)
            throw new InvalidOperationException("Aksiyon bulunamadı.");

        var eskiDurum = aksiyon.Durum;

        aksiyon.Durum = dto.Durum;
        aksiyon.Oncelik = dto.Oncelik;
        aksiyon.HedefTarih = dto.HedefTarih;
        aksiyon.KapanisNotu = dto.KapanisNotu;
        aksiyon.GuncellenmeTarihi = DateTime.UtcNow;
        _aksiyonRepository.Update(aksiyon);
        await _aksiyonRepository.SaveChangesAsync();

        if (eskiDurum != dto.Durum)
        {
            await _gecmisRepository.AddAsync(new AksiyonGecmisi
            {
                AksiyonId = aksiyon.Id,
                EskiDurum = eskiDurum,
                YeniDurum = dto.Durum,
                Aciklama = dto.DegisiklikAciklamasi,
                KullaniciId = guncelleyenKullaniciId
            });
            await _gecmisRepository.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<int> AcikSayiGetirAsync(KapsamFiltresi? kapsam = null)
    {
        var kayitlar = await _aksiyonRepository.GetAllAsync();
        return kayitlar.Count(a =>
            (a.Durum == AksiyonDurumu.Acik || a.Durum == AksiyonDurumu.DevamEdiyor)
            && (kapsam is null || kapsam.Kapsiyor(a.HastaneId, a.BirimId)));
    }

    /// <inheritdoc />
    public async Task<int> GecikmisSayiGetirAsync(KapsamFiltresi? kapsam = null)
    {
        var kayitlar = await _aksiyonRepository.GetAllAsync();
        return kayitlar.Count(a => GecikmisMi(a) && (kapsam is null || kapsam.Kapsiyor(a.HastaneId, a.BirimId)));
    }

    /// <summary>Bir aksiyonun gecikmiş (hedef tarihi geçmiş ve kapanmamış) olup olmadığını belirler.</summary>
    /// <param name="a">Değerlendirilecek aksiyon.</param>
    /// <returns>Aksiyon gecikmişse true.</returns>
    private static bool GecikmisMi(IyilestirmeAksiyonu a)
        => a.HedefTarih.HasValue
           && a.HedefTarih.Value.Date < DateTime.UtcNow.Date
           && a.Durum is not AksiyonDurumu.Tamamlandi and not AksiyonDurumu.Iptal;

    /// <summary>Kullanıcı kimliğine karşılık gelen ad-soyad bilgisini çözer.</summary>
    private static string? KullaniciAdiCoz(string kullaniciId, IReadOnlyDictionary<string, UygulamaKullanicisi> kullanicilar)
        => kullanicilar.TryGetValue(kullaniciId, out var k)
            ? string.Join(" ", new[] { k.Ad, k.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)))
            : null;

    /// <summary>İyileştirme aksiyonu varlığını DTO'ya dönüştürür.</summary>
    private static IyilestirmeAksiyonuDto HaritalaDto(
        IyilestirmeAksiyonu a,
        IReadOnlyDictionary<int, string> hastaneAdlari,
        IReadOnlyDictionary<int, string> birimAdlari,
        IReadOnlyDictionary<int, Doktor> doktorlar,
        IReadOnlyDictionary<string, UygulamaKullanicisi> kullanicilar)
    {
        return new IyilestirmeAksiyonuDto
        {
            Id = a.Id,
            KritikGeriBildirimId = a.KritikGeriBildirimId,
            Baslik = a.Baslik,
            Aciklama = a.Aciklama,
            HastaneId = a.HastaneId,
            HastaneAdi = hastaneAdlari.TryGetValue(a.HastaneId, out var ha) ? ha : null,
            BirimId = a.BirimId,
            BirimAdi = a.BirimId.HasValue && birimAdlari.TryGetValue(a.BirimId.Value, out var ba) ? ba : null,
            DoktorId = a.DoktorId,
            DoktorAdi = a.DoktorId.HasValue && doktorlar.TryGetValue(a.DoktorId.Value, out var dk)
                ? string.Join(" ", new[] { dk.Unvan, dk.Ad, dk.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)))
                : null,
            SorumluKullaniciId = a.SorumluKullaniciId,
            SorumluKullaniciAdi = KullaniciAdiCoz(a.SorumluKullaniciId, kullanicilar),
            Oncelik = a.Oncelik,
            Durum = a.Durum,
            HedefTarih = a.HedefTarih,
            KapanisNotu = a.KapanisNotu,
            OlusturulmaTarihi = a.OlusturulmaTarihi,
            GuncellenmeTarihi = a.GuncellenmeTarihi,
            GecikmisMi = GecikmisMi(a)
        };
    }
}
