using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
   public interface IWebApiService
    {

        #region Personal Info
        /// <summary>
        /// گرفتن اطلاعات پرسنلی با کد پرسنلی
        /// </summary>
        /// <param name="personalCode">کد پرسنلی</param>
        /// <returns></returns>
        ApiResultPersonalInfoDto GetPersonalByPersonalNo(string personalCode);
        #endregion

        #region Organ
        ApiResultOrganInfoDto GetGhararghahByOmdOrgCode(int UnitCode);
        ApiResultOrganInfoDto GetOmdOrgan(int UnitCode);
        ApiResultOrganDto GetGharargah();
        ApiResultOrganDto GetOrganByGharargahId(int gharargahId);
        /// <summary>
        /// تمام یگان های نیروی زمینی
        /// </summary>
        /// <returns></returns>
        ApiResultOrganIdWithTitleDto GetAllOrgan();
        /// <summary>
        /// تمام یگان های نیروی زمینی
        /// </summary>
        /// <returns></returns>
        ApiResultAllOrganDto GetAllOrgan1();
        ApiResultAllOrganDto GetAllOrganNezaja();

        /// <summary>
        /// تمام یگان های نزاجا به همراه قرارگاه ها و اراشد نظامی
        /// </summary>
        /// <returns></returns>
        ApiResultAllOrganDto GetAllOrganWithGharagahNezaja();
        #endregion

        #region Tashvighat
        ApiResultTashvighatDto GetTashvighatByPersonalNo(string personalCode);

        #endregion

        #region Tanbihat
        ApiResultTanbihatDto GetTanbihatByPersonalNo(string personalCode);
        #endregion

        #region Enteghalat
        ApiResultEnteghalDto GetEnteghalByPersonNo(string personalCode);
        #endregion

        #region Person Family
        // Task<List<PersonFamilyInfoViewModel>> GetPFamilyInfo(string personalCode);
        ApiResultPersonFamilyDto GetPFamilyInfo(string personalCode);
        #endregion

        #region TashilateMaskan
        ApiResulMaskanDto GetPTashilatMaskan(string personalCode);
        #endregion

        #region TashilatDabirKhane
        ApiResulDabirKhanehDto GetPTashilatDabirkhaneh(string personalCode);
        #endregion

        #region Fajr
        ItoLogInfoViewModel GetLogForLogInfo();

        int AddLogInfo();

        int AddLog(string userId, string userName, string url);
        #endregion

        #region TashilatDastor
        ApiResulDastorDto GetTashilatDastor(string personalCode);
        //for stimul
        List<TashilatDastorInfoViewModel> GetTashilatDastorForStimul(string personalCode);
        #endregion

        #region TashilatOther
        ApiResultOtherDto GetTashilatOther(string personalCode);
        #endregion

        #region تسهیلات بلاعوض
        ApiResultBelaavazDto GetTashilatBelaavaz(string personalCode);
        #endregion

        #region Nomarat Arzyabi
        ApiResulExamDto GetExam(string personalCode);
        #endregion

        #region Get Token
        string GetToken();
        #endregion

        #region GetCount Tashvighat Tanbihat Nahast Farar job-location
        int? GetCountTashvighat(string prnsNo);
        int? GetCountTanbihat(string prnsNo);
        int? GetCountFarar(string prnsNo);
        int? GetCountNahast(string prnsNo);
        //string GetJobLocation(string prnsNo);
        ApiResultEnteghalDto GetJobLocation(string prnsNo);
        string GetStatusMarrid(string prnsNo);
        string GetEmployDate(string prnsNo);
        #endregion

        #region فیش حقوقی
         ApiResultFishDto GetFishByPrsnCode(string prsnCd);
        #endregion

        #region معسرین
        ApiResultMoeeserDto GetMoeeserByPrsnCode(string prsnCd);
        #endregion
    }
}
