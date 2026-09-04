namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Birim bilgilerini taşıyan veri transfer nesnesi.</summary>
public class BirimDto
{
    /// <summary>Birim kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Bağlı olduğu hastanenin kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Bağlı olduğu hastanenin adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>Birim adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>Birim kodu.</summary>
    public string? Kod { get; set; }
    /// <summary>Birimin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
}
