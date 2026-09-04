using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Hastaneler;

/// <summary>
/// Hastane silme onay sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class SilModel : PageModel
{
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="SilModel"/> örneği oluşturur.</summary>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public SilModel(IHastaneServisi hastaneServisi)
    {
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Silinecek hastane.</summary>
    public HastaneDto? Hastane { get; private set; }

    /// <summary>Silme onayı için hastane verisini yükler.</summary>
    /// <param name="id">Hastane kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hastane = await _hastaneServisi.GetirAsync(id);
        if (Hastane is null)
        {
            TempData["Hata"] = "Hastane bulunamadı.";
            return RedirectToPage("Index");
        }
        return Page();
    }

    /// <summary>Onaylanan silme işlemini gerçekleştirir.</summary>
    /// <param name="id">Hastane kimliği.</param>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _hastaneServisi.SilAsync(id);
            TempData["Basari"] = "Hastane başarıyla silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Hastane silinemedi: {ex.Message}";
        }
        return RedirectToPage("Index");
    }
}
