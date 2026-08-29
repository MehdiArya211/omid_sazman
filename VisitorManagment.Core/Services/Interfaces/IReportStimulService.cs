using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IReportStimulService
    {
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>

        public ReportTestInfoViewModel GetReportFullPersonal(int fileId);
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ReportTestInfoViewModelV2 GetReportFullPersonalV2(int fileId);
      //  public string GetHameshByRoleTypeFinal(int fileId, int roleTypeFinalId, int roleTypeId);

        #endregion
    }
}
