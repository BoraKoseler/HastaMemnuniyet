using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Anketler;

/// <summary>
/// Bir anketin aynı sürüm zincirindeki geçmiş sürümlerini listeleyen sayfanın PageModel'i
/// (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class SurumlerModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="SurumlerModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public SurumlerModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Görüntülenen anketin adı.</summary>
    public string AnketAdi { get; private set; } = string.Empty;

    /// <summary>Aynı zincire ait anket sürümleri (yeni sürümden eskiye).</summary>
    public IReadOnlyList<AnketDto> Surumler { get; private set; } = new List<AnketDto>();

    /// <summary>Verilen anketin sürüm geçmişini getirir.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var anket = await _anketServisi.GetirAsync(id);
        if (anket is null)
        {
            TempData["Hata"] = "Anket bulunamadı.";
            return RedirectToPage("Index");
        }

        AnketAdi = anket.Ad;
        Surumler = await _anketServisi.GecmisSurumleriGetirAsync(id);
        return Page();
    }
}
