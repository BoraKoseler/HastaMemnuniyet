using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Tamamlanmış anket yanıtlarının listelenmesi ve detaylandırılması iş kurallarını uygulayan servis.
/// </summary>
public class YanitServisi : IYanitServisi
{
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly IHastaneRepository _hastaneRepository;
    private readonly IBirimRepository _birimRepository;
    private readonly IDoktorRepository _doktorRepository;

    /// <summary>Yeni bir <see cref="YanitServisi"/> örneği oluşturur.</summary>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="hastaneRepository">Hastane veri erişim bileşeni.</param>
    /// <param name="birimRepository">Birim veri erişim bileşeni.</param>
    /// <param name="doktorRepository">Doktor veri erişim bileşeni.</param>
    public YanitServisi(
        IAnketYanitiRepository yanitRepository,
        IAnketRepository anketRepository,
        IHastaneRepository hastaneRepository,
        IBirimRepository birimRepository,
        IDoktorRepository doktorRepository)
    {
        _yanitRepository = yanitRepository;
        _anketRepository = anketRepository;
        _hastaneRepository = hastaneRepository;
        _birimRepository = birimRepository;
        _doktorRepository = doktorRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnketYanitiDto>> TumunuGetirAsync(
        DateTime? baslangic = null,
        DateTime? bitis = null,
        int? hastaneId = null,
        int? anketId = null,
        KapsamFiltresi? kapsam = null)
    {
        var yanitlar = await _yanitRepository.GetAllAsync();
        var anketAdlari = (await _anketRepository.GetAllAsync()).ToDictionary(a => a.Id, a => a.Ad);
        var hastaneAdlari = (await _hastaneRepository.GetAllAsync()).ToDictionary(h => h.Id, h => h.Ad);
        var birimAdlari = (await _birimRepository.GetAllAsync()).ToDictionary(b => b.Id, b => b.Ad);
        var doktorlar = (await _doktorRepository.GetAllAsync()).ToDictionary(d => d.Id, d => d);

        var sorgu = yanitlar.AsEnumerable();

        if (baslangic.HasValue)
            sorgu = sorgu.Where(y => y.BaslamaTarihi >= baslangic.Value);
        if (bitis.HasValue)
            sorgu = sorgu.Where(y => y.BaslamaTarihi <= bitis.Value);
        if (hastaneId.HasValue)
            sorgu = sorgu.Where(y => y.HastaneId == hastaneId.Value);
        if (anketId.HasValue)
            sorgu = sorgu.Where(y => y.AnketId == anketId.Value);
        if (kapsam is not null)
            sorgu = sorgu.Where(y => kapsam.Kapsiyor(y.HastaneId, y.BirimId));

        return sorgu
            .OrderByDescending(y => y.BaslamaTarihi)
            .Select(y => new AnketYanitiDto
            {
                Id = y.Id,
                DavetId = y.DavetId,
                AnketId = y.AnketId,
                AnketAdi = anketAdlari.TryGetValue(y.AnketId, out var aa) ? aa : null,
                HastaneId = y.HastaneId,
                HastaneAdi = hastaneAdlari.TryGetValue(y.HastaneId, out var ha) ? ha : null,
                BirimId = y.BirimId,
                BirimAdi = y.BirimId.HasValue && birimAdlari.TryGetValue(y.BirimId.Value, out var ba) ? ba : null,
                DoktorId = y.DoktorId,
                DoktorAdi = y.DoktorId.HasValue && doktorlar.TryGetValue(y.DoktorId.Value, out var dk)
                    ? string.Join(" ", new[] { dk.Unvan, dk.Ad, dk.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p)))
                    : null,
                BaslamaTarihi = y.BaslamaTarihi,
                TamamlanmaTarihi = y.TamamlanmaTarihi,
                GecerliMi = y.GecerliMi,
                IpAdresi = y.IpAdresi
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<AnketYanitiDto?> DetayGetirAsync(int id)
    {
        var yanit = await _yanitRepository.CevaplariylaGetirAsync(id);
        if (yanit is null)
            return null;

        var anket = await _anketRepository.GetByIdAsync(yanit.AnketId);
        var hastane = await _hastaneRepository.GetByIdAsync(yanit.HastaneId);
        Birim? birim = yanit.BirimId.HasValue ? await _birimRepository.GetByIdAsync(yanit.BirimId.Value) : null;
        Doktor? doktor = yanit.DoktorId.HasValue ? await _doktorRepository.GetByIdAsync(yanit.DoktorId.Value) : null;

        return new AnketYanitiDto
        {
            Id = yanit.Id,
            DavetId = yanit.DavetId,
            AnketId = yanit.AnketId,
            AnketAdi = anket?.Ad,
            HastaneId = yanit.HastaneId,
            HastaneAdi = hastane?.Ad,
            BirimId = yanit.BirimId,
            BirimAdi = birim?.Ad,
            DoktorId = yanit.DoktorId,
            DoktorAdi = doktor is null
                ? null
                : string.Join(" ", new[] { doktor.Unvan, doktor.Ad, doktor.Soyad }.Where(p => !string.IsNullOrWhiteSpace(p))),
            KanalAdi = yanit.Davet?.GonderimKanali.ToString(),
            BaslamaTarihi = yanit.BaslamaTarihi,
            TamamlanmaTarihi = yanit.TamamlanmaTarihi,
            GecerliMi = yanit.GecerliMi,
            IpAdresi = yanit.IpAdresi,
            Cevaplar = yanit.Cevaplar
                .OrderBy(c => c.Soru != null ? c.Soru.SiraNo : 0)
                .Select(c => new AnketCevabiDto
                {
                    Id = c.Id,
                    YanitId = c.YanitId,
                    SoruId = c.SoruId,
                    SoruMetni = c.Soru?.SoruMetni,
                    SoruTipi = c.Soru?.SoruTipi ?? SoruTipi.AcikUclu,
                    PuanDegeri = c.PuanDegeri,
                    SecenekId = c.SecenekId,
                    SecenekMetni = c.Secenek?.MetinDegeri,
                    MetinDegeri = c.MetinDegeri,
                    BoolDegeri = c.BoolDegeri,
                    SeciliSecenekIdleri = c.SeciliSecenekIdleri
                })
                .ToList()
        };
    }
}
