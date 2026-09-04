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
/// Bir iyileştirme aksiyonunun durumunu ve alanlarını güncelleyen sayfanın PageModel'i
/// (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class DuzenleModel : PageModel
{
    private readonly IAksiyonServisi _aksiyonServisi;
    private readonly IAuditLogger _denetimKaydedici;
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="aksiyonServisi">Aksiyon servisi.</param>
    /// <param name="denetimKaydedici">Denetim kaydı servisi.</param>
    /// <param name="kullaniciYoneticisi">Kullanıcı yöneticisi.</param>
    public DuzenleModel(
        IAksiyonServisi aksiyonServisi,
        IAuditLogger denetimKaydedici,
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi)
    {
        _aksiyonServisi = aksiyonServisi;
        _denetimKaydedici = denetimKaydedici;
        _kullaniciYoneticisi = kullaniciYoneticisi;
    }

    /// <summary>Formdan gelen güncelleme verisi.</summary>
    [BindProperty]
    public AksiyonGuncelleDto Girdi { get; set; } = new();

    /// <summary>Görüntülenen aksiyonun tümü (salt okunur bilgiler ve geçmiş için).</summary>
    public IyilestirmeAksiyonuDto? Aksiyon { get; private set; }

    /// <summary>Durum seçenekleri.</summary>
    public SelectList DurumSecenekleri { get; private set; } = default!;

    /// <summary>Öncelik seçenekleri.</summary>
    public SelectList OncelikSecenekleri { get; private set; } = default!;

    /// <summary>Güncelleme formunu görüntüler.</summary>
    /// <param name="id">Aksiyon kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Aksiyon = await _aksiyonServisi.DetayGetirAsync(id);
        if (Aksiyon is null)
        {
            return NotFound();
        }

        Girdi = new AksiyonGuncelleDto
        {
            Id = Aksiyon.Id,
            Durum = Aksiyon.Durum,
            Oncelik = Aksiyon.Oncelik,
            HedefTarih = Aksiyon.HedefTarih,
            KapanisNotu = Aksiyon.KapanisNotu
        };

        SecenekleriYukle();
        return Page();
    }

    /// <summary>Güncelleme formunu işler ve denetim kaydı oluşturur.</summary>
    /// <param name="id">Aksiyon kimliği.</param>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        Girdi.Id = id;
        if (!ModelState.IsValid)
        {
            Aksiyon = await _aksiyonServisi.DetayGetirAsync(id);
            SecenekleriYukle();
            return Page();
        }

        try
        {
            var kullaniciId = _kullaniciYoneticisi.GetUserId(User) ?? string.Empty;
            await _aksiyonServisi.GuncelleAsync(Girdi, kullaniciId);
            await _denetimKaydedici.LoglaAsync(
                "AksiyonGuncelle",
                "IyilestirmeAksiyonu",
                id.ToString(),
                yeniDeger: GorunumAdlari.AksiyonDurumuAdi(Girdi.Durum),
                detay: Girdi.DegisiklikAciklamasi);
            TempData["Basari"] = "İyileştirme aksiyonu güncellendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            Aksiyon = await _aksiyonServisi.DetayGetirAsync(id);
            SecenekleriYukle();
            return Page();
        }
    }

    private void SecenekleriYukle()
    {
        var durumOgeleri = Enum.GetValues<AksiyonDurumu>()
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = GorunumAdlari.AksiyonDurumuAdi(d)
            });
        DurumSecenekleri = new SelectList(durumOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.Durum);

        var oncelikOgeleri = Enum.GetValues<AksiyonOnceligi>()
            .Select(o => new SelectListItem
            {
                Value = ((int)o).ToString(),
                Text = GorunumAdlari.AksiyonOnceligiAdi(o)
            });
        OncelikSecenekleri = new SelectList(oncelikOgeleri, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.Oncelik);
    }
}
