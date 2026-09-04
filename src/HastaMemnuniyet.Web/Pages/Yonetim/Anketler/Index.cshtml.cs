using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Anketler;

/// <summary>
/// Anket listeleme sayfasının PageModel'i (Admin ve Kalite Birimi erişebilir).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public IndexModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Listelenen anketler.</summary>
    public IReadOnlyList<AnketDto> Anketler { get; private set; } = new List<AnketDto>();

    /// <summary>Anket listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Anketler = await _anketServisi.TumunuGetirAsync();
    }
}
