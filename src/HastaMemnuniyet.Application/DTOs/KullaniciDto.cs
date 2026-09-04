namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Kullanıcı bilgilerini taşıyan veri transfer nesnesi.</summary>
public class KullaniciDto
{
    /// <summary>Kullanıcı kimliği.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Kullanıcı adı (e-posta).</summary>
    public string Eposta { get; set; } = string.Empty;
    /// <summary>Kullanıcının adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>Kullanıcının soyadı.</summary>
    public string Soyad { get; set; } = string.Empty;
    /// <summary>Kullanıcının aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Kullanıcının rolleri.</summary>
    public List<string> Roller { get; set; } = new();
}
