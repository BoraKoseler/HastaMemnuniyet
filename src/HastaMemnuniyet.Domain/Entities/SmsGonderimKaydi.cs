using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir davet için yapılan SMS gönderiminin kayıt varlığı.
/// </summary>
public class SmsGonderimKaydi
{
    /// <summary>Kaydın birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>SMS'in ilişkili olduğu davetin kimliği.</summary>
    public int DavetId { get; set; }

    /// <summary>SMS gönderilen telefon numarasının hash değeri.</summary>
    public string? TelefonHash { get; set; }

    /// <summary>SMS gönderim durumu.</summary>
    public SmsDurumu SmsDurumu { get; set; } = SmsDurumu.Kuyrukta;

    /// <summary>SMS sağlayıcısından dönen yanıt bilgisi.</summary>
    public string? SmsYaniti { get; set; }

    /// <summary>SMS'in gönderildiği tarih (isteğe bağlı).</summary>
    public DateTime? GonderimTarihi { get; set; }

    /// <summary>Kaydın oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>SMS'in ilişkili olduğu davet.</summary>
    public AnketDaveti? Davet { get; set; }
}
