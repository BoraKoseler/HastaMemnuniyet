using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Raporlar;

/// <summary>
/// Bir doktorun belirli bir dönemdeki performans karnesini sunan rapor sayfası.
/// Soru bazlı ortalamalar, genel ortalama, NPS, güçlü/zayıf alanlar ve önceki döneme göre trendi gösterir.
/// Karne Excel (.xlsx) olarak dışa aktarılabilir.
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.UstYonetim)]
public class DoktorKarnesiModel : PageModel
{
    private const string ExcelIcerikTuru = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IDoktorKarnesiServisi _doktorKarnesiServisi;
    private readonly IDoktorServisi _doktorServisi;
    private readonly IDisaAktarmaServisi _disaAktarmaServisi;

    /// <summary>Yeni bir <see cref="DoktorKarnesiModel"/> örneği oluşturur.</summary>
    /// <param name="doktorKarnesiServisi">Doktor karnesi servisi.</param>
    /// <param name="doktorServisi">Doktor seçim listesi için doktor servisi.</param>
    /// <param name="disaAktarmaServisi">Excel dışa aktarma servisi.</param>
    public DoktorKarnesiModel(
        IDoktorKarnesiServisi doktorKarnesiServisi,
        IDoktorServisi doktorServisi,
        IDisaAktarmaServisi disaAktarmaServisi)
    {
        _doktorKarnesiServisi = doktorKarnesiServisi;
        _doktorServisi = doktorServisi;
        _disaAktarmaServisi = disaAktarmaServisi;
    }

    /// <summary>Doktor performans karnesi (doktor seçilmişse doludur).</summary>
    public DoktorKarnesiDto? Karne { get; private set; }

    /// <summary>Doktor seçimi için seçenek listesi.</summary>
    public SelectList DoktorSecenekleri { get; private set; } = new(new List<DoktorDto>(), nameof(DoktorDto.Id), nameof(DoktorDto.TamAd));

    /// <summary>Seçili doktor kimliği.</summary>
    [BindProperty(SupportsGet = true)]
    public int? DoktorId { get; set; }

    /// <summary>Başlangıç tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Baslangic { get; set; }

    /// <summary>Bitiş tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Bitis { get; set; }

    /// <summary>Karne verisini (doktor seçilmişse) getirir.</summary>
    public async Task OnGetAsync()
    {
        await DoktorSecenekleriDoldurAsync();

        if (DoktorId.HasValue)
        {
            Karne = await _doktorKarnesiServisi.KarneGetirAsync(DoktorId.Value, Baslangic, Bitis);
        }
    }

    /// <summary>Seçili doktorun karnesini Excel dosyası olarak indirir.</summary>
    /// <returns>Excel dosyası ya da doktor bulunamazsa sayfaya yönlendirme.</returns>
    public async Task<IActionResult> OnGetExcelIndirAsync()
    {
        if (!DoktorId.HasValue)
        {
            return RedirectToPage();
        }

        var karne = await _doktorKarnesiServisi.KarneGetirAsync(DoktorId.Value, Baslangic, Bitis);
        if (karne is null)
        {
            return RedirectToPage();
        }

        var icerik = _disaAktarmaServisi.DoktorKarnesiDisaAktar(karne);
        var dosyaAdi = $"doktor-karnesi-{DoktorId.Value}-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
        return File(icerik, ExcelIcerikTuru, dosyaAdi);
    }

    private async Task DoktorSecenekleriDoldurAsync()
    {
        var doktorlar = await _doktorServisi.TumunuGetirAsync();
        DoktorSecenekleri = new SelectList(doktorlar, nameof(DoktorDto.Id), nameof(DoktorDto.TamAd), DoktorId);
    }
}
