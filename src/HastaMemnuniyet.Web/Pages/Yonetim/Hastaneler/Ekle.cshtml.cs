using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Hastaneler;

/// <summary>
/// Yeni hastane ekleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class EkleModel : PageModel
{
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public EkleModel(IHastaneServisi hastaneServisi)
    {
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Formdan gelen hastane oluşturma verisi.</summary>
    [BindProperty]
    public HastaneOlusturDto Girdi { get; set; } = new();

    /// <summary>Ekleme formunu görüntüler.</summary>
    public void OnGet()
    {
    }

    /// <summary>Ekleme formunu işler ve yeni hastane oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _hastaneServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "Hastane başarıyla eklendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
