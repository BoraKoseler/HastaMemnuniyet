namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Kritik geri bildirim kuralının değerlendirme tipini belirtir.
/// </summary>
public enum KuralTipi
{
    /// <summary>Puanlama sorusunda belirlenen eşik değerinin altındaki puanları kritik kabul eder.</summary>
    PuanAlti = 1,

    /// <summary>Seçim sorusunda belirli bir seçeneğin işaretlenmesini kritik kabul eder.</summary>
    SecenekEslesmesi = 2,

    /// <summary>Açık uçlu cevaplarda belirlenen anahtar kelimelerin geçmesini kritik kabul eder.</summary>
    AnahtarKelime = 3
}
