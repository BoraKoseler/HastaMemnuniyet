namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Değerlendirmeye konu olan doktor varlığı.
/// </summary>
public class Doktor : BaseEntity
{
    /// <summary>Doktorun adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Doktorun soyadı.</summary>
    public string Soyad { get; set; } = string.Empty;

    /// <summary>Doktorun unvanı (örn. Prof. Dr., Uzm. Dr.).</summary>
    public string? Unvan { get; set; }

    /// <summary>Doktorun aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Doktorun görev yaptığı birim ilişkileri.</summary>
    public ICollection<DoktorBirim> DoktorBirimleri { get; set; } = new List<DoktorBirim>();
}
