namespace HastaMemnuniyet.Infrastructure.Identity;

/// <summary>
/// Uygulamada kullanılan rol adlarını sabit olarak tanımlar.
/// Magic string kullanımını önlemek için tüm rol adları buradan referanslanır.
/// </summary>
public static class RolSabitleri
{
    /// <summary>Sistem yöneticisi rolü.</summary>
    public const string Admin = "Admin";

    /// <summary>Kalite birimi rolü.</summary>
    public const string KaliteBirimi = "KaliteBirimi";

    /// <summary>
    /// Birim yöneticisi rolü. Yalnızca kendi kapsamındaki (hastane/birim) verileri görüntüleyebilir.
    /// </summary>
    public const string BirimYoneticisi = "BirimYoneticisi";

    /// <summary>
    /// Üst yönetim rolü. Kişisel veri içermeyen, kurum genelinde özet ve karşılaştırmalı
    /// raporlara (üst yönetim panosu) erişim sağlar.
    /// </summary>
    public const string UstYonetim = "UstYonetim";

    /// <summary>Sistemde tanımlı tüm roller.</summary>
    public static readonly IReadOnlyList<string> TumRoller = new[] { Admin, KaliteBirimi, BirimYoneticisi, UstYonetim };
}
