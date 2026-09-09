using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages;

/// <summary>
/// Uygulama genelinde hata sayfası. Anonim kullanıcılar da görebilir.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
[AllowAnonymous]
public class ErrorModel : PageModel
{
    /// <summary>İstek kimliği.</summary>
    public string? RequestId { get; set; }

    /// <summary>İstek kimliğinin gösterilip gösterilmeyeceğini belirler.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    /// <summary>Yeni bir <see cref="ErrorModel"/> örneği oluşturur.</summary>
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    /// <summary>Hata sayfasını görüntüler.</summary>
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
