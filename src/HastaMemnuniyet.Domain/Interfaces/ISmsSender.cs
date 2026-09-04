using HastaMemnuniyet.Domain.ValueObjects;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// SMS gönderim sağlayıcısı için soyutlama. Farklı sağlayıcılar bu arayüzü uygular.
/// </summary>
public interface ISmsSender
{
    /// <summary>Verilen numaraya SMS gönderir.</summary>
    /// <param name="telefonNumarasi">Hedef telefon numarası.</param>
    /// <param name="mesaj">Gönderilecek mesaj metni.</param>
    /// <returns>Gönderim sonucu.</returns>
    Task<SmsGonderimSonucu> GonderAsync(string telefonNumarasi, string mesaj);
}
