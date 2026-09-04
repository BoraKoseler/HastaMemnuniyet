using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.KritikGeriBildirimler;

/// <summary>
/// Kritik geri bildirimleri listeleyen ve filtreleyen sayfanın PageModel'i
/// (Admin, Kalite Birimi ve Birim Yöneticisi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.BirimYoneticisi)]
public class IndexModel : PageModel
{
    private readonly IKritikGeriBildirimServisi _kritikServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly IKullaniciKapsamServisi _kapsamServisi;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="kritikServisi">Kritik geri bildirim servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    /// <param name="kapsamServisi">Kullanıcı kapsam servisi.</param>
    /// <param name="kullaniciYoneticisi">Kullanıcı yöneticisi.</param>
    public IndexModel(
        IKritikGeriBildirimServisi kritikServisi,
        IHastaneServisi hastaneServisi,
        IKullaniciKapsamServisi kapsamServisi,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        _kritikServisi = kritikServisi;
        _hastaneServisi = hastaneServisi;
        _kapsamServisi = kapsamServisi;
        _kullaniciYoneticisi = kullaniciYoneticisi;
    }

    /// <summary>Listelenen kritik geri bildirimler.</summary>
    public IReadOnlyList<KritikGeriBildirimDto> KritikGeriBildirimler { get; private set; } = new List<KritikGeriBildirimDto>();

    /// <summary>Hastane filtre seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Hastane filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public int? HastaneId { get; set; }

    /// <summary>Başlangıç tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public DateTime? Baslangic { get; set; }

    /// <summary>Bitiş tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public DateTime? Bitis { get; set; }

    /// <summary>True ise yalnızca henüz aksiyon açılmamış kritik geri bildirimleri gösterir.</summary>
    [BindProperty(SupportsGet = true)]
    public bool SadeceAcik { get; set; }

    /// <summary>Kritik geri bildirim listesini (kullanıcı kapsamına ve filtrelere göre) getirir.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        var kapsam = await KapsamGetirAsync();

        KritikGeriBildirimler = await _kritikServisi.TumunuGetirAsync(
            HastaneId is > 0 ? HastaneId : null,
            Baslangic,
            Bitis,
            SadeceAcik ? true : null,
            kapsam);
    }

    /// <summary>Oturum açan kullanıcının veri kapsamını çözümler (tam yetkili roller için null).</summary>
    private async Task<KapsamFiltresi?> KapsamGetirAsync()
    {
        var tamYetkili = User.IsInRole(RolSabitleri.Admin) || User.IsInRole(RolSabitleri.KaliteBirimi);
        var kullaniciId = _kullaniciYoneticisi.GetUserId(User) ?? string.Empty;
        return await _kapsamServisi.KapsamGetirAsync(kullaniciId, tamYetkili);
    }
}
