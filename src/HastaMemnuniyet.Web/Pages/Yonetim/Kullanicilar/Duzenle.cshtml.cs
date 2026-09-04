using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Kullanicilar;

/// <summary>
/// Kullanıcının rol ve aktiflik durumunu düzenleyen sayfanın PageModel'i (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class DuzenleModel : PageModel
{
    private readonly IKullaniciServisi _kullaniciServisi;

    /// <summary>Yeni bir <see cref="DuzenleModel"/> örneği oluşturur.</summary>
    /// <param name="kullaniciServisi">Kullanıcı servisi.</param>
    public DuzenleModel(IKullaniciServisi kullaniciServisi)
    {
        _kullaniciServisi = kullaniciServisi;
    }

    /// <summary>Düzenlenen kullanıcının kimliği.</summary>
    [BindProperty]
    public string KullaniciId { get; set; } = string.Empty;

    /// <summary>Atanacak rol.</summary>
    [BindProperty]
    [Required(ErrorMessage = "Rol seçimi zorunludur.")]
    public string Rol { get; set; } = string.Empty;

    /// <summary>Kullanıcının aktiflik durumu.</summary>
    [BindProperty]
    public bool AktifMi { get; set; }

    /// <summary>Kullanıcının ad-soyad bilgisi (salt okunur gösterim).</summary>
    public string AdSoyad { get; private set; } = string.Empty;

    /// <summary>Kullanıcının e-postası (salt okunur gösterim).</summary>
    public string Eposta { get; private set; } = string.Empty;

    /// <summary>Rol seçenekleri.</summary>
    public SelectList RolSecenekleri { get; private set; } = default!;

    /// <summary>Düzenleme formunu kullanıcı verisiyle görüntüler.</summary>
    /// <param name="id">Kullanıcı kimliği.</param>
    public async Task<IActionResult> OnGetAsync(string id)
    {
        var kullanici = await _kullaniciServisi.GetirAsync(id);
        if (kullanici is null)
        {
            TempData["Hata"] = "Kullanıcı bulunamadı.";
            return RedirectToPage("Index");
        }

        KullaniciId = kullanici.Id;
        AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}";
        Eposta = kullanici.Eposta;
        Rol = kullanici.Roller.FirstOrDefault() ?? string.Empty;
        AktifMi = kullanici.AktifMi;
        RolSecenekleriniYukle();
        return Page();
    }

    /// <summary>Düzenleme formunu işler; rol ve aktiflik durumunu günceller.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await BilgileriYenidenYukleAsync();
            return Page();
        }

        try
        {
            await _kullaniciServisi.RolDegistirAsync(KullaniciId, Rol);
            await _kullaniciServisi.AktiflikDegistirAsync(KullaniciId, AktifMi);
            TempData["Basari"] = "Kullanıcı başarıyla güncellendi.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await BilgileriYenidenYukleAsync();
            return Page();
        }
    }

    private async Task BilgileriYenidenYukleAsync()
    {
        var kullanici = await _kullaniciServisi.GetirAsync(KullaniciId);
        if (kullanici is not null)
        {
            AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}";
            Eposta = kullanici.Eposta;
        }
        RolSecenekleriniYukle();
    }

    private void RolSecenekleriniYukle()
    {
        RolSecenekleri = new SelectList(RolSabitleri.TumRoller, Rol);
    }
}
