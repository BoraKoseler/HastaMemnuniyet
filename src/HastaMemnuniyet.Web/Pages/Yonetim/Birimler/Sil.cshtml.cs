using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Birimler;

/// <summary>
/// Birim silme onay sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class SilModel : PageModel
{
    private readonly IBirimServisi _birimServisi;

    /// <summary>Yeni bir <see cref="SilModel"/> örneği oluşturur.</summary>
    /// <param name="birimServisi">Birim servisi.</param>
    public SilModel(IBirimServisi birimServisi)
    {
        _birimServisi = birimServisi;
    }

    /// <summary>Silinecek birim.</summary>
    public BirimDto? Birim { get; private set; }

    /// <summary>Silme onayı için birim verisini yükler.</summary>
    /// <param name="id">Birim kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Birim = await _birimServisi.GetirAsync(id);
        if (Birim is null)
        {
            TempData["Hata"] = "Birim bulunamadı.";
            return RedirectToPage("Index");
        }
        return Page();
    }

    /// <summary>Onaylanan silme işlemini gerçekleştirir.</summary>
    /// <param name="id">Birim kimliği.</param>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _birimServisi.SilAsync(id);
            TempData["Basari"] = "Birim başarıyla silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Birim silinemedi: {ex.Message}";
        }
        return RedirectToPage("Index");
    }
}
