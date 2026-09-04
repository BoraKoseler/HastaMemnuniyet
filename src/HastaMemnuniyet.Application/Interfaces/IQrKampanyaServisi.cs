using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>QR anket kampanyalarının yönetimi ve QR kod üretimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IQrKampanyaServisi
{
    /// <summary>Tüm QR kampanyalarını listeler.</summary>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Kampanya listesi.</returns>
    Task<IReadOnlyList<QrAnketKampanyasiDto>> TumunuGetirAsync(KapsamFiltresi? kapsam = null);

    /// <summary>Verilen kimliğe sahip kampanyayı getirir.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    /// <returns>Kampanya ya da null.</returns>
    Task<QrAnketKampanyasiDto?> GetirAsync(int id);

    /// <summary>Yeni bir QR kampanyası oluşturur.</summary>
    /// <param name="dto">Kampanya oluşturma bilgileri.</param>
    /// <returns>Oluşturulan kampanyanın kimliği.</returns>
    Task<int> OlusturAsync(QrKampanyaOlusturDto dto);

    /// <summary>Bir kampanyanın aktiflik durumunu değiştirir.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    /// <param name="aktifMi">Yeni aktiflik durumu.</param>
    Task AktiflikGuncelleAsync(int id, bool aktifMi);

    /// <summary>Verilen kampanya için hedeflenen URL'yi kullanarak PNG biçiminde QR kod üretir.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    /// <param name="hedefUrl">QR koda gömülecek tam URL.</param>
    /// <returns>PNG biçimindeki QR kod bayt dizisi ya da kampanya bulunamazsa null.</returns>
    Task<byte[]?> QrKodUretAsync(int id, string hedefUrl);

    /// <summary>QR kod ile ankete erişildiğinde kampanyanın kullanım sayısını bir artırır.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    Task KullanimArtirAsync(int id);
}
