using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// QR anket kampanyalarının yönetimini ve QR kod üretimini uygulayan servis.
/// </summary>
public class QrKampanyaServisi : IQrKampanyaServisi
{
    private readonly IGenericRepository<QrAnketKampanyasi> _kampanyaRepository;
    private readonly IQrKodUretici _qrKodUretici;
    private readonly IAnketRepository _anketRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IDoktorRepository _doktorRepository;

    /// <summary>Yeni bir <see cref="QrKampanyaServisi"/> örneği oluşturur.</summary>
    /// <param name="kampanyaRepository">Kampanya veri erişim bileşeni.</param>
    /// <param name="qrKodUretici">QR kod üretici bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    public QrKampanyaServisi(
        IGenericRepository<QrAnketKampanyasi> kampanyaRepository,
        IQrKodUretici qrKodUretici,
        IAnketRepository anketRepository,
        IHastaneRepository hastaneRepository,
        IBirimRepository birimRepository,
        IDoktorRepository doktorRepository)
    {
        _kampanyaRepository = kampanyaRepository;
        _qrKodUretici = qrKodUretici;
        _anketRepository = anketRepository;
        _hastaneRepository = hastaneRepository;
        _birimRepository = birimRepository;
        _doktorRepository = doktorRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<QrAnketKampanyasiDto>> TumunuGetirAsync(KapsamFiltresi? kapsam = null)
    {
        var kampanyalar = await _kampanyaRepository.GetAllAsync();
        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);
        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);

        var sorgu = kampanyalar.AsEnumerable();
        if (kapsam is not null)
            sorgu = sorgu.Where(k => kapsam.Kapsiyor(k.HastaneId, k.BirimId));

        return sorgu
            .OrderByDescending(k => k.OlusturulmaTarihi)
            .Select(k => HaritalaDto(k, anketAdlari, hastaneAdlari, birimAdlari, doktorlar))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<QrAnketKampanyasiDto?> GetirAsync(int id)
    {
        var kampanya = await _kampanyaRepository.GetByIdAsync(id);
        if (kampanya is null)
            return null;

        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);
        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);

        return HaritalaDto(kampanya, anketAdlari, hastaneAdlari, birimAdlari, doktorlar);
    }

    /// <inheritdoc />
    public async Task<int> OlusturAsync(QrKampanyaOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var kampanya = new QrAnketKampanyasi
        {
            Ad = dto.Ad,
            AnketId = dto.AnketId,
            HastaneId = dto.HastaneId,
            BirimId = dto.BirimId,
            DoktorId = dto.DoktorId,
            SonKullanmaTarihi = dto.SonKullanmaTarihi,
            AktifMi = true,
            KullanimSayisi = 0
        };

        await _kampanyaRepository.AddAsync(kampanya);
        await _kampanyaRepository.SaveChangesAsync();
        return kampanya.Id;
    }

    /// <inheritdoc />
    public async Task AktiflikGuncelleAsync(int id, bool aktifMi)
    {
        var kampanya = await _kampanyaRepository.GetByIdAsync(id);
        if (kampanya is null)
            throw new InvalidOperationException("Kampanya bulunamadı.");

        kampanya.AktifMi = aktifMi;
        kampanya.GuncellenmeTarihi = DateTime.UtcNow;
        _kampanyaRepository.Update(kampanya);
        await _kampanyaRepository.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<byte[]?> QrKodUretAsync(int id, string hedefUrl)
    {
        var kampanya = await _kampanyaRepository.GetByIdAsync(id);
        if (kampanya is null)
            return null;

        return _qrKodUretici.QrKodUret(hedefUrl);
    }

    /// <inheritdoc />
    public async Task KullanimArtirAsync(int id)
    {
        var kampanya = await _kampanyaRepository.GetByIdAsync(id);
        if (kampanya is null)
            return;

        kampanya.KullanimSayisi += 1;
        kampanya.GuncellenmeTarihi = DateTime.UtcNow;
        _kampanyaRepository.Update(kampanya);
        await _kampanyaRepository.SaveChangesAsync();
    }

    /// <summary>QR kampanyası varlığını DTO'ya dönüştürür.</summary>
    private static QrAnketKampanyasiDto HaritalaDto(
        QrAnketKampanyasi k,
        IReadOnlyDictionary<int, string> anketAdlari,
        IReadOnlyDictionary<int, string> hastaneAdlari,
        IReadOnlyDictionary<int, string> birimAdlari,
        IReadOnlyDictionary<int, Doktor> doktorlar)
    {
        return new QrAnketKampanyasiDto
        {
            Id = k.Id,
            Ad = k.Ad,
            AnketId = k.AnketId,
            AnketAdi = anketAdlari.TryGetValue(k.AnketId, out var an) ? an : null,
            HastaneId = k.HastaneId,
            HastaneAdi = hastaneAdlari.TryGetValue(k.HastaneId, out var ha) ? ha : null,
            BirimId = k.BirimId,
            BirimAdi = k.BirimId.HasValue && birimAdlari.TryGetValue(k.BirimId.Value, out var ba) ? ba : null,
            DoktorId = k.DoktorId,
            DoktorAdi = k.DoktorId.HasValue && doktorlar.TryGetValue(k.DoktorId.Value, out var dk)
                ? string.Join(" ", new[] { dk.Unvan, dk.Ad, dk.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)))
                : null,
            AktifMi = k.AktifMi,
            SonKullanmaTarihi = k.SonKullanmaTarihi,
            KullanimSayisi = k.KullanimSayisi,
            OlusturulmaTarihi = k.OlusturulmaTarihi
        };
    }
}
