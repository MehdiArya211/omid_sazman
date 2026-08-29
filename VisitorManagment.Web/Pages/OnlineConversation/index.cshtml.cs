using System.Linq;
using _0_Framework.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;

namespace NVC.Web.Pages
{
    [Authorize]
    public class ConfModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IFileService _fileService;
        private readonly IPersonService _personService;
        private readonly IWebApiService _webApiService;
        private readonly IHameshService _hameshService;
        private readonly IVamService _vamService;

        public ConfModel(IMeetingService meetingService, IFileService fileService, IPersonService personService, IWebApiService webApiService,
            IHameshService hameshService, IVamService vamService)
        {
            _meetingService = meetingService;
            _fileService = fileService;
            _personService = personService;
            _webApiService = webApiService;
            _hameshService = hameshService;
            _vamService = vamService;
        }

        [BindProperty]
        public ListMeetingViewModel listMeetingViewModel { get; set; }
        public ListFileReferenceViewModel listviewmodel { get; set; }
        /// <summary>
        /// هامش های گردش درخواست
        /// </summary>
        public ListHameshViewModel listHameshViewModel { get; set; }
        public HameshFullInfoFileViewModel hameshFullInfoViewModel { get; set; }


        /// <summary>
        ///  خلاصه وضعیت نفر
        /// </summary>
        public CollectionSpecificationPersonalViewModel collectionSpecificationPersonalViewModel { get; set; }

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int pageId = 1, int filterMeetingStatus = 1, string filterCaption = "")
        {

            #region initial property
            listviewmodel = new ListFileReferenceViewModel();
            listHameshViewModel = new ListHameshViewModel();
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            collectionSpecificationPersonalViewModel.FactPersonal = new FactSpecificationPersonalViewModel();
            collectionSpecificationPersonalViewModel.ListFile = new ListFileInfoViewModel();
            collectionSpecificationPersonalViewModel.ListFile.files = new System.Collections.Generic.List<FileInfoViewModel>();
            hameshFullInfoViewModel = new HameshFullInfoFileViewModel();
            hameshFullInfoViewModel.ListVam = new System.Collections.Generic.List<VamViewModel>() ;
            hameshFullInfoViewModel.file=new FactPersonalViewModel ()   ;
            hameshFullInfoViewModel.hameshAllYegan=new System.Collections.Generic.List<HameshInfoViewModel> ();
            hameshFullInfoViewModel.HameshMoavenats=new System.Collections.Generic.List<HameshInfoViewModel> ();
            hameshFullInfoViewModel.HameshKarbarNezaja = "";
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel = new HameshFullInfoFileViewModel();
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.file =new FactPersonalViewModel ();
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.hameshAllYegan = new System.Collections.Generic.List<HameshInfoViewModel>() ;
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.HameshMoavenats = new System.Collections.Generic.List<HameshInfoViewModel> () ;
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.HameshKarbarNezaja ="" ;
            #endregion

            var userId = int.Parse(User.FindFirst("Id").Value);
            listMeetingViewModel = _meetingService.GetListMeeting(pageId, filterMeetingStatus, filterCaption);
            ViewData["Meets"] = new SelectList(_meetingService.GetListMeetingForOnlineConversation().Meetings, "Id", "Title");

        }

        #region نمایش اعضای جلسه با انتخاب تاریخ جلسه
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetListPersonal(int meetingId)
        {
            var meetingIdFinal = HttpContext.Session.GetString("meetingId") ?? meetingId.ToString();
            listviewmodel = new ListFileReferenceViewModel();
            listviewmodel = _meetingService.GetListFileForEditReferenceForOnlineConversation(int.Parse(meetingIdFinal));


            if (listviewmodel.files.Count() > 0)
            {

                // var dateCOnvert =ConvertDigitToDateMain(date);
                return new JsonResult(listviewmodel.files);
            }

            else
            {
                return new JsonResult("");

            }



        }

        #endregion

        #region خلاصه وضعیت نفر
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetInfoPersonal(int fileId)
        {
            #region initial property
            listviewmodel = new ListFileReferenceViewModel();
            listHameshViewModel = new ListHameshViewModel();
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            collectionSpecificationPersonalViewModel.FactPersonal = new FactSpecificationPersonalViewModel();
            collectionSpecificationPersonalViewModel.ListFile = new ListFileInfoViewModel();
            collectionSpecificationPersonalViewModel.ListFile.files = new System.Collections.Generic.List<FileInfoViewModel>();
            hameshFullInfoViewModel = new HameshFullInfoFileViewModel();
            hameshFullInfoViewModel.ListVam = new System.Collections.Generic.List<VamViewModel>();

            #endregion

            ViewData["FileId"] = fileId;

            var userId = int.Parse(User.FindFirst("Id").Value);


            #region دریافت درخواست ملاقات و هامش های آن   

            var res = _fileService.GetFileForEdit(fileId);
            ViewData["IsAnswerMoavenat"] = res.IsAnswerdMoavenat;
            ViewData["IsArchived"] = res.IsArchived;
            hameshFullInfoViewModel = _hameshService.GetFullInfoFileForOnlineConversation(fileId, userId);
            hameshFullInfoViewModel.hameshKarshenasGharagahAnsarNezaja = _hameshService.GetlastHameshKarshenasgharagahAnsarNezaja(fileId);
            hameshFullInfoViewModel.ListVam = _vamService.getAllVamWithFileId(fileId);

            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel = _hameshService.GetFullInfoFileForOnlineConversation(fileId, userId);
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.hameshKarshenasGharagahAnsarNezaja = _hameshService.GetlastHameshKarshenasgharagahAnsarNezaja(fileId);
            collectionSpecificationPersonalViewModel.hameshFullInfoViewModel.ListVam = _vamService.getAllVamWithFileId(fileId);
            #endregion
            #region خلاصه وضعیت نفر

            var personalId = _fileService.GetFileByFileId(fileId).PersonalId;
            var personalCode = _personService.GetPersonByPersonalId(personalId).PersonalCode;
            ViewData["PersonalCode"] = personalCode;
            HttpContext.Session.SetString("personalCode", personalCode);
            HttpContext.Session.SetString("fileId", fileId.ToString());

            collectionSpecificationPersonalViewModel.ListFile = _fileService.GetListFile(personalId);
            collectionSpecificationPersonalViewModel.FactPersonal = _personService.GetPersonalForEdit(personalId);
            collectionSpecificationPersonalViewModel.FactPersonal.HomDat = _webApiService.GetPersonalByPersonalNo(personalCode).Data.HomDat;
            return new JsonResult(collectionSpecificationPersonalViewModel);
            #endregion




        }

        #endregion

        #region لیست هامش های ثبت شده
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetAllListHamesh()
        {
            listHameshViewModel = new ListHameshViewModel();
            var fileId = HttpContext.Session.GetObjectFromJson<string>("fileId");
            listHameshViewModel = _hameshService.GetHameshIdByFileId(int.Parse(fileId));
            return new JsonResult(listHameshViewModel.hameshes);

        }

        #endregion

        #region سرویس ها



        #region Tashvighat
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashvighat(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");
            //لازمه که تو هر هندلر ویو مدل برای مودالامون رو اینیشیال کنیم!!!
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashvighat = _webApiService.GetTashvighatByPersonalNo(personCode);

            collectionSpecificationPersonalViewModel.Tashvighat = new ApiResultTashvighatDto();

            if (tashvighat.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Tashvighat.Data = tashvighat.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Tashvighat.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }
                // var dateCOnvert =ConvertDigitToDateMain(date);
                return new JsonResult(collectionSpecificationPersonalViewModel.Tashvighat.Data.OrderByDescending(x => x.RowBeginDate));
            }

            else
            {
                return new JsonResult("");

            }



        }
        #endregion

        #region Tanbihat
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTanbihat(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            var tanbihat = _webApiService.GetTanbihatByPersonalNo(personCode);

            collectionSpecificationPersonalViewModel.Tanbihat = new ApiResultTanbihatDto();

            if (tanbihat.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Tanbihat.Data = tanbihat.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Tanbihat.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Tanbihat.Data.OrderByDescending(x => x.RowBeginDate));
            }

            else
            {
                return new JsonResult("");

            }

        }
        #endregion

        #region Entegalat

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetEnteghal(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var enteghal = _webApiService.GetEnteghalByPersonNo(personCode);

            collectionSpecificationPersonalViewModel.Enteghal = new ApiResultEnteghalDto();

            if (enteghal.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Enteghal.Data = enteghal.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.Enteghal.Data)
                {
                    item.WentDate = Tools.ConvertDigitToDateMain(item.WentDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Enteghal.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");

            }


        }
        #endregion

        #region Aele
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetAeleh(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var personFamily = _webApiService.GetPFamilyInfo(personCode);

            collectionSpecificationPersonalViewModel.PersonFamily = new ApiResultPersonFamilyDto();

            if (personFamily.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.PersonFamily.Data = personFamily.Data;

                return new JsonResult(collectionSpecificationPersonalViewModel.PersonFamily.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");

            }

        }

        #endregion

        #region Maskan
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatMaskan(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var maskan = _webApiService.GetPTashilatMaskan(personCode);

            collectionSpecificationPersonalViewModel.TashilatMaskan = new ApiResulMaskanDto();

            if (maskan.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatMaskan.Data = maskan.Data.OrderByDescending(x => x.RegDate).ToList();

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatMaskan.Data)
                {
                    item.MablaqTitle = Tools.FormatNumber(item.Mablaq.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatMaskan.Data)
                {
                    item.VagozariDate = Tools.ConvertDigitToDateMain(item.VagozariDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatMaskan.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");

            }


        }

        #endregion

        #region Tashilat DabirKhaneh
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatDabirKhaneh(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashiladDabirKhaneh = _webApiService.GetPTashilatDabirkhaneh(personCode);

            collectionSpecificationPersonalViewModel.TashilatDabirkhaneh = new ApiResulDabirKhanehDto();

            if (tashiladDabirKhaneh.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data = tashiladDabirKhaneh.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data)
                {
                    item.ConfirmMablaqVamTitle = Tools.FormatNumber(item.ConfirmMablaqVam.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data)
                {
                    item.ConfirmOrderDate = Tools.ConvertDigitToDateMain(item.ConfirmOrderDate);
                }


                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");


            }

        }
        #endregion

        #region TashilatDastor
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatDastor(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var dastor = _webApiService.GetTashilatDastor(personCode);

            collectionSpecificationPersonalViewModel.TashilatDastor = new ApiResulDastorDto();

            if (dastor.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatDastor.Data = dastor.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDastor.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatDastor.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region تسهیلات دیگر
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatOther(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var other = _webApiService.GetTashilatOther(personCode);

            collectionSpecificationPersonalViewModel.TashilatOther = new ApiResultOtherDto();

            if (other.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatOther.Data = other.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.TashilatOther.Data)
                {
                    item.MosaedatDate = Tools.ConvertDigitToDateMain(item.MosaedatDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatOther.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region تسهیلات بلاعوض
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatBelaavaz(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var belaavaz = _webApiService.GetTashilatBelaavaz(personCode);

            collectionSpecificationPersonalViewModel.TashilatBelaavaz = new ApiResultBelaavazDto();

            if (belaavaz.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data = belaavaz.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data)
                {
                    item.MablaqTitle = Tools.FormatNumber(item.Mablaq.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data)
                {
                    item.RegDate = Tools.ConvertDigitToDateMain(item.RegDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region Exam
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetExam(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var exam = _webApiService.GetExam(personCode);

            collectionSpecificationPersonalViewModel.Exam = new ApiResulExamDto();

            if (exam.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Exam.Data = exam.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Exam.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                    item.RowEndDate = Tools.ConvertDigitToDateMain(item.RowEndDate);
                    item.RegisterDate = Tools.ConvertDigitToDateMain(item.RegisterDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.Exam.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region فیش 
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetFish(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var fish = _webApiService.GetFishByPrsnCode(personCode);

            collectionSpecificationPersonalViewModel.Fish = new ApiResultFishDto();

            if (fish.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Fish.Data = fish.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.Fish.Data)
                {
                    item.TotalDebtFormat = Tools.FormatNumber(item.TotalDebt.ToString());
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Fish.Data.OrderByDescending(x => x.DeductionDate));
            }

            else
            {
                return new JsonResult("");
            }


        }
        #endregion

        #region معسرین 
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetMoeeser(string personalCode)
        {
            var personCode = HttpContext.Session.GetObjectFromJson<string>("personalCode");

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var moeeser = _webApiService.GetMoeeserByPrsnCode(personCode);

            collectionSpecificationPersonalViewModel.Moeeser = new ApiResultMoeeserDto();

            if (moeeser.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Moeeser.Data = moeeser.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Moeeser.Data)
                {
                    item.RegDate = Tools.FormatNumber(item.RegDate.ToString());
                    item.LastUpdDate = Tools.FormatNumber(item.LastUpdDate.ToString());
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Moeeser.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }



        #endregion



















        #endregion


        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetDeletePersonFromListMeeting(int fileId)
        {
            var result = _meetingService.ChangeStatusPersonInMeeting(fileId);

            var meetingId =_fileService.GetFileByFileId(fileId).MeetingId.Value.ToString();

            HttpContext.Session.SetString("meetingId", meetingId);

            return new JsonResult(result.Model); 

        }
    }
}
