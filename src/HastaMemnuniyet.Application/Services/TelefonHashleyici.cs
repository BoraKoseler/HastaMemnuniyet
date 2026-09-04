using System.Security.Cryptography;
using System.Text;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Telefon numaralarını geri döndürülemez biçimde hash'lemek için yardımcı sınıf.
/// Ham telefon numarası veritabanında saklanmaz; yalnızca hash değeri tutulur.
/// </summary>
public static class TelefonHashleyici
{
    /// <summary>Verilen telefon numarasının SHA-256 hash değerini üretir.</summary>
    /// <param name="telefonNumarasi">Hash'lenecek telefon numarası.</param>
    /// <returns>Onaltılık (hex) biçiminde hash değeri; numara boşsa boş dizge.</returns>
    public static string Hashle(string? telefonNumarasi)
    {
        if (string.IsNullOrWhiteSpace(telefonNumarasi))
            return string.Empty;

        var normalize = new string(telefonNumarasi.Where(char.IsDigit).ToArray());
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalize));
        return Convert.ToHexString(bytes);
    }
}
