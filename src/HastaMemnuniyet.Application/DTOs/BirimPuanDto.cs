namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Birim bazlı ortalama puan bilgisini taşıyan veri transfer nesnesi.</summary>
public class BirimPuanDto
{
    /// <summary>Birim kimliği.</summary>
    public int BirimId { get; set; }
    /// <summary>Birim adı.</summary>
    public string BirimAdi { get; set; } = string.Empty;
    /// <summary>Bağlı olduğu hastanenin adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>Ortalama puan.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>Yanıt sayısı.</summary>
    public int YanitSayisi { get; set; }
}
