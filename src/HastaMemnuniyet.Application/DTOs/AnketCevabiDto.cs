namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket cevabı bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketCevabiDto
{
    /// <summary>Cevap kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Yanıt kimliği.</summary>
    public int YanitId { get; set; }
    /// <summary>Soru kimliği.</summary>
    public int SoruId { get; set; }
    /// <summary>Soru metni.</summary>
    public string? SoruMetni { get; set; }
    /// <summary>Soru tipi (görüntüleme için).</summary>
    public Domain.Enums.SoruTipi SoruTipi { get; set; }
    /// <summary>Seçilen seçeneğin metni (tek seçim için).</summary>
    public string? SecenekMetni { get; set; }
    /// <summary>Puan değeri.</summary>
    public int? PuanDegeri { get; set; }
    /// <summary>Seçilen seçenek kimliği.</summary>
    public int? SecenekId { get; set; }
    /// <summary>Metin değeri.</summary>
    public string? MetinDegeri { get; set; }
    /// <summary>Boolean değer.</summary>
    public bool? BoolDegeri { get; set; }
    /// <summary>Çoklu seçimde seçili seçenek kimlikleri (virgüllü).</summary>
    public string? SeciliSecenekIdleri { get; set; }
}
