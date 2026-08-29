using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.ReportsAdmin;

namespace VisitorManagment.Core.Services.Interfaces.Reports
{
  public  interface IRequestGhaReportService
    {
        #region گزارش عملکرد قرارگاه و یگان
        List<HameshRequestGhaModel> GetTotalHelpAmountForServiceType(int GharargahId, int RoleId, DateTime? startDateEnglish, DateTime? endDateEnglish);
        #endregion

        /// <summary>
        /// نمایش درخواست  قرارگاه بصورت لیستی
        /// </summary>
        /// <param name="GharargahId"></param>
        /// <param name="RoleId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        List<HameshRequestGhaModel> GetListTotalHelpAmountForServiceType(int GharargahId, int RoleId, DateTime? startDateEnglish, DateTime? endDateEnglish);

        #region سرویس فراوانی مشکلات
        List<ProblemAllNez> GetPeroblemAllNez();
        List<ProblemGhaReport> GetPeroblemGhaNez();
        List<ProblemOmdOrganReport> GetPeroblemOmdOrganNez(int OmdOrganId, int YeganId);
        ChartProblemOmdOrgan GetProblemCountOmdOrganInfo(int OmdOrganId, int YeganId);
        #endregion


        /// <summary>
        /// تعداد درخواست های ملاقات های قرارگاه براساس کد های قرارگاه ، نوع اقدام و تاریخ
        /// </summary>
        /// <param name="codeGha"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        List<HameshActionTypeModel> GetNomrehArzyabiGharargah(int actionTypeId ,DateTime? startDateEnglish, DateTime? endDateEnglish);

        /// <summary>
        /// گزارش نموداری رتبه بندی یگان های قرارگاه در صفحه اول سایت
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        List<HameshActionTypeModel> GetArzyabiAllYeganForGharargah(int actionTypeId , int unitCode , int codeGha , int roleTypeId);

        /// <summary>
        ///   رتبه  یگان در صفحه اول سایت
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        int GradYegan(int actionTypeId , int unitCode , int codeGha , int roleTypeId);

    }
}
