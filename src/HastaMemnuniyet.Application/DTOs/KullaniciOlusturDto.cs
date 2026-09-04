using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Yeni kullanıcı oluşturmak için kullanılan veri transfer nesnesi.</summary>
public class KullaniciOlusturDto
{
    /// <summary>Kullanıcı e-postası.</summary>
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    public string Eposta { get; set; } = string.Empty;
    /// <summary>Kullanıcının adı.</summary>
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;
    /// <summary>Kullanıcının soyadı.</summary>
    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(100)]
    public string Soyad { get; set; } = string.Empty;
    /// <summary>Kullanıcı parolası.</summary>
    [Required(ErrorMessage = "Parola zorunludur.")]
    [StringLength(100, MinimumLength = 6)]
    public string Parola { get; set; } = string.Empty;
    /// <summary>Kullanıcıya atanacak rol.</summary>
    [Required(ErrorMessage = "Rol seçimi zorunludur.")]
    public string Rol { get; set; } = string.Empty;
    /// <summary>Kullanıcının aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; } = true;
}
