namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir kullanıcının bir birime erişim yetkisini tanımlayan kapsam varlığı.
/// </summary>
public class KullaniciBirimKapsami
{
    /// <summary>Kapsam kaydının birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>Yetkili kullanıcının kimliği.</summary>
    public string KullaniciId { get; set; } = string.Empty;

    /// <summary>Kapsamdaki birimin kimliği.</summary>
    public int BirimId { get; set; }

    /// <summary>Yetkili kullanıcı.</summary>
    public UygulamaKullanicisi? Kullanici { get; set; }

    /// <summary>Kapsamdaki birim.</summary>
    public Birim? Birim { get; set; }
}
