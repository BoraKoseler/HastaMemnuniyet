using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Doktorlar;

/// <summary>
/// Doktor silme onay sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class SilModel : PageModel
{
    private readonly IDoktorServisi _doktorServisi;

    /// <summary>Yeni bir <see cref="SilModel"/> örneği oluşturur.</summary>
    /// <param name="doktorServisi">Doktor servisi.</param>
    public SilModel(IDoktorServisi doktorServisi)
    {
        _doktorServisi = doktorServisi;
    }

    /// <summary>Silinecek doktor.</summary>
    public DoktorDto? Doktor { get; private set; }

    /// <summary>Silme onayı için doktor verisini yükler.</summary>
    /// <param name="id">Doktor kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Doktor = await _doktorServisi.GetirAsync(id);
        if (Doktor is null)
        {
            TempData["Hata"] = "Doktor bulunamadı.";
            return RedirectToPage("Index");
        }
        return Page();
    }

    /// <summary>Onaylanan silme işlemini gerçekleştirir.</summary>
    /// <param name="id">Doktor kimliği.</param>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _doktorServisi.SilAsync(id);
            TempData["Basari"] = "Doktor başarıyla silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Doktor silinemedi: {ex.Message}";
        }
        return RedirectToPage("Index");
    }
}
