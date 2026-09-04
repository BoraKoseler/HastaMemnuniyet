using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Kritik geri bildirimlerin ve kurallarının yönetimini uygulayan servis.
/// </summary>
public class KritikGeriBildirimServisi : IKritikGeriBildirimServisi
{
    private readonly IKritikGeriBildirimRepository _kritikRepository;
    private readonly IGenericRepository<KritikGeriBildirimKurali> _kuralRepository;
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IDoktorRepository _doktorRepository;

    /// <summary>Yeni bir <see cref="KritikGeriBildirimServisi"/> örneği oluşturur.</summary>
    /// <param name="kritikRepository">Kritik geri bildirim veri erişim bileşeni.</param>
    /// <param name="kuralRepository">Kural veri erişim bileşeni.</param>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    public KritikGeriBildirimServisi(
        IKritikGeriBildirimRepository kritikRepository,
        IGenericRepository<KritikGeriBildirimKurali> kuralRepository,
        IAnketYanitiRepository yanitRepository,
        IAnketRepository anketRepository,
        IHastaneRepository hastaneRepository,
        IBirimRepository birimRepository,
        IDoktorRepository doktorRepository)
    {
        _kritikRepository = kritikRepository;
        _kuralRepository = kuralRepository;
        _yanitRepository = yanitRepository;
        _anketRepository = anketRepository;
        _hastaneRepository = hastaneRepository;
        _birimRepository = birimRepository;
        _doktorRepository = doktorRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<KritikGeriBildirimDto>> TumunuGetirAsync(
        int? hastaneId = null,
        DateTime? baslangic = null,
        DateTime? bitis = null,
        bool? acikAksiyonMu = null,
        KapsamFiltresi? kapsam = null)
    {
        var kayitlar = await _kritikRepository.TumunuIliskileriyleGetirAsync();
        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);
        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);

        var sorgu = kayitlar.AsEnumerable();

        if (hastaneId.HasValue)
            sorgu = sorgu.Where(k => k.HastaneId == hastaneId.Value);
        if (baslangic.HasValue)
            sorgu = sorgu.Where(k => k.OlusturulmaTarihi >= baslangic.Value);
        if (bitis.HasValue)
            sorgu = sorgu.Where(k => k.OlusturulmaTarihi <= bitis.Value);
        if (acikAksiyonMu == true)
            sorgu = sorgu.Where(k => k.Aksiyonlar.Count == 0);
        if (kapsam is not null)
            sorgu = sorgu.Where(k => kapsam.Kapsiyor(k.HastaneId, k.BirimId));

        return sorgu
            .OrderByDescending(k => k.OlusturulmaTarihi)
            .Select(k => HaritalaDto(k, hastaneAdlari, birimAdlari, doktorlar, anketAdlari))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<KritikGeriBildirimDto?> DetayGetirAsync(int id)
    {
        var kayit = await _kritikRepository.DetayGetirAsync(id);
        if (kayit is null)
            return null;

        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);
        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);

        var dto = HaritalaDto(kayit, hastaneAdlari, birimAdlari, doktorlar, anketAdlari);

        // Kaynak yanıtın cevaplarını detay görünümü için ekle.
        var yanit = await _yanitRepository.CevaplariylaGetirAsync(kayit.YanitId);
        if (yanit is not null)
        {
            dto.Cevaplar = yanit.Cevaplar
                .OrderBy(c => c.Soru != null ? c.Soru.SiraNo : 0)
                .Select(c => new AnketCevabiDto
                {
                    Id = c.Id,
                    YanitId = c.YanitId,
                    SoruId = c.SoruId,
                    SoruMetni = c.Soru?.SoruMetni,
                    SoruTipi = c.Soru?.SoruTipi ?? SoruTipi.AcikUclu,
                    PuanDegeri = c.PuanDegeri,
                    SecenekId = c.SecenekId,
                    SecenekMetni = c.Secenek?.MetinDegeri,
                    MetinDegeri = c.MetinDegeri,
                    BoolDegeri = c.BoolDegeri,
                    SeciliSecenekIdleri = c.SeciliSecenekIdleri
                })
                .ToList();
        }

        dto.Aksiyonlar = kayit.Aksiyonlar
            .OrderByDescending(a => a.OlusturulmaTarihi)
            .Select(a => new IyilestirmeAksiyonuDto
            {
                Id = a.Id,
                KritikGeriBildirimId = a.KritikGeriBildirimId,
                Baslik = a.Baslik,
                Aciklama = a.Aciklama,
                HastaneId = a.HastaneId,
                HastaneAdi = hastaneAdlari.TryGetValue(a.HastaneId, out var ha) ? ha : null,
                BirimId = a.BirimId,
                BirimAdi = a.BirimId.HasValue && birimAdlari.TryGetValue(a.BirimId.Value, out var ba) ? ba : null,
                Oncelik = a.Oncelik,
                Durum = a.Durum,
                HedefTarih = a.HedefTarih,
                OlusturulmaTarihi = a.OlusturulmaTarihi,
                GecikmisMi = a.HedefTarih.HasValue
                    && a.HedefTarih.Value.Date < DateTime.UtcNow.Date
                    && a.Durum is not AksiyonDurumu.Tamamlandi and not AksiyonDurumu.Iptal
            })
            .ToList();

        return dto;
    }

    /// <inheritdoc />
    public async Task<int> SayiGetirAsync(KapsamFiltresi? kapsam = null)
    {
        var kayitlar = await _kritikRepository.GetAllAsync();
        return kapsam is null
            ? kayitlar.Count
            : kayitlar.Count(k => kapsam.Kapsiyor(k.HastaneId, k.BirimId));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<KritikGeriBildirimKuraliDto>> KurallariGetirAsync()
    {
        var kurallar = await _kuralRepository.GetAllAsync();
        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);

        return kurallar
            .OrderByDescending(k => k.OlusturulmaTarihi)
            .Select(k => new KritikGeriBildirimKuraliDto
            {
                Id = k.Id,
                AnketId = k.AnketId,
                AnketAdi = k.AnketId.HasValue && anketAdlari.TryGetValue(k.AnketId.Value, out var ad) ? ad : null,
                SoruId = k.SoruId,
                KuralTipi = k.KuralTipi,
                EsikDegeri = k.EsikDegeri,
                AnahtarKelimeler = k.AnahtarKelimeler,
                HedefSecenekId = k.HedefSecenekId,
                AktifMi = k.AktifMi,
                OlusturulmaTarihi = k.OlusturulmaTarihi
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<KritikGeriBildirimKuraliDto?> KuralGetirAsync(int id)
    {
        var kural = await _kuralRepository.GetByIdAsync(id);
        if (kural is null)
            return null;

        var anket = kural.AnketId.HasValue ? await _anketRepository.GetByIdAsync(kural.AnketId.Value) : null;

        return new KritikGeriBildirimKuraliDto
        {
            Id = kural.Id,
            AnketId = kural.AnketId,
            AnketAdi = anket?.Ad,
            SoruId = kural.SoruId,
            KuralTipi = kural.KuralTipi,
            EsikDegeri = kural.EsikDegeri,
            AnahtarKelimeler = kural.AnahtarKelimeler,
            HedefSecenekId = kural.HedefSecenekId,
            AktifMi = kural.AktifMi,
            OlusturulmaTarihi = kural.OlusturulmaTarihi
        };
    }

    /// <inheritdoc />
    public async Task<int> KuralOlusturAsync(KritikKuralOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var kural = new KritikGeriBildirimKurali
        {
            AnketId = dto.AnketId,
            SoruId = dto.SoruId,
            KuralTipi = dto.KuralTipi,
            EsikDegeri = dto.KuralTipi == KuralTipi.PuanAlti ? dto.EsikDegeri : null,
            AnahtarKelimeler = dto.KuralTipi == KuralTipi.AnahtarKelime ? dto.AnahtarKelimeler : null,
            HedefSecenekId = dto.KuralTipi == KuralTipi.SecenekEslesmesi ? dto.HedefSecenekId : null,
            AktifMi = dto.AktifMi
        };

        await _kuralRepository.AddAsync(kural);
        await _kuralRepository.SaveChangesAsync();
        return kural.Id;
    }

    /// <inheritdoc />
    public async Task KuralAktiflikGuncelleAsync(int id, bool aktifMi)
    {
        var kural = await _kuralRepository.GetByIdAsync(id);
        if (kural is null)
            throw new InvalidOperationException("Kural bulunamadı.");

        kural.AktifMi = aktifMi;
        kural.GuncellenmeTarihi = DateTime.UtcNow;
        _kuralRepository.Update(kural);
        await _kuralRepository.SaveChangesAsync();
    }

    /// <summary>Kritik geri bildirim varlığını DTO'ya dönüştürür.</summary>
    private static KritikGeriBildirimDto HaritalaDto(
        KritikGeriBildirim k,
        IReadOnlyDictionary<int, string> hastaneAdlari,
        IReadOnlyDictionary<int, string> birimAdlari,
        IReadOnlyDictionary<int, Doktor> doktorlar,
        IReadOnlyDictionary<int, string> anketAdlari)
    {
        return new KritikGeriBildirimDto
        {
            Id = k.Id,
            YanitId = k.YanitId,
            CevapId = k.CevapId,
            KuralId = k.KuralId,
            KuralTipi = k.Kural?.KuralTipi,
            HastaneId = k.HastaneId,
            HastaneAdi = hastaneAdlari.TryGetValue(k.HastaneId, out var ha) ? ha : null,
            BirimId = k.BirimId,
            BirimAdi = k.BirimId.HasValue && birimAdlari.TryGetValue(k.BirimId.Value, out var ba) ? ba : null,
            DoktorId = k.DoktorId,
            DoktorAdi = k.DoktorId.HasValue && doktorlar.TryGetValue(k.DoktorId.Value, out var dk)
                ? string.Join(" ", new[] { dk.Unvan, dk.Ad, dk.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)))
                : null,
            AnketAdi = k.Yanit is not null && anketAdlari.TryGetValue(k.Yanit.AnketId, out var an) ? an : null,
            Aciklama = k.Aciklama,
            OlusturulmaTarihi = k.OlusturulmaTarihi,
            AksiyonSayisi = k.Aksiyonlar.Count
        };
    }
}
