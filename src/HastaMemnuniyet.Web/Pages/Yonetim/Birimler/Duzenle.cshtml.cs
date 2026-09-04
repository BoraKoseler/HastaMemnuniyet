using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Birimler;

/// <summary>
/// Birim düzenleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class DuzenleModel : PageModel
{
    private readonly IBirimServisi _birimServisi;
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="birimServisi">Birim servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public DuzenleModel(IBirimServisi birimServisi, IHastaneServisi hastaneServisi)
    {
        _birimServisi = birimServisi;
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Düzenlenen birim verisi.</summary>
    [BindProperty]
    public BirimDto Girdi { get; set; } = new();

    /// <summary>Hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Düzenleme formunu ilgili birim verisiyle görüntüler.</summary>
    /// <param name="id">Birim kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var birim = await _birimServisi.GetirAsync(id);
        if (birim is null)
        {
            TempData["Hata"] = "Birim bulunamadı.";
            return RedirectToPage("Index");
        }

        Girdi = birim;
        await HastaneleriYukleAsync();
        return Page();
    }

    /// <summary>Düzenleme formunu işler ve birimi günceller.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await HastaneleriYukleAsync();
            return Page();
        }

        try
        {
            await _birimServisi.GuncelleAsync(Girdi);
            TempData["Basari"] = "Birim başarıyla güncellendi.";
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
