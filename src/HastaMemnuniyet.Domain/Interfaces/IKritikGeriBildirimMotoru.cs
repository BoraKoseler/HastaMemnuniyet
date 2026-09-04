using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Anket yanıtlarını tanımlı kurallara göre değerlendirip kritik geri bildirimleri üreten motor arayüzü.
/// </summary>
public interface IKritikGeriBildirimMotoru
{
    /// <summary>Verilen yanıt ve cevapları tanımlı kurallara göre değerlendirir.</summary>
    /// <param name="yanit">Değerlendirilecek anket yanıtı.</param>
    /// <param name="cevaplar">Yanıta ait cevaplar.</param>
    /// <returns>Tetiklenen kurallar sonucu oluşan kritik geri bildirimler.</returns>
    Task<List<KritikGeriBildirim>> DegerlendiAsync(AnketYaniti yanit, List<AnketCevabi> cevaplar);
}
