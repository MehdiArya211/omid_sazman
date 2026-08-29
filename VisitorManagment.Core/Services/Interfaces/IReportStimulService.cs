using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IReportStimulService
    {
        public ReportTestInfoViewModel GetReportFullPersonal(int fileId);
        public ReportTestInfoViewModelV2 GetReportFullPersonalV2(int fileId);
      //  public string GetHameshByRoleTypeFinal(int fileId, int roleTypeFinalId, int roleTypeId);

    }
}
