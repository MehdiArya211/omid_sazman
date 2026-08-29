using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs.Ranking;
using VisitorManagment.Core.Services.Interfaces.Ranking;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    public class RegRankingIndexModel : PageModel
    {
        private readonly IRankingService _rankingService;
        public RegRankingIndexModel(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        public ListPointViewModel model { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet()
        {
            model = _rankingService.GetAll();
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost()
        {

            _rankingService.FinalCalculate();
            return Redirect("/Admin/RegRanking/Index");
        }
        #endregion
    }
}
