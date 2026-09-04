using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Doktorlar;

/// <summary>
/// Yeni doktor ekleme sayfasının PageModel'i. Birim atamalarını da yönetir (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class EkleModel : PageModel
{
    private readonly IDoktorServisi _doktorServisi;
    private readonly IBirimServisi _birimServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="doktorServisi">Doktor servisi.</param>
    /// <param name="birimServisi">Birim servisi.</param>
    public EkleModel(IDoktorServisi doktorServisi, IBirimServisi birimServisi)
    {
        _doktorServisi = doktorServisi;
        _birimServisi = birimServisi;
    }

    /// <summary>Formdan gelen doktor oluşturma verisi.</summary>
    [BindProperty]
    public DoktorOlusturDto Girdi { get; set; } = new();

    /// <summary>Hastaneye göre gruplanmış birim seçenekleri.</summary>
    public List<IGrouping<string, BirimDto>> BirimGruplari { get; private set; } = new();

    /// <summary>Ekleme formunu görüntüler.</summary>
    public async Task OnGetAsync()
    {
        await BirimleriYukleAsync();
    }

    /// <summary>Ekleme formunu işler ve yeni doktor oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await BirimleriYukleAsync();
            return Page();
        }

        try
        {
            await _doktorServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "Doktor başarıyla eklendi.";
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
