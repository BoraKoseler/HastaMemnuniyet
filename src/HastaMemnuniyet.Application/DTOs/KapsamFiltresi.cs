namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// Kullanıcının erişebileceği veri kapsamını (hastane ve birim) tanımlayan filtre nesnesi.
/// Servis metotlarına null geçildiğinde kapsam kısıtlaması uygulanmaz (tam yetki).
/// </summary>
public class KapsamFiltresi
{
    /// <summary>Kullanıcının erişebileceği hastane kimlikleri.</summary>
    public IReadOnlyList<int> HastaneIdleri { get; set; } = new List<int>();

    /// <summary>Kullanıcının erişebileceği birim kimlikleri. Boş ise hastane kapsamındaki tüm birimlere erişilir.</summary>
    public IReadOnlyList<int> BirimIdleri { get; set; } = new List<int>();

    /// <summary>Birim bazlı bir kısıtlama olup olmadığını belirtir.</summary>
    public bool BirimKisitiVar => BirimIdleri.Count > 0;

    /// <summary>Verilen hastane ve birimin bu kapsam içinde olup olmadığını belirler.</summary>
    /// <param name="hastaneId">Değerlendirilecek hastane kimliği.</param>
    /// <param name="birimId">Değerlendirilecek birim kimliği (isteğe bağlı).</param>
    /// <returns>Kayıt kapsam içindeyse true.</returns>
    public bool Kapsiyor(int hastaneId, int? birimId)
    {
        if (HastaneIdleri.Count > 0 && !HastaneIdleri.Contains(hastaneId))
            return false;
        if (BirimKisitiVar)
            return birimId.HasValue && BirimIdleri.Contains(birimId.Value);
        return true;
    }
}
