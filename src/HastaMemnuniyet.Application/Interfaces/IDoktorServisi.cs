using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Doktor yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IDoktorServisi
{
    /// <summary>Tüm doktorları getirir.</summary>
    /// <returns>Doktor listesi.</returns>
    Task<IReadOnlyList<DoktorDto>> TumunuGetirAsync();

    /// <summary>Verilen birimde görev yapan doktorları getirir.</summary>
    /// <param name="birimId">Birim kimliği.</param>
    /// <returns>Doktor listesi.</returns>
    Task<IReadOnlyList<DoktorDto>> BirimeGoreGetirAsync(int birimId);

    /// <summary>Verilen kimliğe sahip doktoru getirir.</summary>
    /// <param name="id">Doktor kimliği.</param>
    /// <returns>Doktor ya da null.</returns>
    Task<DoktorDto?> GetirAsync(int id);

    /// <summary>Yeni bir doktor oluşturur.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <returns>Oluşturulan doktor.</returns>
    Task<DoktorDto> OlusturAsync(DoktorOlusturDto dto);

    /// <summary>Mevcut bir doktoru günceller.</summary>
    /// <param name="dto">Güncelleme bilgileri.</param>
    /// <returns>Güncellenen doktor.</returns>
    Task<DoktorDto> GuncelleAsync(DoktorDto dto);

    /// <summary>Verilen kimliğe sahip doktoru siler.</summary>
    /// <param name="id">Doktor kimliği.</param>
    Task SilAsync(int id);
}
