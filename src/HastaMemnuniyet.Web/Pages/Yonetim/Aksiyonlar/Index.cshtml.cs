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

namespace HastaMemnuniyet.Web.Pages.Yonetim.Aksiyonlar;

/// <summary>
/// İyileştirme aksiyonlarını listeleyen ve filtreleyen sayfanın PageModel'i
/// (Admin, Kalite Birimi ve Birim Yöneticisi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.BirimYoneticisi)]
public class IndexModel : PageModel
{
    private readonly IAksiyonServisi _aksiyonServisi;
    private readonly IKullaniciKapsamServisi _kapsamServisi;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="aksiyonServisi">Aksiyon servisi.</param>
    /// <param name="kapsamServisi">Kullanıcı kapsam servisi.</param>
    /// <param name="kullaniciYoneticisi">Kullanıcı yöneticisi.</param>
    public IndexModel(
        IAksiyonServisi aksiyonServisi,
        IKullaniciKapsamServisi kapsamServisi,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        _aksiyonServisi = aksiyonServisi;
        _kapsamServisi = kapsamServisi;
        _kullaniciYoneticisi = kullaniciYoneticisi;
    }

    /// <summary>Listelenen aksiyonlar.</summary>
    public IReadOnlyList<IyilestirmeAksiyonuDto> Aksiyonlar { get; private set; } = new List<IyilestirmeAksiyonuDto>();

    /// <summary>Durum filtre seçenekleri.</summary>
    public SelectList DurumSecenekleri { get; private set; } = default!;

    /// <summary>Durum filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public AksiyonDurumu? Durum { get; set; }

    /// <summary>True ise yalnızca gecikmiş aksiyonlar gösterilir.</summary>
    [BindProperty(SupportsGet = true)]
    public bool SadeceGecikmis { get; set; }

    /// <summary>Aksiyon listesini (kullanıcı kapsamına ve filtrelere göre) getirir.</summary>
    public async Task OnGetAsync()
    {
        var durumOgeleri = Enum.GetValues<AksiyonDurumu>()
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = GorunumAdlari.AksiyonDurumuAdi(d)
            });
        DurumSecenekleri = new SelectList(durumOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), Durum.HasValue ? (int)Durum.Value : null);

        var tamYetkili = User.IsInRole(RolSabitleri.Admin) || User.IsInRole(RolSabitleri.KaliteBirimi);
        var kullaniciId = _kullaniciYoneticisi.GetUserId(User) ?? string.Empty;
        var kapsam = await _kapsamServisi.KapsamGetirAsync(kullaniciId, tamYetkili);

        Aksiyonlar = await _aksiyonServisi.TumunuGetirAsync(Durum, SadeceGecikmis, null, kapsam);
    }
}
