using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Yonetim.VeriYonetimi;

/// <summary>
/// KVKK veri saklama politikası yönetim sayfasının PageModel'i. Süresi dolan kişisel verileri
/// listeler ve anonimleştirme işlemlerini yürütür (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IVeriSaklamaPolitikasiServisi _veriSaklamaServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="veriSaklamaServisi">Veri saklama politikası servisi.</param>
    public IndexModel(IVeriSaklamaPolitikasiServisi veriSaklamaServisi)
    {
        _veriSaklamaServisi = veriSaklamaServisi;
    }

    /// <summary>Veri saklama durumunun özeti.</summary>
    public VeriSaklamaOzetiDto Ozet { get; private set; } = new();

    /// <summary>Süresi dolan ve anonimleştirilebilecek kayıtlar.</summary>
    public IReadOnlyList<SuresiDolanKayitDto> SuresiDolanlar { get; private set; }
        = new List<SuresiDolanKayitDto>();

    /// <summary>İşlem sonucu başarı mesajı.</summary>
    [TempData]
    public string? BasariMesaji { get; set; }

    /// <summary>İşlem sonucu hata mesajı.</summary>
    [TempData]
    public string? HataMesaji { get; set; }

    /// <summary>Özet ve süresi dolan kayıt listesini yükler.</summary>
    public async Task OnGetAsync()
    {
        await VerileriYukleAsync();
    }

    /// <summary>Tek bir yanıtı anonimleştirir.</summary>
    /// <param name="id">Anonimleştirilecek yanıtın kimliği.</param>
    /// <returns>Aynı sayfaya yönlendirme sonucu.</returns>
    public async Task<IActionResult> OnPostAnonimlestirAsync(int id)
    {
        try
        {
            await _veriSaklamaServisi.AnonimlestirAsync(id);
            BasariMesaji = "Seçili kaydın kişisel verileri anonimleştirildi.";
        }
        catch (InvalidOperationException ex)
        {
            HataMesaji = ex.Message;
        }

        return RedirectToPage();
    }

    /// <summary>Süresi dolan tüm yanıtları toplu olarak anonimleştirir.</summary>
    /// <returns>Aynı sayfaya yönlendirme sonucu.</returns>
    public async Task<IActionResult> OnPostTumunuAnonimlestirAsync()
    {
        try
        {
            var rapor = await _veriSaklamaServisi.SuresiDolanlariAnonimlestirAsync();
            BasariMesaji = rapor.AnonimlestirilenYanitSayisi > 0
                ? $"{rapor.AnonimlestirilenYanitSayisi} adet kaydın kişisel verileri anonimleştirildi."
                : "Anonimleştirilecek süresi dolmuş kayıt bulunamadı.";
        }
        catch (InvalidOperationException ex)
        {
            HataMesaji = ex.Message;
        }

        return RedirectToPage();
    }

    private async Task VerileriYukleAsync()
    {
        Ozet = await _veriSaklamaServisi.OzetGetirAsync();
        SuresiDolanlar = await _veriSaklamaServisi.SuresiDolanKayitlariGetirAsync();
    }
}
