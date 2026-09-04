using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketDto
{
    /// <summary>Anket kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Anket adı.</summary>
    public string Ad { get; set; } = string.Empty;
    /// <summary>Anket açıklaması.</summary>
    public string? Aciklama { get; set; }
    /// <summary>Anket türü.</summary>
    public AnketTuru AnketTuru { get; set; }
    /// <summary>Anketin aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Anket sürüm numarası.</summary>
    public int SurumNo { get; set; }
    /// <summary>Sürüm zincirinin kök (ilk sürüm) anket kimliği. İlk sürümde null olabilir.</summary>
    public int? AnaSurumAnketId { get; set; }
    /// <summary>Gizlilik metni.</summary>
    public string? GizlilikMetni { get; set; }
    /// <summary>Tahmini doldurulma süresi (dakika).</summary>
    public int TahminiSureDakika { get; set; }
    /// <summary>Ankete ait soru sayısı.</summary>
    public int SoruSayisi { get; set; }
    /// <summary>Ankete ait sorular.</summary>
    public List<AnketSorusuDto> Sorular { get; set; } = new();
}
