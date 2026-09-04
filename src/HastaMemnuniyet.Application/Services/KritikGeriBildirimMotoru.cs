using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Anket yanıtlarını tanımlı kritik geri bildirim kurallarına göre değerlendiren motor.
/// </summary>
public class KritikGeriBildirimMotoru : IKritikGeriBildirimMotoru
{
    private readonly IGenericRepository<KritikGeriBildirimKurali> _kuralRepository;

    /// <summary>Yeni bir <see cref="KritikGeriBildirimMotoru"/> örneği oluşturur.</summary>
    /// <param name="kuralRepository">Kural veri erişim bileşeni.</param>
    public KritikGeriBildirimMotoru(IGenericRepository<KritikGeriBildirimKurali> kuralRepository)
    {
        _kuralRepository = kuralRepository;
    }

    /// <inheritdoc />
    public async Task<List<KritikGeriBildirim>> DegerlendiAsync(AnketYaniti yanit, List<AnketCevabi> cevaplar)
    {
        ArgumentNullException.ThrowIfNull(yanit);
        ArgumentNullException.ThrowIfNull(cevaplar);

        var sonuclar = new List<KritikGeriBildirim>();
        var tumKurallar = await _kuralRepository.GetAllAsync();
        var aktifKurallar = tumKurallar
            .Where(k => k.AktifMi && (!k.AnketId.HasValue || k.AnketId.Value == yanit.AnketId))
            .ToList();

        foreach (var kural in aktifKurallar)
        {
            var degerlendirilecek = kural.SoruId.HasValue
                ? cevaplar.Where(c => c.SoruId == kural.SoruId.Value)
                : cevaplar;

            foreach (var cevap in degerlendirilecek)
            {
                if (!KuralTetiklendiMi(kural, cevap, out var aciklama))
                    continue;

                sonuclar.Add(new KritikGeriBildirim
                {
                    YanitId = yanit.Id,
                    CevapId = cevap.Id == 0 ? null : cevap.Id,
                    KuralId = kural.Id,
                    HastaneId = yanit.HastaneId,
                    BirimId = yanit.BirimId,
                    DoktorId = yanit.DoktorId,
                    Aciklama = aciklama
                });
            }
        }

        return sonuclar;
    }

    /// <summary>Bir kuralın verilen cevap için tetiklenip tetiklenmediğini değerlendirir.</summary>
    /// <param name="kural">Değerlendirilecek kural.</param>
    /// <param name="cevap">Değerlendirilecek cevap.</param>
    /// <param name="aciklama">Kural tetiklendiğinde üretilen açıklama.</param>
    /// <returns>Kural tetiklendiyse true.</returns>
    private static bool KuralTetiklendiMi(KritikGeriBildirimKurali kural, AnketCevabi cevap, out string aciklama)
    {
        aciklama = string.Empty;

        switch (kural.KuralTipi)
        {
            case KuralTipi.PuanAlti:
                if (kural.EsikDegeri.HasValue && cevap.PuanDegeri.HasValue
                    && cevap.PuanDegeri.Value <= kural.EsikDegeri.Value)
                {
                    aciklama = $"Puan ({cevap.PuanDegeri.Value}) belirlenen eşik değerinin ({kural.EsikDegeri.Value}) altında veya eşit.";
                    return true;
                }
                return false;

            case KuralTipi.SecenekEslesmesi:
                if (kural.HedefSecenekId.HasValue && SecenekEslesiyorMu(cevap, kural.HedefSecenekId.Value))
                {
                    aciklama = "Kritik olarak işaretlenen seçenek tercih edildi.";
                    return true;
                }
                return false;

            case KuralTipi.AnahtarKelime:
                if (!string.IsNullOrWhiteSpace(kural.AnahtarKelimeler)
                    && !string.IsNullOrWhiteSpace(cevap.MetinDegeri))
                {
                    var eslesen = AnahtarKelimeBul(kural.AnahtarKelimeler, cevap.MetinDegeri);
                    if (eslesen is not null)
                    {
                        aciklama = $"Açık uçlu cevapta kritik anahtar kelime tespit edildi: '{eslesen}'.";
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }

    /// <summary>Cevabın (tek ya da çoklu seçim) hedef seçeneği içerip içermediğini denetler.</summary>
    /// <param name="cevap">Denetlenecek cevap.</param>
    /// <param name="hedefSecenekId">Hedef seçenek kimliği.</param>
    /// <returns>Seçenek eşleşiyorsa true.</returns>
    private static bool SecenekEslesiyorMu(AnketCevabi cevap, int hedefSecenekId)
    {
        if (cevap.SecenekId.HasValue && cevap.SecenekId.Value == hedefSecenekId)
            return true;

        if (!string.IsNullOrWhiteSpace(cevap.SeciliSecenekIdleri))
        {
            var idler = cevap.SeciliSecenekIdleri
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var id in idler)
            {
                if (int.TryParse(id, out var deger) && deger == hedefSecenekId)
                    return true;
            }
        }

        return false;
    }

    /// <summary>Metin içinde geçen ilk kritik anahtar kelimeyi bulur.</summary>
    /// <param name="anahtarKelimeler">Virgülle ayrılmış anahtar kelime listesi.</param>
    /// <param name="metin">Aranacak metin.</param>
    /// <returns>Bulunan anahtar kelime ya da null.</returns>
    private static string? AnahtarKelimeBul(string anahtarKelimeler, string metin)
    {
        var kelimeler = anahtarKelimeler
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var kelime in kelimeler)
        {
            if (metin.Contains(kelime, StringComparison.OrdinalIgnoreCase))
                return kelime;
        }

        return null;
    }
}
