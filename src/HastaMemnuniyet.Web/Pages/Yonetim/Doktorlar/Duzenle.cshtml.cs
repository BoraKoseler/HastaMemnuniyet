using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Doktorlar;

/// <summary>
/// Doktor düzenleme sayfasının PageModel'i. Birim atamalarını da yönetir (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class DuzenleModel : PageModel
{
    private readonly IDoktorServisi _doktorServisi;
    private readonly IBirimServisi _birimServisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="doktorServisi">Doktor servisi.</param>
    /// <param name="birimServisi">Birim servisi.</param>
    public DuzenleModel(IDoktorServisi doktorServisi, IBirimServisi birimServisi)
    {
        _doktorServisi = doktorServisi;
        _birimServisi = birimServisi;
    }

    /// <summary>Düzenlenen doktor verisi.</summary>
    [BindProperty]
    public DoktorDto Girdi { get; set; } = new();

    /// <summary>Hastaneye göre gruplanmış birim seçenekleri.</summary>
    public List<IGrouping<string, BirimDto>> BirimGruplari { get; private set; } = new();

    /// <summary>Düzenleme formunu ilgili doktor verisiyle görüntüler.</summary>
    /// <param name="id">Doktor kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var doktor = await _doktorServisi.GetirAsync(id);
        if (doktor is null)
        {
            TempData["Hata"] = "Doktor bulunamadı.";
            return RedirectToPage("Index");
        }

        Girdi = doktor;
        await BirimleriYukleAsync();
        return Page();
    }

    /// <summary>Düzenleme formunu işler ve doktoru günceller.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await BirimleriYukleAsync();
            return Page();
        }

        try
        {
            await _doktorServisi.GuncelleAsync(Girdi);
            TempData["Basari"] = "Doktor başarıyla güncellendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await BirimleriYukleAsync();
            return Page();
        }
    }

    private async Task BirimleriYukleAsync()
    {
        var birimler = await _birimServisi.TumunuGetirAsync();
        BirimGruplari = birimler
            .GroupBy(b => b.HastaneAdi ?? "Diğer")
            .OrderBy(g => g.Key)
            .ToList();
    }
}
