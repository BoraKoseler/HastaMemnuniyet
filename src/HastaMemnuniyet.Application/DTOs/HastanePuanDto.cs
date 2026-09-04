namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Hastane bazlı ortalama puan bilgisini taşıyan veri transfer nesnesi.</summary>
public class HastanePuanDto
{
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string HastaneAdi { get; set; } = string.Empty;
    /// <summary>Ortalama puan.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>Yanıt sayısı.</summary>
    public int YanitSayisi { get; set; }
}
