using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Anket;

/// <summary>
/// Geçersiz veya süresi dolmuş anket bağlantıları için gösterilen sayfanın PageModel'i.
/// </summary>
[AllowAnonymous]
public class SuresiDolmusModel : PageModel
{
    /// <summary>Sayfayı görüntüler.</summary>
    public void OnGet()
    {
    }
}
