using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.DenetimKayitlari;

/// <summary>
/// Denetim kayıtlarını listeleyen yönetim sayfasının PageModel'i. Yalnızca yöneticiler erişebilir.
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IDenetimServisi _denetimServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="denetimServisi">Denetim kaydı servisi.</param>
    public IndexModel(IDenetimServisi denetimServisi)
    {
        _denetimServisi = denetimServisi;
    }

    /// <summary>Görüntülenen denetim kayıtları.</summary>
    public IReadOnlyList<DenetimKaydiDto> Kayitlar { get; private set; } = new List<DenetimKaydiDto>();

    /// <summary>İşlem türü filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public string? Islem { get; set; }

    /// <summary>Kullanıcı adı filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public string? KullaniciAdi { get; set; }

    /// <summary>Denetim kayıtlarını filtrelere göre yükler.</summary>
    /// <returns>Sayfa sonucu.</returns>
    public async Task OnGetAsync()
    {
        Kayitlar = await _denetimServisi.SonKayitlariGetirAsync(200, Islem, KullaniciAdi);
    }
}
