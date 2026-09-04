namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir davete karşılık gelen, hastanın anketi doldurma oturumunu temsil eden yanıt varlığı.
/// </summary>
public class AnketYaniti : BaseEntity
{
    /// <summary>Yanıtın ilişkili olduğu davetin kimliği.</summary>
    public int DavetId { get; set; }

    /// <summary>Yanıtın ilişkili olduğu anketin kimliği.</summary>
    public int AnketId { get; set; }

    /// <summary>Yanıtın ilişkili olduğu hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>Yanıtın ilişkili olduğu birimin kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }

    /// <summary>Yanıtın ilişkili olduğu doktorun kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }

    /// <summary>Anketin doldurulmaya başlandığı tarih.</summary>
    public DateTime BaslamaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Anketin tamamlandığı tarih (isteğe bağlı).</summary>
    public DateTime? TamamlanmaTarihi { get; set; }

    /// <summary>Yanıtın gönderildiği IP adresi.</summary>
    public string? IpAdresi { get; set; }

    /// <summary>Yanıtın gönderildiği tarayıcı/istemci bilgisi (User-Agent).</summary>
    public string? KullaniciAjan { get; set; }

    /// <summary>Yanıtın geçerli sayılıp sayılmadığını belirtir.</summary>
    public bool GecerliMi { get; set; } = true;

    /// <summary>Yanıtın ilişkili olduğu davet.</summary>
    public AnketDaveti? Davet { get; set; }

    /// <summary>Yanıta ait tekil cevaplar.</summary>
    public ICollection<AnketCevabi> Cevaplar { get; set; } = new List<AnketCevabi>();
}
