namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Sistemde yapılan işlemlerin izlenebilirliği için tutulan denetim kaydı varlığı.
/// </summary>
public class DenetimKaydi
{
    /// <summary>Kaydın birincil anahtarı.</summary>
    public long Id { get; set; }

    /// <summary>İşlemi yapan kullanıcının kimliği (isteğe bağlı).</summary>
    public string? KullaniciId { get; set; }

    /// <summary>İşlemi yapan kullanıcının adı (isteğe bağlı).</summary>
    public string? KullaniciAdi { get; set; }

    /// <summary>Yapılan işlemin türü (örn. Ekle, Guncelle, Sil).</summary>
    public string Islem { get; set; } = string.Empty;

    /// <summary>İşlemin uygulandığı tablo/varlık adı.</summary>
    public string? Tablo { get; set; }

    /// <summary>İşlemin uygulandığı kaydın kimliği.</summary>
    public string? KayitId { get; set; }

    /// <summary>Değişiklik öncesi değer (JSON).</summary>
    public string? EskiDeger { get; set; }

    /// <summary>Değişiklik sonrası değer (JSON).</summary>
    public string? YeniDeger { get; set; }

    /// <summary>İşlemin gerçekleştiği IP adresi.</summary>
    public string? IpAdresi { get; set; }

    /// <summary>İşlemin gerçekleştiği tarih.</summary>
    public DateTime Tarih { get; set; } = DateTime.UtcNow;

    /// <summary>İşleme dair ek detay bilgisi.</summary>
    public string? Detay { get; set; }
}
