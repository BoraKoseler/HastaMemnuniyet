using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Davetler;

/// <summary>
/// Davet listeleme sayfasının PageModel'i. Duruma göre filtreleme sağlar (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IDavetServisi _davetServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="davetServisi">Davet servisi.</param>
    public IndexModel(IDavetServisi davetServisi)
    {
        _davetServisi = davetServisi;
    }

    /// <summary>Listelenen davetler.</summary>
    public IReadOnlyList<AnketDavetiDto> Davetler { get; private set; } = new List<AnketDavetiDto>();

    /// <summary>Durum filtresi seçenekleri.</summary>
    public SelectList DurumSecenekleri { get; private set; } = default!;

    /// <summary>Seçili durum filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public DavetDurumu? Durum { get; set; }

    /// <summary>İşlem sonucu başarı mesajı.</summary>
    [TempData]
    public string? BasariMesaji { get; set; }

    /// <summary>İşlem sonucu hata mesajı.</summary>
    [TempData]
    public string? HataMesaji { get; set; }

    /// <summary>Davet listesini (varsa filtreli) getirir.</summary>
    public async Task OnGetAsync()
    {
        var ogeler = Enum.GetValues<DavetDurumu>()
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = GorunumAdlari.DavetDurumuAdi(d)
            });
        DurumSecenekleri = new SelectList(ogeler, nameof(SelectListItem.Value), nameof(SelectListItem.Text), Durum.HasValue ? (int)Durum.Value : null);

        var tumu = await _davetServisi.TumunuGetirAsync();
        Davetler = Durum.HasValue
            ? tumu.Where(d => d.Durum == Durum.Value).ToList()
            : tumu;
    }

    /// <summary>
    /// Seçili davete hatırlatma gönderir. İş kuralı ihlallerinde kullanıcıya hata mesajı gösterir.
    /// </summary>
    /// <param name="id">Hatırlatma gönderilecek davetin kimliği.</param>
    /// <returns>Aynı sayfaya yönlendirme sonucu.</returns>
    public async Task<IActionResult> OnPostHatirlatmaAsync(int id)
    {
        try
        {
            await _davetServisi.HatirlatmaGonderAsync(id);
            BasariMesaji = "Hatırlatma başarıyla gönderildi.";
        }
        catch (InvalidOperationException ex)
        {
            HataMesaji = ex.Message;
        }

        return RedirectToPage(new { Durum });
    }
}
