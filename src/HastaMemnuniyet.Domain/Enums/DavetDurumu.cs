namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Anket davetinin yaşam döngüsündeki durumunu belirtir.
/// </summary>
public enum DavetDurumu
{
    /// <summary>Davet oluşturuldu, henüz gönderilmedi.</summary>
    Olusturuldu = 1,

    /// <summary>Davet hastaya gönderildi.</summary>
    Gonderildi = 2,

    /// <summary>Davet bağlantısı hasta tarafından açıldı.</summary>
    Acildi = 3,

    /// <summary>Anket kısmen dolduruldu.</summary>
    KismiTamamlandi = 4,

    /// <summary>Anket tamamen dolduruldu.</summary>
    Tamamlandi = 5,

    /// <summary>Davetin geçerlilik süresi doldu.</summary>
    SuresiDoldu = 6,

    /// <summary>Davet geçersiz kılındı.</summary>
    Gecersiz = 7
}
