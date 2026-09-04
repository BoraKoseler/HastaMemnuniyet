namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Geri bildirime dayalı aksiyonun durumunu belirtir. (İleride kullanılacak.)
/// </summary>
public enum AksiyonDurumu
{
    /// <summary>Aksiyon açık, henüz ele alınmadı.</summary>
    Acik = 1,

    /// <summary>Aksiyon üzerinde çalışılıyor.</summary>
    DevamEdiyor = 2,

    /// <summary>Aksiyon tamamlandı.</summary>
    Tamamlandi = 3,

    /// <summary>Aksiyon iptal edildi.</summary>
    Iptal = 4
}
