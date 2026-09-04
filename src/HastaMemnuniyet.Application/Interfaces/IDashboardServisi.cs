using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Dashboard istatistiklerini üreten servis arayüzü.</summary>
public interface IDashboardServisi
{
    /// <summary>Verilen tarih aralığı için dashboard istatistiklerini getirir.</summary>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Dashboard istatistikleri.</returns>
    Task<DashboardDto> IstatistikleriGetirAsync(DateTime? baslangic = null, DateTime? bitis = null, KapsamFiltresi? kapsam = null);

    /// <summary>Kurum genelinde hastane bazlı karşılaştırma özetini getirir (kişisel veri içermez).</summary>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <returns>Hastane karşılaştırma listesi.</returns>
    Task<IReadOnlyList<HastaneKarsilastirmaDto>> HastaneKarsilastirmaGetirAsync(DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null);

    /// <summary>Doktor bazlı ortalama puanları getirir. Yalnızca minimum örneklem eşiğini karşılayan doktorlar döndürülür.</summary>
    /// <param name="minimumOrneklem">Bir doktorun listelenebilmesi için gereken en az geçerli yanıt sayısı.</param>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <returns>Doktor puan listesi.</returns>
    Task<IReadOnlyList<DoktorPuanDto>> DoktorPuanlariGetirAsync(int minimumOrneklem, DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null);

    /// <summary>Son belirtilen ay sayısı için aylık memnuniyet puanı trendini getirir.</summary>
    /// <param name="aySayisi">Geriye dönük dâhil edilecek ay sayısı.</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <returns>Kronolojik sıralı aylık trend listesi.</returns>
    Task<IReadOnlyList<AylikTrendDto>> AylikTrendGetirAsync(int aySayisi, int? hastaneId = null);

    /// <summary>
    /// Verilen tarih aralığı ve kapsam için Net Tavsiye Skorunu (NPS) hesaplar.
    /// Yalnızca <see cref="Domain.Enums.SoruTipi.Nps"/> tipindeki sorulara verilen 0-10 puanlar dikkate alınır.
    /// </summary>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <param name="birimId">Belirli bir birime göre filtre (null ise tüm birimler).</param>
    /// <returns>NPS hesaplama sonucu.</returns>
    Task<NpsSonucuDto> NpsHesaplaAsync(DateTime? baslangic = null, DateTime? bitis = null, int? hastaneId = null, int? birimId = null);

    /// <summary>
    /// İki farklı dönemin memnuniyet göstergelerini (ortalama puan, yanıt sayısı, yanıt oranı, NPS)
    /// karşılaştırarak dönemsel değişimi raporlar.
    /// </summary>
    /// <param name="oncekiBaslangic">Önceki (kıyas) dönemin başlangıç tarihi.</param>
    /// <param name="oncekiBitis">Önceki (kıyas) dönemin bitiş tarihi.</param>
    /// <param name="guncelBaslangic">Güncel dönemin başlangıç tarihi.</param>
    /// <param name="guncelBitis">Güncel dönemin bitiş tarihi.</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <returns>Dönemsel karşılaştırma sonucu.</returns>
    Task<DonemselKarsilastirmaDto> DonemselKarsilastirAsync(
        DateTime oncekiBaslangic, DateTime oncekiBitis,
        DateTime guncelBaslangic, DateTime guncelBitis,
        int? hastaneId = null);
}
