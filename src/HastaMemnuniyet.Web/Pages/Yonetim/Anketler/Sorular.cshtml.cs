using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Infrastructure.Identity;
using HastaMemnuniyet.Web.Yardimcilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Anketler;

/// <summary>
/// Bir ankete ait soruların ve seçeneklerin yönetildiği sayfanın PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class SorularModel : PageModel
{
    private readonly IAnketServisi _anketServisi;

    /// <summary>Yeni bir <see cref="SorularModel"/> örneği oluşturur.</summary>
    /// <param name="anketServisi">Anket servisi.</param>
    public SorularModel(IAnketServisi anketServisi)
    {
        _anketServisi = anketServisi;
    }

    /// <summary>Soruları yönetilen anket.</summary>
    public AnketDto Anket { get; private set; } = new();

    /// <summary>Yeni soru ekleme formundan gelen veri.</summary>
    [BindProperty]
    public AnketSorusuOlusturDto Girdi { get; set; } = new();

    /// <summary>Soru tipi seçenekleri.</summary>
    public SelectList TipSecenekleri { get; private set; } = default!;

    /// <summary>Anket sorularını görüntüler.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var anket = await _anketServisi.GetirAsync(id);
        if (anket is null)
        {
            TempData["Hata"] = "Anket bulunamadı.";
            return RedirectToPage("Index");
        }

        Anket = anket;
        Girdi.AnketId = id;
        TipSecenekleriniYukle();
        return Page();
    }

    /// <summary>Ankete yeni bir soru ekler.</summary>
    /// <param name="id">Anket kimliği.</param>
    public async Task<IActionResult> OnPostSoruEkleAsync(int id)
    {
        Girdi.AnketId = id;

        // Soru tipine uygun olmayan alanları temizle.
        if (Girdi.SoruTipi is not SoruTipi.TekSecim and not SoruTipi.CokluSecim)
        {
            Girdi.Secenekler = new();
        }
        else
        {
            Girdi.Secenekler = Girdi.Secenekler
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (Girdi.Secenekler.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Seçmeli sorular için en az bir seçenek girmelisiniz.");
            }
        }

        if (!ModelState.IsValid)
        {
            var anket = await _anketServisi.GetirAsync(id);
            if (anket is null)
            {
                TempData["Hata"] = "Anket bulunamadı.";
                return RedirectToPage("Index");
            }
            Anket = anket;
            TipSecenekleriniYukle();
            return Page();
        }

        try
        {
            await _anketServisi.SoruEkleAsync(Girdi);
            TempData["Basari"] = "Soru başarıyla eklendi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Soru eklenemedi: {ex.Message}";
        }
        return RedirectToPage("Sorular", new { id });
    }

    /// <summary>Bir soruyu siler.</summary>
    /// <param name="id">Anket kimliği.</param>
    /// <param name="soruId">Silinecek soru kimliği.</param>
    public async Task<IActionResult> OnPostSoruSilAsync(int id, int soruId)
    {
        try
        {
            await _anketServisi.SoruSilAsync(soruId);
            TempData["Basari"] = "Soru silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Soru silinemedi: {ex.Message}";
        }
        return RedirectToPage("Sorular", new { id });
    }

    /// <summary>Bir soruya seçenek ekler.</summary>
    /// <param name="id">Anket kimliği.</param>
    /// <param name="soruId">Soru kimliği.</param>
    /// <param name="metin">Seçenek metni.</param>
    public async Task<IActionResult> OnPostSecenekEkleAsync(int id, int soruId, string metin)
    {
        if (string.IsNullOrWhiteSpace(metin))
        {
            TempData["Hata"] = "Seçenek metni boş olamaz.";
            return RedirectToPage("Sorular", new { id });
        }

        try
        {
            await _anketServisi.SecenekEkleAsync(soruId, metin.Trim());
            TempData["Basari"] = "Seçenek eklendi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Seçenek eklenemedi: {ex.Message}";
        }
        return RedirectToPage("Sorular", new { id });
    }

    /// <summary>Bir seçeneği siler.</summary>
    /// <param name="id">Anket kimliği.</param>
    /// <param name="secenekId">Silinecek seçenek kimliği.</param>
    public async Task<IActionResult> OnPostSecenekSilAsync(int id, int secenekId)
    {
        try
        {
            await _anketServisi.SecenekSilAsync(secenekId);
            TempData["Basari"] = "Seçenek silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = $"Seçenek silinemedi: {ex.Message}";
        }
        return RedirectToPage("Sorular", new { id });
    }

    private void TipSecenekleriniYukle()
    {
        var ogeler = Enum.GetValues<SoruTipi>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = GorunumAdlari.SoruTipiAdi(t)
            });
        TipSecenekleri = new SelectList(ogeler, nameof(SelectListItem.Value), nameof(SelectListItem.Text), (int)Girdi.SoruTipi);
    }
}
