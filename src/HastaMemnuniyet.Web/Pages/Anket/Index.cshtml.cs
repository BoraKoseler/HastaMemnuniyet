using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Anket;

/// <summary>
/// Anonim hasta için anket karşılama sayfasının PageModel'i. Token doğrulaması yapar.
/// </summary>
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IAnketDoldurmaServisi _anketDoldurmaServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="anketDoldurmaServisi">Anket doldurma servisi.</param>
    public IndexModel(IAnketDoldurmaServisi anketDoldurmaServisi)
    {
        _anketDoldurmaServisi = anketDoldurmaServisi;
    }

    /// <summary>Görüntülenen anket.</summary>
    public AnketDto? Anket { get; private set; }

    /// <summary>Davet token değeri.</summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>Token'ı doğrular ve anket bilgilerini yükler.</summary>
    /// <param name="token">Davet token değeri.</param>
    public async Task<IActionResult> OnGetAsync(string token)
    {
        Token = token;

        if (!await _anketDoldurmaServisi.TokenGecerliMiAsync(token))
        {
            return RedirectToPage("SuresiDolmus");
        }

        Anket = await _anketDoldurmaServisi.AnketBilgileriniGetirAsync(token);
        if (Anket is null)
        {
            return RedirectToPage("SuresiDolmus");
        }

        return Page();
    }
}
