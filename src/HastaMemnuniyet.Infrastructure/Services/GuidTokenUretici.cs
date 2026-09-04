using System.Security.Cryptography;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// Guid ve kriptografik rastgele bayt birleşimiyle benzersiz, tahmin edilemez token üreten servis.
/// </summary>
public class GuidTokenUretici : ITokenUretici
{
    /// <summary>Üretilen kriptografik rastgele kısmın bayt uzunluğu.</summary>
    private const int RastgeleBaytUzunlugu = 16;

    /// <inheritdoc />
    public string BenzersizTokenUret()
    {
        var guidKismi = Guid.NewGuid().ToString("N");
        var rastgeleBaytlar = RandomNumberGenerator.GetBytes(RastgeleBaytUzunlugu);
        var rastgeleKisim = Convert.ToHexString(rastgeleBaytlar).ToLowerInvariant();
        return $"{guidKismi}{rastgeleKisim}";
    }
}
