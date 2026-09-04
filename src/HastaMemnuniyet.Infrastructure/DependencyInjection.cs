using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using HastaMemnuniyet.Infrastructure.Repositories;
using HastaMemnuniyet.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HastaMemnuniyet.Infrastructure;

/// <summary>
/// Infrastructure katmanı bileşenlerinin (DbContext, Identity, repository ve servisler)
/// bağımlılık enjeksiyonu kayıtlarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure katmanı bileşenlerini servis koleksiyonuna kaydeder.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Uygulama yapılandırması (connection string için).</param>
    /// <returns>Zincirleme için servis koleksiyonu.</returns>
    public static IServiceCollection InfrastructureKatmaniniEkle(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var baglantiDizesi = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("'DefaultConnection' bağlantı dizesi bulunamadı.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(baglantiDizesi, npgsql =>
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddIdentity<UygulamaKullanicisi, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Genel ve özel repository kayıtları
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IHastaneRepository, HastaneRepository>();
        services.AddScoped<IBirimRepository, BirimRepository>();
        services.AddScoped<IDoktorRepository, DoktorRepository>();
        services.AddScoped<IAnketRepository, AnketRepository>();
        services.AddScoped<IAnketDavetiRepository, AnketDavetiRepository>();
        services.AddScoped<IAnketYanitiRepository, AnketYanitiRepository>();
        services.AddScoped<ISistemAyariRepository, SistemAyariRepository>();

        // Faz 4 özel repository kayıtları
        services.AddScoped<IKritikGeriBildirimRepository, KritikGeriBildirimRepository>();
        services.AddScoped<IIyilestirmeAksiyonuRepository, IyilestirmeAksiyonuRepository>();

        // Altyapı servisleri
        services.AddScoped<ISmsSender, FakeSmsGonderici>();
        services.AddSingleton<ITokenUretici, GuidTokenUretici>();

        // Faz 4 altyapı servisleri
        services.AddHttpContextAccessor();
        services.AddScoped<IQrKodUretici, QrKodUreticiServisi>();
        services.AddScoped<IAuditLogger, AuditLogServisi>();
        services.AddScoped<IDisaAktarmaServisi, ExcelDisaAktarmaServisi>();
        services.AddScoped<IDenetimServisi, DenetimServisi>();

        // Faz 5 – KVKK veri saklama politikası servisi
        services.AddScoped<IVeriSaklamaPolitikasiServisi, VeriSaklamaPolitikasiServisi>();

        return services;
    }
}
