using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Application.Services;
using HastaMemnuniyet.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HastaMemnuniyet.Application;

/// <summary>
/// Application katmanı servislerinin bağımlılık enjeksiyonu kayıtlarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Application katmanı use-case servislerini servis koleksiyonuna kaydeder.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <returns>Zincirleme için servis koleksiyonu.</returns>
    public static IServiceCollection ApplicationKatmaniniEkle(this IServiceCollection services)
    {
        services.AddScoped<IHastaneServisi, HastaneServisi>();
        services.AddScoped<IBirimServisi, BirimServisi>();
        services.AddScoped<IDoktorServisi, DoktorServisi>();
        services.AddScoped<IAnketServisi, AnketServisi>();
        services.AddScoped<IDavetServisi, DavetServisi>();
        services.AddScoped<IAnketDoldurmaServisi, AnketDoldurmaServisi>();
        services.AddScoped<IDashboardServisi, DashboardServisi>();
        services.AddScoped<IKullaniciServisi, KullaniciServisi>();
        services.AddScoped<IYanitServisi, YanitServisi>();
        services.AddScoped<IKullaniciKapsamServisi, KullaniciKapsamServisi>();
        services.AddScoped<IKritikGeriBildirimMotoru, KritikGeriBildirimMotoru>();
        services.AddScoped<IKritikGeriBildirimServisi, KritikGeriBildirimServisi>();
        services.AddScoped<IAksiyonServisi, AksiyonServisi>();
        services.AddScoped<IQrKampanyaServisi, QrKampanyaServisi>();
        services.AddScoped<IMetinAnaliziServisi, MetinAnaliziServisi>();
        services.AddScoped<IDoktorKarnesiServisi, DoktorKarnesiServisi>();
        return services;
    }
}
