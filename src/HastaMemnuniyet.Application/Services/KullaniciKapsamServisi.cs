using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Kullanıcının hastane ve birim kapsamlarını çözümleyen servis.
/// </summary>
public class KullaniciKapsamServisi : IKullaniciKapsamServisi
{
    private readonly IGenericRepository<KullaniciHastaneKapsami> _hastaneKapsamRepository;
    private readonly IGenericRepository<KullaniciBirimKapsami> _birimKapsamRepository;
    private readonly IBirimRepository _birimRepository;

    /// <summary>Yeni bir <see cref="KullaniciKapsamServisi"/> örneği oluşturur.</summary>
    /// <param name="hastaneKapsamRepository">Kullanıcı hastane kapsamı veri erişim bileşeni.</param>
    /// <param name="birimKapsamRepository">Kullanıcı birim kapsamı veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    public KullaniciKapsamServisi(
        IGenericRepository<KullaniciHastaneKapsami> hastaneKapsamRepository,
        IGenericRepository<KullaniciBirimKapsami> birimKapsamRepository,
        IBirimRepository birimRepository)
    {
        _hastaneKapsamRepository = hastaneKapsamRepository;
        _birimKapsamRepository = birimKapsamRepository;
        _birimRepository = birimRepository;
    }

    /// <inheritdoc />
    public async Task<KapsamFiltresi?> KapsamGetirAsync(string kullaniciId, bool tamYetkiliMi)
    {
        if (tamYetkiliMi || string.IsNullOrWhiteSpace(kullaniciId))
            return null;

        var birimKapsamlari = (await _birimKapsamRepository.BulAsync(k => k.KullaniciId == kullaniciId))
            .Select(k => k.BirimId)
            .Distinct()
            .ToList();

        var hastaneIdleri = (await _hastaneKapsamRepository.BulAsync(k => k.KullaniciId == kullaniciId))
            .Select(k => k.HastaneId)
            .ToList();

        // Birim kapsamlarına ait hastaneleri de kapsama dahil et.
        if (birimKapsamlari.Count > 0)
        {
            var birimler = await _birimRepository.GetAllAsync();
            var birimHastaneleri = birimler
                .Where(b => birimKapsamlari.Contains(b.Id))
                .Select(b => b.HastaneId);
            hastaneIdleri.AddRange(birimHastaneleri);
        }

        return new KapsamFiltresi
        {
            HastaneIdleri = hastaneIdleri.Distinct().ToList(),
            BirimIdleri = birimKapsamlari
        };
    }
}
