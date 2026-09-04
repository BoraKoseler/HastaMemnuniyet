using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>QR anket kampanyası bilgilerini taşıyan veri transfer nesnesi.</summary>
public class QrAnketKampanyasiDto
{
    /// <summary>Kampanya kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Kampanya adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>İlişkili anket kimliği.</summary>
    public int AnketId { get; set; }
    /// <summary>İlişkili anket adı.</summary>
    public string? AnketAdi { get; set; }
    /// <summary>İlişkili hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>İlişkili hastane adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>İlişkili birim kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }
    /// <summary>İlişkili birim adı.</summary>
    public string? BirimAdi { get; set; }
    /// <summary>İlişkili doktor kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }
    /// <summary>İlişkili doktor adı.</summary>
    public string? DoktorAdi { get; set; }
    /// <summary>Kampanyanın aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Son kullanma tarihi (isteğe bağlı).</summary>
    public DateTime? SonKullanmaTarihi { get; set; }
    /// <summary>QR kodun kaç kez kullanıldığı.</summary>
    public int KullanimSayisi { get; set; }
    /// <summary>Oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; }
}

/// <summary>Yeni QR anket kampanyası oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class QrKampanyaOlusturDto
{
    /// <summary>Kampanya adı.</summary>
    [Required(ErrorMessage = "Kampanya adı zorunludur.")]
    [StringLength(200, ErrorMessage = "Kampanya adı en fazla 200 karakter olabilir.")]
    [Display(Name = "Kampanya Adı")]
    public string Ad { get; set; } = string.Empty;

    /// <summary>İlişkili anket kimliği.</summary>
    [Required(ErrorMessage = "Anket zorunludur.")]
    [Display(Name = "Anket")]
    public int AnketId { get; set; }

    /// <summary>İlişkili hastane kimliği.</summary>
    [Required(ErrorMessage = "Hastane zorunludur.")]
    [Display(Name = "Hastane")]
    public int HastaneId { get; set; }

    /// <summary>İlişkili birim kimliği (isteğe bağlı).</summary>
    [Display(Name = "Birim")]
    public int? BirimId { get; set; }

    /// <summary>İlişkili doktor kimliği (isteğe bağlı).</summary>
    [Display(Name = "Doktor")]
    public int? DoktorId { get; set; }

    /// <summary>Son kullanma tarihi (isteğe bağlı).</summary>
    [Display(Name = "Son Kullanma Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? SonKullanmaTarihi { get; set; }
}
