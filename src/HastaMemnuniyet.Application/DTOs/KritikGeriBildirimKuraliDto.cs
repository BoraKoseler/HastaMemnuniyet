using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Kritik geri bildirim kuralının görüntüleme bilgilerini taşıyan veri transfer nesnesi.</summary>
public class KritikGeriBildirimKuraliDto
{
    /// <summary>Kural kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Uygulanacak anket kimliği (boş ise tüm anketler).</summary>
    public int? AnketId { get; set; }
    /// <summary>Uygulanacak anket adı.</summary>
    public string? AnketAdi { get; set; }
    /// <summary>Uygulanacak soru kimliği (isteğe bağlı).</summary>
    public int? SoruId { get; set; }
    /// <summary>Kuralın değerlendirme tipi.</summary>
    public KuralTipi KuralTipi { get; set; }
    /// <summary>Puan altı kuralları için eşik değeri.</summary>
    public int? EsikDegeri { get; set; }
    /// <summary>Anahtar kelime kuralları için virgülle ayrılmış kelime listesi.</summary>
    public string? AnahtarKelimeler { get; set; }
    /// <summary>Seçenek eşleşmesi kuralları için hedef seçenek kimliği.</summary>
    public int? HedefSecenekId { get; set; }
    /// <summary>Kuralın aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Kuralın oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; }
}

/// <summary>Yeni kritik geri bildirim kuralı oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class KritikKuralOlusturDto
{
    /// <summary>Uygulanacak anket kimliği (boş ise tüm anketler).</summary>
    [Display(Name = "Anket")]
    public int? AnketId { get; set; }

    /// <summary>Uygulanacak soru kimliği (isteğe bağlı).</summary>
    [Display(Name = "Soru")]
    public int? SoruId { get; set; }

    /// <summary>Kuralın değerlendirme tipi.</summary>
    [Required(ErrorMessage = "Kural tipi zorunludur.")]
    [Display(Name = "Kural Tipi")]
    public KuralTipi KuralTipi { get; set; } = KuralTipi.PuanAlti;

    /// <summary>Puan altı kuralları için eşik değeri.</summary>
    [Display(Name = "Eşik Değeri")]
    [Range(1, 100, ErrorMessage = "Eşik değeri 1 ile 100 arasında olmalıdır.")]
    public int? EsikDegeri { get; set; }

    /// <summary>Anahtar kelime kuralları için virgülle ayrılmış kelime listesi.</summary>
    [Display(Name = "Anahtar Kelimeler")]
    [StringLength(500, ErrorMessage = "Anahtar kelimeler en fazla 500 karakter olabilir.")]
    public string? AnahtarKelimeler { get; set; }

    /// <summary>Seçenek eşleşmesi kuralları için hedef seçenek kimliği.</summary>
    [Display(Name = "Hedef Seçenek")]
    public int? HedefSecenekId { get; set; }

    /// <summary>Kuralın aktif olup olmadığı.</summary>
    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;
}
