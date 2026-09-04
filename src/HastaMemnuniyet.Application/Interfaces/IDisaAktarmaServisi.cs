using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Verilerin Excel biçiminde dışa aktarılması iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IDisaAktarmaServisi
{
    /// <summary>Anket yanıtlarını Excel (.xlsx) çalışma kitabına aktarır.</summary>
    /// <param name="yanitlar">Dışa aktarılacak yanıtlar.</param>
    /// <returns>Excel dosyasının bayt dizisi.</returns>
    byte[] YanitlariDisaAktar(IReadOnlyList<AnketYanitiDto> yanitlar);

    /// <summary>Dashboard rapor istatistiklerini Excel (.xlsx) çalışma kitabına aktarır.</summary>
    /// <param name="rapor">Dışa aktarılacak dashboard istatistikleri.</param>
    /// <returns>Excel dosyasının bayt dizisi.</returns>
    byte[] RaporuDisaAktar(DashboardDto rapor);

    /// <summary>Bir doktorun performans karnesini Excel (.xlsx) çalışma kitabına aktarır.</summary>
    /// <param name="karne">Dışa aktarılacak doktor karnesi.</param>
    /// <returns>Excel dosyasının bayt dizisi.</returns>
    byte[] DoktorKarnesiDisaAktar(DoktorKarnesiDto karne);
}
