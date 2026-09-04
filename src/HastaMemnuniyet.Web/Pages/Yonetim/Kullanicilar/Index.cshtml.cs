using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Kullanicilar;

/// <summary>
/// Kullanıcı listeleme sayfasının PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IKullaniciServisi _kullaniciServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="kullaniciServisi">Kullanıcı servisi.</param>
    public IndexModel(IKullaniciServisi kullaniciServisi)
    {
        _kullaniciServisi = kullaniciServisi;
    }

    /// <summary>Listelenen kullanıcılar.</summary>
    public IReadOnlyList<KullaniciDto> Kullanicilar { get; private set; } = new List<KullaniciDto>();

    /// <summary>Kullanıcı listesini getirir.</summary>
    public async Task OnGetAsync()
    {
        Kullanicilar = await _kullaniciServisi.TumunuGetirAsync();
    }
}
