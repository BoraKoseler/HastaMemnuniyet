using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni anket sorusu oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class AnketSorusuOlusturDto
{
    /// <summary>Bağlı olduğu anketin kimliği.</summary>
    [Required]
    public int AnketId { get; set; }
    /// <summary>Soru metni.</summary>
    [Required(ErrorMessage = "Soru metni zorunludur.")]
    [StringLength(500)]
    public string SoruMetni { get; set; } = string.Empty;
    /// <summary>Soru tipi.</summary>
    [Required(ErrorMessage = "Soru tipi zorunludur.")]
    public SoruTipi SoruTipi { get; set; }
    /// <summary>Sıra numarası.</summary>
    public int SiraNo { get; set; }
    /// <summary>Zorunlu olup olmadığı.</summary>
    public bool ZorunluMu { get; set; } = true;
    /// <summary>Soru kategorisi.</summary>
    [StringLength(100)]
    public string? Kategori { get; set; }
    /// <summary>Puanlama alt sınırı.</summary>
    public int? PuanlamaAltSinir { get; set; }
    /// <summary>Puanlama üst sınırı.</summary>
    public int? PuanlamaUstSinir { get; set; }
    /// <summary>Açık uçlu sorular için maksimum karakter sayısı.</summary>
    public int? MaksimumKarakterSayisi { get; set; }
    /// <summary>Görünürlüğün bağlı olduğu sorunun kimliği (koşullu soru için, isteğe bağlı).</summary>
    public int? KosulBagliSoruId { get; set; }
    /// <summary>Koşulun sağlanması için bağlı sorunun taşıması gereken değer (isteğe bağlı).</summary>
    [StringLength(200)]
    public string? KosulDegeri { get; set; }
    /// <summary>Soru seçenekleri (metin değerleri).</summary>
    public List<string> Secenekler { get; set; } = new();
}
