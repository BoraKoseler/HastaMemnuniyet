using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Hastaneler;

/// <summary>
/// Hastane düzenleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class DuzenleModel : PageModel
{
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public DuzenleModel(IHastaneServisi hastaneServisi)
    {
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Formdan gelen güncelleme verisi.</summary>
    [BindProperty]
    public HastaneGuncelleDto Girdi { get; set; } = new();

    /// <summary>Düzenlenecek hastanenin mevcut verilerini yükler.</summary>
    /// <param name="id">Hastane kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var hastane = await _hastaneServisi.GetirAsync(id);
        if (hastane is null)
        {
            TempData["Hata"] = "Hastane bulunamadı.";
            return RedirectToPage("Index");
        }

        Girdi = new HastaneGuncelleDto
        {
            Id = hastane.Id,
            Ad = hastane.Ad,
            Kod = hastane.Kod,
            Adres = hastane.Adres,
            Telefon = hastane.Telefon,
            AktifMi = hastane.AktifMi
        };
        return Page();
    }

    /// <summary>Güncelleme formunu işler.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _hastaneServisi.GuncelleAsync(Girdi);
            TempData["Basari"] = "Hastane başarıyla güncellendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
