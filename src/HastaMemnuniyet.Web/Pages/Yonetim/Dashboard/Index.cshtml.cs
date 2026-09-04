using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Dashboard;

/// <summary>
/// Genel istatistikleri ve puan dağılımlarını gösteren dashboard sayfasının PageModel'i
/// (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IDashboardServisi _dashboardServisi;
    private readonly IDisaAktarmaServisi _disaAktarmaServisi;
    private readonly IAuditLogger _denetimKaydedici;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="dashboardServisi">Dashboard servisi.</param>
    /// <param name="disaAktarmaServisi">Excel dışa aktarma servisi.</param>
    /// <param name="denetimKaydedici">Denetim kaydı loglayıcısı.</param>
    public IndexModel(
        IDashboardServisi dashboardServisi,
        IDisaAktarmaServisi disaAktarmaServisi,
        IAuditLogger denetimKaydedici)
    {
        _dashboardServisi = dashboardServisi;
        _disaAktarmaServisi = disaAktarmaServisi;
        _denetimKaydedici = denetimKaydedici;
    }

    /// <summary>Görüntülenen istatistikler.</summary>
    public DashboardDto Istatistik { get; private set; } = new();

    /// <summary>Seçili dönem için Net Tavsiye Skoru (NPS) sonucu.</summary>
    public NpsSonucuDto Nps { get; private set; } = new();

    /// <summary>Başlangıç tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Baslangic { get; set; }

    /// <summary>Bitiş tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Bitis { get; set; }

    /// <summary>İstatistikleri (varsa tarih aralığıyla) getirir.</summary>
    public async Task OnGetAsync()
    {
        Istatistik = await _dashboardServisi.IstatistikleriGetirAsync(Baslangic, Bitis);
        Nps = await _dashboardServisi.NpsHesaplaAsync(Baslangic, Bitis);
    }

    /// <summary>Dashboard istatistiklerini Excel raporu olarak dışa aktarır.</summary>
    /// <returns>Excel (.xlsx) dosyası.</returns>
    public async Task<IActionResult> OnGetRaporIndirAsync()
    {
        var istatistik = await _dashboardServisi.IstatistikleriGetirAsync(Baslangic, Bitis);
        var icerik = _disaAktarmaServisi.RaporuDisaAktar(istatistik);

        await _denetimKaydedici.LoglaAsync(
            "RaporDisaAktar",
            detay: $"Dashboard raporu Excel olarak dışa aktarıldı ({istatistik.BaslangicTarihi:dd.MM.yyyy} - {istatistik.BitisTarihi:dd.MM.yyyy}).");

        var dosyaAdi = $"dashboard-raporu-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
        return File(icerik, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dosyaAdi);
    }
}
