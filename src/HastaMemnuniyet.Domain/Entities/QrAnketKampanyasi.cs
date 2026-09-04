namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// QR kod aracılığıyla ankete erişimi sağlayan kampanyayı temsil eder.
/// </summary>
public class QrAnketKampanyasi : BaseEntity
{
    /// <summary>Kampanyanın adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Kampanyanın ilişkili olduğu anketin kimliği.</summary>
    public int AnketId { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu birimin kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu doktorun kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }

    /// <summary>Kampanyanın aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Kampanyanın son kullanma tarihi (isteğe bağlı).</summary>
    public DateTime? SonKullanmaTarihi { get; set; }

    /// <summary>Kampanya QR kodunun kaç kez kullanıldığını belirtir.</summary>
    public int KullanimSayisi { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu anket.</summary>
    public Anket? Anket { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu hastane.</summary>
    public Hastane? Hastane { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu birim (isteğe bağlı).</summary>
    public Birim? Birim { get; set; }

    /// <summary>Kampanyanın ilişkili olduğu doktor (isteğe bağlı).</summary>
    public Doktor? Doktor { get; set; }
}
