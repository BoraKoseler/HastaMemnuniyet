using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>
/// Açık uçlu (serbest metin) yanıtlar üzerinde anahtar kelime/frekans analizi yapan servis arayüzü.
/// </summary>
public interface IMetinAnaliziServisi
{
    /// <summary>
    /// Verilen tarih aralığı ve kapsam için açık uçlu yanıtlardaki en sık geçen anahtar kelimeleri çıkarır.
    /// Türkçe etkisiz kelimeler (stop-word) analiz dışında bırakılır.
    /// </summary>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <param name="hastaneId">Belirli bir hastaneye göre filtre (null ise tüm hastaneler).</param>
    /// <param name="birimId">Belirli bir birime göre filtre (null ise tüm birimler).</param>
    /// <param name="enFazlaKelime">Döndürülecek en fazla anahtar kelime sayısı.</param>
    /// <returns>Anahtar kelime analizi sonucu.</returns>
    Task<MetinAnaliziSonucuDto> AnahtarKelimeleriGetirAsync(
        DateTime? baslangic = null,
        DateTime? bitis = null,
        int? hastaneId = null,
        int? birimId = null,
        int enFazlaKelime = 30);
}
