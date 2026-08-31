using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{

    #region اطلاعات پرسنلی
    public class ApiResultPersonalInfoDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public FactPersonalViewModel Data { get; set; }
    }
    #endregion

    #region ارگان

    public class ApiResultOrganIdWithTitleDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<OrganViewModelDto> Data { get; set; } = new List<OrganViewModelDto>();
    }
    public class ApiResultOrganDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<OrganViewModelDto> Data { get; set; } = new List<OrganViewModelDto>();
    }

    public class ApiResultOrganInfoDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OrganViewModel Data { get; set; }
    }

    public class AllOrganViewModelDto
    {
        public int Id { get; set; }
        public string Title { get; set; }


    }

    public class ApiResultAllOrganDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AllOrganViewModelDto> Data { get; set; } = new List<AllOrganViewModelDto>();
    }
    #endregion


    #region تشویقات

    public class ApiResultTashvighatDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashvighatInfoViewModel> Data { get; set; }
    }
    #endregion

    #region تنبیهات

    public class ApiResultTanbihatDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TanbihatInfoViewModel> Data { get; set; }
    }
    #endregion

    #region انتقالات

    public class ApiResultEnteghalDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EnteghaInfolViewModel> Data { get; set; }
    }
    #endregion

    #region عائله

    public class ApiResultPersonFamilyDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PersonalFamilyViewModel> Data { get; set; }
    }
    #endregion


    #region مسکن

    public class ApiResulMaskanDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashilatMaskanInfoViewModel> Data { get; set; }
    }
    #endregion

    #region دبیرخانه

    public class ApiResulDabirKhanehDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashilatDabirkhanehInfoViewModel> Data { get; set; }
    }
    #endregion

    #region دستور

    public class ApiResulDastorDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashilatDastorInfoViewModel> Data { get; set; }
    }
    #endregion

    #region دیگر other

    public class ApiResultOtherDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashilatOtherInfoViewModel> Data { get; set; }
    }
    #endregion

    #region تسهیلات بلاعوض 

    public class ApiResultBelaavazDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TashilatFacilityBAViewModel> Data { get; set; }
    }
    #endregion

    #region نمره ارزیابی

    public class ApiResulExamDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ExamInfoViewModel> Data { get; set; }
    }
    #endregion

    #region فیش حقوقی
    public class ApiResultFishDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FishViewModel> Data { get; set; }
    }
    #endregion

    #region  معسرین
    public class ApiResultMoeeserDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MoeeserViewModel> Data { get; set; }
    }
    #endregion





}
