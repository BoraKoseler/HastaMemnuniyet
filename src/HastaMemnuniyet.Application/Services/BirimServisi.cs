using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Birim yönetimi iş kurallarını uygulayan servis.
/// </summary>
public class BirimServisi : IBirimServisi
{
    private readonly IBirimRepository _birimRepository;
    private readonly IHastaneRepository _hastaneRepository;

    /// <summary>Yeni bir <see cref="BirimServisi"/> örneği oluşturur.</summary>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    public BirimServisi(IBirimRepository birimRepository, IHastaneRepository hastaneRepository)
    {
        _birimRepository = birimRepository;
        _hastaneRepository = hastaneRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BirimDto>> TumunuGetirAsync()
    {
        var birimler = await _birimRepository.GetAllAsync();
        return birimler.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BirimDto>> HastaneyeGoreGetirAsync(int hastaneId)
    {
        var birimler = await _birimRepository.HastaneyeGoreGetirAsync(hastaneId);
        return birimler.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<BirimDto?> GetirAsync(int id)
    {
        var birim = await _birimRepository.GetByIdAsync(id);
        return birim is null ? null : HaritalaDto(birim);
    }

    /// <inheritdoc />
    public async Task<BirimDto> OlusturAsync(BirimOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var hastane = await _hastaneRepository.GetByIdAsync(dto.HastaneId)
            ?? throw new InvalidOperationException($"{dto.HastaneId} kimlikli hastane bulunamadı.");

        var birim = new Birim
        {
            HastaneId = hastane.Id,
            Ad = dto.Ad.Trim(),
            Kod = dto.Kod,
            AktifMi = dto.AktifMi
        };

        await _birimRepository.AddAsync(birim);
        await _birimRepository.SaveChangesAsync();
        return HaritalaDto(birim);
    }

    /// <inheritdoc />
    public async Task<BirimDto> GuncelleAsync(BirimDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var birim = await _birimRepository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException($"{dto.Id} kimlikli birim bulunamadı.");

        birim.Ad = dto.Ad.Trim();
        birim.Kod = dto.Kod;
        birim.HastaneId = dto.HastaneId;
        birim.AktifMi = dto.AktifMi;
        birim.GuncellenmeTarihi = DateTime.UtcNow;

        _birimRepository.Update(birim);
        await _birimRepository.SaveChangesAsync();
        return HaritalaDto(birim);
    }

    /// <inheritdoc />
    public async Task SilAsync(int id)
    {
        var birim = await _birimRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"{id} kimlikli birim bulunamadı.");
        _birimRepository.Delete(birim);
        await _birimRepository.SaveChangesAsync();
    }

    private static BirimDto HaritalaDto(Birim birim) => new()
    {
        Id = birim.Id,
        HastaneId = birim.HastaneId,
        HastaneAdi = birim.Hastane?.Ad,
        Ad = birim.Ad,
        Kod = birim.Kod,
        AktifMi = birim.AktifMi
    };
}
