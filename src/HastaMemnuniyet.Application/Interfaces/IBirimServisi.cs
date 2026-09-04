using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Birim yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IBirimServisi
{
    /// <summary>Tüm birimleri getirir.</summary>
    /// <returns>Birim listesi.</returns>
    Task<IReadOnlyList<BirimDto>> TumunuGetirAsync();

    /// <summary>Verilen hastaneye ait birimleri getirir.</summary>
    /// <param name="hastaneId">Hastane kimliği.</param>
    /// <returns>Birim listesi.</returns>
    Task<IReadOnlyList<BirimDto>> HastaneyeGoreGetirAsync(int hastaneId);

    /// <summary>Verilen kimliğe sahip birimi getirir.</summary>
    /// <param name="id">Birim kimliği.</param>
    /// <returns>Birim ya da null.</returns>
    Task<BirimDto?> GetirAsync(int id);

    /// <summary>Yeni bir birim oluşturur.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <returns>Oluşturulan birim.</returns>
    Task<BirimDto> OlusturAsync(BirimOlusturDto dto);

    /// <summary>Mevcut bir birimi günceller.</summary>
    /// <param name="dto">Güncelleme bilgileri.</param>
    /// <returns>Güncellenen birim.</returns>
    Task<BirimDto> GuncelleAsync(BirimDto dto);

    /// <summary>Verilen kimliğe sahip birimi siler.</summary>
    /// <param name="id">Birim kimliği.</param>
    Task SilAsync(int id);
}
