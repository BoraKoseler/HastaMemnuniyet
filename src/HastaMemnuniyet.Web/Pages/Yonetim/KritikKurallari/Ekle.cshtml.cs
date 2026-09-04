using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.KritikKurallari;

/// <summary>
/// Yeni kritik geri bildirim kuralı oluşturma sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class EkleModel : PageModel
{
    private readonly IKritikGeriBildirimServisi _kritikServisi;
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="kritikServisi">Kritik geri bildirim servisi.</param>
    /// <param name="anketServisi">Anket servisi.</param>
    public EkleModel(IKritikGeriBildirimServisi kritikServisi, IAnketServisi anketServisi)
    {
        _kritikServisi = kritikServisi;
        _anketServisi = anketServisi;
    }

    /// <summary>Formdan gelen kural oluşturma verisi.</summary>
    [BindProperty]
    public KritikKuralOlusturDto Girdi { get; set; } = new();

    /// <summary>Anket seçenekleri.</summary>
    public SelectList AnketSecenekleri { get; private set; } = default!;

    /// <summary>Kural tipi seçenekleri.</summary>
    public SelectList KuralTipiSecenekleri { get; private set; } = default!;

    /// <summary>Oluşturma formunu görüntüler.</summary>
    public async Task OnGetAsync()
    {
        await SecenekleriYukleAsync();
    }

    /// <summary>Oluşturma formunu işler ve kuralı kaydeder.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await SecenekleriYukleAsync();
            return Page();
        }

        try
        {
            await _kritikServisi.KuralOlusturAsync(Girdi);
            TempData["Basari"] = "Kural başarıyla oluşturuldu.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await SecenekleriYukleAsync();
            return Page();
        }
    }

    private async Task SecenekleriYukleAsync()
    {
        var anketler = await _anketServisi.TumunuGetirAsync();
        AnketSecenekleri = new SelectList(anketler, nameof(AnketDto.Id), nameof(AnketDto.Ad), Girdi.AnketId);

        var tipOgeleri = Enum.GetValues<KuralTipi>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = GorunumAdlari.KuralTipiAdi(t)
            });
        KuralTipiSecenekleri = new SelectList(tipOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.KuralTipi);
    }
}
