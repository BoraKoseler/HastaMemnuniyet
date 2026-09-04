namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir hastaneye bağlı birim (klinik / bölüm) varlığı.
/// </summary>
public class Birim : BaseEntity
{
    /// <summary>Birimin bağlı olduğu hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>Birimin adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Birimin kodu.</summary>
    public string? Kod { get; set; }

    /// <summary>Birimin aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Birimin bağlı olduğu hastane.</summary>
    public Hastane? Hastane { get; set; }

    /// <summary>Bu birime atanmış doktor-birim ilişkileri.</summary>
    public ICollection<DoktorBirim> DoktorBirimleri { get; set; } = new List<DoktorBirim>();

    /// <summary>Bu birimi kapsayan kullanıcı yetki kapsamları.</summary>
    public ICollection<KullaniciBirimKapsami> KullaniciKapsamlari { get; set; } = new List<KullaniciBirimKapsami>();
}
