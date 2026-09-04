namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Tüm varlıklar için ortak kimlik ve zaman damgası alanlarını sağlayan temel varlık sınıfı.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Varlığın birincil anahtar kimliği.</summary>
    public int Id { get; set; }

    /// <summary>Varlığın oluşturulma tarihi.</summary>
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Varlığın son güncellenme tarihi. Hiç güncellenmediyse null olabilir.</summary>
    public DateTime? GuncellenmeTarihi { get; set; }
}
