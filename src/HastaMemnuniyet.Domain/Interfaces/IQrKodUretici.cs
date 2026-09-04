namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Verilen metni ya da URL'yi QR kod görüntüsüne dönüştüren üretici arayüzü.
/// </summary>
public interface IQrKodUretici
{
    /// <summary>Verilen metin için PNG biçiminde QR kod üretir.</summary>
    /// <param name="icerik">QR koda gömülecek metin ya da URL.</param>
    /// <returns>PNG biçimindeki QR kod görüntüsünün bayt dizisi.</returns>
    byte[] QrKodUret(string icerik);
}
