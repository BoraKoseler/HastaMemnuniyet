namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// SMS gönderim kaydının durumunu belirtir.
/// </summary>
public enum SmsDurumu
{
    /// <summary>SMS gönderim kuyruğunda bekliyor.</summary>
    Kuyrukta = 1,

    /// <summary>SMS sağlayıcısına gönderildi.</summary>
    Gonderildi = 2,

    /// <summary>SMS gönderimi başarısız oldu.</summary>
    Basarisiz = 3,

    /// <summary>SMS hedef numaraya ulaştırıldı.</summary>
    Ulastirildi = 4
}
