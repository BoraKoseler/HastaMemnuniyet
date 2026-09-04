using HastaMemnuniyet.Domain.Interfaces;
using QRCoder;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// QRCoder kütüphanesini kullanarak PNG biçiminde QR kod üreten servis.
/// </summary>
public class QrKodUreticiServisi : IQrKodUretici
{
    /// <inheritdoc />
    public byte[] QrKodUret(string icerik)
    {
        if (string.IsNullOrWhiteSpace(icerik))
            throw new ArgumentException("QR kod içeriği boş olamaz.", nameof(icerik));

        using var generator = new QRCodeGenerator();
        using var veri = generator.CreateQrCode(icerik, QRCodeGenerator.ECCLevel.Q);
        var pngQrKod = new PngByteQRCode(veri);
        return pngQrKod.GetGraphic(20);
    }
}
