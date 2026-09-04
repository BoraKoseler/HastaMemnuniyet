using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Kullanıcı ve rol yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IKullaniciServisi
{
    /// <summary>Tüm kullanıcıları getirir.</summary>
    /// <returns>Kullanıcı listesi.</returns>
    Task<IReadOnlyList<KullaniciDto>> TumunuGetirAsync();

    /// <summary>Verilen kimliğe sahip kullanıcıyı getirir.</summary>
    /// <param name="id">Kullanıcı kimliği.</param>
    /// <returns>Kullanıcı ya da null.</returns>
    Task<KullaniciDto?> GetirAsync(string id);

    /// <summary>Yeni bir kullanıcı oluşturur ve rol atar.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <returns>Oluşturulan kullanıcı.</returns>
    Task<KullaniciDto> OlusturAsync(KullaniciOlusturDto dto);

    /// <summary>Bir kullanıcıya rol atar.</summary>
    /// <param name="kullaniciId">Kullanıcı kimliği.</param>
    /// <param name="rol">Atanacak rol.</param>
    Task RolAtaAsync(string kullaniciId, string rol);

    /// <summary>Bir kullanıcının mevcut rollerini kaldırıp yeni tek rolü atar.</summary>
    /// <param name="kullaniciId">Kullanıcı kimliği.</param>
    /// <param name="yeniRol">Atanacak yeni rol.</param>
    Task RolDegistirAsync(string kullaniciId, string yeniRol);

    /// <summary>Bir kullanıcının aktiflik durumunu değiştirir.</summary>
    /// <param name="kullaniciId">Kullanıcı kimliği.</param>
    /// <param name="aktifMi">Yeni aktiflik durumu.</param>
    Task AktiflikDegistirAsync(string kullaniciId, bool aktifMi);
}
