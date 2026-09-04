using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Hastane yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IHastaneServisi
{
    /// <summary>Tüm hastaneleri getirir.</summary>
    /// <returns>Hastane listesi.</returns>
    Task<IReadOnlyList<HastaneDto>> TumunuGetirAsync();

    /// <summary>Verilen kimliğe sahip hastaneyi getirir.</summary>
    /// <param name="id">Hastane kimliği.</param>
    /// <returns>Hastane ya da null.</returns>
    Task<HastaneDto?> GetirAsync(int id);

    /// <summary>Yeni bir hastane oluşturur.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <returns>Oluşturulan hastane.</returns>
    Task<HastaneDto> OlusturAsync(HastaneOlusturDto dto);

    /// <summary>Mevcut bir hastaneyi günceller.</summary>
    /// <param name="dto">Güncelleme bilgileri.</param>
    /// <returns>Güncellenen hastane.</returns>
    Task<HastaneDto> GuncelleAsync(HastaneGuncelleDto dto);

    /// <summary>Verilen kimliğe sahip hastaneyi siler.</summary>
    /// <param name="id">Hastane kimliği.</param>
    Task SilAsync(int id);
}
