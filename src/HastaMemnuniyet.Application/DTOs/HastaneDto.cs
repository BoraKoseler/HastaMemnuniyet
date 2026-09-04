namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Hastane bilgilerini taşıyan veri transfer nesnesi.</summary>
public class HastaneDto
{
    /// <summary>Hastane kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Hastane adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>Hastane kodu.</summary>
    public string Kod { get; set; } = string.Empty;
    /// <summary>Hastane adresi.</summary>
    public string? Adres { get; set; }
    /// <summary>Hastane telefonu.</summary>
    public string? Telefon { get; set; }
    /// <summary>Hastanenin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
}
