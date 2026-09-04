using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Kritik geri bildirim bilgilerini taşıyan veri transfer nesnesi.</summary>
public class KritikGeriBildirimDto
{
    /// <summary>Kritik geri bildirim kimliği.</summary>
    public int Id { get; set; }
    /// <summary>İlgili anket yanıtının kimliği.</summary>
    public int YanitId { get; set; }
    /// <summary>Tetikleyen cevabın kimliği (isteğe bağlı).</summary>
    public int? CevapId { get; set; }
    /// <summary>Tetiklenen kuralın kimliği.</summary>
    public int KuralId { get; set; }
    /// <summary>Tetiklenen kuralın tipi.</summary>
    public KuralTipi? KuralTipi { get; set; }
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string? HastaneAdi { get; set; }
    /// <summary>Birim kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }
    /// <summary>Birim adı.</summary>
    public string? BirimAdi { get; set; }
    /// <summary>Doktor kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }
    /// <summary>Doktor adı.</summary>
    public string? DoktorAdi { get; set; }
    /// <summary>Anket adı.</summary>
    public string? AnketAdi { get; set; }
    /// <summary>Kritik geri bildirime dair açıklama.</summary>
    public string Aciklama { get; set; } = string.Empty;
    /// <summary>Oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; }
    /// <summary>Bu kritik geri bildirime bağlı aksiyon sayısı.</summary>
    public int AksiyonSayisi { get; set; }
    /// <summary>İlgili yanıta ait cevaplar (detay görünümü için).</summary>
    public List<AnketCevabiDto> Cevaplar { get; set; } = new();
    /// <summary>Bu kritik geri bildirime bağlı aksiyonlar (detay görünümü için).</summary>
    public List<IyilestirmeAksiyonuDto> Aksiyonlar { get; set; } = new();
}
