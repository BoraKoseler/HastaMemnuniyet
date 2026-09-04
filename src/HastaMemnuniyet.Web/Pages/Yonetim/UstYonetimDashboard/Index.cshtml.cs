using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HastaMemnuniyet.Application;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.UstYonetimDashboard;

/// <summary>
/// Üst yönetime yönelik, kişisel veri içermeyen kurum geneli özet panosu.
/// Kurum özetleri, hastane/birim karşılaştırmaları, doktor puan tablosu (asgari örneklem eşiği ile),
/// kritik/aksiyon sayıları ve aylık trend bilgilerini sunar.
/// </summary>
[Authorize(Roles = RolSabitleri.UstYonetim + "," + RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private const int TrendAySayisi = 6;
    private const int VarsayilanMinimumOrneklem = 5;

    private readonly IDashboardServisi _dashboardServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly ISistemAyariRepository _sistemAyariRepository;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="dashboardServisi">Dashboard/istatistik servisi.</param>
    /// <param name="hastaneServisi">Hastane filtresi için hastane servisi.</param>
    /// <param name="sistemAyariRepository">Sistem ayarı deposu (asgari örneklem eşiği).</param>
    public IndexModel(
        IDashboardServisi dashboardServisi,
        IHastaneServisi hastaneServisi,
        ISistemAyariRepository sistemAyariRepository)
    {
        _dashboardServisi = dashboardServisi;
        _hastaneServisi = hastaneServisi;
        _sistemAyariRepository = sistemAyariRepository;
    }

    /// <summary>Kurum geneli özet istatistikler.</summary>
    public DashboardDto Ozet { get; private set; } = new();

    /// <summary>Hastane bazlı karşılaştırma listesi.</summary>
    public IReadOnlyList<HastaneKarsilastirmaDto> HastaneKarsilastirma { get; private set; } = new List<HastaneKarsilastirmaDto>();

    /// <summary>Asgari örneklem eşiğini karşılayan doktor puanları.</summary>
    public IReadOnlyList<DoktorPuanDto> DoktorPuanlari { get; private set; } = new List<DoktorPuanDto>();

    /// <summary>Son aylara ait ortalama puan trendi.</summary>
    public IReadOnlyList<AylikTrendDto> AylikTrend { get; private set; } = new List<AylikTrendDto>();

    /// <summary>Doktor puan tablosunda kullanılan asgari örneklem (yanıt) eşiği.</summary>
    public int MinimumOrneklem { get; private set; } = VarsayilanMinimumOrneklem;

    /// <summary>Hastane filtresi için seçenek listesi.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = new(new List<HastaneDto>(), nameof(HastaneDto.Id), nameof(HastaneDto.Ad));

    /// <summary>Başlangıç tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Baslangic { get; set; }

    /// <summary>Bitiş tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Bitis { get; set; }

    /// <summary>Hastane filtresi (opsiyonel).</summary>
    [BindProperty(SupportsGet = true)]
    public int? HastaneId { get; set; }

    /// <summary>Pano verilerini (varsa tarih/hastane filtresiyle) getirir.</summary>
    public async Task OnGetAsync()
    {
        MinimumOrneklem = await MinimumOrneklemGetirAsync();

        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        var kapsam = HastaneId.HasValue
            ? new KapsamFiltresi { HastaneIdleri = new List<int> { HastaneId.Value } }
            : null;

        Ozet = await _dashboardServisi.IstatistikleriGetirAsync(Baslangic, Bitis, kapsam);
        HastaneKarsilastirma = await _dashboardServisi.HastaneKarsilastirmaGetirAsync(Baslangic, Bitis, HastaneId);
        DoktorPuanlari = await _dashboardServisi.DoktorPuanlariGetirAsync(MinimumOrneklem, Baslangic, Bitis, HastaneId);
        AylikTrend = await _dashboardServisi.AylikTrendGetirAsync(TrendAySayisi, HastaneId);
    }

    private async Task<int> MinimumOrneklemGetirAsync()
    {
        var deger = await _sistemAyariRepository.DegerGetirAsync(AyarAnahtarlari.DusukOrneklemEsigi, VarsayilanMinimumOrneklem.ToString(CultureInfo.InvariantCulture));
        return int.TryParse(deger, NumberStyles.Integer, CultureInfo.InvariantCulture, out var esik) && esik > 0
            ? esik
            : VarsayilanMinimumOrneklem;
    }
}
