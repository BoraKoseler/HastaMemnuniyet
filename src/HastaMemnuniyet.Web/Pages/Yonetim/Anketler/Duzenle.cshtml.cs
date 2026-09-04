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
/// Anket düzenleme sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class DuzenleModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public DuzenleModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Düzenlenen anket verisi.</summary>
    [BindProperty]
    public AnketDto Girdi { get; set; } = new();

    /// <summary>Anket türü seçenekleri.</summary>
    public SelectList TurSecenekleri { get; private set; } = default!;

    /// <summary>Düzenleme formunu ilgili anket verisiyle görüntüler.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var anket = await _anketServisi.GetirAsync(id);
        if (anket is null)
        {
            TempData["Hata"] = "Anket bulunamadı.";
            return RedirectToPage("Index");
        }

        Girdi = anket;
        TurSecenekleriniYukle();
        return Page();
    }

    /// <summary>Düzenleme formunu işler ve anketi günceller.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            TurSecenekleriniYukle();
            return Page();
        }

        try
        {
            await _anketServisi.GuncelleAsync(Girdi);
            TempData["Basari"] = "Anket başarıyla güncellendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TurSecenekleriniYukle();
            return Page();
        }
    }

    /// <summary>Mevcut anketin sorularıyla birlikte yeni bir sürümünü oluşturur.</summary>
    /// <param name="id">Kaynak anket kimliği.</param>
    public async Task<IActionResult> OnPostSurumOlusturAsync(int id)
    {
        try
        {
            var yeniSurum = await _anketServisi.SurumOlusturAsync(id);
            TempData["Basari"] = $"Yeni sürüm (v{yeniSurum.SurumNo}) başarıyla oluşturuldu. Önceki sürüm pasifleştirildi.";
            return RedirectToPage("Duzenle", new { id = yeniSurum.Id });
        }
        catch (Exception ex)
        {
            TempData["Hata"] = ex.Message;
            return RedirectToPage("Duzenle", new { id });
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
