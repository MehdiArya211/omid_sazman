using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisitorManagment.Web.Pages.OnlineConversation
{
    [Authorize]
    public class TestServerModel : PageModel
    {
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {
        }
    }
}
