using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>İyileştirme aksiyonlarının yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IAksiyonServisi
{
    /// <summary>Aksiyonları verilen filtrelere göre listeler.</summary>
    /// <param name="durum">Durum filtresi (isteğe bağlı).</param>
    /// <param name="sadeceGecikmis">True ise yalnızca gecikmiş aksiyonlar getirilir (isteğe bağlı).</param>
    /// <param name="hastaneId">Hastane filtresi (isteğe bağlı).</param>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Aksiyon listesi.</returns>
    Task<IReadOnlyList<IyilestirmeAksiyonuDto>> TumunuGetirAsync(
        AksiyonDurumu? durum = null,
        bool sadeceGecikmis = false,
        int? hastaneId = null,
        KapsamFiltresi? kapsam = null);

    /// <summary>Verilen kimliğe sahip aksiyonu detaylarıyla (geçmiş dahil) getirir.</summary>
    /// <param name="id">Aksiyon kimliği.</param>
    /// <returns>Aksiyon ya da null.</returns>
    Task<IyilestirmeAksiyonuDto?> DetayGetirAsync(int id);

    /// <summary>Yeni bir iyileştirme aksiyonu oluşturur.</summary>
    /// <param name="dto">Aksiyon oluşturma bilgileri.</param>
    /// <param name="olusturanKullaniciId">Aksiyonu oluşturan kullanıcının kimliği.</param>
    /// <returns>Oluşturulan aksiyonun kimliği.</returns>
    Task<int> OlusturAsync(AksiyonOlusturDto dto, string olusturanKullaniciId);

    /// <summary>Bir aksiyonun durumunu ve ilgili alanlarını günceller, değişikliği geçmişe yazar.</summary>
    /// <param name="dto">Güncelleme bilgileri.</param>
    /// <param name="guncelleyenKullaniciId">Güncellemeyi yapan kullanıcının kimliği.</param>
    Task GuncelleAsync(AksiyonGuncelleDto dto, string guncelleyenKullaniciId);

    /// <summary>Açık (devam eden veya yeni) aksiyon sayısını getirir.</summary>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Açık aksiyon sayısı.</returns>
    Task<int> AcikSayiGetirAsync(KapsamFiltresi? kapsam = null);

    /// <summary>Gecikmiş (hedef tarihi geçmiş ve kapanmamış) aksiyon sayısını getirir.</summary>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Gecikmiş aksiyon sayısı.</returns>
    Task<int> GecikmisSayiGetirAsync(KapsamFiltresi? kapsam = null);
}
