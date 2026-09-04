using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni doktor oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class DoktorOlusturDto
{
    /// <summary>Doktorun adı.</summary>
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;
    /// <summary>Doktorun soyadı.</summary>
    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(100)]
    public string Soyad { get; set; } = string.Empty;
    /// <summary>Doktorun unvanı.</summary>
    [StringLength(50)]
    public string? Unvan { get; set; }
    /// <summary>Atanacağı birimlerin kimlikleri.</summary>
    public List<int> BirimIdleri { get; set; } = new();
    /// <summary>Doktorun aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; } = true;
}
