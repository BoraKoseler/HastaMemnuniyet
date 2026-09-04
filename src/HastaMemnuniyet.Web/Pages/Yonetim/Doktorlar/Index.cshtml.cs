using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Doktorlar;

/// <summary>
/// Doktor listeleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IDoktorServisi _doktorServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="doktorServisi">Doktor servisi.</param>
    public IndexModel(IDoktorServisi doktorServisi)
    {
        _doktorServisi = doktorServisi;
    }

    /// <summary>Listelenen doktorlar.</summary>
    public IReadOnlyList<DoktorDto> Doktorlar { get; private set; } = new List<DoktorDto>();

    /// <summary>Doktor listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Doktorlar = await _doktorServisi.TumunuGetirAsync();
    }
}
