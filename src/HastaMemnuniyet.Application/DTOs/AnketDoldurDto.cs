using System.ComponentModel.DataAnnotations;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Hastanın anketi doldururken gönderdiği cevapları taşıyan veri transfer nesnesi.</summary>
public class AnketDoldurDto
{
    /// <summary>Davet token değeri.</summary>
    [Required]
    public string Token { get; set; } = string.Empty;
    /// <summary>Verilen cevaplar.</summary>
    public List<AnketDoldurCevapDto> Cevaplar { get; set; } = new();
}

/// <summary>Anket doldururken tek bir soruya verilen cevabı taşıyan veri transfer nesnesi.</summary>
public class AnketDoldurCevapDto
{
    /// <summary>Cevaplanan sorunun kimliği.</summary>
    public int SoruId { get; set; }
    /// <summary>Puanlama tipi cevap değeri.</summary>
    public int? PuanDegeri { get; set; }
    /// <summary>Tek seçim cevabı için seçenek kimliği.</summary>
    public int? SecenekId { get; set; }
    /// <summary>Açık uçlu cevap metni.</summary>
    public string? MetinDegeri { get; set; }
    /// <summary>Evet/Hayır cevabı.</summary>
    public bool? BoolDegeri { get; set; }
    /// <summary>Çoklu seçim için seçilen seçenek kimlikleri.</summary>
    public List<int> SeciliSecenekIdleri { get; set; } = new();
}
