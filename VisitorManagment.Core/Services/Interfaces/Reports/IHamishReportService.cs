using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;

namespace VisitorManagment.Core.Services.Interfaces.Reports
{
    public interface IHamishReportService
    {

        /// <summary>
        /// گرفتن تعداد اقدام های انجام شده فرمانده براساس نوع اقدام
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<HameshActionTypeModel> GetTotalHelpAmountForServiceType(int userId, DateTime? startDateEnglish, DateTime? endDateEnglish);
        /// <summary>
        /// گزارش فرماندهان براساس نوع اقدام انجام شده
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        ChartFarmandehActivityDto ReportFarmandehByActionCode(string personCode ,DateTime? startDateEnglish ,DateTime? endDateEnglish);
        /// <summary>
        /// گزارش فرماندهان براساس مشکلات
        /// </summary>
        /// <param name="prsnCd"></param>
        /// <returns></returns>
        ChartProblemOmdOrgan ReportProblemFarmandehInfo(string prsnCd, DateTime? startDateEnglish, DateTime? endDateEnglish);

    }
}
