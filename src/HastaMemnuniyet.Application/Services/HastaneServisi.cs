using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Hastane yönetimi iş kurallarını uygulayan servis.
/// </summary>
public class HastaneServisi : IHastaneServisi
{
    private readonly IHastaneRepository _hastaneRepository;

    /// <summary>Yeni bir <see cref="HastaneServisi"/> örneği oluşturur.</summary>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    public HastaneServisi(IHastaneRepository hastaneRepository)
    {
        _hastaneRepository = hastaneRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<HastaneDto>> TumunuGetirAsync()
    {
        var hastaneler = await _hastaneRepository.GetAllAsync();
        return hastaneler.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<HastaneDto?> GetirAsync(int id)
    {
        var hastane = await _hastaneRepository.GetByIdAsync(id);
        return hastane is null ? null : HaritalaDto(hastane);
    }

    /// <inheritdoc />
    public async Task<HastaneDto> OlusturAsync(HastaneOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Ad))
            throw new ArgumentException("Hastane adı boş olamaz.", nameof(dto));

        var hastane = new Hastane
        {
            Ad = dto.Ad.Trim(),
            Kod = dto.Kod.Trim(),
            Adres = dto.Adres,
            Telefon = dto.Telefon,
            AktifMi = dto.AktifMi
        };

        await _hastaneRepository.AddAsync(hastane);
        await _hastaneRepository.SaveChangesAsync();
        return HaritalaDto(hastane);
    }

    /// <inheritdoc />
    public async Task<HastaneDto> GuncelleAsync(HastaneGuncelleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var hastane = await _hastaneRepository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException($"{dto.Id} kimlikli hastane bulunamadı.");

        hastane.Ad = dto.Ad.Trim();
        hastane.Kod = dto.Kod.Trim();
        hastane.Adres = dto.Adres;
        hastane.Telefon = dto.Telefon;
        hastane.AktifMi = dto.AktifMi;
        hastane.GuncellenmeTarihi = DateTime.UtcNow;

        _hastaneRepository.Update(hastane);
        await _hastaneRepository.SaveChangesAsync();
        return HaritalaDto(hastane);
    }

    /// <inheritdoc />
    public async Task SilAsync(int id)
    {
        var hastane = await _hastaneRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"{id} kimlikli hastane bulunamadı.");
        _hastaneRepository.Delete(hastane);
        await _hastaneRepository.SaveChangesAsync();
    }

    private static HastaneDto HaritalaDto(Hastane hastane) => new()
    {
        Id = hastane.Id,
        Ad = hastane.Ad,
        Kod = hastane.Kod,
        Adres = hastane.Adres,
        Telefon = hastane.Telefon,
        AktifMi = hastane.AktifMi
    };
}
