using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Yanitlar;

/// <summary>
/// Bir anket yanıtının tüm cevaplarını gösteren detay sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class DetayModel : PageModel
{
    private readonly IYanitServisi _yanitServisi;

    /// <summary>Yeni bir <see cref="DetayModel"/> örneği oluşturur.</summary>
    /// <param name="yanitServisi">Yanıt servisi.</param>
    public DetayModel(IYanitServisi yanitServisi)
    {
        _yanitServisi = yanitServisi;
    }

    /// <summary>Görüntülenen yanıt.</summary>
    public AnketYanitiDto? Yanit { get; private set; }

    /// <summary>Yanıt detayını getirir.</summary>
    /// <param name="id">Yanıt kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Yanit = await _yanitServisi.DetayGetirAsync(id);
        if (Yanit is null)
        {
            TempData["Hata"] = "Yanıt bulunamadı.";
            return RedirectToPage("Index");
        }
        return Page();
    }
}
