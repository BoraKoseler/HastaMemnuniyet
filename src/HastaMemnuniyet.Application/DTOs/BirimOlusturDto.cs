using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni birim oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class BirimOlusturDto
{
    /// <summary>Bağlı olduğu hastanenin kimliği.</summary>
    [Required(ErrorMessage = "Hastane seçimi zorunludur.")]
    public int HastaneId { get; set; }
    /// <summary>Birim adı.</summary>
    [Required(ErrorMessage = "Birim adı zorunludur.")]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;
    /// <summary>Birim kodu.</summary>
    [StringLength(50)]
    public string? Kod { get; set; }
    /// <summary>Birimin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; } = true;
}
