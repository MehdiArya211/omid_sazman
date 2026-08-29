
using IdentityModel.Client;
using ITOWebApiClient;
using ITOWebApiClient.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Fajr;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Core.Services
{
    public class WebApiService : IWebApiService
    {
        private string apiUrl = string.Empty;
        private string apiOrganUrl = string.Empty;
        private string apiTashvighatUrl = string.Empty;
        private string apiTanbihatUrl = string.Empty;
        private string apiEntehgalatUrl = string.Empty;
        private string apitashilateMaskanUrl = string.Empty;
        private string apiTashilatDabirKhaneUrl = string.Empty;
        private string apiPersonFamilyUrl = string.Empty;
        private string apiFajrUrl = string.Empty;
        private string apiDastorUrl = string.Empty;
        private string apiTashilatDastorUrl = string.Empty;
        private string apiTashilatOtherUrl = string.Empty;
        private string apiExamUrl = string.Empty;
        private string apiFishUrl = string.Empty;
        private string apiMoeeserUrl = string.Empty;

        private readonly IConfiguration _configuration;
        private readonly VisitorManagmentContext _context;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly HttpClient _client;

        public WebApiService(
            VisitorManagmentContext context,
            IConfiguration configuration,
            ApiTokenCacheClient apiTokenClient)
        {
            _context = context;
            _configuration = configuration;
            _apiTokenClient = apiTokenClient;

            //برای سرویس پرسنل
            apiUrl = CustomSettings.Instance.ApiPersonelUrl;

            //برای سرویس ارگان
            apiOrganUrl = CustomSettings.Instance.ApiOrganUrl;
            apiTashvighatUrl = CustomSettings.Instance.ApiTashvighatUrl;
            apiTanbihatUrl = CustomSettings.Instance.ApiTanbihat;
            apiEntehgalatUrl = CustomSettings.Instance.ApiEnteghalat;
            apitashilateMaskanUrl = CustomSettings.Instance.ApiTashilatMaskan;
            apiTashilatDabirKhaneUrl = CustomSettings.Instance.ApiTashilatDabirKhaneh;
            apiPersonFamilyUrl = CustomSettings.Instance.ApiPersonFamily;
            apiFajrUrl = CustomSettings.Instance.ApiFajr;
            apiDastorUrl = CustomSettings.Instance.ApiTashilatDastor;
            apiTashilatOtherUrl = CustomSettings.Instance.ApiTashilatOther;
            apiExamUrl = CustomSettings.Instance.ApiExam;
            apiFishUrl = CustomSettings.Instance.ApiFish;
            apiMoeeserUrl = CustomSettings.Instance.ApiMoeeser;

            _client = new HttpClient();
        }

        #region Api Token

        /// <summary>
        /// دریافت توکن API
        /// </summary>
        /// <returns>Access Token</returns>
        private string GetApiToken()
        {
            return _apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).Result;
        }

        /// <summary>
        /// ست کردن توکن روی هدر درخواست‌ها
        /// </summary>
        private void SetBearerToken()
        {
            var accessToken = GetApiToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// دریافت توکن برای استفاده بیرون از سرویس، در صورت نیاز
        /// </summary>
        public string GetToken()
        {
            return GetApiToken();
        }

        #endregion

        #region Personal Info

        /// <summary>
        /// گرفتن اطلاعات پرسنلی با کد پرسنلی
        /// </summary>
        /// <param name="personalCode">کد پرسنلی</param>
        /// <returns></returns>
        public ApiResultPersonalInfoDto GetPersonalByPersonalNo1(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiUrl + "/GetByPrsnCdOrNCd/" + personalCode).Result;

            var person = JsonConvert.DeserializeObject<ApiResultPersonalInfoDto>(result);

            person.Data.TotAml = person.Data.TOT_AML;
            person.Data.TotAml2 = person.Data.TOT_AML2;
            person.Data.DrsadJa = person.Data.DRSAD_JA;
            person.Data.DrsadJb = person.Data.DRSAD_JB;
            person.Data.Respond = "Ok";

            return person;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultPersonalInfoDto GetPersonalByPersonalNo0(string personalCode)
        {
            if (string.IsNullOrEmpty(personalCode) || !Regex.IsMatch(personalCode, @"^\d{1,9}$"))
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "کد پرسنلی نامعتبر است."
                };
            }

            SetBearerToken();

            var result = _client.GetStringAsync(apiUrl + "/GetByPrsnCdOrNCd/" + personalCode).Result;

            var person = JsonConvert.DeserializeObject<ApiResultPersonalInfoDto>(result);

            if (person == null || person.Data == null)
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "پرسنلی با این مشخصات یافت نشد."
                };
            }

            person.Data.TotAml = person.Data.TOT_AML;
            person.Data.TotAml2 = person.Data.TOT_AML2;
            person.Data.DrsadJa = person.Data.DRSAD_JA;
            person.Data.DrsadJb = person.Data.DRSAD_JB;
            person.Data.Respond = "Ok";

            return person;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultPersonalInfoDto GetPersonalByPersonalNo2(string personalCode)
        {
            if (string.IsNullOrWhiteSpace(personalCode) || !Regex.IsMatch(personalCode.Trim(), @"^\d{1,9}$"))
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "کد پرسنلی نامعتبر است."
                };
            }

            personalCode = personalCode.Trim();

            try
            {
                SetBearerToken();

                var result = _client.GetStringAsync(apiUrl + "/GetByPrsnCdOrNCd/" + personalCode).Result;

                var person = JsonConvert.DeserializeObject<ApiResultPersonalInfoDto>(result);

                if (person == null || person.Data == null)
                {
                    return new ApiResultPersonalInfoDto
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "پرسنلی با این مشخصات یافت نشد."
                    };
                }

                person.IsSuccess = true;
                person.StatusCode = 200;
                person.Message = "Ok";

                person.Data.TotAml = person.Data.TOT_AML;
                person.Data.TotAml2 = person.Data.TOT_AML2;
                person.Data.DrsadJa = person.Data.DRSAD_JA;
                person.Data.DrsadJb = person.Data.DRSAD_JB;
                person.Data.Respond = "Ok";

                return person;
            }
            catch
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "سرویس با خطا مواجه شد. دقایقی دیگر مجددا امتحان کنید."
                };
            }
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultPersonalInfoDto GetPersonalByPersonalNo(string personalCode)
        {
            personalCode = ConvertDigitsToEnglish(personalCode);

            if (string.IsNullOrWhiteSpace(personalCode) || !Regex.IsMatch(personalCode, @"^\d{1,9}$"))
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "کد پرسنلی نامعتبر است."
                };
            }

            try
            {
                SetBearerToken();

                var result = _client.GetStringAsync(apiUrl + "/GetByPrsnCdOrNCd/" + personalCode).Result;

                var person = JsonConvert.DeserializeObject<ApiResultPersonalInfoDto>(result);

                if (person == null || person.Data == null)
                {
                    return new ApiResultPersonalInfoDto
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "پرسنلی با این مشخصات یافت نشد."
                    };
                }

                person.IsSuccess = true;
                person.StatusCode = 200;
                person.Message = "Ok";

                person.Data.TotAml = person.Data.TOT_AML;
                person.Data.TotAml2 = person.Data.TOT_AML2;
                person.Data.DrsadJa = person.Data.DRSAD_JA;
                person.Data.DrsadJb = person.Data.DRSAD_JB;
                person.Data.Respond = "Ok";

                return person;
            }
            catch
            {
                return new ApiResultPersonalInfoDto
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "سرویس با خطا مواجه شد. دقایقی دیگر مجددا امتحان کنید."
                };
            }
        }

        /// <summary>
        /// تبدیل اعداد فارسی و عربی به انگلیسی
        /// </summary>
        /// <param name="input">متن ورودی</param>
        /// <returns>متن با اعداد انگلیسی</returns>
        private string ConvertDigitsToEnglish(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return input.Trim()
                .Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")

                // اعداد عربی
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

        #endregion

        #region Organ

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOrganInfoDto GetGhararghahByOmdOrgCode(int UnitCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetGararghahByOmdOrgCode/" + UnitCode).Result;

            var gharargah = JsonConvert.DeserializeObject<ApiResultOrganInfoDto>(result);

            return gharargah;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOrganInfoDto GetOmdOrgan(int UnitCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetById/" + UnitCode).Result;

            var gharargah = JsonConvert.DeserializeObject<ApiResultOrganInfoDto>(result);

            return gharargah;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOrganDto GetGharargah()
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetGararghah/").Result;

            var gharargah = JsonConvert.DeserializeObject<ApiResultOrganDto>(result);

            return gharargah;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOrganDto GetOrganByGharargahId(int gharargahId)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetOmdOrgansByGhId/" + gharargahId).Result;

            var unitCode = JsonConvert.DeserializeObject<ApiResultOrganDto>(result);

            return unitCode;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOrganIdWithTitleDto GetAllOrgan()
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetTitleOrgan/").Result;

            var unit = JsonConvert.DeserializeObject<ApiResultOrganIdWithTitleDto>(result);

            return unit;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultAllOrganDto GetAllOrgan1()
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetOmdOrgan/").Result;

            var unit = JsonConvert.DeserializeObject<ApiResultAllOrganDto>(result);

            return unit;
        }

        /// <summary>
        /// تمام یگان های نزاجا به همراه قرارگاه ها و اراشد نظامی
        /// </summary>
        /// <returns></returns>
        public ApiResultAllOrganDto GetAllOrganWithGharagahNezaja()
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetOmdOrgan/").Result;

            var result1 = JsonConvert.DeserializeObject<ApiResultAllOrganDto>(result);

            return result1;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultAllOrganDto GetAllOrganNezaja()
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiOrganUrl + "/GetAll/").Result;

            var unit = JsonConvert.DeserializeObject<ApiResultAllOrganDto>(result);

            return unit;
        }

        #endregion

        #region Personal

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public FactSpecificationPersonalViewModel GetPersonalByPersonalId(int personalId)
        {
            var personalCode = _context.Personals
                .Where(p => p.Id == personalId)
                .Select(p => p.PersonalCode)
                .SingleOrDefault();

            SetBearerToken();

            var result = _client.GetStringAsync(apiUrl + "/GetPersonalByPersonalCode/" + personalCode).Result;

            var person = JsonConvert.DeserializeObject<FactSpecificationPersonalViewModel>(result);

            return person;
        }

        #endregion

        #region Tashvighat

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultTashvighatDto GetTashvighatByPersonalNo(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTashvighatUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashvighat = JsonConvert.DeserializeObject<ApiResultTashvighatDto>(result);

            return tashvighat;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetCountTashvighat(string prnsNo)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTashvighatUrl + "/GetByPrsnCd/" + prnsNo).Result;

            var tashvighat = JsonConvert.DeserializeObject<ApiResultTashvighatDto>(result);

            if (tashvighat.Data == null)
            {
                return 0;
            }

            return tashvighat.Data.Count();
        }

        #endregion

        #region Tanbihat

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultTanbihatDto GetTanbihatByPersonalNo(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTanbihatUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tanbihat = JsonConvert.DeserializeObject<ApiResultTanbihatDto>(result);

            return tanbihat;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetCountTanbihat(string prnsNo)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTanbihatUrl + "/GetByPrsnCd/" + prnsNo).Result;

            var tanbihat = JsonConvert.DeserializeObject<ApiResultTanbihatDto>(result);

            if (tanbihat.Data == null)
            {
                return 0;
            }

            return tanbihat.Data.Count;
        }

        #endregion

        #region Enteghalat

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultEnteghalDto GetEnteghalByPersonNo(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiEntehgalatUrl + "/GetByPrsnCd/" + personalCode).Result;

            var enteghalat = JsonConvert.DeserializeObject<ApiResultEnteghalDto>(result);

            return enteghalat;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResulMaskanDto GetPTashilatMaskan(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apitashilateMaskanUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashilatMaskan = JsonConvert.DeserializeObject<ApiResulMaskanDto>(result);

            return tashilatMaskan;
        }

        /// <summary>
        /// انتقالات / محل های خدمتی
        /// </summary>
        public ApiResultEnteghalDto GetJobLocation(string prnsNo)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiEntehgalatUrl + "/GetByPrsnCd/" + prnsNo).Result;

            var enteghalat = JsonConvert.DeserializeObject<ApiResultEnteghalDto>(result);

            return enteghalat;
        }

        #endregion

        #region Tashilat Dabirkhane

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResulDabirKhanehDto GetPTashilatDabirkhaneh(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTashilatDabirKhaneUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashilatDabirKhane = JsonConvert.DeserializeObject<ApiResulDabirKhanehDto>(result);

            return tashilatDabirKhane;
        }

        #endregion

        #region Person Family

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultPersonFamilyDto GetPFamilyInfo(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiPersonFamilyUrl + "/GetByPrsnCd/" + personalCode).Result;

            var pFamily = JsonConvert.DeserializeObject<ApiResultPersonFamilyDto>(result);

            return pFamily;
        }

        #endregion

        #region Fajr Log

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ItoLogInfoViewModel GetLogForLogInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int AddLogInfo()
        {
            SetBearerToken();

            var itoLogModel = new ItoLogInfoViewModel
            {
                Time = DateTime.Now,
                SoftwareVersion = "1.0.0",
                SoftwareId = "1",
                Url = "itoService.nez.net",
                SoftwareName = "service",
                ServerIp = "10.128.155.213",
                PortNumber = "8080",
                ServerHostname = "10.128.151.151",
                ClientHostname = "10.128.155.218",
                ClientIp = "10.125.125.12",
                PageTitle = "fajrTest",
                UserUniqueId = "95005555",
                Username = "M.Reveshtian",
                Sensitivity = "up",
                Importance = "low",
                ActionType = "hi",
                SubType = "NO",
                SubTypeDescription = "{hi,testWebServiceFajr}",
                Flag = "iran"
            };

            string jsonFajrInfo = JsonConvert.SerializeObject(itoLogModel);

            StringContent content = new StringContent(jsonFajrInfo, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(apiFajrUrl, content).Result;

            if (res.IsSuccessStatusCode)
            {
                return (int)res.StatusCode;
            }

            return 0;
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int AddLog(string userId, string userName, string url)
        {
            SetBearerToken();

            string clientHostName = Dns.GetHostName();

            var itoLogModel = new ItoFajrDTO
            {
                Time = DateTime.Now,
                SoftwareVersion = "1.0.0",
                SoftwareId = "19",
                Url = "itovisitor.nez.net/" + url,
                SoftwareName = "visitor",
                ServerIp = "10.128.155.213",
                PortNumber = "80",
                ServerHostname = "ItoWeb",
                ClientHostname = clientHostName,
                ClientIp = Dns.GetHostEntry(clientHostName).AddressList[1].ToString(),
                PageTitle = "visitor",
                UserUniqueId = userId,
                Username = userName,
                Sensitivity = "up",
                Importance = "high",
                ActionType = "post",
                SubType = " ",
                SubTypeDescription = " ",
                Flag = "nez.net"
            };

            string jsonFajrInfo = JsonConvert.SerializeObject(itoLogModel);

            StringContent content = new StringContent(jsonFajrInfo, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(apiFajrUrl, content).Result;

            if (res.IsSuccessStatusCode)
            {
                return (int)res.StatusCode;
            }

            return 0;
        }

        #endregion

        #region Dastoor

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResulDastorDto GetTashilatDastor(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiDastorUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashilatDastor = JsonConvert.DeserializeObject<ApiResulDastorDto>(result);

            return tashilatDastor;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<TashilatDastorInfoViewModel> GetTashilatDastorForStimul(string personalCode)
        {
            string result = string.Empty;

            SetBearerToken();

            try
            {
                result = _client.GetStringAsync(apiDastorUrl + "/GetTashilatDastor/" + personalCode).Result;
            }
            catch
            {
                result = string.Empty;
            }

            var tashilatDastor = JsonConvert.DeserializeObject<List<TashilatDastorInfoViewModel>>(result);

            return tashilatDastor;
        }

        #endregion

        #region TashilatOther

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultOtherDto GetTashilatOther(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTashilatOtherUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashilatOther = JsonConvert.DeserializeObject<ApiResultOtherDto>(result);

            return tashilatOther;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultBelaavazDto GetTashilatBelaavaz(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiTashilatOtherUrl + "/GetByPrsnCd/" + personalCode).Result;

            var tashilatBelaavaz = JsonConvert.DeserializeObject<ApiResultBelaavazDto>(result);

            return tashilatBelaavaz;
        }

        #endregion

        #region Nomarat Arzyabi

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResulExamDto GetExam(string personalCode)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiExamUrl + "/GetByPrsnCd/" + personalCode).Result;

            var exam = JsonConvert.DeserializeObject<ApiResulExamDto>(result);

            return exam;
        }

        #endregion

        #region گرفتن تعداد تشویقات و تنبیهات و نهست و فرار نفر

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetCountFarar(string prnsNo)
        {
            return null;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetCountNahast(string prnsNo)
        {
            return null;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetStatusMarrid(string prnsNo)
        {
            var result = GetPersonalByPersonalNo(prnsNo);

            return result.Data.MarridTitle;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetEmployDate(string prnsNo)
        {
            var result = GetPersonalByPersonalNo(prnsNo);

            return result.Data.EmploymentDate;
        }

        #endregion

        #region فیش حقوقی

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultFishDto GetFishByPrsnCode(string prsnCd)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiFishUrl + "/GetByPrsnCd/" + prsnCd).Result;

            var fish = JsonConvert.DeserializeObject<ApiResultFishDto>(result);

            return fish;
        }

        #endregion

        #region معسرین

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ApiResultMoeeserDto GetMoeeserByPrsnCode(string prsnCd)
        {
            SetBearerToken();

            var result = _client.GetStringAsync(apiMoeeserUrl + "/GetByPrsnCd/" + prsnCd).Result;

            var moeeser = JsonConvert.DeserializeObject<ApiResultMoeeserDto>(result);

            return moeeser;
        }

        #endregion
    }
}