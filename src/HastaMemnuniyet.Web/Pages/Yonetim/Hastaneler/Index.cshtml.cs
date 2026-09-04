using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Hastaneler;

/// <summary>
/// Hastane listeleme sayfasının PageModel'i (yalnızca Admin erişebilir).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public IndexModel(IHastaneServisi hastaneServisi)
    {
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Listelenen hastaneler.</summary>
    public IReadOnlyList<HastaneDto> Hastaneler { get; private set; } = new List<HastaneDto>();

    /// <summary>Hastane listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Hastaneler = await _hastaneServisi.TumunuGetirAsync();
    }
}
