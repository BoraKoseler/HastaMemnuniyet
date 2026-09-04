namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Sistemde yapılan kritik işlemleri denetim kaydı olarak loglayan altyapı arayüzü.
/// </summary>
public interface IAuditLogger
{
    /// <summary>Bir denetim kaydı oluşturur.</summary>
    /// <param name="islem">Yapılan işlemin türü (örn. Giris, AnketGuncelle, DisaAktar).</param>
    /// <param name="tablo">İşlemin uygulandığı tablo/varlık adı (isteğe bağlı).</param>
    /// <param name="kayitId">İşlemin uygulandığı kaydın kimliği (isteğe bağlı).</param>
    /// <param name="eskiDeger">Değişiklik öncesi değer (isteğe bağlı).</param>
    /// <param name="yeniDeger">Değişiklik sonrası değer (isteğe bağlı).</param>
    /// <param name="detay">İşleme dair ek açıklama (isteğe bağlı).</param>
    /// <returns>Asenkron işlem görevi.</returns>
    Task LoglaAsync(string islem, string? tablo = null, string? kayitId = null, string? eskiDeger = null, string? yeniDeger = null, string? detay = null);
}
