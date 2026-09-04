using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>
/// KVKK veri saklama politikası kapsamında süresi dolan kişisel verilerin
/// tespit edilmesi ve anonimleştirilmesi iş kurallarını tanımlayan servis arayüzü.
/// </summary>
public interface IVeriSaklamaPolitikasiServisi
{
    /// <summary>
    /// Yapılandırılan saklama süreleri ışığında veri saklama durumunun özetini getirir.
    /// </summary>
    /// <returns>Saklama politikası özeti.</returns>
    Task<VeriSaklamaOzetiDto> OzetGetirAsync();

    /// <summary>
    /// Kişisel veri saklama süresi dolmuş ve hâlâ kişisel veri içeren yanıtları getirir.
    /// </summary>
    /// <returns>Süresi dolan kayıtların listesi.</returns>
    Task<IReadOnlyList<SuresiDolanKayitDto>> SuresiDolanKayitlariGetirAsync();

    /// <summary>
    /// Tek bir anket yanıtındaki kişisel verileri (IP, istemci bilgisi, telefon hash) temizler.
    /// Bu işlem geri alınamaz.
    /// </summary>
    /// <param name="yanitId">Anonimleştirilecek yanıtın kimliği.</param>
    Task AnonimlestirAsync(int yanitId);

    /// <summary>
    /// Kişisel veri saklama süresi dolmuş tüm yanıtları toplu olarak anonimleştirir.
    /// </summary>
    /// <returns>İşlemin sonucunu özetleyen imha raporu.</returns>
    Task<ImhaRaporuDto> SuresiDolanlariAnonimlestirAsync();
}
