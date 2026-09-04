namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Anketin hedeflediği hasta/başvuru türünü belirtir.
/// </summary>
public enum AnketTuru
{
    /// <summary>Ayaktan (poliklinik) hasta anketi.</summary>
    Ayaktan = 1,

    /// <summary>Yatan hasta anketi.</summary>
    Yatan = 2,

    /// <summary>Acil servis hasta anketi.</summary>
    Acil = 3,

    /// <summary>Taburculuk sürecine yönelik anket.</summary>
    Taburculuk = 4,

    /// <summary>Refakatçiye yönelik anket.</summary>
    Refakatci = 5
}
