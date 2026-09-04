using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.QrKampanyalari;

/// <summary>
/// QR anket kampanyalarını listeleyen ve QR kodlarını görüntüleyen sayfanın PageModel'i
/// (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IQrKampanyaServisi _qrKampanyaServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="qrKampanyaServisi">QR kampanya servisi.</param>
    public IndexModel(IQrKampanyaServisi qrKampanyaServisi)
    {
        _qrKampanyaServisi = qrKampanyaServisi;
    }

    /// <summary>Listelenen kampanyalar.</summary>
    public IReadOnlyList<QrAnketKampanyasiDto> Kampanyalar { get; private set; } = new List<QrAnketKampanyasiDto>();

    /// <summary>Kampanya listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Kampanyalar = await _qrKampanyaServisi.TumunuGetirAsync();
    }

    /// <summary>Bir kampanyanın aktiflik durumunu değiştirir.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    /// <param name="aktifMi">Yeni aktiflik durumu.</param>
    public async Task<IActionResult> OnPostAktiflikAsync(int id, bool aktifMi)
    {
        await _qrKampanyaServisi.AktiflikGuncelleAsync(id, aktifMi);
        TempData["Basari"] = "Kampanya durumu güncellendi.";
        return RedirectToPage("Index");
    }

    /// <summary>Bir kampanyanın QR kodunu PNG görsel olarak üretir.</summary>
    /// <param name="id">Kampanya kimliği.</param>
    public async Task<IActionResult> OnGetQrKoduAsync(int id)
    {
        var hedefUrl = $"{Request.Scheme}://{Request.Host}/Anket/Qr/{id}";
        var qr = await _qrKampanyaServisi.QrKodUretAsync(id, hedefUrl);
        if (qr is null)
        {
            return NotFound();
        }
        return File(qr, "image/png");
    }
}
