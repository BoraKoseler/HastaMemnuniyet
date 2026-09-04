namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir yanıt içindeki tek bir soruya verilen cevabı temsil eden varlık.
/// </summary>
public class AnketCevabi
{
    /// <summary>Cevabın birincil anahtarı.</summary>
    public int Id { get; set; }

    /// <summary>Cevabın ilişkili olduğu yanıtın kimliği.</summary>
    public int YanitId { get; set; }

    /// <summary>Cevabın verildiği sorunun kimliği.</summary>
    public int SoruId { get; set; }

    /// <summary>Puanlama tipi sorular için verilen puan değeri.</summary>
    public int? PuanDegeri { get; set; }

    /// <summary>Tek seçim tipi sorular için seçilen seçeneğin kimliği.</summary>
    public int? SecenekId { get; set; }

    /// <summary>Açık uçlu sorular için girilen metin değeri.</summary>
    public string? MetinDegeri { get; set; }

    /// <summary>Evet/Hayır tipi sorular için verilen boolean değer.</summary>
    public bool? BoolDegeri { get; set; }

    /// <summary>Cevabın verildiği tarih.</summary>
    public DateTime YanitTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Çoklu seçim tipi sorular için seçilen seçenek kimliklerinin virgülle ayrılmış listesi.</summary>
    public string? SeciliSecenekIdleri { get; set; }

    /// <summary>Cevabın ilişkili olduğu yanıt.</summary>
    public AnketYaniti? Yanit { get; set; }

    /// <summary>Cevabın verildiği soru.</summary>
    public AnketSorusu? Soru { get; set; }

    /// <summary>Tek seçim tipi cevaplarda seçilen seçenek.</summary>
    public AnketSoruSecenegi? Secenek { get; set; }
}
