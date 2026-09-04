using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni anket oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class AnketOlusturDto
{
    /// <summary>Anket adı.</summary>
    [Required(ErrorMessage = "Anket adı zorunludur.")]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;
    /// <summary>Anket açıklaması.</summary>
    [StringLength(1000)]
    public string? Aciklama { get; set; }
    /// <summary>Anket türü.</summary>
    [Required(ErrorMessage = "Anket türü zorunludur.")]
    public AnketTuru AnketTuru { get; set; }
    /// <summary>Gizlilik metni.</summary>
    public string? GizlilikMetni { get; set; }
    /// <summary>Tahmini doldurulma süresi (dakika).</summary>
    [Range(0, 240)]
    public int TahminiSureDakika { get; set; }
    /// <summary>Anketin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; } = true;
}
