namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket yanıtı bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketYanitiDto
{
    /// <summary>Yanıt kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Davet kimliği.</summary>
    public int DavetId { get; set; }
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
    /// <summary>Doktor adı.</summary>
    public string? DoktorAdi { get; set; }
    /// <summary>Gönderim kanalı adı.</summary>
    public string? KanalAdi { get; set; }
    /// <summary>Yanıtın gönderildiği IP adresi.</summary>
    public string? IpAdresi { get; set; }
    /// <summary>Başlama tarihi.</summary>
    public DateTime BaslamaTarihi { get; set; }
    /// <summary>Tamamlanma tarihi.</summary>
    public DateTime? TamamlanmaTarihi { get; set; }
    /// <summary>Yanıtın geçerli olup olmadığı.</summary>
    public bool GecerliMi { get; set; }
    /// <summary>Yanıta ait cevaplar.</summary>
    public List<AnketCevabiDto> Cevaplar { get; set; } = new();
}
