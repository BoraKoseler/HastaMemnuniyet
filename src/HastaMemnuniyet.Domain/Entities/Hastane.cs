namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Değerlendirmeye konu olan hastane (sağlık kurumu) varlığı.
/// </summary>
public class Hastane : BaseEntity
{
    /// <summary>Hastanenin adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Hastanenin benzersiz kodu.</summary>
    public string Kod { get; set; } = string.Empty;

    /// <summary>Hastanenin adresi.</summary>
    public string? Adres { get; set; }

    /// <summary>Hastanenin telefon numarası.</summary>
    public string? Telefon { get; set; }

    /// <summary>Hastanenin aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Hastaneye bağlı birimler.</summary>
    public ICollection<Birim> Birimler { get; set; } = new List<Birim>();

    /// <summary>Bu hastaneyi kapsayan kullanıcı yetki kapsamları.</summary>
    public ICollection<KullaniciHastaneKapsami> KullaniciKapsamlari { get; set; } = new List<KullaniciHastaneKapsami>();
}
