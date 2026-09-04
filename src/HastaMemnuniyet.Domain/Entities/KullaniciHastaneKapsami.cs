namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir kullanıcının bir hastaneye erişim yetkisini tanımlayan kapsam varlığı.
/// </summary>
public class KullaniciHastaneKapsami
{
    /// <summary>Kapsam kaydının birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>Yetkili kullanıcının kimliği.</summary>
    public string KullaniciId { get; set; } = string.Empty;

    /// <summary>Kapsamdaki hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>Yetkili kullanıcı.</summary>
    public UygulamaKullanicisi? Kullanici { get; set; }

    /// <summary>Kapsamdaki hastane.</summary>
    public Hastane? Hastane { get; set; }
}
