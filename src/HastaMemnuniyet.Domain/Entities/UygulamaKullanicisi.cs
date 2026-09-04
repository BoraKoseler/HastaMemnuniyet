using Microsoft.AspNetCore.Identity;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// ASP.NET Core Identity kullanıcısını genişleten uygulama kullanıcısı varlığı.
/// </summary>
public class UygulamaKullanicisi : IdentityUser
{
    /// <summary>Kullanıcının adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Kullanıcının soyadı.</summary>
    public string Soyad { get; set; } = string.Empty;

    /// <summary>Kullanıcının aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Kullanıcının oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Kullanıcının erişebildiği hastane kapsamları.</summary>
    public ICollection<KullaniciHastaneKapsami> HastaneKapsamlari { get; set; } = new List<KullaniciHastaneKapsami>();

    /// <summary>Kullanıcının erişebildiği birim kapsamları.</summary>
    public ICollection<KullaniciBirimKapsami> BirimKapsamlari { get; set; } = new List<KullaniciBirimKapsami>();
}
