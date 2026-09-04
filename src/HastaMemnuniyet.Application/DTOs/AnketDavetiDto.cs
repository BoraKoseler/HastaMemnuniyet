using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket daveti bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketDavetiDto
{
    /// <summary>Davet kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Anket kimliği.</summary>
    public int AnketId { get; set; }
    /// <summary>Anket adı.</summary>
    public string? AnketAdi { get; set; }
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>Birim kimliği.</summary>
    public int? BirimId { get; set; }
    /// <summary>Birim adı.</summary>
    public string? BirimAdi { get; set; }
    /// <summary>Doktor kimliği.</summary>
    public int? DoktorId { get; set; }
    /// <summary>Davet token değeri.</summary>
    public string Token { get; set; } = string.Empty;
    /// <summary>Gönderim kanalı.</summary>
    public GonderimKanali GonderimKanali { get; set; }
    /// <summary>Davet durumu.</summary>
    public DavetDurumu Durum { get; set; }
    /// <summary>Hizmet tarihi.</summary>
    public DateTime? HizmetTarihi { get; set; }
    /// <summary>Son geçerlilik tarihi.</summary>
    public DateTime SonGecerlilikTarihi { get; set; }
    /// <summary>Oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; }
    /// <summary>Bu davet için gönderilen hatırlatma sayısı.</summary>
    public int HatirlatmaSayisi { get; set; }
}
