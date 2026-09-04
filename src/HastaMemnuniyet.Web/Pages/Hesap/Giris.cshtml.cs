using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Hesap;

/// <summary>
/// Kullanıcı giriş (oturum açma) sayfasının PageModel'i.
/// ASP.NET Core Identity <see cref="SignInManager{TUser}"/> kullanır.
/// </summary>
[AllowAnonymous]
public class GirisModel : PageModel
{
    private readonly SignInManager<UygulamaKullanicisi> _girisYoneticisi;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;
    private readonly IAuditLogger _denetimKaydedici;

    /// <summary>Yeni bir <see cref="GirisModel"/> örneği oluşturur.</summary>
    /// <param name="girisYoneticisi">Identity oturum yöneticisi.</param>
    /// <param name="kullaniciYoneticisi">Identity kullanıcı yöneticisi.</param>
    /// <param name="denetimKaydedici">Denetim kaydı loglayıcısı.</param>
    public GirisModel(
        SignInManager<UygulamaKullanicisi> girisYoneticisi,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi,
        IAuditLogger denetimKaydedici)
    {
        _girisYoneticisi = girisYoneticisi;
        _kullaniciYoneticisi = kullaniciYoneticisi;
        _denetimKaydedici = denetimKaydedici;
    }

    /// <summary>Formdan gelen giriş bilgilerini taşır.</summary>
    [BindProperty]
    public GirisGirdisi Girdi { get; set; } = new();

    /// <summary>Başarılı girişten sonra yönlendirilecek adres.</summary>
    public string? DonusAdresi { get; set; }

    /// <summary>Giriş formu için veri modeli.</summary>
    public class GirisGirdisi
    {
        /// <summary>Kullanıcı e-postası.</summary>
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [Display(Name = "E-posta")]
        public string Eposta { get; set; } = string.Empty;

        /// <summary>Kullanıcı parolası.</summary>
        [Required(ErrorMessage = "Parola zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Parola { get; set; } = string.Empty;

        /// <summary>Beni hatırla seçeneği.</summary>
        [Display(Name = "Beni hatırla")]
        public bool BeniHatirla { get; set; }
    }

    /// <summary>Giriş sayfasını görüntüler.</summary>
    /// <param name="donusAdresi">Girişten sonra yönlendirilecek adres.</param>
    public IActionResult OnGet(string? donusAdresi = null)
    {
        // Zaten giriş yapmış kullanıcı doğrudan dashboard'a gönderilir.
        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToPage("/Yonetim/Dashboard/Index");
        }
        DonusAdresi = donusAdresi;
        return Page();
    }

    /// <summary>Giriş formunu işler ve kullanıcıyı doğrular.</summary>
    /// <param name="donusAdresi">Girişten sonra yönlendirilecek adres.</param>
    public async Task<IActionResult> OnPostAsync(string? donusAdresi = null)
    {
        DonusAdresi = donusAdresi;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var kullanici = await _kullaniciYoneticisi.FindByEmailAsync(Girdi.Eposta);
        if (kullanici is null || !kullanici.AktifMi)
        {
            ModelState.AddModelError(string.Empty, "E-posta veya parola hatalı ya da hesap pasif.");
            return Page();
        }

        var sonuc = await _girisYoneticisi.PasswordSignInAsync(
            kullanici.UserName!, Girdi.Parola, Girdi.BeniHatirla, lockoutOnFailure: false);

        if (sonuc.Succeeded)
        {
            await _denetimKaydedici.LoglaAsync(
                "Giris",
                tablo: "UygulamaKullanicisi",
                kayitId: kullanici.Id,
                detay: $"{kullanici.Email} kullanıcısı sisteme giriş yaptı.");

            if (!string.IsNullOrWhiteSpace(donusAdresi) && Url.IsLocalUrl(donusAdresi))
            {
                return LocalRedirect(donusAdresi);
            }
            return RedirectToPage("/Yonetim/Dashboard/Index");
        }

        ModelState.AddModelError(string.Empty, "E-posta veya parola hatalı.");
        return Page();
    }
}
