using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Bir kullanıcının erişebileceği veri kapsamını (hastane/birim) çözümleyen servis arayüzü.</summary>
public interface IKullaniciKapsamServisi
{
    /// <summary>Kullanıcının veri kapsamını getirir.</summary>
    /// <param name="kullaniciId">Kapsamı çözümlenecek kullanıcının kimliği.</param>
    /// <param name="tamYetkiliMi">True ise (Admin/KaliteBirimi) kapsam kısıtı uygulanmaz ve null döner.</param>
    /// <returns>Kapsam filtresi ya da tam yetkili kullanıcı için null.</returns>
    Task<KapsamFiltresi?> KapsamGetirAsync(string kullaniciId, bool tamYetkiliMi);
}
