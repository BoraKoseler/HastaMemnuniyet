using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.KritikGeriBildirimler;

/// <summary>
/// Bir kritik geri bildirimin ayrıntılarını (cevaplar ve bağlı aksiyonlar) gösteren
/// sayfanın PageModel'i (Admin, Kalite Birimi ve Birim Yöneticisi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.BirimYoneticisi)]
public class DetayModel : PageModel
{
    private readonly IKritikGeriBildirimServisi _kritikServisi;

    /// <summary>Yeni bir <see cref="DetayModel"/> örneği oluşturur.</summary>
    /// <param name="kritikServisi">Kritik geri bildirim servisi.</param>
    public DetayModel(IKritikGeriBildirimServisi kritikServisi)
    {
        _kritikServisi = kritikServisi;
    }

    /// <summary>Görüntülenen kritik geri bildirim.</summary>
    public KritikGeriBildirimDto? KritikGeriBildirim { get; private set; }

    /// <summary>Kritik geri bildirim ayrıntılarını getirir.</summary>
    /// <param name="id">Kritik geri bildirim kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        KritikGeriBildirim = await _kritikServisi.DetayGetirAsync(id);
        if (KritikGeriBildirim is null)
        {
            return NotFound();
        }
        return Page();
    }
}
