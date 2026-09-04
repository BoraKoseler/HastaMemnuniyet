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
/// Açık uçlu (serbest metin) yanıtlar üzerinde anahtar kelime/frekans analizi sunan rapor sayfası.
/// En sık geçen kelimeleri, frekansa göre büyüyen bir "kelime bulutu" olarak görselleştirir.
/// </summary>
[Authorize(Roles = RolSabitleri.Admin + "," + RolSabitleri.KaliteBirimi + "," + RolSabitleri.UstYonetim)]
public class AnahtarKelimeAnaliziModel : PageModel
{
    private const int EnFazlaKelime = 40;

    private readonly IMetinAnaliziServisi _metinAnaliziServisi;
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="AnahtarKelimeAnaliziModel"/> örneği oluşturur.</summary>
    /// <param name="metinAnaliziServisi">Anahtar kelime analizi servisi.</param>
    /// <param name="hastaneServisi">Hastane filtresi için hastane servisi.</param>
    public AnahtarKelimeAnaliziModel(
        IMetinAnaliziServisi metinAnaliziServisi,
        IHastaneServisi hastaneServisi)
    {
        _metinAnaliziServisi = metinAnaliziServisi;
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Anahtar kelime analizi sonucu.</summary>
    public MetinAnaliziSonucuDto Sonuc { get; private set; } = new();

    /// <summary>Kelime bulutunda en büyük kelimeyi belirlemek için en yüksek frekans.</summary>
    public int EnYuksekFrekans { get; private set; } = 1;

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

    /// <summary>Analiz sonucunu (varsa tarih/hastane filtresiyle) getirir.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        Sonuc = await _metinAnaliziServisi.AnahtarKelimeleriGetirAsync(Baslangic, Bitis, HastaneId, null, EnFazlaKelime);

        if (Sonuc.EnSikKelimeler.Count > 0)
        {
            EnYuksekFrekans = Sonuc.EnSikKelimeler.Max(k => k.Sayi);
        }
    }

    /// <summary>
    /// Bir kelimenin frekansına göre kelime bulutunda kullanılacak yazı tipi boyutunu (rem) hesaplar.
    /// En yüksek frekanslı kelime en büyük, en düşük frekanslı kelime en küçük gösterilir.
    /// </summary>
    /// <param name="frekans">Kelimenin geçiş sıklığı.</param>
    /// <returns>rem cinsinden yazı tipi boyutu.</returns>
    public double YaziBoyutu(int frekans)
    {
        const double enKucuk = 0.9;
        const double enBuyuk = 2.8;
        if (EnYuksekFrekans <= 1)
        {
            return enKucuk;
        }
        var oran = (double)(frekans - 1) / (EnYuksekFrekans - 1);
        return Math.Round(enKucuk + oran * (enBuyuk - enKucuk), 2);
    }
}
