using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Anketler;

/// <summary>
/// Anket silme onay sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class SilModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="SilModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public SilModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Silinecek anket.</summary>
    public AnketDto? Anket { get; private set; }

    /// <summary>Silme onayı için anket verisini yükler.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Anket = await _anketServisi.GetirAsync(id);
        if (Anket is null)
        {
            TempData["Hata"] = "Anket bulunamadı.";
            return RedirectToPage("Index");
        }
        return Page();
    }

    /// <summary>Onaylanan silme işlemini gerçekleştirir.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _anketServisi.SilAsync(id);
            TempData["Basari"] = "Anket başarıyla silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Anket silinemedi: {ex.Message}";
        }
        return RedirectToPage("Index");
    }
}
