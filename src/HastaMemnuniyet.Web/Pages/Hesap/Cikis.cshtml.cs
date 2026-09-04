using HastaMemnuniyet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Hesap;

/// <summary>
/// Kullanıcı çıkış (oturum kapatma) işlemini yürüten PageModel.
/// </summary>
[AllowAnonymous]
public class CikisModel : PageModel
{
    private readonly SignInManager<UygulamaKullanicisi> _girisYoneticisi;

    /// <summary>Yeni bir <see cref="CikisModel"/> örneği oluşturur.</summary>
    /// <param name="girisYoneticisi">Identity oturum yöneticisi.</param>
    public CikisModel(SignInManager<UygulamaKullanicisi> girisYoneticisi)
    {
        _girisYoneticisi = girisYoneticisi;
    }

    /// <summary>GET ile gelindiğinde ana sayfaya yönlendirir (doğrudan erişim).</summary>
    public IActionResult OnGet() => RedirectToPage("/Index");

    /// <summary>POST ile oturumu kapatır ve ana sayfaya yönlendirir.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        await _girisYoneticisi.SignOutAsync();
        return RedirectToPage("/Index");
    }
}
