namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Doktor bilgilerini taşıyan veri transfer nesnesi.</summary>
public class DoktorDto
{
    /// <summary>Doktor kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Doktorun adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>Doktorun soyadı.</summary>
    public string Soyad { get; set; } = string.Empty;
    /// <summary>Doktorun unvanı.</summary>
    public string? Unvan { get; set; }
    /// <summary>Doktorun aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Doktorun atandığı birimlerin kimlikleri.</summary>
    public List<int> BirimIdleri { get; set; } = new();
    /// <summary>Doktorun atandığı birimlerin görünen adları (hastane - birim).</summary>
    public List<string> BirimAdlari { get; set; } = new();
    /// <summary>Doktorun tam adı (unvan + ad + soyad).</summary>
    public string TamAd => string.Join(" ", new[] { Unvan, Ad, Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)));
}
