using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.ReportsAdmin;

namespace VisitorManagment.Core.Services.Interfaces.Reports
{
    public interface IProblemNezReportService
    {
        List<ProblemAllNez> GetPeroblemAllNez();
        List<ProblemGhaReport> GetPeroblemGhaNez();
        List<ProblemOmdOrganReport> GetPeroblemOmdOrganNez();
        ChartProblemOmdOrgan GetProblemCountOmdOrganInfo(int OmdOrganId);
        /// <summary>
        /// گزارش  فراوانی تعداد مشکلات براساس قرارگاه و یگان های عمده
        /// </summary>
        /// <param name="GhaCd"></param>
        /// <param name="OmdCd"></param>
        /// <returns></returns>
        ChartProblemOmdOrgan GetProblemCountGhCd_OmdCd(int GhaCd, int OmdCd, DateTime? startDateEnglish, DateTime? endDateEnglish);

        List<ProblemOmdOrganReport> GetProblemOmdOrgByGhCd_OmdCd(int GhaCd, int OmdCd, DateTime? startDateEnglish, DateTime? endDateEnglish);


    }
}
