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
        public void OnGet()
        {
            model = _rankingService.GetAll();
        }

        public IActionResult OnPost()
        {

            _rankingService.FinalCalculate();
            return Redirect("/Admin/RegRanking/Index");
        }
    }
}
