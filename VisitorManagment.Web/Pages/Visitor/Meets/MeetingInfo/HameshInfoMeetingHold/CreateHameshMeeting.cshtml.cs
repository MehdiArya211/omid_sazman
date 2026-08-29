using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using ITOWebApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Visitor.Meets.MeetingInfo.HameshInfoMeetingHold
{
	[Authorize]
    public class CreateHameshMeetingModel : PageModel
    {
        #region Ctor
        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;
        private readonly ICartableService _cartableService;
        private readonly IUserService _userService;
        private readonly IHameshService _hameshService;
        private readonly IMeetingService _meetingService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IPersonService _personService;
        private readonly ISmsService _smsService;
        private readonly IVamService _vamService;
        public CreateHameshMeetingModel(IFileService fileService, IWebApiService webApiService,
            ICartableService cartableService, IUserService userService, IHameshService hameshService,
            IMeetingService meetingService, ApiTokenCacheClient apiTokenClient, ISmsService smsSerivce,
            IPersonService personService, IVamService vamService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _cartableService = cartableService;
            _userService = userService;
            _hameshService = hameshService;
            _meetingService = meetingService;
            _apiTokenClient = apiTokenClient;
            _smsService = smsSerivce;
            _personService = personService;
            _vamService = vamService;
        }
        #endregion
        #region Property
        [BindProperty]
        public HameshFullInfoFileViewModel hameshFullInfoViewModel { get; set; }
        public SMSInfoViewModel smsInfoViewModel { get; set; }
        public ListSMSInfoViewModel listSMSInfoViewModel { get; set; }
        public List<Users> usersForSend { get; set; }
        public List<Users> usersForAoudat { get; set; }


        /// <summary>
        ///  خلاصه وضعیت نفر
        /// </summary>
        public CollectionSpecificationPersonalViewModel collectionSpecificationPersonalViewModel { get; set; }

        /// <summary>
        /// لیست جلسات ملاقات
        /// </summary>
        public ListMeetingForHameshFormViewModel listMeetingForHameshFormViewModel { get; set; }

        public ListHameshViewModel listHameshViewModel { get; set; }
        #endregion

        public void OnGet(int id)
        {
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            ViewData["fileId"] = id;

            ViewData["meetingId"] = _hameshService.GetMeetingIdByFileId(id);
            ViewData["codeVam"] = new SelectList(_hameshService.GetVamCode(), "Id", "Title");

            #region Initial
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            #endregion

            #region Cliam
            var RoleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            var RoleTypeTitle = User.FindFirst("RoleTypeTitle").Value;
            var userId = int.Parse(User.FindFirst("Id").Value);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "fileId", id);
            #endregion

            #region خلاصه وضعیت نفر

            var personalId = _fileService.GetFileByFileId(id).PersonalId;
            var personalCode = _personService.GetPersonByPersonalId(personalId).PersonalCode;
            ViewData["PersonalCode"] = personalCode;
            collectionSpecificationPersonalViewModel.ListFile = _fileService.GetListFile(personalId);
            collectionSpecificationPersonalViewModel.FactPersonal = _personService.GetPersonalForEdit(personalId);

            #endregion

            #region دریافت درخواست ملاقات و هامش های آن   

            var res = _fileService.GetFileForEdit(id);

            hameshFullInfoViewModel = _hameshService.GetFullInfoFile(id, userId);
            #endregion



            #region لیست هامش های ثبت شده
            listHameshViewModel = _hameshService.GetHameshIdByFileId(id);
            #endregion

        }

        public IActionResult OnGetCreateVam(int vamCode, string title, int fileId)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var vam = _vamService.CreateVam(vamCode, title, fileId, userId);
            //اگر قبلا وامی برای این نفر با این کد ثبت شد
            if (vam == 0)
            {
                return new JsonResult(false);
            }
            return new JsonResult(true);
        }

        public IActionResult OnGetDeleteVam(int vamId)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var vam = _vamService.DeleteVam(vamId);

            if (vam.Status)
            {
                return new JsonResult(true);
            }

            return new JsonResult(false);
        }

        public IActionResult OnPost()
        {
            var fileId = HttpContext.Session.GetObjectFromJson<int>("fileId");
            int? MeetingId = _meetingService.GetMeetingIdByFileId(fileId);

            #region  Cliam

            var userid = int.Parse(User.FindFirst("Id").Value);
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            var roleTypeTitle = User.FindFirst("RoleTypeTitle").Value;
            var roleTypeFinalId = int.Parse(User.FindFirst("RoleTypeIdFinal").Value);
            var roleTypeFinalTitle = User.FindFirst("RoleTypeTitleFinal").Value;

            #endregion

            var res = _hameshService.RegHameshHeiatRaeise(fileId, hameshFullInfoViewModel , roleTypeId , roleTypeTitle ,roleTypeFinalId , roleTypeFinalTitle , userid);
            if (res.Status)
            {
                return RedirectToPage("/Visitor/Meets/MeetingInfo/MeetingHold/ListPersonalMeetingHold", new { id = MeetingId });

            }

            AddModelError(res.Message);
            return Page();
            //return RedirectToPage("/Visitor/Meets/MeetingInfo/MeetingHold/ListPersonalMeetingHold", new { id = MeetingId });

        }

        private void AddModelError(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }


        #region سرویس ها

        #region Tashvighat
        public IActionResult OnGetGetTashvighat(string personalCode)
        {
            //لازمه که تو هر هندلر ویو مدل برای مودالامون رو اینیشیال کنیم!!!
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashvighat = _webApiService.GetTashvighatByPersonalNo(personalCode);

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
        public IActionResult OnGetGetTanbihat(string personalCode)
        {
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            var tanbihat = _webApiService.GetTanbihatByPersonalNo(personalCode);

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

        public IActionResult OnGetGetEnteghal(string personalCode)
        {
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var enteghal = _webApiService.GetEnteghalByPersonNo(personalCode);

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
        public IActionResult OnGetGetAeleh(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var personFamily = _webApiService.GetPFamilyInfo(personalCode);

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
        public IActionResult OnGetGetTashilatMaskan(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var maskan = _webApiService.GetPTashilatMaskan(personalCode);

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
        public IActionResult OnGetGetTashilatDabirKhaneh(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashiladDabirKhaneh = _webApiService.GetPTashilatDabirkhaneh(personalCode);

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
        public IActionResult OnGetGetTashilatDastor(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var dastor = _webApiService.GetTashilatDastor(personalCode);

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
        public IActionResult OnGetGetTashilatOther(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var other = _webApiService.GetTashilatOther(personalCode);

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
        public IActionResult OnGetGetTashilatBelaavaz(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var belaavaz = _webApiService.GetTashilatBelaavaz(personalCode);

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
        public IActionResult OnGetGetExam(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var exam = _webApiService.GetExam(personalCode);

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
        public IActionResult OnGetGetFish(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var fish = _webApiService.GetFishByPrsnCode(personalCode);

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
        public IActionResult OnGetGetMoeeser(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var moeeser = _webApiService.GetMoeeserByPrsnCode(personalCode);

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
    }
}
