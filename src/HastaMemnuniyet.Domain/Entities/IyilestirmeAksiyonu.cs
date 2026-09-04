using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Kritik geri bildirimlere ya da genel kalite iyileştirmelerine yönelik alınan aksiyonu temsil eder.
/// </summary>
public class IyilestirmeAksiyonu : BaseEntity
{
    /// <summary>Aksiyonun bağlı olduğu kritik geri bildirimin kimliği (isteğe bağlı).</summary>
    public int? KritikGeriBildirimId { get; set; }

    /// <summary>Aksiyonun başlığı.</summary>
    public string Baslik { get; set; } = string.Empty;

    /// <summary>Aksiyonun açıklaması.</summary>
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>İlgili hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>İlgili birimin kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }

    /// <summary>İlgili doktorun kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }

    /// <summary>Aksiyondan sorumlu kullanıcının kimliği.</summary>
    public string SorumluKullaniciId { get; set; } = string.Empty;

    /// <summary>Aksiyonun öncelik seviyesi.</summary>
    public AksiyonOnceligi Oncelik { get; set; } = AksiyonOnceligi.Orta;

    /// <summary>Aksiyonun mevcut durumu.</summary>
    public AksiyonDurumu Durum { get; set; } = AksiyonDurumu.Acik;

    /// <summary>Aksiyonun hedeflenen tamamlanma tarihi (isteğe bağlı).</summary>
    public DateTime? HedefTarih { get; set; }

    /// <summary>Aksiyon kapatılırken girilen kapanış notu (isteğe bağlı).</summary>
    public string? KapanisNotu { get; set; }

    /// <summary>Aksiyonu oluşturan kullanıcının kimliği.</summary>
    public string OlusturanKullaniciId { get; set; } = string.Empty;

    /// <summary>Aksiyonun bağlı olduğu kritik geri bildirim (isteğe bağlı).</summary>
    public KritikGeriBildirim? KritikGeriBildirim { get; set; }

    /// <summary>Aksiyondan sorumlu kullanıcı.</summary>
    public UygulamaKullanicisi? SorumluKullanici { get; set; }

    /// <summary>Aksiyona ait durum değişiklik geçmişi.</summary>
    public ICollection<AksiyonGecmisi> Gecmis { get; set; } = new List<AksiyonGecmisi>();
}
