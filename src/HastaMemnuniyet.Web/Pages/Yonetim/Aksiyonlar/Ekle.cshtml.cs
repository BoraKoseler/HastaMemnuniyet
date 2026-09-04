using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Aksiyonlar;

/// <summary>
/// Yeni iyileştirme aksiyonu oluşturma sayfasının PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class EkleModel : PageModel
{
    private readonly IAksiyonServisi _aksiyonServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly IBirimServisi _birimServisi;
    private readonly IDoktorServisi _doktorServisi;
    private readonly IKullaniciServisi _kullaniciServisi;
    private readonly IAuditLogger _denetimKaydedici;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;

    /// <summary>Yeni bir <see cref="EkleModel"/> örneği oluşturur.</summary>
    /// <param name="aksiyonServisi">Aksiyon servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    /// <param name="birimServisi">Birim servisi.</param>
    /// <param name="doktorServisi">Doktor servisi.</param>
    /// <param name="kullaniciServisi">Kullanıcı servisi.</param>
    /// <param name="denetimKaydedici">Denetim kaydı servisi.</param>
    /// <param name="kullaniciYoneticisi">Kullanıcı yöneticisi.</param>
    public EkleModel(
        IAksiyonServisi aksiyonServisi,
        IHastaneServisi hastaneServisi,
        IBirimServisi birimServisi,
        IDoktorServisi doktorServisi,
        IKullaniciServisi kullaniciServisi,
        IAuditLogger denetimKaydedici,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        _aksiyonServisi = aksiyonServisi;
        _hastaneServisi = hastaneServisi;
        _birimServisi = birimServisi;
        _doktorServisi = doktorServisi;
        _kullaniciServisi = kullaniciServisi;
        _denetimKaydedici = denetimKaydedici;
        _kullaniciYoneticisi = kullaniciYoneticisi;
    }

    /// <summary>Formdan gelen aksiyon oluşturma verisi.</summary>
    [BindProperty]
    public AksiyonOlusturDto Girdi { get; set; } = new();

    /// <summary>Hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Birim seçenekleri.</summary>
    public SelectList BirimSecenekleri { get; private set; } = default!;

    /// <summary>Doktor seçenekleri.</summary>
    public SelectList DoktorSecenekleri { get; private set; } = default!;

    /// <summary>Sorumlu kullanıcı seçenekleri.</summary>
    public SelectList SorumluSecenekleri { get; private set; } = default!;

    /// <summary>Öncelik seçenekleri.</summary>
    public SelectList OncelikSecenekleri { get; private set; } = default!;

    /// <summary>Oluşturma formunu görüntüler. İsteğe bağlı olarak bir kritik geri bildirime bağlar.</summary>
    /// <param name="kritikId">Bağlanacak kritik geri bildirim kimliği (isteğe bağlı).</param>
    public async Task OnGetAsync(int? kritikId)
    {
        if (kritikId is > 0)
        {
            Girdi.KritikGeriBildirimId = kritikId;
        }
        await SecenekleriYukleAsync();
    }

    /// <summary>Oluşturma formunu işler, aksiyonu kaydeder ve denetim kaydı oluşturur.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await SecenekleriYukleAsync();
            return Page();
        }

        try
        {
            var kullaniciId = _kullaniciYoneticisi.GetUserId(User) ?? string.Empty;
            var yeniId = await _aksiyonServisi.OlusturAsync(Girdi, kullaniciId);
            await _denetimKaydedici.LoglaAsync(
                "AksiyonOlustur",
                "IyilestirmeAksiyonu",
                yeniId.ToString(),
                detay: $"'{Girdi.Baslik}' başlıklı iyileştirme aksiyonu oluşturuldu.");
            TempData["Basari"] = "İyileştirme aksiyonu başarıyla oluşturuldu.";
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
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), Girdi.HastaneId);

        var birimler = await _birimServisi.TumunuGetirAsync();
        BirimSecenekleri = new SelectList(
            birimler.Select(b => new { b.Id, Ad = b.HastaneAdi is null ? b.Ad : $"{b.HastaneAdi} - {b.Ad}" }),
            "Id", "Ad", Girdi.BirimId);

        var doktorlar = await _doktorServisi.TumunuGetirAsync();
        DoktorSecenekleri = new SelectList(
            doktorlar.Select(d => new { d.Id, d.TamAd }),
            "Id", "TamAd", Girdi.DoktorId);

        var kullanicilar = await _kullaniciServisi.TumunuGetirAsync();
        SorumluSecenekleri = new SelectList(
            kullanicilar.Where(k => k.AktifMi).Select(k => new { k.Id, Ad = $"{k.Ad} {k.Soyad} ({k.Eposta})" }),
            "Id", "Ad", Girdi.SorumluKullaniciId);

        var oncelikOgeleri = Enum.GetValues<AksiyonOnceligi>()
            .Select(o => new SelectListItem
            {
                Value = ((int)o).ToString(),
                Text = GorunumAdlari.AksiyonOnceligiAdi(o)
            });
        OncelikSecenekleri = new SelectList(oncelikOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.Oncelik);
    }
}
