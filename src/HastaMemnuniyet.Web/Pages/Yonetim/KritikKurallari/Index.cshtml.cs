using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.KritikKurallari;

/// <summary>
/// Kritik geri bildirim kurallarını listeleyen ve aktiflik durumunu yöneten sayfanın
/// PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IKritikGeriBildirimServisi _kritikServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="kritikServisi">Kritik geri bildirim servisi.</param>
    public IndexModel(IKritikGeriBildirimServisi kritikServisi)
    {
        _kritikServisi = kritikServisi;
    }

    /// <summary>Listelenen kurallar.</summary>
    public IReadOnlyList<KritikGeriBildirimKuraliDto> Kurallar { get; private set; } = new List<KritikGeriBildirimKuraliDto>();

    /// <summary>Kural listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Kurallar = await _kritikServisi.KurallariGetirAsync();
    }

    /// <summary>Bir kuralın aktiflik durumunu değiştirir.</summary>
    /// <param name="id">Kural kimliği.</param>
    /// <param name="aktifMi">Yeni aktiflik durumu.</param>
    public async Task<IActionResult> OnPostAktiflikAsync(int id, bool aktifMi)
    {
        await _kritikServisi.KuralAktiflikGuncelleAsync(id, aktifMi);
        TempData["Basari"] = "Kural durumu güncellendi.";
        return RedirectToPage("Index");
    }
}
