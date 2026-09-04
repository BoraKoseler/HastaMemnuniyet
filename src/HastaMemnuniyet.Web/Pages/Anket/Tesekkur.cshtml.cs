using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HastaMemnuniyet.Web.Pages.Anket;

/// <summary>
/// Anket başarıyla tamamlandıktan sonra gösterilen teşekkür sayfasının PageModel'i.
/// </summary>
[AllowAnonymous]
public class TesekkurModel : PageModel
{
    /// <summary>Sayfayı görüntüler.</summary>
    public void OnGet()
    {
    }
}
