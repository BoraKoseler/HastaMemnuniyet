using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Doktor yönetimi iş kurallarını uygulayan servis.
/// </summary>
public class DoktorServisi : IDoktorServisi
{
    private readonly IDoktorRepository _doktorRepository;

    /// <summary>Yeni bir <see cref="DoktorServisi"/> örneği oluşturur.</summary>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    public DoktorServisi(IDoktorRepository doktorRepository)
    {
        _doktorRepository = doktorRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DoktorDto>> TumunuGetirAsync()
    {
        var doktorlar = await _doktorRepository.BirimleriyleTumunuGetirAsync();
        return doktorlar.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DoktorDto>> BirimeGoreGetirAsync(int birimId)
    {
        var doktorlar = await _doktorRepository.BirimeGoreGetirAsync(birimId);
        return doktorlar.Select(HaritalaDto).ToList();
    }

    /// <inheritdoc />
    public async Task<DoktorDto?> GetirAsync(int id)
    {
        var doktor = await _doktorRepository.BirimleriyleGetirAsync(id);
        return doktor is null ? null : HaritalaDto(doktor);
    }

    /// <inheritdoc />
    public async Task<DoktorDto> OlusturAsync(DoktorOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var doktor = new Doktor
        {
            Ad = dto.Ad.Trim(),
            Soyad = dto.Soyad.Trim(),
            Unvan = dto.Unvan,
            AktifMi = dto.AktifMi
        };

        foreach (var birimId in dto.BirimIdleri.Distinct())
        {
            doktor.DoktorBirimleri.Add(new DoktorBirim { BirimId = birimId, AktifMi = true });
        }

        await _doktorRepository.AddAsync(doktor);
        await _doktorRepository.SaveChangesAsync();
        return HaritalaDto(doktor);
    }

    /// <inheritdoc />
    public async Task<DoktorDto> GuncelleAsync(DoktorDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var doktor = await _doktorRepository.BirimleriyleGetirAsync(dto.Id)
            ?? throw new InvalidOperationException($"{dto.Id} kimlikli doktor bulunamadı.");

        doktor.Ad = dto.Ad.Trim();
        doktor.Soyad = dto.Soyad.Trim();
        doktor.Unvan = dto.Unvan;
        doktor.AktifMi = dto.AktifMi;
        doktor.GuncellenmeTarihi = DateTime.UtcNow;

        // Birim atamalarını senkronize et (istenen kümeye göre ekle/çıkar).
        var istenenBirimIdleri = dto.BirimIdleri.Distinct().ToHashSet();
        var mevcutBirimIdleri = doktor.DoktorBirimleri.Select(db => db.BirimId).ToHashSet();

        foreach (var kaldirilacak in doktor.DoktorBirimleri.Where(db => !istenenBirimIdleri.Contains(db.BirimId)).ToList())
        {
            doktor.DoktorBirimleri.Remove(kaldirilacak);
        }

        foreach (var eklenecekId in istenenBirimIdleri.Where(id => !mevcutBirimIdleri.Contains(id)))
        {
            doktor.DoktorBirimleri.Add(new DoktorBirim { BirimId = eklenecekId, AktifMi = true });
        }

        _doktorRepository.Update(doktor);
        await _doktorRepository.SaveChangesAsync();

        var guncel = await _doktorRepository.BirimleriyleGetirAsync(dto.Id);
        return HaritalaDto(guncel!);
    }

    /// <inheritdoc />
    public async Task SilAsync(int id)
    {
        var doktor = await _doktorRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"{id} kimlikli doktor bulunamadı.");
        _doktorRepository.Delete(doktor);
        await _doktorRepository.SaveChangesAsync();
    }

    private static DoktorDto HaritalaDto(Doktor doktor) => new()
    {
        Id = doktor.Id,
        Ad = doktor.Ad,
        Soyad = doktor.Soyad,
        Unvan = doktor.Unvan,
        AktifMi = doktor.AktifMi,
        BirimIdleri = doktor.DoktorBirimleri.Select(db => db.BirimId).ToList(),
        BirimAdlari = doktor.DoktorBirimleri
            .Where(db => db.Birim is not null)
            .Select(db => db.Birim!.Hastane is not null
                ? $"{db.Birim!.Hastane!.Ad} - {db.Birim!.Ad}"
                : db.Birim!.Ad)
            .ToList()
    };
}
