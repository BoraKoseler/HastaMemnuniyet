namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Geri bildirime dayalı aksiyonun öncelik seviyesini belirtir. (İleride kullanılacak.)
/// </summary>
public enum AksiyonOnceligi
{
    /// <summary>Düşük öncelik.</summary>
    Dusuk = 1,

    /// <summary>Orta öncelik.</summary>
    Orta = 2,

    /// <summary>Yüksek öncelik.</summary>
    Yuksek = 3,

    /// <summary>Kritik öncelik.</summary>
    Kritik = 4
}
