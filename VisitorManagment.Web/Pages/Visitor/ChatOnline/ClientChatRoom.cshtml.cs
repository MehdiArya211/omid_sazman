using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stimulsoft.System.Windows.Forms;

namespace VisitorManagment.Web.Pages.Visitor.ChatOnline
{
    public class ClientChatRoomModel : PageModel
    {
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {
            var user = User.FindFirst("FullName").Value;
            ViewData["FullName"]=user;


        }
    }
}
