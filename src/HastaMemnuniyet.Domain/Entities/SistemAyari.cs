namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Anahtar-değer biçiminde yapılandırılabilir sistem ayarını temsil eden varlık.
/// </summary>
public class SistemAyari
{
    /// <summary>Ayarın birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>Ayarın benzersiz anahtarı.</summary>
    public string Anahtar { get; set; } = string.Empty;

    /// <summary>Ayarın değeri.</summary>
    public string Deger { get; set; } = string.Empty;

    /// <summary>Ayarın açıklaması.</summary>
    public string? Aciklama { get; set; }

    /// <summary>Ayarın ait olduğu kategori.</summary>
    public string? Kategori { get; set; }

    /// <summary>Ayarın son güncellenme tarihi.</summary>
    public DateTime? GuncellenmeTarihi { get; set; }
}
