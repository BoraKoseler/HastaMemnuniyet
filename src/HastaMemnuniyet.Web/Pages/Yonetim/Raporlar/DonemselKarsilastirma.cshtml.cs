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
/// İki dönemin memnuniyet göstergelerini (ortalama puan, yanıt sayısı, yanıt oranı, NPS)
/// karşılaştıran rapor sayfası. Varsayılan olarak bu ay ile geçen ayı kıyaslar.
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.UstYonetim)]
public class DonemselKarsilastirmaModel : PageModel
{
    private readonly IDashboardServisi _dashboardServisi;
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="DonemselKarsilastirmaModel"/> örneği oluşturur.</summary>
    /// <param name="dashboardServisi">Dönemsel karşılaştırma servisi.</param>
    /// <param name="hastaneServisi">Hastane filtresi için hastane servisi.</param>
    public DonemselKarsilastirmaModel(
        IDashboardServisi dashboardServisi,
        IHastaneServisi hastaneServisi)
    {
        _dashboardServisi = dashboardServisi;
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Dönemsel karşılaştırma sonucu.</summary>
    public DonemselKarsilastirmaDto Sonuc { get; private set; } = new();

    /// <summary>Hastane filtresi için seçenek listesi.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = new(new List<HastaneDto>(), nameof(HastaneDto.Id), nameof(HastaneDto.Ad));

    /// <summary>Önceki (kıyas) dönemin başlangıç tarihi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? OncekiBaslangic { get; set; }

    /// <summary>Önceki (kıyas) dönemin bitiş tarihi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? OncekiBitis { get; set; }

    /// <summary>Güncel dönemin başlangıç tarihi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? GuncelBaslangic { get; set; }

    /// <summary>Güncel dönemin bitiş tarihi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? GuncelBitis { get; set; }

    /// <summary>Hastane filtresi (opsiyonel).</summary>
    [BindProperty(SupportsGet = true)]
    public int? HastaneId { get; set; }

    /// <summary>Karşılaştırma sonucunu getirir. Tarih verilmezse bu ay ile geçen ay kıyaslanır.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        var bugun = DateTime.Today;
        var buAyBaslangic = new DateTime(bugun.Year, bugun.Month, 1);
        var gecenAyBaslangic = buAyBaslangic.AddMonths(-1);

        var guncelBaslangic = GuncelBaslangic ?? buAyBaslangic;
        var guncelBitis = GuncelBitis ?? bugun;
        var oncekiBaslangic = OncekiBaslangic ?? gecenAyBaslangic;
        var oncekiBitis = OncekiBitis ?? buAyBaslangic.AddDays(-1);

        // Filtre kutularının dolu görünmesi için değerleri geri yaz.
        GuncelBaslangic = guncelBaslangic;
        GuncelBitis = guncelBitis;
        OncekiBaslangic = oncekiBaslangic;
        OncekiBitis = oncekiBitis;

        Sonuc = await _dashboardServisi.DonemselKarsilastirAsync(
            oncekiBaslangic, oncekiBitis, guncelBaslangic, guncelBitis, HastaneId);
    }
}
