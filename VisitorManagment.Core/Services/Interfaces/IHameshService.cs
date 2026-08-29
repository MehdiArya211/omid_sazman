using System.Collections.Generic;
using System.Dynamic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
	public interface IHameshService
    {
        public List<ActionType> GetActionType();
        public List<VamCode> GetVamCode();
        /// <summary>
        /// ویرایش هامش خالی که نفر هنگام ثبت درخواست ثبت کرده است
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="roleTypeTitle"></param>
        /// <param name="userDesc"></param>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="mablaghVamDarkhasti"></param>
        /// <param name="mablaghVamMohaghaghShode"></param>
        public BaseResult EditHamesh(int actionTypeId,int roleTypeId , string roleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, string userDesc , 
            int userId, int fileId  , double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode);
        public void EditHameshForMeetingViewModel(int actionTypeId, MeetingHoldViewModel meetingHoldViewModel, int userId, int fileId);
        /// <summary>
        /// زمانیکه یه درخواست ملاقات ثبت میشه باید یه رکورد خالی هم تو هامش بخوره با شناسه نفری که لاگین کرده
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="FileId"></param>
        /// <param name="RoleTypeId"></param>
        /// <param name="RoleTypeTitle"></param>
        /// <returns></returns>
        public BaseResult AddToHameshWhenCreateFile(int userId, int FileId , int RoleTypeId , string RoleTypeTitle,   int RoleTypeIdFinal, string RoleTypeTitleFinal );
        public void AddToHameshWhenSendListFileToFarmandehiNezaja(List<int> rcvrUserId, List<Files> files, int RoleTypeId, string RoleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, int userId);
        /// <summary>
        /// وقتی درخواست ملاقات نفر رو به کارتابل هر کی ارسال میکنیم یه هامش خالی به گیرنده میفرستیم
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="rcvrUserId"></param>
        /// <param name="RoleTypeId"></param>
        /// <param name="RoleTypeTitle"></param>
        public BaseResult AddToHameshWhenSendFileToCartable(int userId, int fileId, List<int> rcvrUserId , int RoleTypeId , string RoleTypeTitle, int RoleTypeIdFinal, string RoleTypeTitleFinal);
        public void AddToHameshWhenSendListFileToCartable(int userId, List<Files> files, List<int> rcvrUserId , int RoleTypeId , string RoleTypeTitle);
        /// <summary>
        /// وقتی هامش روش میزنه و عودت رو میزنه یه هامش خالی روش میزنه 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="rcvrUserId"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="roleTypeTitle"></param>
        public void AddToHameshWhenSendFileToCartableWhenBackFile(int userId, int fileId, List<int> rcvrUserId, int roleTypeId, string roleTypeTitle);
        public void AddToHameshWhenSendFileToCartableInMeetingHold(int userId, int fileId, /*List<int>*/  int rcvrUserId , Hamesh hamehsViewModel);
        public BaseResult AddHamesh(Hamesh hamesh);
        public ListHameshViewModel GetHameshIdByFileId(int fileId , int pageId = 1, int requestsubject = 0, string filterCaption = "");
        public int GetHameshIdByUseerIdAndFileId(int userId, int fileId);
        /// <summary>
        /// هامش کسی که لاگین کرده برای درخواست نفر
        /// </summary>
        /// <param name="userId">کسی که لایگن کرده</param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Hamesh GetHameshByUserIdAndFileId(int userId, int fileId);
        public string GetHameshHeiatReeiseByUserIdAndFileId(int userId, int fileId);
        public Hamesh GetHameshByUserIdAndFileId2(int userId, int? fileId);
        public int? GetParentIdHameshByUserIdAndFileId(int userId, int fileId);
       
        public List<Users> GetUserByParentId(int? parentId);
        BaseResult UpdateHamesh(Hamesh hamesh);
        int? GetMeetingIdByFileId(int fileId);

        /// <summary>
        /// آخرین هامش ثبت شده
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Hamesh GetPervHameshForFRadeBalatar(int fileId);

        //get last hameshkarshenasgharargahansarnezaja
        string GetlastHameshKarshenasgharagahAnsarNezaja(int fileId);
        string GetFirstHameshKarshenasgharagahAnsarNezaja(int fileId);
        //هامش کاربر نزاجا
        string GetlastHameshKarbarNezaja(int fileId);
        //get Perv Hamesh
        List<Hamesh> GetHameshMoavenatForFRadeBalatar(int fileId);
        //Get Role Type Person
        HameshInfoViewModel GetRoleTypePerson(int id);
        //get all hamesh for any person
        List<HameshInfoViewModel> getAllHameshWithOutMoavenat(int fileId);
        /// <summary>
        /// هامش معاونت ها
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        List<HameshInfoViewModel> getAllHameshMoavenat(int fileId);

        /// <summary>
        ///  گرفتن تمام اطلاعات مربوط به نفر برای هامش
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        HameshFullInfoFileViewModel GetFullInfoFile(int fileId , int userId);

        /// <summary>
        ///  گرفتن تمام اطلاعات مربوط به نفر برای هامش در صفحه ارتباط تصویری
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        HameshFullInfoFileViewModel GetFullInfoFileForOnlineConversation(int fileId, int userId);


        #region Hamesh For Stimul
        //farmandeh Unit Duty
        string GetHameshFUnitDuty(int fileId);
        //farmandeh Uint
        string GetHameshFUnit(int fileId);
        //farmandeh Gharargah
        string GetHameshFGharargah(int fileId);
        #endregion


        /// <summary>
        /// عملیات کلی ثبت هامش 
        /// </summary>
        /// <returns></returns>
        public BaseResult RegHamesh(int actionTypeId, int roleTypeId, string roleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, string userDesc, int userId, int fileId, double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode , List<int> rcvrUserId);

        /// <summary>
        /// ثبت درخواست ملاقات و ثبت رکورد در کارتابل و ثبت رکورد خالی در هامش
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        BaseResult RegFileAndAddToCartableAndRegHamesh(FactPersonalViewModel model, int userId, int roleTypeId, string roleTypeTitle, int roleTypeFinalId, string roleTypeFinalTitle);

        BaseResult RegHameshHeiatRaeise(int fileId, HameshFullInfoFileViewModel hamesh , int roleTypeId , string roleTypeTitle , int roleTypeIdFinal ,  string roleTypeTitleFinal, int userId);



    }
}
