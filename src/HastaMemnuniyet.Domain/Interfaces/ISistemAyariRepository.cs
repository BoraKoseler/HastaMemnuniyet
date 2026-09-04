using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Sistem ayarı varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface ISistemAyariRepository
{
    /// <summary>Verilen anahtara sahip ayarı getirir.</summary>
    /// <param name="anahtar">Ayar anahtarı.</param>
    /// <returns>Bulunan ayar ya da null.</returns>
    Task<SistemAyari?> AnahtaraGoreGetirAsync(string anahtar);

    /// <summary>Tüm sistem ayarlarını getirir.</summary>
    /// <returns>Ayar listesi.</returns>
    Task<IReadOnlyList<SistemAyari>> TumunuGetirAsync();

    /// <summary>Verilen anahtarın değerini getirir; yoksa varsayılan değeri döner.</summary>
    /// <param name="anahtar">Ayar anahtarı.</param>
    /// <param name="varsayilan">Ayar bulunamazsa dönecek varsayılan değer.</param>
    /// <returns>Ayar değeri ya da varsayılan.</returns>
    Task<string> DegerGetirAsync(string anahtar, string varsayilan);

    /// <summary>Yeni bir ayar ekler.</summary>
    /// <param name="ayar">Eklenecek ayar.</param>
    /// <returns>Eklenen ayar.</returns>
    Task<SistemAyari> AddAsync(SistemAyari ayar);

    /// <summary>Bir ayarı günceller.</summary>
    /// <param name="ayar">Güncellenecek ayar.</param>
    void Update(SistemAyari ayar);

    /// <summary>Bekleyen değişiklikleri kaydeder.</summary>
    /// <returns>Etkilenen kayıt sayısı.</returns>
    Task<int> SaveChangesAsync();
}
