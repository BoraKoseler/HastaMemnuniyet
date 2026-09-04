using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Hastaya gönderilen ve ankete erişimi sağlayan davet varlığı.
/// </summary>
public class AnketDaveti : BaseEntity
{
    /// <summary>Davete konu anketin kimliği.</summary>
    public int AnketId { get; set; }

    /// <summary>Davetin ilişkili olduğu hastanenin kimliği.</summary>
    public int HastaneId { get; set; }

    /// <summary>Davetin ilişkili olduğu birimin kimliği (isteğe bağlı).</summary>
    public int? BirimId { get; set; }

    /// <summary>Davetin ilişkili olduğu doktorun kimliği (isteğe bağlı).</summary>
    public int? DoktorId { get; set; }

    /// <summary>Ankete erişim için kullanılan benzersiz, tahmin edilemez token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Hastanın telefon numarasının hash değeri.</summary>
    public string? TelefonHash { get; set; }

    /// <summary>Davetin gönderim kanalı.</summary>
    public GonderimKanali GonderimKanali { get; set; }

    /// <summary>Davetin güncel durumu.</summary>
    public DavetDurumu Durum { get; set; } = DavetDurumu.Olusturuldu;

    /// <summary>Hizmetin/ziyaretin gerçekleştiği tarih (isteğe bağlı).</summary>
    public DateTime? HizmetTarihi { get; set; }

    /// <summary>Davetin son geçerlilik tarihi.</summary>
    public DateTime SonGecerlilikTarihi { get; set; }

    /// <summary>Daveti oluşturan kullanıcının kimliği.</summary>
    public string? OlusturanKullaniciId { get; set; }

    /// <summary>Bu davet için gönderilen hatırlatma sayısı.</summary>
    public int HatirlatmaSayisi { get; set; }

    /// <summary>Davete konu anket.</summary>
    public Anket? Anket { get; set; }

    /// <summary>Davetin ilişkili olduğu hastane.</summary>
    public Hastane? Hastane { get; set; }

    /// <summary>Davetin ilişkili olduğu birim.</summary>
    public Birim? Birim { get; set; }

    /// <summary>Davetin ilişkili olduğu doktor.</summary>
    public Doktor? Doktor { get; set; }

    /// <summary>Davete verilen anket yanıtları.</summary>
    public ICollection<AnketYaniti> Yanitlar { get; set; } = new List<AnketYaniti>();

    /// <summary>Davete ait SMS gönderim kayıtları.</summary>
    public ICollection<SmsGonderimKaydi> SmsGonderimKayitlari { get; set; } = new List<SmsGonderimKaydi>();
}
