using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir iyileştirme aksiyonunun durum değişikliklerini izleyen geçmiş kaydını temsil eder.
/// </summary>
public class AksiyonGecmisi
{
    /// <summary>Geçmiş kaydının birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>İlgili iyileştirme aksiyonunun kimliği.</summary>
    public int AksiyonId { get; set; }

    /// <summary>Değişiklik öncesi durum.</summary>
    public AksiyonDurumu EskiDurum { get; set; }

    /// <summary>Değişiklik sonrası durum.</summary>
    public AksiyonDurumu YeniDurum { get; set; }

    /// <summary>Değişikliğe dair açıklama (isteğe bağlı).</summary>
    public string? Aciklama { get; set; }

    /// <summary>Değişikliği yapan kullanıcının kimliği.</summary>
    public string KullaniciId { get; set; } = string.Empty;

    /// <summary>Değişikliğin gerçekleştiği tarih.</summary>
    public DateTime Tarih { get; set; } = DateTime.UtcNow;

    /// <summary>Geçmiş kaydının ilişkili olduğu iyileştirme aksiyonu.</summary>
    public IyilestirmeAksiyonu? Aksiyon { get; set; }
}
