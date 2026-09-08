using HastaMemnuniyet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages;

/// <summary>
/// Herkese açık ana sayfa. Veritabanından güncel istatistikleri çekerek
/// ziyaretçilere sistemin ölçeğini gösteren sayısal göstergeler sunar.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IHastaneServisi _hastaneServisi;
    private readonly IDoktorServisi _doktorServisi;
    private readonly IAnketServisi _anketServisi;
    private readonly IDashboardServisi _dashboardServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    public IndexModel(
        IHastaneServisi hastaneServisi,
        IDoktorServisi doktorServisi,
        IAnketServisi anketServisi,
        IDashboardServisi dashboardServisi)
    {
        _hastaneServisi = hastaneServisi;
        _doktorServisi = doktorServisi;
        _anketServisi = anketServisi;
        _dashboardServisi = dashboardServisi;
    }

    /// <summary>Aktif hastane sayısı.</summary>
    public int HastaneSayisi { get; set; }

    /// <summary>Aktif doktor sayısı.</summary>
    public int DoktorSayisi { get; set; }

    /// <summary>Aktif anket sayısı.</summary>
    public int AnketSayisi { get; set; }

    /// <summary>Toplam tamamlanan yanıt sayısı.</summary>
    public int YanitSayisi { get; set; }

    /// <summary>Genel ortalama memnuniyet puanı (1-5).</summary>
    public double OrtalamaPuan { get; set; }

    /// <summary>Toplam gönderilmiş davet sayısı.</summary>
    public int DavetSayisi { get; set; }

    /// <summary>Ana sayfa GET isteğini işler ve istatistikleri yükler.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSayisi = hastaneler.Count(h => h.AktifMi);

        var doktorlar = await _doktorServisi.TumunuGetirAsync();
        DoktorSayisi = doktorlar.Count(d => d.AktifMi);

        var anketler = await _anketServisi.TumunuGetirAsync();
        AnketSayisi = anketler.Count(a => a.AktifMi);

        var dashboard = await _dashboardServisi.IstatistikleriGetirAsync();
        YanitSayisi = dashboard.ToplamYanitSayisi;
        OrtalamaPuan = dashboard.OrtalamaPuan;
        DavetSayisi = dashboard.ToplamDavetSayisi;
    }

    /// <summary>
    /// Sayıyı "3+" gibi bir gösterim formatına çevirir.
    /// Sıfır ise "0" döner; değilse sayıdan 1 çıkarıp "+" ekler.
    /// </summary>
    /// <param name="sayi">Gösterilecek sayı.</param>
    /// <returns>Formatlanmış gösterim metni.</returns>
    public static string ArtiFormatla(int sayi)
        => sayi <= 0 ? "0" : $"{sayi - 1}+";

    /// <summary>
    /// Puanı "4.2+" gibi bir gösterim formatına çevirir.
    /// </summary>
    /// <param name="puan">Gösterilecek puan.</param>
    /// <returns>Formatlanmış gösterim metni.</returns>
    public static string PuanFormatla(double puan)
        => puan <= 0 ? "—" : $"{puan:F1}";
}
