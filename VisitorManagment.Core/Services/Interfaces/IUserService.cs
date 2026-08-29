using VisitorManagment.Core.DTOs;
using VisitorManagment.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using System.Threading.Tasks;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IUserService
    {
        bool IsExistUserName(string userName);
        Users GetUserByActiveCode(string activeCode);
        Users GetUserByUserName(string username);
        //متد زیر فعلا استفاده نشده
        //FactPersonalViewModel GetUserByUserNameJustForGetAdmin(string username);
        Users GetUserByUserNameIfDeleteTrue(string username);
        Users LoginUser(LoginViewModel login);
        int AddUser(Users user);
        Users GetUserByUserId(int userId);
        void UpdateUser(Users user);
        InformationUserViewModel GetUserInformation(int userId);
        void DeleteUser(int userId);
        CreateUserViewModel GetPersonalByPersonalNo(string Id , string token);
        #region AdminPanel
        UserForAdminViewModel GetUsers(int roleTypeId , string userLoggin , int pageId = 1, string filterFullName = "", string filterUserName = "" );
        UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterFullName = "", string filterUserName = "");
        int AddUserFromAdmin(CreateUserViewModel user);
        EditUserViewModel GetUserForShowInEditMode(int userId);
        void EditUserFromAdmin(EditUserViewModel editUserViewModel , string password ,int EditUserId , int userId);
        #endregion

        #region User
        int GetUserIdByPersonalCode(string personalCode);
        Users GetUserByPersonalCode(string personalCode);
        UserInfoViewModel GetUserByPersonalNo(string personalCode);
        //Users GetUserByCodeGhaWhenAdminGharargah(string personalCodeUserInput , string unitCodeUserInput, string CodeGhaUserInput , string unitCodeUserLogin , string CodeGhaUserLogin);
        List<Users> GetUsers(int unitDutyCode);
        void AddAccessTypeToRole(List<int> accessRoleIds, int roleId , int addUserId);
        //id = userid

        checkingFPrsnNoViewModel CheckingCreateFYeganOmdeh(int unitCode,  int codeGha);
        #endregion

        #region Change Password
        bool ForgetPassword(ForgetPasswordViewModel forgetPassword);
        #endregion

        #region SignUp
        int SignUpUser(SignUpViewModel user);
        #endregion

        #region Other Service

        #endregion

        #region Role
        // int GetRoleTypeByUserId(int userId);
        #endregion

        /// <summary>
        /// گرفتن لیست فرماندهان یگان ها
        /// </summary>
        /// <returns></returns>
        ListUserViewModel GetListFarmandehanUnit();


        /// <summary>
        /// گرفتن لیست فرماندهان قرارگاه
        /// </summary>
        /// <returns></returns>
        ListUserViewModel GetListFarmandehanGha();

        /// <summary>
        /// ذخیره اطلاعات سیستمی کاربری که لاگین کرده
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CreateDate"></param>
        /// <param name="ipUser"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task AddUserLoginHistory(string userId, DateTime CreateDate, string ipUser, bool status);
        /// <summary>
        /// گرفتن اطلاعات سیستمی نفر لاگین کرده
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetInfoUserLoginHistory(int userName);

    }
}
