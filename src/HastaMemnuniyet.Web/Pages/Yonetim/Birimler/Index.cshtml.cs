using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaMemnuniyet.Web.Pages.Yonetim.Birimler;

/// <summary>
/// Birim listeleme sayfasının PageModel'i. Hastane bazlı filtreleme sağlar (yalnızca Admin).
/// </summary>
[Authorize(Roles = RolSabitleri.Admin)]
public class IndexModel : PageModel
{
    private readonly IBirimServisi _birimServisi;
    private readonly IHastaneServisi _hastaneServisi;

    /// <summary>Yeni bir <see cref="IndexModel"/> örneği oluşturur.</summary>
    /// <param name="birimServisi">Birim servisi.</param>
    /// <param name="hastaneServisi">Hastane servisi.</param>
    public IndexModel(IBirimServisi birimServisi, IHastaneServisi hastaneServisi)
    {
        _birimServisi = birimServisi;
        _hastaneServisi = hastaneServisi;
    }

    /// <summary>Listelenen birimler.</summary>
    public IReadOnlyList<BirimDto> Birimler { get; private set; } = new List<BirimDto>();

    /// <summary>Filtre için hastane seçenekleri.</summary>
    public SelectList HastaneSecenekleri { get; private set; } = default!;

    /// <summary>Seçili hastane filtresi.</summary>
    [BindProperty(SupportsGet = true)]
    public int? HastaneId { get; set; }

    /// <summary>Birim listesini (varsa filtreli) getirir.</summary>
    public async Task OnGetAsync()
    {
        var hastaneler = await _hastaneServisi.TumunuGetirAsync();
        HastaneSecenekleri = new SelectList(hastaneler, nameof(HastaneDto.Id), nameof(HastaneDto.Ad), HastaneId);

        Birimler = HastaneId.HasValue && HastaneId.Value > 0
            ? await _birimServisi.HastaneyeGoreGetirAsync(HastaneId.Value)
            : await _birimServisi.TumunuGetirAsync();
    }
}
