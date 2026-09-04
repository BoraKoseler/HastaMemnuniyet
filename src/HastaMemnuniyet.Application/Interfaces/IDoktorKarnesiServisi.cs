using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Doktor performans karnesi üreten servis arayüzü.</summary>
public interface IDoktorKarnesiServisi
{
    /// <summary>
    /// Verilen doktor ve tarih aralığı için performans karnesi üretir.
    /// Soru bazlı ortalamalar, genel ortalama, NPS, güçlü/zayıf alanlar ve önceki döneme göre trend içerir.
    /// </summary>
    /// <param name="doktorId">Karnesi üretilecek doktorun kimliği.</param>
    /// <param name="baslangic">Başlangıç tarihi (null ise varsayılan aralık kullanılır).</param>
    /// <param name="bitis">Bitiş tarihi (null ise bugün kullanılır).</param>
    /// <returns>Doktor karnesi ya da doktor bulunamazsa null.</returns>
    Task<DoktorKarnesiDto?> KarneGetirAsync(int doktorId, DateTime? baslangic = null, DateTime? bitis = null);
}
