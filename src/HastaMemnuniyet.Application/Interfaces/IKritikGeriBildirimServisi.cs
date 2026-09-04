using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Kritik geri bildirimlerin ve kurallarının yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IKritikGeriBildirimServisi
{
    /// <summary>Kritik geri bildirimleri verilen filtrelere göre listeler.</summary>
    /// <param name="hastaneId">Hastane filtresi (isteğe bağlı).</param>
    /// <param name="baslangic">Başlangıç tarihi (isteğe bağlı).</param>
    /// <param name="bitis">Bitiş tarihi (isteğe bağlı).</param>
    /// <param name="acikAksiyonMu">True ise yalnızca hiç aksiyonu olmayanlar getirilir (isteğe bağlı).</param>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Kritik geri bildirim listesi.</returns>
    Task<IReadOnlyList<KritikGeriBildirimDto>> TumunuGetirAsync(
        int? hastaneId = null,
        DateTime? baslangic = null,
        DateTime? bitis = null,
        bool? acikAksiyonMu = null,
        KapsamFiltresi? kapsam = null);

    /// <summary>Verilen kimliğe sahip kritik geri bildirimi detaylarıyla getirir.</summary>
    /// <param name="id">Kritik geri bildirim kimliği.</param>
    /// <returns>Kritik geri bildirim ya da null.</returns>
    Task<KritikGeriBildirimDto?> DetayGetirAsync(int id);

    /// <summary>Aktif kritik geri bildirim sayısını getirir.</summary>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Kritik geri bildirim sayısı.</returns>
    Task<int> SayiGetirAsync(KapsamFiltresi? kapsam = null);

    /// <summary>Tüm kritik geri bildirim kurallarını listeler.</summary>
    /// <returns>Kural listesi.</returns>
    Task<IReadOnlyList<KritikGeriBildirimKuraliDto>> KurallariGetirAsync();

    /// <summary>Verilen kimliğe sahip kuralı getirir.</summary>
    /// <param name="id">Kural kimliği.</param>
    /// <returns>Kural ya da null.</returns>
    Task<KritikGeriBildirimKuraliDto?> KuralGetirAsync(int id);

    /// <summary>Yeni bir kritik geri bildirim kuralı oluşturur.</summary>
    /// <param name="dto">Kural oluşturma bilgileri.</param>
    /// <returns>Oluşturulan kuralın kimliği.</returns>
    Task<int> KuralOlusturAsync(KritikKuralOlusturDto dto);

    /// <summary>Bir kuralın aktiflik durumunu değiştirir.</summary>
    /// <param name="id">Kural kimliği.</param>
    /// <param name="aktifMi">Yeni aktiflik durumu.</param>
    Task KuralAktiflikGuncelleAsync(int id, bool aktifMi);
}
