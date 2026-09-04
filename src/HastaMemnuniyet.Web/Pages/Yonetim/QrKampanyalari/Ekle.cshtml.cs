using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.QrKampanyalari;

/// <summary>
/// Yeni QR anket kampanyası oluşturma sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class EkleModel : PageModel
{
    private readonly IQrKampanyaServisi _qrKampanyaServisi;
    private readonly IAnketServisi _anketServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly IBirimServisi _birimServisi;
    private readonly IDoktorServisi _doktorServisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="qrKampanyaServisi">QR kampanya servisi.</param>
    /// <param name="anketServisi">Anket servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    /// <param name="birimServisi">Birim servisi.</param>
    /// <param name="doktorServisi">Doktor servisi.</param>
    public EkleModel(
        IQrKampanyaServisi qrKampanyaServisi,
        IAnketServisi anketServisi,
        IHastaneServisi hastaneServisi,
        IBirimServisi birimServisi,
        IDoktorServisi doktorServisi)
    {
        _qrKampanyaServisi = qrKampanyaServisi;
        _anketServisi = anketServisi;
        _hastaneServisi = hastaneServisi;
        _birimServisi = birimServisi;
        _doktorServisi = doktorServisi;
    }

    /// <summary>Formdan gelen kampanya oluşturma verisi.</summary>
    [BindProperty]
    public QrKampanyaOlusturDto Girdi { get; set; } = new();

    /// <summary>Anket seçenekleri.</summary>
    public SelectList AnketSecenekleri { get; private set; } = default!;

    /// <summary>Hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Birim seçenekleri.</summary>
    public SelectList BirimSecenekleri { get; private set; } = default!;

    /// <summary>Doktor seçenekleri.</summary>
    public SelectList DoktorSecenekleri { get; private set; } = default!;

    /// <summary>Oluşturma formunu görüntüler.</summary>
    public async Task OnGetAsync()
    {
        await SecenekleriYukleAsync();
    }

    /// <summary>Oluşturma formunu işler ve kampanyayı kaydeder.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await SecenekleriYukleAsync();
            return Page();
        }

        try
        {
            await _qrKampanyaServisi.OlusturAsync(Girdi);
            TempData["Basari"] = "QR kampanyası başarıyla oluşturuldu.";
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
        AnketSecenekleri = new SelectList(anketler.Where(a => a.AktifMi), nameof(AnketDto.Id), nameof(AnketDto.Ad), Girdi.AnketId);

        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), Girdi.HastaneId);

        var birimler = await _birimServisi.TumunuGetirAsync();
        BirimSecenekleri = new SelectList(
            birimler.Select(b => new { b.Id, Ad = b.HastaneAdi is null ? b.Ad : $"{b.HastaneAdi} - {b.Ad}" }),
            "Id", "Ad", Girdi.BirimId);

        var doktorlar = await _doktorServisi.TumunuGetirAsync();
        DoktorSecenekleri = new SelectList(
            doktorlar.Select(d => new { d.Id, d.TamAd }),
            "Id", "TamAd", Girdi.DoktorId);
    }
}
