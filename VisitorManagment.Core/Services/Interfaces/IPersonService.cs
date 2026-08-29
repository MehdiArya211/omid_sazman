using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IPersonService
    {
        Personal GetPersonByPersonalId(int personsalId);
        FactSpecificationPersonalViewModel GetPersonalForEdit(int personalId);
        Personal GetPersonalByPersonalCode(string personalCode);

        //**********************************************************************
        FactSpecificationPersonalViewModel GetPersonalByPersonalNo(int fileId);
        ListFileViewModel GetFile(int FileId);
        List<TashvighatInfoViewModel> GetTashvighatByPersonalNo(int fileId);
        List<TanbihatInfoViewModel> GetTanbihatByPersonalNo(int fileId);

        List<FarmandehInfo> GetFarmandehInfos();
        List<FarmandehInfo> GetKarshenashAnsarInfos();
        List<FarmandehInfo> GetFarmandehAndKarshenasInfos();

        string GetAvatarUserByPrsnCd(int prsnCd);

        FarmandehReportDTO GetPrsnInfoByPrsnId(string personalCode);
    }
}

