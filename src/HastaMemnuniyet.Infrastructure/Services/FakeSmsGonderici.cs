using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// Gerçek SMS göndermeyen, mesajı log'a yazan sahte SMS gönderim sağlayıcısı.
/// Geliştirme ve test ortamları için kullanılır. Faz 2+ ile gerçek sağlayıcı ile değiştirilebilir.
/// </summary>
public class FakeSmsGonderici : ISmsSender
{
    private readonly ILogger<FakeSmsGonderici> _logger;

    /// <summary>Yeni bir <see cref="FakeSmsGonderici"/> örneği oluşturur.</summary>
    /// <param name="logger">Günlükleme bileşeni.</param>
    public FakeSmsGonderici(ILogger<FakeSmsGonderici> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<SmsGonderimSonucu> GonderAsync(string telefonNumarasi, string mesaj)
    {
        _logger.LogInformation(
            "[SAHTE SMS] Numara: {Telefon} | Mesaj: {Mesaj}",
            telefonNumarasi,
            mesaj);

        var referans = $"FAKE-{Guid.NewGuid():N}";
        return Task.FromResult(SmsGonderimSonucu.BasariliOlustur(
            "Sahte SMS sağlayıcısı üzerinden başarıyla loglandı.",
            referans));
    }
}
