using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni anket daveti oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class AnketDavetiOlusturDto
{
    /// <summary>Anket kimliği.</summary>
    [Required(ErrorMessage = "Anket seçimi zorunludur.")]
    public int AnketId { get; set; }
    /// <summary>Hastane kimliği.</summary>
    [Required(ErrorMessage = "Hastane seçimi zorunludur.")]
    public int HastaneId { get; set; }
    /// <summary>Birim kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }
    /// <summary>Doktor kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }
    /// <summary>Hastanın telefon numarası (SMS kanalı için).</summary>
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    public string? TelefonNumarasi { get; set; }
    /// <summary>Gönderim kanalı.</summary>
    public GonderimKanali GonderimKanali { get; set; } = GonderimKanali.Sms;
    /// <summary>Hizmet tarihi.</summary>
    public DateTime? HizmetTarihi { get; set; }
}
