using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Hasta memnuniyetini ölçen anket şablonu varlığı.
/// </summary>
public class Anket : BaseEntity
{
    /// <summary>Anketin adı.</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Anketin açıklaması.</summary>
    public string? Aciklama { get; set; }

    /// <summary>Anketin hedeflediği hasta/başvuru türü.</summary>
    public AnketTuru AnketTuru { get; set; }

    /// <summary>Anketin aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Anketin sürüm numarası.</summary>
    public int SurumNo { get; set; } = 1;

    /// <summary>
    /// Sürüm zincirinin kök (ilk) anketinin kimliği. Aynı anketin farklı sürümlerini
    /// gruplamak için kullanılır. İlk sürümde null'dır; sonraki sürümler kök anketi işaret eder.
    /// </summary>
    public int? AnaSurumAnketId { get; set; }

    /// <summary>Ankete katılım öncesi gösterilecek gizlilik metni.</summary>
    public string? GizlilikMetni { get; set; }

    /// <summary>Anketin tahmini doldurulma süresi (dakika).</summary>
    public int TahminiSureDakika { get; set; }

    /// <summary>Ankete ait sorular.</summary>
    public ICollection<AnketSorusu> Sorular { get; set; } = new List<AnketSorusu>();

    /// <summary>Ankete ait davetler.</summary>
    public ICollection<AnketDaveti> Davetler { get; set; } = new List<AnketDaveti>();
}
