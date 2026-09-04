using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Anketler;

/// <summary>
/// Yeni anket ekleme sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class EkleModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public EkleModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Formdan gelen anket oluşturma verisi.</summary>
    [BindProperty]
    public AnketOlusturDto Girdi { get; set; } = new();

    /// <summary>Anket türü seçenekleri.</summary>
    public SelectList TurSecenekleri { get; private set; } = default!;

    /// <summary>Ekleme formunu görüntüler.</summary>
    public void OnGet()
    {
        TurSecenekleriniYukle();
    }

    /// <summary>Ekleme formunu işler ve yeni anket oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            TurSecenekleriniYukle();
            return Page();
        }

        try
        {
            var anket = await _anketServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "Anket başarıyla eklendi. Şimdi sorularını ekleyebilirsiniz.";
            return RedirectToPage("Sorular", new { id = anket.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TurSecenekleriniYukle();
            return Page();
        }
    }

    private void TurSecenekleriniYukle()
    {
        var ogeler = Enum.GetValues<AnketTuru>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = GorunumAdlari.AnketTuruAdi(t)
            });
        TurSecenekleri = new SelectList(ogeler, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.AnketTuru);
    }
}
