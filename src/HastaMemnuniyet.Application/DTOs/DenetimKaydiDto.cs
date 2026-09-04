namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Denetim kaydı görüntüleme bilgilerini taşıyan veri transfer nesnesi.</summary>
public class DenetimKaydiDto
{
    /// <summary>Kaydın birincil anahtarı.</summary>
    public long Id { get; set; }

    /// <summary>İşlemi yapan kullanıcının kimliği.</summary>
    public string? KullaniciId { get; set; }

    /// <summary>İşlemi yapan kullanıcının adı.</summary>
    public string? KullaniciAdi { get; set; }

    /// <summary>Yapılan işlemin türü.</summary>
    public string Islem { get; set; } = string.Empty;

    /// <summary>İşlemin uygulandığı tablo/varlık adı.</summary>
    public string? Tablo { get; set; }

    /// <summary>İşlemin uygulandığı kaydın kimliği.</summary>
    public string? KayitId { get; set; }

    /// <summary>Değişiklik öncesi değer.</summary>
    public string? EskiDeger { get; set; }

    /// <summary>Değişiklik sonrası değer.</summary>
    public string? YeniDeger { get; set; }

    /// <summary>İşlemin gerçekleştiği IP adresi.</summary>
    public string? IpAdresi { get; set; }

    /// <summary>İşlemin gerçekleştiği tarih.</summary>
    public DateTime Tarih { get; set; }

    /// <summary>İşleme dair ek detay bilgisi.</summary>
    public string? Detay { get; set; }
}
