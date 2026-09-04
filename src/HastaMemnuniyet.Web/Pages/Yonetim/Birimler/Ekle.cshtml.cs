using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Birimler;

/// <summary>
/// Yeni birim ekleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class EkleModel : PageModel
{
    private readonly IBirimServisi _birimServisi;
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="birimServisi">Birim servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public EkleModel(IBirimServisi birimServisi, IHastaneServisi hastaneServisi)
    {
        _birimServisi = birimServisi;
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Formdan gelen birim oluşturma verisi.</summary>
    [BindProperty]
    public BirimOlusturDto Girdi { get; set; } = new();

    /// <summary>Hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Ekleme formunu görüntüler.</summary>
    public async Task OnGetAsync()
    {
        await HastaneleriYukleAsync();
    }

    /// <summary>Ekleme formunu işler ve yeni birim oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await HastaneleriYukleAsync();
            return Page();
        }

        try
        {
            await _birimServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "Birim başarıyla eklendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await HastaneleriYukleAsync();
            return Page();
        }
    }

    private async Task HastaneleriYukleAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), Girdi.HastaneId);
    }
}
