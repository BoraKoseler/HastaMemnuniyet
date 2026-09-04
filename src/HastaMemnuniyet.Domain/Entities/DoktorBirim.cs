namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Doktor ile birim arasındaki çoktan-çoğa ilişkiyi temsil eden bağlantı varlığı.
/// Birleşik anahtar (DoktorId, BirimId) kullanılır.
/// </summary>
public class DoktorBirim
{
    /// <summary>İlişkideki doktorun kimliği.</summary>
    public int DoktorId { get; set; }

    /// <summary>İlişkideki birimin kimliği.</summary>
    public int BirimId { get; set; }

    /// <summary>İlişkinin aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>İlişkideki doktor.</summary>
    public Doktor? Doktor { get; set; }

    /// <summary>İlişkideki birim.</summary>
    public Birim? Birim { get; set; }
}
