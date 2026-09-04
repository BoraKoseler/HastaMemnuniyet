namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Anket davetinin hastaya ulaştırıldığı kanalı belirtir.
/// </summary>
public enum GonderimKanali
{
    /// <summary>SMS bağlantısı ile gönderim.</summary>
    Sms = 1,

    /// <summary>QR kod ile erişim.</summary>
    Qr = 2,

    /// <summary>Manuel (elle) oluşturulan davet.</summary>
    Manuel = 3
}
