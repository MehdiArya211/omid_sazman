using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisitorManagment.Web.Pages.OnlineConversation
{
    [Authorize]
    public class TestServerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
