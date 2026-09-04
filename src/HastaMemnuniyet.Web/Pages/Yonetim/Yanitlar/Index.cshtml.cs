using System.ComponentModel.DataAnnotations;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Yanitlar;

/// <summary>
/// Anket yanıtlarını listeleyen ve filtreleyen sayfanın PageModel'i (Admin ve Kalite Birimi).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi)]
public class IndexModel : PageModel
{
    private readonly IYanitServisi _yanitServisi;
    private readonly IHastaneServisi _hastaneServisi;
    private readonly IAnketServisi _anketServisi;
    private readonly IDisaAktarmaServisi _disaAktarmaServisi;
    private readonly IAuditLogger _denetimKaydedici;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="yanitServisi">Yanıt servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    /// <param name="anketServisi">Anket servisi.</param>
    /// <param name="disaAktarmaServisi">Excel dışa aktarma servisi.</param>
    /// <param name="denetimKaydedici">Denetim kaydı loglayıcısı.</param>
    public IndexModel(
        IYanitServisi yanitServisi,
        IHastaneServisi hastaneServisi,
        IAnketServisi anketServisi,
        IDisaAktarmaServisi disaAktarmaServisi,
        IAuditLogger denetimKaydedici)
    {
        _yanitServisi = yanitServisi;
        _hastaneServisi = hastaneServisi;
        _anketServisi = anketServisi;
        _disaAktarmaServisi = disaAktarmaServisi;
        _denetimKaydedici = denetimKaydedici;
    }

    /// <summary>Bir sayfada gösterilecek yanıt sayısı.</summary>
    private const int SayfaBoyutu = 20;

    /// <summary>Listelenen yanıtlar (geçerli sayfa).</summary>
    public IReadOnlyList<AnketYanitiDto> Yanitlar { get; private set; } = new List<AnketYanitiDto>();

    /// <summary>Geçerli sayfa numarası (1 tabanlı).</summary>
    [BindProperty(SupportsGet = true)]
    public int SayfaNo { get; set; } = 1;

    /// <summary>Filtreye uyan toplam yanıt sayısı.</summary>
    public int ToplamKayit { get; private set; }

    /// <summary>Toplam sayfa sayısı.</summary>
    public int ToplamSayfa => ToplamKayit == 0 ? 1 : (int)Math.Ceiling(ToplamKayit / (double)SayfaBoyutu);

    /// <summary>Hastane filtre seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Anket filtre seçenekleri.</summary>
    public SelectList AnketSecenekleri { get; private set; } = default!;

    /// <summary>Başlangıç tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Baslangic { get; set; }

    /// <summary>Bitiş tarihi filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Bitis { get; set; }

    /// <summary>Hastane filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public int? HastaneId { get; set; }

    /// <summary>Anket filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public int? AnketId { get; set; }

    /// <summary>Yanıt listesini (varsa filtreli) getirir.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        var anketler = await _anketServisi.TumunuGetirAsync();
        AnketSecenekleri = new SelectList(anketler, nameof(AnketDto.Id), nameof(AnketDto.Ad), AnketId);

        var tumYanitlar = await _yanitServisi.TumunuGetirAsync(
            Baslangic,
            Bitis,
            HastaneId is > 0 ? HastaneId : null,
            AnketId is > 0 ? AnketId : null);

        ToplamKayit = tumYanitlar.Count;
        if (SayfaNo < 1)
        {
            SayfaNo = 1;
        }
        if (SayfaNo > ToplamSayfa)
        {
            SayfaNo = ToplamSayfa;
        }

        Yanitlar = tumYanitlar
            .Skip((SayfaNo - 1) * SayfaBoyutu)
            .Take(SayfaBoyutu)
            .ToList();
    }

    /// <summary>Filtrelenmiş yanıtları Excel (.xlsx) dosyası olarak dışa aktarır.</summary>
    /// <returns>Excel dosyası.</returns>
    public async Task<IActionResult> OnGetExcelAsync()
    {
        var yanitlar = await _yanitServisi.TumunuGetirAsync(
            Baslangic,
            Bitis,
            HastaneId is > 0 ? HastaneId : null,
            AnketId is > 0 ? AnketId : null);

        var icerik = _disaAktarmaServisi.YanitlariDisaAktar(yanitlar);

        await _denetimKaydedici.LoglaAsync(
            "YanitDisaAktar",
            tablo: "AnketYaniti",
            detay: $"{yanitlar.Count} adet anket yanıtı Excel olarak dışa aktarıldı.");

        var dosyaAdi = $"anket-yanitlari-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
        return File(icerik, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dosyaAdi);
    }
}
