using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Kullanicilar;

/// <summary>
/// Yeni kullanıcı ekleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class EkleModel : PageModel
{
    private readonly IKullaniciServisi _kullaniciServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="kullaniciServisi">Kullanıcı servisi.</param>
    public EkleModel(IKullaniciServisi kullaniciServisi)
    {
        _kullaniciServisi = kullaniciServisi;
    }

    /// <summary>Formdan gelen kullanıcı oluşturma verisi.</summary>
    [BindProperty]
    public KullaniciOlusturDto Girdi { get; set; } = new();

    /// <summary>Rol seçenekleri.</summary>
    public SelectList RolSecenekleri { get; private set; } = default!;

    /// <summary>Ekleme formunu görüntüler.</summary>
    public void OnGet()
    {
        RolSecenekleriniYukle();
    }

    /// <summary>Ekleme formunu işler ve yeni kullanıcı oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            RolSecenekleriniYukle();
            return Page();
        }

        try
        {
            await _kullaniciServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "Kullanıcı başarıyla eklendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            RolSecenekleriniYukle();
            return Page();
        }
    }

    private void RolSecenekleriniYukle()
    {
        RolSecenekleri = new SelectList(RolSabitleri.TumRoller, Girdi.Rol);
    }
}
