using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Anket;

/// <summary>
/// Anonim hastanın anketi doldurduğu sayfanın PageModel'i. Soruları tipine göre işler ve
/// cevapları kaydeder.
/// </summary>
[AllowAnonymous]
public class DoldurModel : PageModel
{
    private readonly IAnketDoldurmaServisi _anketDoldurmaServisi;

    /// <summary>Yeni bir <see cref="DoldurModel"/> örneği oluşturur.</summary>
    /// <param name="anketDoldurmaServisi">Anket doldurma servisi.</param>
    public DoldurModel(IAnketDoldurmaServisi anketDoldurmaServisi)
    {
        _anketDoldurmaServisi = anketDoldurmaServisi;
    }

    /// <summary>Doldurulacak anket.</summary>
    public AnketDto? Anket { get; private set; }

    /// <summary>Formdan gelen cevap verisi.</summary>
    [BindProperty]
    public AnketDoldurDto Girdi { get; set; } = new();

    /// <summary>Token'ı doğrular ve anket sorularını yükler.</summary>
    /// <param name="token">Davet token değeri.</param>
    public async Task<IActionResult> OnGetAsync(string token)
    {
        if (!await _anketDoldurmaServisi.TokenGecerliMiAsync(token))
        {
            return RedirectToPage("SuresiDolmus");
        }

        Anket = await _anketDoldurmaServisi.AnketBilgileriniGetirAsync(token);
        if (Anket is null)
        {
            return RedirectToPage("SuresiDolmus");
        }

        Girdi.Token = token;
        return Page();
    }

    /// <summary>Cevapları doğrular ve kaydeder.</summary>
    /// <param name="token">Davet token değeri.</param>
    public async Task<IActionResult> OnPostAsync(string token)
    {
        Girdi.Token = token;

        if (!await _anketDoldurmaServisi.TokenGecerliMiAsync(token))
        {
            return RedirectToPage("SuresiDolmus");
        }

        Anket = await _anketDoldurmaServisi.AnketBilgileriniGetirAsync(token);
        if (Anket is null)
        {
            return RedirectToPage("SuresiDolmus");
        }

        DogrulamaYap();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var kullaniciAjan = Request.Headers.UserAgent.ToString();
            await _anketDoldurmaServisi.YanitKaydetAsync(Girdi, ip, kullaniciAjan);
            return RedirectToPage("Tesekkur", new { token });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Yanıtınız kaydedilemedi: {ex.Message}");
            return Page();
        }
    }

    /// <summary>Zorunlu soruların cevaplanıp cevaplanmadığını doğrular.</summary>
    private void DogrulamaYap()
    {
        if (Anket is null)
        {
            return;
        }

        var cevapHaritasi = Girdi.Cevaplar.ToDictionary(c => c.SoruId, c => c);

        foreach (var soru in Anket.Sorular.Where(s => s.ZorunluMu))
        {
            if (!cevapHaritasi.TryGetValue(soru.Id, out var cevap) || !CevapVarMi(soru.SoruTipi, cevap))
            {
                ModelState.AddModelError(string.Empty, $"\"{soru.SoruMetni}\" sorusu zorunludur.");
            }
        }
    }

    private static bool CevapVarMi(SoruTipi tip, AnketDoldurCevapDto cevap) => tip switch
    {
        SoruTipi.Puanlama => cevap.PuanDegeri.HasValue,
        SoruTipi.TekSecim => cevap.SecenekId.HasValue,
        SoruTipi.CokluSecim => cevap.SeciliSecenekIdleri.Count > 0,
        SoruTipi.EvetHayir => cevap.BoolDegeri.HasValue,
        SoruTipi.AcikUclu => !string.IsNullOrWhiteSpace(cevap.MetinDegeri),
        _ => false
    };
}
