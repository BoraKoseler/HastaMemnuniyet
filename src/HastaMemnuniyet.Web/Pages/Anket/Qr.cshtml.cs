using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HastaMemnuniyet.Web.Pages.Anket;

/// <summary>
/// QR kod ile erişilen anonim giriş sayfasının PageModel'i. Kampanyayı doğrular,
/// yeni bir anket daveti oluşturur ve hastayı anket doldurma sayfasına yönlendirir.
/// </summary>
[AllowAnonymous]
public class QrModel : PageModel
{
    private readonly IQrKampanyaServisi _qrKampanyaServisi;
    private readonly IDavetServisi _davetServisi;
    private readonly ILogger<QrModel> _logger;

    /// <summary>Yeni bir <see cref="QrModel"/> örneği oluşturur.</summary>
    /// <param name="qrKampanyaServisi">QR kampanya servisi.</param>
    /// <param name="davetServisi">Anket daveti servisi.</param>
    /// <param name="logger">Loglama servisi.</param>
    public QrModel(
        IQrKampanyaServisi qrKampanyaServisi,
        IDavetServisi davetServisi,
        ILogger<QrModel> logger)
    {
        _qrKampanyaServisi = qrKampanyaServisi;
        _davetServisi = davetServisi;
        _logger = logger;
    }

    /// <summary>Hata mesajı (varsa).</summary>
    public string? HataMesaji { get; set; }

    /// <summary>
    /// QR kampanyasını doğrular, aktif ve süresi geçmemişse yeni bir anket daveti
    /// oluşturarak hastayı ilgili ankete yönlendirir.
    /// </summary>
    /// <param name="id">QR kampanya kimliği.</param>
    /// <returns>Ankete ya da süresi dolmuş bilgilendirme sayfasına yönlendirme sonucu.</returns>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var kampanya = await _qrKampanyaServisi.GetirAsync(id);

            if (kampanya is null || !kampanya.AktifMi)
            {
                return RedirectToPage("SuresiDolmus");
            }

            if (kampanya.SonKullanmaTarihi.HasValue && kampanya.SonKullanmaTarihi.Value.Date < DateTime.Today)
            {
                return RedirectToPage("SuresiDolmus");
            }

            var davet = await _davetServisi.OlusturAsync(new AnketDavetiOlusturDto
            {
                AnketId = kampanya.AnketId,
                HastaneId = kampanya.HastaneId,
                BirimId = kampanya.BirimId,
                DoktorId = kampanya.DoktorId,
                GonderimKanali = GonderimKanali.Qr
            }, null);

            await _qrKampanyaServisi.KullanimArtirAsync(id);

            return RedirectToPage("Index", new { token = davet.Token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QR kampanya ({KampanyaId}) işlenirken hata oluştu.", id);
            HataMesaji = "Anket bağlantısı oluşturulurken bir sorun oluştu. Lütfen daha sonra tekrar deneyiniz.";
            return Page();
        }
    }
}
