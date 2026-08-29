using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.Core.Generator;
using VisitorManagment.Core.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisitorManagment.Core.Convertors;
using VisitorManagment.DataLayer.Entities.User;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using ITOWebApiClient.DTOs;
using System.Net.Http.Headers;
using ITOWebApiClient;
using System.Threading.Tasks;

namespace VisitorManagment.Core.Services
{
    public class UserService : IUserService
    {
        private string apiUrl;
        private readonly IConfiguration _configuration;
        private HttpClient _client;
        private readonly VisitorManagmentContext _context;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiService _webApiService;

        public UserService(VisitorManagmentContext context, IConfiguration configuration, IWebApiService webApiService)
        {
            _webApiService = webApiService;
            _context = context;
            _configuration = configuration;
            apiUrl = CustomSettings.Instance.ApiPersonelUrl;
            _client = new HttpClient();

        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public CreateUserViewModel GetPersonalByPersonalNo(string Id, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = _client.GetStringAsync(apiUrl + "/GetPersonalByPersonalCode/" + Id).Result;
            //convert
            var person = JsonConvert.DeserializeObject<CreateUserViewModel>(result);

            return person;
        }

        /// <summary>
        /// شرایط موردنظر را بررسی می‌کند.
        /// </summary>
        public bool IsExistUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public Users LoginUser(LoginViewModel login)
        {
            var pass = PasswordHelper.EncodePasswordMd5(login.Password);
            var username = login.UserName;

            var result= _context.Users.Include(u => u.UserRoles)
                .SingleOrDefault(u => u.UserName == username && u.Password == pass);

            return result;
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserByUserId(userId);

            InformationUserViewModel information = new InformationUserViewModel()
            {

                FirstName = user.FirstName,
                LastName = user.LastName,
                SaveDate = user.RegDate.ToShamsi(),
                UserName = user.UserName,

            };

            return information;
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void DeleteUser(int userId)
        {
            var user = GetUserByUserId(userId);

            user.IsDelete = true;

            UpdateUser(user);
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int AddUser(Users user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Id;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetUserByUserId(int userId)
        {
            return _context.Users.Find(userId);
        }

        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public void UpdateUser(Users user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetUserByUserName(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }
      
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetUserByUserNameIfDeleteTrue(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        #region AdminPanel

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int AddUserFromAdmin(CreateUserViewModel user)
        {

            Users addUser = new Users();
            addUser.Password = PasswordHelper.EncodePasswordMd5(user.Password);
            addUser.ActiveCode = NameGenerator.GenerateUniqCode();
            addUser.FirstName = user.FirstName;
            addUser.LastName = user.LastName;
            addUser.IsActive = true;
            addUser.RegDate = DateTime.Now;
            addUser.UserName = user.PersonalCode;
            addUser.RegUserId = user.AddUserId;
            addUser.UnitDutyCode = user.UnitDutyCode;
            addUser.UnitDutyTitle = user.UnitDutyTitle;
            addUser.UnitCode = user.UnitCode;
            addUser.CodGhaTitle = user.CodGhaTitle;
            addUser.CodGha = user.CodGha;
            addUser.UnitTitle = user.UnitTitle;
            addUser.RankCode = user.RankCode;
            addUser.RankTitle = user.RankTitle;
            addUser.BranchCode = user.BranchCode;
            addUser.BranchTitle = user.BranchTitle;
            addUser.JobDes = user.JobDes;


            #region Save Avatar

            if (user.UserAvatar != null)
            {
                string imagePath = "";
                addUser.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(user.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", addUser.UserAvatar);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }
            }
            else
            {
                addUser.UserAvatar = "Default.jpg";
            }

            #endregion

            return AddUser(addUser);

        }

        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public void EditUserFromAdmin(EditUserViewModel editUserViewModel, string password, int EditUserId, int userId)

        {
            var editUser = GetUserByUserId(userId);

            if (!string.IsNullOrEmpty(password))
                editUser.Password = PasswordHelper.EncodePasswordMd5(password);

            editUser.FirstName = editUserViewModel.FirstName;
            editUser.LastName = editUserViewModel.LastName;
            editUser.BranchTitle = editUserViewModel.BranchTitle;
            editUser.BranchCode = editUserViewModel.BranchCode;
            editUser.JobDes = editUserViewModel.JobDes;
            editUser.RankTitle = editUserViewModel.RankTitle;
            editUser.RankCode = editUserViewModel.RankCode;
            editUser.UnitDutyCode = editUserViewModel.UnitDutyCode;
            editUser.UnitCode = editUserViewModel.UnitCode;
            editUser.UnitDutyTitle = editUserViewModel.UnitDutyTitle;
            editUser.UnitTitle = editUserViewModel.UnitTitle;
            editUser.CodGhaTitle = editUserViewModel.CodGhaTitle;
            editUser.CodGha = editUserViewModel.CodGha;
            editUser.IsActive = true;
            editUser.EditDate = DateTime.Now;
            editUser.UserName = editUserViewModel.UserName;
            editUser.EditUserId = EditUserId;


            #region Save Avatar

            if (editUserViewModel.UserAvatar != null)
            {
                string imagePath = "";
                if (editUserViewModel.AvatarName != "Default.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUserViewModel.AvatarName);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }

                }

                editUser.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(editUserViewModel.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUser.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    editUserViewModel.UserAvatar.CopyTo(stream);
                }
            }

            #endregion

            UpdateUser(editUser);
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterFullName = "", string filterUserName = "")
        {
            IQueryable<Users> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            if (!string.IsNullOrEmpty(filterFullName))
            {
                result = result.Where(u => u.LastName.Contains(filterFullName));
            }

            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            // Show Item In Page
            int take = 10;
            int skip = (pageId - 1) * take;


            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.Select(t => new ListUserViewModel()
            {
                Id = t.Id,
                UserName = t.UserName,
                Password = t.Password,
                FirstName = t.FirstName,
                LastName = t.LastName,
                AvatarName = t.UserAvatar,
                BranchTitle = t.BranchTitle,
                BranchCode = t.BranchCode,
                JobDes = t.JobDes,
                IsActive = t.IsActive,
                RankTitle = t.RankTitle,
                RankCode = t.RankCode,
                IsDelete = t.IsDelete,
                UnitDutyCode = t.UnitDutyCode,
                UnitCode = t.UnitCode,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                CodGha = t.CodGha,
                RegUserId = t.RegUserId,
                RegUserTitle = t.FirstName + "**" + t.LastName + "**" + t.UserName,

            }).OrderBy(u => u.Id).Skip(skip).Take(take).ToList();




            return list;
        }



        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            var roleId = _context.UserRoles.Where(u => u.UserId == userId).Select(u => u.RoleId).SingleOrDefault();
            Role role = _context.Roles.Where(r => r.RoleId == roleId).SingleOrDefault();

            return _context.Users.Where(u => u.Id == userId).Select(c => new EditUserViewModel()
            {
                UserId = c.Id,
                UserName = c.UserName,
                FirstName = c.FirstName,
                LastName = c.LastName,
                BranchTitle = c.BranchTitle,
                BranchCode = c.BranchCode,
                JobDes = c.JobDes,
                RankTitle = c.RankTitle,
                RankCode = c.RankCode,
                UnitDutyCode = c.UnitDutyCode,
                UnitCode = c.UnitCode,
                UnitDutyTitle = c.UnitDutyTitle,
                UnitTitle = c.UnitTitle,
                CodGhaTitle = c.CodGhaTitle,
                CodGha = c.CodGha,
                AvatarName = c.UserAvatar,

                UserRolesTitle = role.Title,
                UserRolesId = roleId
            }).SingleOrDefault();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public UserForAdminViewModel GetUsers(int roleTypeId, string userIdLoggin, int pageId = 1, string filterFullName = "", string filterUserName = "")
        {

            IQueryable<Users> result;

            //if (roleTypeId == 100) 
            //{
                result = _context.Users;
            //}

            //میگه فقط یوزر هایی رو که هر مدیر قرارگاهی اضافه کرده نشون بده

            //else
            //{
            //    result = _context.Users.Where(x => x.RegUserId == int.Parse(userIdLoggin));
            //}


            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName) || u.LastName.Contains(filterUserName));
            }

            // Show Item In Page
            //int take = 10;
            //int skip = (pageId - 1) * take;


            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
           // list.skip = skip;
            list.count = result.Count();
           // list.PageCount = result.Count() / take;
            list.Users = result.Select(t => new ListUserViewModel()
            {
                Id = t.Id,
                UserName = t.UserName,
                RoleTitle = t.UserRoles.Select(x=>x.Role.Title).FirstOrDefault(),
                Password = t.Password,
                FirstName = t.FirstName,
                LastName = t.LastName,
                AvatarName = t.UserAvatar,
                BranchTitle = t.BranchTitle,
                BranchCode = t.BranchCode,
                JobDes = t.JobDes,
                IsActive = t.IsActive,
                RankTitle = t.RankTitle,
                RankCode = t.RankCode,
                IsDelete = t.IsDelete,
                UnitDutyCode = t.UnitDutyCode,
                UnitCode = t.UnitCode,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                CodGha = t.CodGha,
                RegUserId = t.RegUserId,
                RegUserTitle = result.Where(x => x.Id == t.RegUserId).Select(x => x.FirstName + " " + x.LastName + "**" + x.UserName).SingleOrDefault(),
                RegDate = t.RegDate,
                EditUserId = t.EditUserId,
                EditDate = t.EditDate,

          //  }).OrderByDescending(u => u.RegDate).Skip(skip).Take(take).ToList();
            }).OrderByDescending(u => u.RegDate).ToList();

            return list;
        }

        #endregion

        #region useraccess
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int GetUserIdByPersonalCode(string personalCode)
        {
            return _context.Users.Where(u => u.UserName == personalCode).Select(u => u.Id).SingleOrDefault();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetUserByPersonalCode(string personalCode)
        {
            return _context.Users.Where(u => u.UserName == personalCode).SingleOrDefault();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public UserInfoViewModel GetUserByPersonalNo(string personalCode)
        {
            return _context.Users.Include(x => x.UserRoles)
                .Where(x => x.UserName == personalCode)
                .Select(x => new UserInfoViewModel
            {

                FirstName = x.FirstName,
                LastName = x.LastName,
                RankTitle = x.RankTitle,

            }).SingleOrDefault();
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Users> GetUsers(int unitDutyCode)
        {
            return _context.Users
                .Where(u => u.UnitDutyCode == unitDutyCode && u.IsActive == true && u.IsDelete == false)
                .ToList();
        }

        //public void AddAccessTypeToUser(List<int> accessRoleIds, int addUserId, int roleId)
        //{
        //    foreach (int roleid in accessRoleIds)
        //    {
        //        _context.UserAccesses.Add(new UserAccess()
        //        {
        //            AccessRoleId = roleid,
        //            RoleId = roleId,
        //            AddUserId = addUserId,
        //            SaveDate = DateTime.Now,
        //        }) ;
        //    }

        //    _context.SaveChanges();
        //}


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddAccessTypeToRole(List<int> accessRoleIds, int roleId, int addUserId)
        {
            //ابتدا دسترسی های داده شده به یک نقش را حذف کرده سپس مجددا ثبت میکنیم
            _context.WorkFlows.Where(w => w.SndrRoleId == roleId).ToList()
                .ForEach(w => _context.WorkFlows.Remove(w));

            foreach (int accessroleid in accessRoleIds)
            {
                _context.WorkFlows.Add(new WorkFlow()
                {
                    RcvrRoleId = accessroleid,
                    SndrRoleId = roleId,
                    RegUserId = addUserId,
                    RegDate = DateTime.Now,

                });
            }

            _context.SaveChanges();
        }

        #endregion

        #region ChangePassword
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public bool ForgetPassword(ForgetPasswordViewModel forgetPassword)
        {
            var user = _context.Users.Where(x => x.UserName == forgetPassword.UserName).SingleOrDefault();
            if (user == null)
            {
                return false;
            }
            user.Password = PasswordHelper.EncodePasswordMd5(forgetPassword.Password);
            UpdateUser(user);
            return true;

        }


        #endregion

        #region SignUp
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public int SignUpUser(SignUpViewModel user)
        {
            int userId = 0;
            var existUser = _context.Users.Where(x => x.UserName == user.UserName && x.IsDelete == false).Select(x => x.Id).SingleOrDefault();

            Users User = new Users();


            if (string.IsNullOrEmpty(existUser.ToString()))
            {

            }

            else
            {
                var access_token = _webApiService.GetToken();

                var personal = GetPersonalByPersonalNo(user.UserName, access_token);

                //User.RoleId = 14;
                //User.RoleType = 1;
                //User.IsDelete = false;
                //User.UserName = user.UserName;
                //User.Password = user.Password;
                //User.UserAvatar ="Default.jpg";
                //User.ExistUser =false;

                User.Password = PasswordHelper.EncodePasswordMd5(user.Password);
                User.ActiveCode = NameGenerator.GenerateUniqCode();
                User.FirstName = personal.FirstName;
                User.LastName = personal.LastName;
                User.IsActive = true;
                User.RegDate = DateTime.Now;
                User.UserName = personal.PersonalCode;
                User.RegUserId = personal.AddUserId;
                User.UnitDutyCode = personal.UnitDutyCode;
                User.UnitDutyTitle = personal.UnitDutyTitle;
                User.UnitCode = personal.UnitCode;
                User.CodGhaTitle = personal.CodGhaTitle;
                User.CodGha = personal.CodGha;
                User.UnitTitle = personal.UnitTitle;
                User.RankCode = personal.RankCode;
                User.RankTitle = personal.RankTitle;
                User.BranchCode = personal.BranchCode;
                User.BranchTitle = personal.BranchTitle;
                User.JobDes = personal.JobDes;
                User.UserAvatar = "Default.jpg";

                userId = AddUser(User);
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = 14,
                    UserId = userId
                });

                return userId;

            }

            return userId;
        }

        /// <summary>
        /// شرایط موردنظر را بررسی می‌کند.
        /// </summary>
        public checkingFPrsnNoViewModel CheckingCreateFYeganOmdeh(int unitCode ,  int codeGha)
        {
            checkingFPrsnNoViewModel result=new checkingFPrsnNoViewModel ();

            //اگر نفر قرارگاه بود
            if (unitCode==codeGha)
            {
                 result.PrsnNo = _context.UserRoles.Include(x => x.User).Where(x => x.RoleId == 7 && x.User.UnitCode == unitCode)
                    .Select(x=>x.User.UserName).SingleOrDefault();
                if (result.PrsnNo==null)
                {
                    result.Respond = false;
                }
                else
                {
                    result.Respond = true;
                }

                return result;
            }

             result.PrsnNo = _context.UserRoles.Include(x => x.User).Where(x => x.RoleId == 6 && x.User.UnitCode == unitCode)
                .Select(x => x.User.UserName).SingleOrDefault();

            if (result.PrsnNo == null)
            {
                result.Respond = false;
            }
            else
            {
                result.Respond = true;
            }

            return result;


           
        }

        /// <summary>
        /// گرفتن لیست فرماندهان یگان ها
        /// </summary>
        /// <returns></returns>
        public ListUserViewModel GetListFarmandehanUnit()
        {
            ////var users=_context.Users.Include(x=>x.UserRoles).ThenInclude(x=>x.Role)
            ////    .Where(x=>x.UserRoles.Select(x=>x.Role).Where(x=>x.RoleType==3).ToList()).ToList();

            //var res = _context.Roles.Include(x => x.RoleType).Include(x => x.UserRoles).ThenInclude(x => x.User)
            //    .Where(x => x.RoleType == 3).Select(x=>new ListUserViewModel()
            //    {
            //        //اینجا رو پر کنم
            //    });

            //return res;

            throw new NotImplementedException();
        }



        /// <summary>
        /// گرفتن لیست فرماندهان قرارگاه
        /// </summary>
        /// <returns></returns>
        public ListUserViewModel GetListFarmandehanGha()
        {
            throw new NotImplementedException();
        }





        #endregion

        #region Role
        //public int GetRoleTypeByUserId(int userId)
        //{
        //    var res = _context.Users.Where(x => x.Id == userId).Include(x => x.UserRoles).ThenInclude(x=>x.Role)
        //        .Select(x=>x.UserRoles.Select(x=>x.Role.RoleType)).SingleOrDefault();
        //    return res.SingleOrDefault();
        //}
        #endregion


        /// <summary>
        /// ذخیره اطلاعات سیستمی کاربری که لاگین کرده
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CreateDate"></param>
        /// <param name="ipUser"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task AddUserLoginHistory(string userId, DateTime CreateDate, string ipUser, bool status)
        {
            var user = new VisitorManagment.DataLayer.Entities.User.UserLoginHistory()
            {
                UserName = userId,
                CreateDate = CreateDate,
                Ip = ipUser,
                Status = status
            };
            _context.UserLoginHistories.Add(user);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetInfoUserLoginHistory(int userName)
        {
            var info = _context.UserLoginHistories.Where(x => x.UserName == userName.ToString()).FirstOrDefault();
            var res="کاربر گرامی با کد پرسنلی " + info.UserName + " و شماره آی پی سیستم " + info.Ip ;
            return res;
        }
    }
}
