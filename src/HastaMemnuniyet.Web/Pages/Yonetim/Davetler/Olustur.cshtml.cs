using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Davetler;

/// <summary>
/// Yeni davet oluşturma sayfasının PageModel'i. Kademeli (hastane-birim-doktor) seçim ve SMS
/// gönderimini yönetir (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class OlusturModel : PageModel
{
    private readonly IDavetServisi _davetServisi;
    private readonly IAnketServisi _anketServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;

    /// <summary>Yeni bir <see cref="OlusturModel"/> örneği oluşturur.</summary>
    /// <param name="davetServisi">Davet servisi.</param>
    /// <param name="anketServisi">Anket servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    /// <param name="kullaniciYoneticisi">Kullanıcı yöneticisi.</param>
    public OlusturModel(
        IDavetServisi davetServisi,
        IAnketServisi anketServisi,
        IHastaneServisi hastaneServisi,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        _davetServisi = davetServisi;
        _anketServisi = anketServisi;
        _hastaneServisi = hastaneServisi;
        _kullaniciYoneticisi = kullaniciYoneticisi;
    }

    /// <summary>Formdan gelen davet oluşturma verisi.</summary>
    [BindProperty]
    public AnketDavetiOlusturDto Girdi { get; set; } = new();

    /// <summary>Anket seçenekleri.</summary>
    public SelectList AnketSecenekleri { get; private set; } = default!;

    /// <summary>Hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Gönderim kanalı seçenekleri.</summary>
    public SelectList KanalSecenekleri { get; private set; } = default!;

    /// <summary>Oluşturma formunu görüntüler.</summary>
    public async Task OnGetAsync()
    {
        await SecenekleriYukleAsync();
    }

    /// <summary>Oluşturma formunu işler, davet oluşturur ve SMS gönderimini tetikler.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await SecenekleriYukleAsync();
            return Page();
        }

        try
        {
            var kullaniciId = _kullaniciYoneticisi.GetUserId(User);
            await _davetServisi.OlusturAsync(Girdi, kullaniciId);
            TempData["Basari"] = "Davet başarıyla oluşturuldu ve gönderildi.";
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
        AnketSecenekleri = new SelectList(
            anketler.Where(a => a.AktifMi),
            nameof(AnketDto.Id),
            nameof(AnketDto.Ad),
            Girdi.AnketId);

        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), Girdi.HastaneId);

        var kanalOgeleri = Enum.GetValues<GonderimKanali>()
            .Select(k => new SelectListItem
            {
                Value = ((int)k).ToString(),
                Text = GorunumAdlari.KanalAdi(k)
            });
        KanalSecenekleri = new SelectList(kanalOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.GonderimKanali);
    }
}
