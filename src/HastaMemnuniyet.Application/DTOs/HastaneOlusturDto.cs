using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni hastane oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class HastaneOlusturDto
{
    /// <summary>Hastane adı.</summary>
    [Required(ErrorMessage = "Hastane adı zorunludur.")]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;
    /// <summary>Hastane kodu.</summary>
    [Required(ErrorMessage = "Hastane kodu zorunludur.")]
    [StringLength(50)]
    public string Kod { get; set; } = string.Empty;
    /// <summary>Hastane adresi.</summary>
    [StringLength(500)]
    public string? Adres { get; set; }
    /// <summary>Hastane telefonu.</summary>
    [StringLength(30)]
    public string? Telefon { get; set; }
    /// <summary>Hastanenin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; } = true;
}
