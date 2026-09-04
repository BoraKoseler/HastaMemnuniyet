using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>İyileştirme aksiyonu bilgilerini taşıyan veri transfer nesnesi.</summary>
public class IyilestirmeAksiyonuDto
{
    /// <summary>Aksiyon kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Bağlı kritik geri bildirim kimliği (isteğe bağlı).</summary>
    public int? KritikGeriBildirimId { get; set; }
    /// <summary>Aksiyon başlığı.</summary>
    public string Baslik { get; set; } = string.Empty;
    /// <summary>Aksiyon açıklaması.</summary>
    public string Aciklama { get; set; } = string.Empty;
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>Birim kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }
    /// <summary>Birim adı.</summary>
    public string? BirimAdi { get; set; }
    /// <summary>Doktor kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }
    /// <summary>Doktor adı.</summary>
    public string? DoktorAdi { get; set; }
    /// <summary>Sorumlu kullanıcının kimliği.</summary>
    public string SorumluKullaniciId { get; set; } = string.Empty;
    /// <summary>Sorumlu kullanıcının adı.</summary>
    public string? SorumluKullaniciAdi { get; set; }
    /// <summary>Aksiyonun öncelik seviyesi.</summary>
    public AksiyonOnceligi Oncelik { get; set; }
    /// <summary>Aksiyonun mevcut durumu.</summary>
    public AksiyonDurumu Durum { get; set; }
    /// <summary>Hedeflenen tamamlanma tarihi (isteğe bağlı).</summary>
    public DateTime? HedefTarih { get; set; }
    /// <summary>Kapanış notu (isteğe bağlı).</summary>
    public string? KapanisNotu { get; set; }
    /// <summary>Oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; }
    /// <summary>Son güncellenme tarihi (isteğe bağlı).</summary>
    public DateTime? GuncellenmeTarihi { get; set; }
    /// <summary>Aksiyonun hedef tarihini geçip geçmediğini belirtir (açık/devam eden ve hedef tarihi geçmiş).</summary>
    public bool GecikmisMi { get; set; }
    /// <summary>Aksiyona ait durum değişiklik geçmişi.</summary>
    public List<AksiyonGecmisiDto> Gecmis { get; set; } = new();
}

/// <summary>Yeni iyileştirme aksiyonu oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class AksiyonOlusturDto
{
    /// <summary>Bağlı kritik geri bildirim kimliği (isteğe bağlı).</summary>
    public int? KritikGeriBildirimId { get; set; }

    /// <summary>Aksiyon başlığı.</summary>
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    /// <summary>Aksiyon açıklaması.</summary>
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Hastane kimliği.</summary>
    [Required(ErrorMessage = "Hastane zorunludur.")]
    [Display(Name = "Hastane")]
    public int HastaneId { get; set; }

    /// <summary>Birim kimliği (isteğe bağlı).</summary>
    [Display(Name = "Birim")]
    public int? BirimId { get; set; }

    /// <summary>Doktor kimliği (isteğe bağlı).</summary>
    [Display(Name = "Doktor")]
    public int? DoktorId { get; set; }

    /// <summary>Sorumlu kullanıcının kimliği.</summary>
    [Required(ErrorMessage = "Sorumlu kullanıcı zorunludur.")]
    [Display(Name = "Sorumlu Kullanıcı")]
    public string SorumluKullaniciId { get; set; } = string.Empty;

    /// <summary>Aksiyonun öncelik seviyesi.</summary>
    [Display(Name = "Öncelik")]
    public AksiyonOnceligi Oncelik { get; set; } = AksiyonOnceligi.Orta;

    /// <summary>Hedeflenen tamamlanma tarihi (isteğe bağlı).</summary>
    [Display(Name = "Hedef Tarih")]
    [DataType(DataType.Date)]
    public DateTime? HedefTarih { get; set; }
}

/// <summary>Mevcut iyileştirme aksiyonunu (durum ve kapanış notu) güncellemek için kullanılan veri transfer nesnesi.</summary>
public class AksiyonGuncelleDto
{
    /// <summary>Güncellenecek aksiyonun kimliği.</summary>
    public int Id { get; set; }

    /// <summary>Aksiyonun yeni durumu.</summary>
    [Required(ErrorMessage = "Durum zorunludur.")]
    [Display(Name = "Durum")]
    public AksiyonDurumu Durum { get; set; }

    /// <summary>Aksiyonun öncelik seviyesi.</summary>
    [Display(Name = "Öncelik")]
    public AksiyonOnceligi Oncelik { get; set; }

    /// <summary>Hedeflenen tamamlanma tarihi (isteğe bağlı).</summary>
    [Display(Name = "Hedef Tarih")]
    [DataType(DataType.Date)]
    public DateTime? HedefTarih { get; set; }

    /// <summary>Kapanış notu (isteğe bağlı, tamamlama/iptal için önerilir).</summary>
    [StringLength(2000, ErrorMessage = "Kapanış notu en fazla 2000 karakter olabilir.")]
    [Display(Name = "Kapanış Notu")]
    public string? KapanisNotu { get; set; }

    /// <summary>Durum değişikliğine dair açıklama (geçmişe yazılır).</summary>
    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    [Display(Name = "Değişiklik Açıklaması")]
    public string? DegisiklikAciklamasi { get; set; }
}

/// <summary>Aksiyon durum değişikliği geçmişini taşıyan veri transfer nesnesi.</summary>
public class AksiyonGecmisiDto
{
    /// <summary>Geçmiş kaydı kimliği.</summary>
    public int Id { get; set; }
    /// <summary>İlgili aksiyon kimliği.</summary>
    public int AksiyonId { get; set; }
    /// <summary>Değişiklik öncesi durum.</summary>
    public AksiyonDurumu EskiDurum { get; set; }
    /// <summary>Değişiklik sonrası durum.</summary>
    public AksiyonDurumu YeniDurum { get; set; }
    /// <summary>Değişikliğe dair açıklama.</summary>
    public string? Aciklama { get; set; }
    /// <summary>Değişikliği yapan kullanıcının kimliği.</summary>
    public string KullaniciId { get; set; } = string.Empty;
    /// <summary>Değişikliği yapan kullanıcının adı.</summary>
    public string? KullaniciAdi { get; set; }
    /// <summary>Değişiklik tarihi.</summary>
    public DateTime Tarih { get; set; }
}
