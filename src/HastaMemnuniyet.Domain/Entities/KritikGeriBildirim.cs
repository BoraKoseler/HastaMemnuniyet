namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir kuralın tetiklenmesi sonucu oluşturulan kritik geri bildirim kaydını temsil eder.
/// </summary>
public class KritikGeriBildirim : BaseEntity
{
    /// <summary>Kritik geri bildirimin kaynaklandığı anket yanıtının kimliği.</summary>
    public int YanitId { get; set; }

    /// <summary>Kritik geri bildirimi tetikleyen tekil cevabın kimliği (isteğe bağlı).</summary>
    public int? CevapId { get; set; }

    /// <summary>Tetiklenen kuralın kimliği.</summary>
    public int KuralId { get; set; }

    /// <summary>İlgili hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>İlgili birimin kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }

    /// <summary>İlgili doktorun kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }

    /// <summary>Kritik geri bildirime dair açıklama.</summary>
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Kritik geri bildirimin kaynaklandığı anket yanıtı.</summary>
    public AnketYaniti? Yanit { get; set; }

    /// <summary>Kritik geri bildirimi tetikleyen cevap (isteğe bağlı).</summary>
    public AnketCevabi? Cevap { get; set; }

    /// <summary>Tetiklenen kural.</summary>
    public KritikGeriBildirimKurali? Kural { get; set; }

    /// <summary>Bu kritik geri bildirime bağlı iyileştirme aksiyonları.</summary>
    public ICollection<IyilestirmeAksiyonu> Aksiyonlar { get; set; } = new List<IyilestirmeAksiyonu>();
}
