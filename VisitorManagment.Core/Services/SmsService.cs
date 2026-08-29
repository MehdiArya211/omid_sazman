using ITOWebApiClient;
using ITOWebApiClient.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
	public class SmsService : ISmsService
    {
        private string apiUrl = string.Empty;//"http://localhost:65504/SMSInfo";
        private string apiUrlCardIsar = string.Empty;
        private HttpClient _client;
        //******************************************************

        private readonly VisitorManagmentContext _context;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        private readonly ApiTokenCacheClient _apiTokenClient;



        public SmsService(VisitorManagmentContext context , IHameshService hameshService, ApiTokenCacheClient apiTokenClient , IWebApiService webApiService)
        {
            _webApiService = webApiService;
            _hameshService = hameshService;
            _context = context;
            _apiTokenClient = apiTokenClient;
            _client = new HttpClient();

            apiUrl = CustomSettings.Instance.ApiSmsUrl;

            apiUrlCardIsar = CustomSettings.Instance.ApiCardIsarUrl;
        }

        #region SMS


        public ListSMSInfoViewModel GetFileForSMSInfo(int? meetingId)
        {
            IQueryable<Files> result = _context.Files.Where(f => f.MeetingId == meetingId);
            ListSMSInfoViewModel list = new ListSMSInfoViewModel() { };
            string meetingTitle = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).Title;

            list.sMSInfos = result.Select(t => new SMSInfoViewModel()
            {
                CreateDate = DateTime.Now,
                SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 1).TitleFa,
                Mobile = t.Phone,
                PrsnNo = t.PersonalCode,
                NationalNo = t.MelliCode,
                //دو خط زیر ثابته
                SMSTypeId = 1,
                SystemTypeId = 1,
                FullName = t.FirstName + t.LastName,

            }).OrderBy(u => u.PrsnNo).ToList();
            return list;

        }


        public int AddSMSInfo(SMSInfoViewModel smsInfo)
        {
            var token=_webApiService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string jsonSmsInfo = JsonConvert.SerializeObject(smsInfo);
            StringContent content = new StringContent(jsonSmsInfo, Encoding.UTF8, "application/json");
            var res = _client.PostAsync(apiUrl, content).Result;

            if (res.IsSuccessStatusCode)
                return ((int)res.StatusCode);

            else return 0;

        }

        public SMSInfoViewModel GetFileIdForSendSmsInEachProcess(int id, int fileId, string roleTypeTitle, int? actionTypeId)
        {

            Files result = _context.Files.Include(x => x.Personal).Where(f => f.Id == fileId).SingleOrDefault();
            SMSInfoViewModel sms = new SMSInfoViewModel() { };

            switch (id)
            {
                case 1:
                    //دعوت به جلسه
                    sms.CreateDate = DateTime.Now;
                    sms.SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 1).TitleFa;
                    sms.Mobile = result.Phone;
                    sms.PrsnNo = result.Personal.PersonalCode;
                    sms.NationalNo = result.Personal.MelliCode;
                    sms.Mobile = result.Phone;
                    //دو خط زیر ثابته
                    sms.SMSTypeId = 1;
                    sms.SystemTypeId = 1;
                    sms.FullName = result.Personal.FirstName + result.Personal.LastName;
                    return sms;

                case 2:
                    //برگزاری جلسه
                    sms.CreateDate = DateTime.Now;
                    sms.SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 2).TitleFa;
                    sms.Mobile = result.Phone;
                    sms.PrsnNo = result.Personal.PersonalCode;
                    sms.NationalNo = result.Personal.MelliCode;
                    sms.Mobile = result.Phone;
                    //دو خط زیر ثابته
                    sms.SMSTypeId = 1;
                    sms.SystemTypeId = 1;
                    sms.FullName = result.Personal.FirstName + result.Personal.LastName;
                    return sms;

                case 3:
                    //ملاقات ثبت شد
                    sms.CreateDate = DateTime.Now;
                    sms.SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 3).TitleFa;
                    sms.Mobile = result.Phone;
                    sms.PrsnNo = result.Personal.PersonalCode;
                    sms.NationalNo = result.Personal.MelliCode;
                    sms.Mobile = result.Phone;
                    //دو خط زیر ثابته
                    sms.SMSTypeId = 1;
                    sms.SystemTypeId = 1;
                    sms.FullName = result.Personal.FirstName + result.Personal.LastName;
                    return sms;

                case 4:
                    //ملاقات ویرایش شد
                    sms.CreateDate = DateTime.Now;
                    sms.SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 4).TitleFa;
                    sms.Mobile = result.Phone;
                    sms.PrsnNo = result.Personal.PersonalCode;
                    sms.NationalNo = result.Personal.MelliCode;
                    sms.Mobile = result.Phone;
                    //دو خط زیر ثابته
                    sms.SMSTypeId = 1;
                    sms.SystemTypeId = 1;
                    sms.FullName = result.Personal.FirstName + result.Personal.LastName;
                    return sms;

                case 5:

                    //هامش

                    sms.CreateDate = DateTime.Now;
                    if (actionTypeId == 1)
                    {
                        sms.SmsBody = " سلام ، درخواست ملاقات شما در مرحله اقدام قرار گرفت" + "\n" + "اقدام کننده: " + roleTypeTitle;
                    }
                    if (actionTypeId == 2)
                    {
                        sms.SmsBody = " سلام ، درخواست ملاقات شما در مرحله ثبت نظریه قرار گرفت" + "\n" + "اقدام کننده: " + roleTypeTitle;
                    }
                    if (actionTypeId == 3)
                    {
                        sms.SmsBody = " سلام ، درخواست ملاقات شما در مرحله رد درخواست و عودت قرار گرفت" + "\n" + "اقدام کننده: " + roleTypeTitle;
                    }
                    if (actionTypeId == 4 && actionTypeId == 1002)
                    {
                        sms.SmsBody = " سلام ، درخواست ملاقات شما در مرحله انتظار قرار گرفت" + "\n" + "اقدام کننده: " + roleTypeTitle;
                    }

                    sms.Mobile = result.Phone;
                    sms.PrsnNo = result.Personal.PersonalCode;
                    sms.NationalNo = result.Personal.MelliCode;
                    sms.Mobile = result.Phone;
                    //دو خط زیر ثابته
                    sms.SMSTypeId = 1;
                    sms.SystemTypeId = 1;
                    sms.FullName = result.Personal.FirstName + result.Personal.LastName;
                    return sms;

                default:

                    break;
            }

            return null;
        }


        public void SendSmsToSelseleMaratebYeganNafar(int fileId)
        {
            //وقتی درخواست ملاقات نفر تو جلسه توسط هیئت رییسخ دستور داده میشه ما یه لیستی از سلسه مراتب برمیداریم تا بهشون پیام بدیم
            var listPhoneSelseleMaratebYeganNafar = _hameshService.getAllHameshWithOutMoavenat(fileId);
            Files result = _context.Files.Include(x => x.Personal).Where(f => f.Id == fileId).SingleOrDefault();
            SMSInfoViewModel sms = new SMSInfoViewModel() { };

            foreach (var item in listPhoneSelseleMaratebYeganNafar)
            {
                //ارسال پیامک به ف قرارگاه ، ف عمده ، خوده نفر
                sms.CreateDate = DateTime.Now;
                sms.SmsBody = "با سلام ، جلسه ملاقات " + result.Personal.FirstName + result.Personal.LastName + "انجام شد و برای مشاهده نتیجه به کارشناس قرارگاه انصار یگان مراجعه کنید";
                sms.Mobile = item.PhoneSelseleMaratebYeganNafar;
                sms.PrsnNo = result.Personal.PersonalCode;
                sms.NationalNo = result.Personal.MelliCode;
                //دو خط زیر ثابته
                sms.SMSTypeId = 1;
                sms.SystemTypeId = 1;
                sms.FullName = result.Personal.FirstName + result.Personal.LastName;


                AddSMSInfo(sms);
            }


        }

        public void SendSmsToMemberAddToMeeting(int meetingId)
        {

            var listSMSInfoViewModel = new ListSMSInfoViewModel();
            listSMSInfoViewModel =GetFileForSMSInfo(meetingId);


            foreach (var item in listSMSInfoViewModel.sMSInfos)
            {
                AddSMSInfo(item);
            }
        }





        #endregion

    }
}
