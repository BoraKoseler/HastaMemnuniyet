namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Davetler için benzersiz, tahmin edilemez token üretimini tanımlar.
/// </summary>
public interface ITokenUretici
{
    /// <summary>Benzersiz ve tahmin edilemez bir token üretir.</summary>
    /// <returns>Üretilen token değeri.</returns>
    string BenzersizTokenUret();
}
