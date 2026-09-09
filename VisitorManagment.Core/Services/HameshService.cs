using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VisitorManagment.Core.Constants;
using VisitorManagment.Core.Convertors;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class HameshService : IHameshService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IFileService _fileService;
        private readonly ICartableService _cartableService;
        private readonly IVamService _vamService;

        public int GetHameshIdByFileIdAndUserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HameshService(VisitorManagmentContext context, IFileService fileService, IVamService vamService, ICartableService cartableService)
        {
            _context = context;
            _fileService = fileService;
            _vamService = vamService;
            _cartableService = cartableService;
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<ActionType> GetActionType()
        {
            return _context.ActionTypes.ToList();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<VamCode> GetVamCode()
        {
            var allVam = _context.VamCodes.ToList();

            var vams = new List<VamCode>();

            foreach (var item in allVam)
            {
                var vam = new VamCode()
                {
                    Id = item.Id,
                    Title = item.Title + "---" + item.Code,
                };

                vams.Add(vam);
            }

            return vams;
        }

        /// <summary>
        /// فقط برای تکمیل رکوردهای هامش خالی قدیمی نگهداری شده است.
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="roleTypeTitle"></param>
        /// <param name="userDesc"></param>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="mablaghVamDarkhasti"></param>
        /// <param name="mablaghVamMohaghaghShode"></param>
        public BaseResult EditHamesh(int actionTypeId, int roleTypeId, string roleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal,
            string userDesc, int userId, int fileId, double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode)
        {
            var editHamesh = _context.Hameshes
                .Where(item => item.UserId == userId && item.FileId == fileId &&
                               (item.UserDesc == null || item.UserDesc.Trim() == ""))
                .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                .FirstOrDefault();

            if (editHamesh == null)
            {
                return new BaseResult(false, "هامش در انتظار اقدام برای این کاربر یافت نشد.");
            }

            if (actionTypeId <= 0 || string.IsNullOrWhiteSpace(userDesc))
            {
                return new BaseResult(false, "نوع اقدام و متن نظریه/هامش الزامی است.");
            }

            editHamesh.ActionTypeId = actionTypeId;
            editHamesh.UserDesc = userDesc.Trim();
            editHamesh.RoleTypeId = roleTypeId;
            editHamesh.RoleTypeTitle = roleTypeTitle;
            editHamesh.RoleTypeFinalId = roleTypeIdFinal;
            editHamesh.RoleTypeFinalTitle = roleTypeTitleFinal;
            editHamesh.RegDate = DateTime.Now;
            editHamesh.MablaghVamDarkhasti = mablaghVamDarkhasti;
            editHamesh.MablaghVamMohaghaghSode = mablaghVamMohaghaghShode;

            _context.Update(editHamesh);
            var result = _context.SaveChanges();

            if (result != 0)
            {
                return new BaseResult
                {
                    Message = "ویرایش هامش موفق",
                    Model = editHamesh,
                    Status = true
                };

            }

            return new BaseResult
            {
                Message = "ویرایش هامش ناموفق",
                Model = editHamesh,
                Status = false
            };

        }



        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public void EditHameshForMeetingViewModel(int actionTypeId, MeetingHoldViewModel meetingHoldViewModel, int userId, int fileId)
        {
            var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);

            if (editHamesh == null || meetingHoldViewModel == null || string.IsNullOrWhiteSpace(meetingHoldViewModel.UserDesc))
                return;

            editHamesh.ActionTypeId = actionTypeId;
            editHamesh.UserDesc = meetingHoldViewModel.UserDesc.Trim();
            editHamesh.RegDate = DateTime.Now;
            UpdateHamesh(editHamesh);
        }
        /// <summary>
        /// ثبت اولیه درخواست را تأیید می‌کند؛ وضعیت انتظار اقدام فقط در کارتابل نگهداری می‌شود.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="FileId"></param>
        /// <param name="RoleTypeId"></param>
        /// <param name="RoleTypeTitle"></param>
        /// <returns></returns>
        public BaseResult AddToHameshWhenCreateFile(int userId, int FileId, int RoleTypeId, string RoleTypeTitle, int RoleTypeIdFinal, string RoleTypeTitleFinal)
        {
            // انتظار اقدام در جدول Cartable نگهداری می‌شود؛ هامش فقط برای اقدام واقعی و دارای متن است.
            return userId > 0 && FileId > 0
                ? new BaseResult(true, "درخواست در کارتابل ایجادکننده قرار گرفت.")
                : new BaseResult(false, "شناسه کاربر یا درخواست معتبر نیست.");
        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddToHameshWhenSendListFileToFarmandehiNezaja(List<int> rcvrUserId, List<Files> files, int RoleTypeId, string RoleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, int userId)
        {
            // گیرنده‌های عملیات گروهی فقط در Cartable ثبت می‌شوند؛ ایجاد هامش خالی متوقف شده است.
        }
        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult AddToHameshWhenSendFileToCartable(int userId, int fileId, List<int> rcvrUserId, int RoleTypeId, string RoleTypeTitle, int RoleTypeIdFinal, string RoleTypeTitleFinal)
        {
            var receiverIds = (rcvrUserId ?? new List<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            // برای سازگاری امضای قدیمی متد باقی مانده، اما دیگر هامش خالی برای گیرنده تولید نمی‌کند.
            return receiverIds.Any()
                ? new BaseResult(true, "گیرنده‌های کارتابل ثبت شدند.")
                : new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");

        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddToHameshWhenSendListFileToCartable(int userId, List<Files> files, List<int> rcvrUserId, int RoleTypeId, string RoleTypeTitle)
        {
            foreach (var file in files ?? new List<Files>())
            {
                var parentId = _context.Hameshes
                    .Where(item => item.FileId == file.Id && item.UserDesc != null && item.UserDesc.Trim() != "")
                    .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                    .Select(item => (int?)item.Id).FirstOrDefault();

                _context.Hameshes.Add(new Hamesh
                {
                    FileId = file.Id,
                    UserId = userId,
                    ActionTypeId = 1,
                    UserDesc = "سلام علیکم، جهت اضافه‌شدن به جلسه ملاقات معرفی گردید.",
                    RoleTypeId = RoleTypeId,
                    RoleTypeTitle = RoleTypeTitle ?? "نامشخص",
                    RoleTypeFinalId = 0,
                    RoleTypeFinalTitle = "نامشخص",
                    ParentId = parentId,
                    RegDate = DateTime.Now
                });
            }

            _context.SaveChanges();

        }



        /// <summary>
        /// وقتی هامش روش میزنه و عودت رو میزنه یه هامش خالی روش میزنه 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="rcvrUserId"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="roleTypeTitle"></param>
        public void AddToHameshWhenSendFileToCartableWhenBackFile(int userId, int fileId, List<int> rcvrUserId, int roleTypeId, string roleTypeTitle)
        {
            // عودت واقعی توسط RegHamesh ثبت می‌شود؛ گیرنده نباید رکورد هامش خالی داشته باشد.
        }




        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddToHameshWhenSendFileToCartableInMeetingHold(int userId, int fileId, int rcvrUserId, Hamesh hamehsViewModel)
        {
            var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);
            if (editHamesh == null || hamehsViewModel == null || string.IsNullOrWhiteSpace(hamehsViewModel.UserDesc))
                return;
            editHamesh.FileId = fileId;
            editHamesh.ActionTypeId = hamehsViewModel.ActionTypeId;
            editHamesh.UserDesc = hamehsViewModel.UserDesc.Trim();
            editHamesh.RegDate = DateTime.Now;
            UpdateHamesh(editHamesh);
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult AddHamesh(Hamesh hamesh)
        {
            _context.Hameshes.Add(hamesh);
            var result = _context.SaveChanges();
            if (result != 0)
            {
                return new BaseResult
                {
                    Message = "ثبت موفق",
                    Model = result,
                    Status = true
                };
            }

            return new BaseResult
            {
                Message = "ثبت ناموفق",
                Model = result,
                Status = false
            };

        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetHameshIdByFileId(int fileId)
        {
            return _context.Hameshes.Where(h => h.FileId == fileId)
                .OrderByDescending(h => h.RegDate).ThenByDescending(h => h.Id)
                .Select(h => h.ParentId).FirstOrDefault();
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int GetHameshIdByUseerIdAndFileId(int userId, int fileId)
        {
            var hameshId = _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId)
                .OrderByDescending(h => h.RegDate).ThenByDescending(h => h.Id)
                .Select(h => h.Id).FirstOrDefault();
            return hameshId;
        }
        /// <summary>
        /// هامش کسی که لاگین کرده برای درخواست نفر
        /// </summary>
        /// <param name="userId">کسی که لایگن کرده</param>
        /// <param name="fileId"></param>
        /// <returns></returns>

        public Hamesh GetHameshByUserIdAndFileId(int userId, int fileId)
        {
            return _context.Hameshes
                .Where(h => h.UserId == userId && h.FileId == fileId)
                .OrderByDescending(h => h.RegDate).ThenByDescending(h => h.Id).FirstOrDefault();
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Hamesh GetHameshByUserIdAndFileId2(int userId, int? fileId)
        {
            return _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId)
                .OrderByDescending(h => h.RegDate).ThenByDescending(h => h.Id).FirstOrDefault();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetParentIdHameshByUserIdAndFileId(int userId, int fileId)
        {
            return _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId)
                .OrderByDescending(h => h.RegDate).ThenByDescending(h => h.Id)
                .Select(h => h.ParentId).FirstOrDefault();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Users> GetUserByParentId(int? parentId)
        {
            return _context.Hameshes.Include(u => u.User).Where(h => h.Id == parentId).Select(u => u.User).ToList();
        }

        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public BaseResult UpdateHamesh(Hamesh hamesh)
        {
            _context.Update(hamesh);
            var res = _context.SaveChanges();
            if (res == 1)
            {
                return new BaseResult
                {
                    Message = "ویرایش هام با موفقیت انجام شد",
                    Model = hamesh,
                    Status = true
                };
            }
            return new BaseResult
            {
                Message = "ویرایش هام با خطا مواجه شد",
                Model = hamesh,
                Status = false
            };
        }


        //meetingid => fileId
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ListHameshViewModel GetHameshIdByFileId(int fileId, int pageId = 1, int requestsubject = 0, string filterCaption = "")
        {
            int fileid = _context.Files.Where(f => f.Id == fileId).Select(f => f.Id).SingleOrDefault();
            int? meetingId = _context.Files.Where(f => f.Id == fileId).Select(f => f.MeetingId).SingleOrDefault();

            IQueryable<Hamesh> result = _context.Hameshes
                .Where(r => r.FileId == fileId && r.UserDesc != null && r.UserDesc.Trim() != "")
                .OrderBy(x => x.RegDate).ThenBy(x => x.Id);

            var take = 1000;
            var skip = (pageId - 1) * take;

            ListHameshViewModel list = new ListHameshViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
            list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);  // result.Count() / take;

            list.MeetingId = meetingId;

            list.hameshes = result.Select(t => new HameshInfoViewModel()
            {
                Id = t.Id,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                RankTitle = t.File.Personal.RankTitle,
                ActionTypeTitle = t.ActionType.Title,
                UserDesc = t.UserDesc,
                RegDate = t.RegDate,
                FirstNamePersonel = t.File.Personal.FirstName,
                LastNamePersonel = t.File.Personal.LastName,
                RankTitlePersonal = t.File.Personal.RankTitle,
                FarmandehPersonalName = t.File.FarmandehPersonalName,
                RcvrUserName = t.User.FirstName + " " + t.User.LastName + "**" + t.User.UserName,

            }).Skip(skip).Take(take).ToList();
            return list;
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ListHameshViewModel GetHameshIdByFileId2(int fileId, int pageId = 1, int requestsubject = 0, string filterCaption = "")
        {
            const int take = 1000; // تعداد نتایج در هر صفحه
            var skip = (pageId - 1) * take; // محاسبه تعداد نتایج که باید رد شوند

            try
            {
                // دریافت فایل و جلسه مرتبط با آن
                var fileDetails = _context.Files
                    .Where(f => f.Id == fileId)
                    .Select(f => new
                    {
                        f.Id,
                        f.MeetingId,
                        Hameshes = _context.Hameshes
                            .Where(r => r.FileId == fileId && r.UserDesc != null && r.UserDesc.Trim() != "")
                            .OrderBy(x => x.RegDate)
                            .ToList() // تبدیل به لیست
                    })
                    .FirstOrDefault();

                // بررسی وجود فایل
                if (fileDetails == null)
                {
                    return new ListHameshViewModel
                    {
                        CurrentPage = pageId,
                        skip = skip,
                        count = 0,
                        PageCount = 0,
                        MeetingId = null,
                        hameshes = new List<HameshInfoViewModel>(),
                        ErrorMessage = "فایل مورد نظر یافت نشد."
                    };
                }

                // محاسبه تعداد کل Hameshها
                var totalCount = fileDetails.Hameshes.Count();

                // ایجاد و پر کردن مدل نتیجه
                var list = new ListHameshViewModel
                {
                    CurrentPage = pageId,
                    skip = skip,
                    count = totalCount,
                    PageCount = (int)Math.Ceiling(totalCount / (double)take),
                    MeetingId = fileDetails.MeetingId,
                    hameshes = fileDetails.Hameshes.Select(t => new HameshInfoViewModel
                    {
                        Id = t.Id,
                        FirstName = t.User?.FirstName ?? "نام کاربر موجود نیست.", // کنترل null و پیام مناسب
                        LastName = t.User?.LastName ?? "نام خانوادگی کاربر موجود نیست.", // کنترل null و پیام مناسب
                        RankTitle = t.File?.Personal?.RankTitle ?? "رتبه موجود نیست.", // کنترل null و پیام مناسب
                        ActionTypeTitle = t.ActionType?.Title ?? "نوع عمل موجود نیست.", // کنترل null و پیام مناسب
                        UserDesc = t.UserDesc ?? "توضیحات کاربر موجود نیست.", // کنترل null و پیام مناسب
                        RegDate = t.RegDate,
                        FirstNamePersonel = t.File?.Personal?.FirstName ?? "نام پرسنل موجود نیست.", // کنترل null و پیام مناسب
                        LastNamePersonel = t.File?.Personal?.LastName ?? "نام خانوادگی پرسنل موجود نیست.", // کنترل null و پیام مناسب
                        RankTitlePersonal = t.File?.Personal?.RankTitle ?? "رتبه پرسنل موجود نیست.", // کنترل null و پیام مناسب
                        FarmandehPersonalName = t.File?.FarmandehPersonalName ?? "نام فرمانده موجود نیست.", // کنترل null و پیام مناسب
                        RcvrUserName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}**{t.User.UserName}" : "نام کاربری موجود نیست." // کنترل null و پیام مناسب


                    })
                    .Skip(skip)
                    .Take(take)
                    .ToList(),
                    ErrorMessage = null // عدم وجود خطا
                };

                return list;
            }
            catch (Exception ex) // کنترل خطا
            {
                // در اینجا می‌توانید خطا را در لاگ ثبت کنید
                // مثلاً: _logger.LogError(ex, "خطایی در دریافت Hamesh ها به وجود آمد.");

                return new ListHameshViewModel
                {
                    CurrentPage = pageId,
                    skip = skip,
                    count = 0,
                    PageCount = 0,
                    MeetingId = null,
                    hameshes = new List<HameshInfoViewModel>(),
                    ErrorMessage = "خطایی در پردازش درخواست به وجود آمد. لطفاً دوباره تلاش کنید."
                };
            }
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int? GetMeetingIdByFileId(int fileId)
        {
            return _context.Files.Where(f => f.Id == fileId).Select(f => f.MeetingId).SingleOrDefault();
        }



        /// <summary>
        /// آخرین هامش ثبت شده
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Hamesh GetPervHameshForFRadeBalatar(int fileId)
        {
            return _context.Hameshes
                .Where(item => item.FileId == fileId && item.UserDesc != null && item.UserDesc.Trim() != "")
                .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                .FirstOrDefault();
        }

        //get Perv Hamesh
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Hamesh> GetHameshMoavenatForFRadeBalatar(int fileId)
        {
            var result = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                .Where(x => x.FileId == fileId && (x.RoleTypeId == SystemRoleTypes.UnitCommand || x.RoleTypeId == SystemRoleTypes.DeputyOffice) && x.UserDesc != null && x.UserDesc.Trim() != "")
                .Distinct().ToList();
            // return _context.Hameshes.Where(x => x.FileId == fileId).Include(x => x.User).ThenInclude(x => x.UserRoles).Where(y=>y.).Where(x=>x.);
            return result;
            //داخل اینکلود میشه ور گزاشت
        }
        #region  Hamesh For Stimul
        //farmandeh Unit Duty
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetHameshFUnitDuty(int fileId)
        {
            //var result1 = _context.Hameshes.Include(x => x.User)
            //    .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x=>x.RoleTypeFinal)
            //    .Where(x => x.FileId == fileId && x.RoleTypeFinalId == 2 && x.UserDesc != "")
            //    .Select(x => x.UserDesc).SingleOrDefault();

            var result = _context.Hameshes.Include(x => x.User)
    .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
    .Where(x => x.FileId == fileId && x.RoleTypeFinalId == SystemRoleFinalTypes.UnitCommander && x.UserDesc != null && x.UserDesc.Trim() != "").OrderBy(x => x.RegDate)
    .Select(x => x.UserDesc).LastOrDefault();


            return result;
        }
        //farmandeh Unit 
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetHameshFUnit(int fileId)
        {
            var res = _context.Hameshes.Include(x => x.User)
                .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
                .Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.UnitCommanderLegacy)
                .OrderBy(x => x.RegDate)
                .Select(x => x.UserDesc + "\n" + x.User.RankTitle + " " + x.User.FirstName + " " + x.User.LastName + "\n" + x.RegDate.ToShamsi())
                .LastOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                return res = "! این درخواست ملاقات در قرارگاه ثبت شده است";
            }

            return res;
        }
        //farmandeh Gharargah 
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetHameshFGharargah(int fileId)
        {
            var res = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles)
                .ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
                .Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.GharargahCommanderLegacy && x.UserDesc != null && x.UserDesc.Trim() != "")
                .OrderBy(x => x.RegDate)
                .Select(x => x.UserDesc + "\n" + x.User.RankTitle + " " + x.User.FirstName + " " + x.User.LastName + "\n" + x.RegDate.ToShamsi()).LastOrDefault();
            if (string.IsNullOrEmpty(res))
            {
                return res = "! این پرونده در ستاد نیروی زمینی میباشد";
            }
            return res;
        }
        #endregion

        /// <summary>
        /// سمت اصلی کاربر را برای ثبت هامش و ساخت Claimهای ورود دریافت می‌کند.
        /// در صورت نداشتن نقش فعال، مقدار null برگردانده می‌شود تا ورود کاربر با پیام مناسب متوقف شود.
        /// </summary>
        public HameshInfoViewModel GetRoleTypePerson(int userId)
        {
            if (userId <= 0) return null;

            return _context.UserRoles.AsNoTracking()
                .Where(userRole => userRole.UserId == userId && !userRole.Role.IsDelete)
                .OrderBy(userRole => userRole.Role.SortNum)
                .ThenBy(userRole => userRole.UR_Id)
                .Select(userRole => new HameshInfoViewModel
                {
                    RoleTypeId = userRole.Role.RoleType,
                    RoleTypeTitle = userRole.Role.Title,
                    RoleTypeIdFinal = userRole.Role.RoleTypeFinalId,
                    RoleTypeTitleFinal = userRole.Role.RoleTypeFinal == null ? null : userRole.Role.RoleTypeFinal.Title
                })
                .FirstOrDefault();
        }

        //get all hamesh
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public List<HameshInfoViewModel> getAllHameshWithOutMoavenat(int fileId)
        {
            // آرایه محلی باعث می‌شود EF Core شرط Contains را مستقیماً به IN در SQL تبدیل کند.
            var excludedRoleTypes = SystemRoleTypes.GetMainWorkflowTypes().ToArray();
            var listhamesh = _context.Hameshes.Include(x => x.File)
                .Where(x => x.FileId == fileId && !excludedRoleTypes.Contains(x.RoleTypeId) && x.UserDesc != null && x.UserDesc.Trim() != "")
                .Select(x => new HameshInfoViewModel
                {
                    FirstName = x.User.RankTitle + " " + x.User.FirstName + " " + x.User.LastName,
                    PhoneSelseleMaratebYeganNafar = x.File.Phone,
                    UserDesc = x.UserDesc,
                    RoleTypeTitle = x.RoleTypeTitle,
                    RegDate = x.RegDate
                }).Distinct().OrderBy(x => x.RegDate).ToList();

            return listhamesh;
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetlastHameshKarshenasgharagahAnsarNezaja(int fileId)
        {
            string result = _context.Hameshes.Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.AnsarHeadquartersExpert && x.UserDesc != null && x.UserDesc.Trim() != "")
               .OrderBy(x => x.RegDate).Select(x => x.UserDesc).LastOrDefault();

            if (result == null)
            {
                return "نظریه ای یافت نشد";
            }

            return result;

        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetlastHameshKarbarNezaja(int fileId)
        {
            var result = "";
            try
            {
                result = _context.Hameshes.Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.NezajaOperator && x.UserDesc != null && x.UserDesc.Trim() != "")
                    .OrderByDescending(x => x.RegDate).ThenByDescending(x => x.Id).Select(x => x.UserDesc).FirstOrDefault();

                if (result == null)
                {
                    return "";
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {

                if (result == null)
                {
                    return "";
                }
                else
                {
                    return result;
                }
            }
        }


        /// <summary>
        ///  گرفتن تمام اطلاعات مربوط به نفر برای هامش
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>

        public HameshFullInfoFileViewModel GetFullInfoFile(int fileId, int userId)
        {
            var file = _context.Files.Include(x => x.RequestSubject)
                                      .FirstOrDefault(x => x.Id == fileId);

            var listAttachmnetDastor = _context.FileAttachments
                                               .Where(x => x.FileId == fileId)
                                               .Select(x => x.FileUplodeAttacmentDastor)
                                               .ToList();

            var hamesh = new HameshFullInfoFileViewModel
            {
                file = new FactPersonalViewModel()
            };

            var hameshUserLogin = GetHameshByUserIdAndFileId(userId, fileId) ?? new Hamesh();

            if (file != null)
            {
                hamesh.file.Id = file.Id;
                hamesh.file.ReqSubTitle = file.RequestSubject?.Title ?? "";
                hamesh.file.ProblemDescription = file.ProblemDescription ?? "";
                hamesh.file.RequestDescription = file.RequestDescription ?? "";
                hamesh.file.FishAttachmentFileName = file.FishAttachment ?? "";
                hamesh.file.attachDastor = file.AttachDastor ?? "";
                hamesh.file.AttachmentFileName = file.Attachment ?? "";

                var hameshKarshenasGharagahAnsarNezaja = GetlastHameshKarshenasgharagahAnsarNezaja(fileId);
                hamesh.hameshKarshenasGharagahAnsarNezaja = hameshKarshenasGharagahAnsarNezaja ?? "";

                var hameshAllYegan = getAllHameshWithOutMoavenat(fileId);
                hamesh.hameshAllYegan = hameshAllYegan ?? new List<HameshInfoViewModel>();

                var hameshMoavenats = getAllHameshMoavenat(fileId);
                hamesh.HameshMoavenats = hameshMoavenats ?? new List<HameshInfoViewModel>();

                var hameshKarbarNezaja = GetlastHameshKarbarNezaja(fileId);
                hamesh.HameshKarbarNezaja = hameshKarbarNezaja ?? "";

                var pervHamesh = GetPervHameshForFRadeBalatar(fileId);
                hamesh.PervHamesh = pervHamesh?.UserDesc ?? "";

                hamesh.HameshUserLogin = hameshUserLogin.UserDesc ?? "";
                hamesh.ActionTypeIdUserLogin = hameshUserLogin.ActionTypeId;

                var hameshHeiatReeise = GetHameshHeiatReeiseByUserIdAndFileId(userId, fileId);
                hamesh.hameshHeiatReeise = hameshHeiatReeise ?? "";

                var listVam = _vamService.getAllVamWithFileId(fileId);
                hamesh.ListVam = listVam ?? new List<VamViewModel>();

                hamesh.fishAttacmentFileName = file.FishAttachment ?? "";
                hamesh.SumMablaghVamDarkhasti = file.SumMablaghVamDarkhasti ?? 0;
                hamesh.MablaghVamMohaghaghSode = file.MablaghVamMohaghaghSode ?? 0;
            }
            else
            {
                throw new Exception("File not found");
            }



            hamesh.AttachDastorName = listAttachmnetDastor ?? new List<string>();

            return hamesh;
        }



        /// <summary>
        /// تمام هامش های معاونت ها
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public List<HameshInfoViewModel> getAllHameshMoavenat(int fileId)
        {
            var listhamesh = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                .Where(x => x.FileId == fileId && (x.RoleTypeId == SystemRoleTypes.UnitCommand || x.RoleTypeId == SystemRoleTypes.DeputyOffice) && x.UserDesc != null && x.UserDesc.Trim() != "")
                .Select(x => new HameshInfoViewModel
                {

                    UserDesc = x.UserDesc,
                    RoleTypeId = x.RoleTypeId,
                    RoleTypeTitle = x.RoleTypeTitle,
                    RegDate = x.RegDate
                }).Distinct().ToList();

            return listhamesh;
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetHameshHeiatReeiseByUserIdAndFileId(int userId, int fileId)
        {
            //کسی که داره میفرسته از جدول کارتابل رو میگیره تا بتونیم هامش نفر قبلی رو از روش پیدا کنیم
            var hamesh = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                            .Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.PresidingBoard && x.UserDesc != null && x.UserDesc.Trim() != "")
                            .OrderByDescending(x => x.RegDate).ThenByDescending(x => x.Id).Select(x => x.UserDesc).FirstOrDefault();

            return hamesh;
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public HameshFullInfoFileViewModel GetFullInfoFileForOnlineConversation(int fileId, int userId)
        {
            var file = _context.Files.Include(x => x.RequestSubject).Where(x => x.Id == fileId).FirstOrDefault();

            if (file == null)
                throw new InvalidOperationException("درخواست ملاقات یافت نشد.");

            var listAttachmnetDastor = _context.FileAttachments.Where(x => x.FileId == fileId).ToList();

            var hamesh = new HameshFullInfoFileViewModel();

            hamesh.file = new FactPersonalViewModel();

            hamesh.file.Id = file.Id;
            hamesh.file.ReqSubTitle = file.RequestSubject?.Title ?? "";
            hamesh.file.ProblemDescription = file.ProblemDescription ?? "";
            hamesh.file.RequestDescription = file.RequestDescription ?? "";
            hamesh.file.FishAttachmentFileName = file.FishAttachment ?? "";
            hamesh.file.attachDastor = file.AttachDastor ?? "";
            hamesh.file.AttachmentFileName = file.Attachment ?? "";
            hamesh.hameshKarshenasGharagahAnsarNezaja = GetlastHameshKarshenasgharagahAnsarNezaja(fileId);
            hamesh.hameshAllYegan = getAllHameshWithOutMoavenat(fileId);
            hamesh.HameshMoavenats = getAllHameshMoavenat(fileId);
            hamesh.HameshKarbarNezaja = GetlastHameshKarbarNezaja(fileId);
            hamesh.PervHamesh = GetPervHameshForFRadeBalatar(fileId)?.UserDesc ?? "";
            // hamesh.HameshUserLogin = GetHameshByUserIdAndFileId(userId, fileId).UserDesc;
            // hamesh.ActionTypeIdUserLogin = GetHameshByUserIdAndFileId(userId, fileId).ActionTypeId;
            hamesh.hameshHeiatReeise = GetHameshHeiatReeiseByUserIdAndFileId(userId, fileId);
            hamesh.ListVam = _vamService.getAllVamWithFileId(fileId);
            hamesh.fishAttacmentFileName = file.FishAttachment;
            hamesh.SumMablaghVamDarkhasti = file.SumMablaghVamDarkhasti;
            hamesh.MablaghVamMohaghaghSode = file.MablaghVamMohaghaghSode;

            if (listAttachmnetDastor != null)
            {
                hamesh.AttachDastorName = new List<string>();
                foreach (var item in listAttachmnetDastor)
                {
                    var listFileAttachmentDastor = item.FileUplodeAttacmentDastor;

                    hamesh.AttachDastorName.Add(item.FileUplodeAttacmentDastor);
                }


            }

            return hamesh;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public string GetFirstHameshKarshenasgharagahAnsarNezaja(int fileId)
        {
            string result = _context.Hameshes
                .Where(x => x.FileId == fileId && x.RoleTypeId == SystemRoleTypes.AnsarHeadquartersExpert && x.UserDesc != null && x.UserDesc.Trim() != "")
                   .OrderByDescending(x => x.RegDate)
                   .Select(x => x.UserDesc)
                   .FirstOrDefault();

            if (result == null)
            {
                return "نظریه ای یافت نشد";
            }

            return result;
        }

        //public BaseResult RegHameshMain(int actionTypeId, int roleTypeId, string roleTypeTitle, string userDesc, int userId, int fileId, long? mablaghVamDarkhasti, long? mablaghVamMohaghaghShode, List<int> rcvrUserId)
        //{
        //    try
        //    {
        //        //عملیات ویرایش هامش
        //        EditHamesh(actionTypeId, roleTypeId, roleTypeTitle, userDesc, userId, fileId, mablaghVamDarkhasti, mablaghVamMohaghaghShode);

        //        //عملیات ویرایش فایل
        //        _fileService.EditFileWhenSendHamesh(fileId, actionTypeId, mablaghVamDarkhasti, mablaghVamMohaghaghShode, roleTypeId);



        //        //حذف از کارتابل نفرات قدیم

        //        var cartable = _context.Cartables
        //                                 .Where(x => x.FileId == fileId && x.RcvrUserId == userId && x.IsDone == false)
        //                                 .FirstOrDefault();

        //        _context.Remove(cartable);

        //        //ارسال به کارتابل
        //        #region ارسال به کارتابل
        //        var file = _context.Files.Where(f => f.Id == fileId).FirstOrDefault();


        //        foreach (int rcvrId in rcvrUserId)
        //        {
        //            _context.Cartables.Add(new Cartable()
        //            {
        //                RcvrUserId = rcvrId,
        //                SndrUserId = userId,
        //                FileId = fileId,
        //                StateCd = 0,
        //                IsView = false,
        //                IsDone = false,
        //                RegDate = file.RegDate
        //            });
        //        }
        //        #endregion

        //        var result = _context.SaveChanges();

        //        if (result > 0)
        //        {
        //            return new BaseResult()
        //            {
        //                Model = "",
        //                Message = "عملیات با موفقیت انجام شد",
        //                Status = true
        //            };
        //        }



        //        return new BaseResult()
        //        {
        //            Model = "",
        //            Message = "عملیات با موفقیت خطا مواجه شد",
        //            Status = false
        //        };

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //}

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult RegHamesh(int actionTypeId, int roleTypeId, string roleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, string userDesc, int userId, int fileId, double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode, List<int> rcvrUserId)
        {
            var receiverIds = (rcvrUserId ?? new List<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (actionTypeId <= 0)
                return new BaseResult(false, "نوع اقدام انتخاب نشده است.");
            if (string.IsNullOrWhiteSpace(userDesc))
                return new BaseResult(false, "متن نظریه/هامش الزامی است.");
            if (userId <= 0 || fileId <= 0)
                return new BaseResult(false, "اطلاعات کاربر یا درخواست معتبر نیست.");
            if (!receiverIds.Any())
                return new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");

            var validReceiverIds = _context.Users
                .Where(user => receiverIds.Contains(user.Id) && user.IsActive && !user.IsDelete)
                .Select(user => user.Id)
                .ToList();
            if (validReceiverIds.Count != receiverIds.Count)
                return new BaseResult(false, "یک یا چند گیرنده یافت نشد یا حساب آن‌ها غیرفعال است.");

            if (!_context.ActionTypes.Any(item => item.Id == actionTypeId))
                return new BaseResult(false, "نوع اقدام انتخاب‌شده معتبر نیست.");
            if (string.IsNullOrWhiteSpace(roleTypeTitle))
                return new BaseResult(false, "اطلاعات نقش ثبت‌کننده کامل نیست.");
            if (userDesc.Trim().Length > 4000)
                return new BaseResult(false, "متن هامش نمی‌تواند بیشتر از ۴۰۰۰ کاراکتر باشد.");

            using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var file = _context.Files.FirstOrDefault(item => item.Id == fileId);
                    if (file == null) return new BaseResult(false, "درخواست ملاقات یافت نشد.");

                    var senderHasCartable = _context.Cartables.Any(item =>
                        item.FileId == fileId && item.RcvrUserId == userId && !item.IsDone);
                    if (!senderHasCartable)
                        return new BaseResult(false, "این درخواست در کارتابل شما نیست یا قبلاً منتقل شده است.");

                    var latestCompletedHameshId = _context.Hameshes
                        .Where(item => item.FileId == fileId && item.UserDesc != null && item.UserDesc.Trim() != "")
                        .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                        .Select(item => (int?)item.Id).FirstOrDefault();

                    // داده‌های قدیمی ممکن است یک هامش خالی انتظار اقدام داشته باشند؛ همان رکورد تکمیل می‌شود.
                    var pendingHamesh = _context.Hameshes
                        .Where(item => item.FileId == fileId && item.UserId == userId &&
                                       (item.UserDesc == null || item.UserDesc.Trim() == ""))
                        .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                        .FirstOrDefault();

                    var completedHamesh = pendingHamesh ?? new Hamesh { FileId = fileId, UserId = userId };
                    completedHamesh.ActionTypeId = actionTypeId;
                    completedHamesh.ParentId = pendingHamesh == null ? latestCompletedHameshId : pendingHamesh.ParentId;
                    completedHamesh.UserDesc = userDesc.Trim();
                    completedHamesh.RoleTypeId = roleTypeId;
                    completedHamesh.RoleTypeTitle = roleTypeTitle ?? "نامشخص";
                    completedHamesh.RoleTypeFinalId = roleTypeIdFinal;
                    completedHamesh.RoleTypeFinalTitle = string.IsNullOrWhiteSpace(roleTypeTitleFinal) ? "نامشخص" : roleTypeTitleFinal;
                    completedHamesh.RegDate = DateTime.Now;
                    completedHamesh.MablaghVamDarkhasti = mablaghVamDarkhasti;
                    completedHamesh.MablaghVamMohaghaghSode = mablaghVamMohaghaghShode;
                    if (pendingHamesh == null) _context.Hameshes.Add(completedHamesh);

                    // عملیات ویرایش فایل
                    var resFile = _fileService.EditFileWhenSendHamesh(fileId, actionTypeId, mablaghVamDarkhasti, mablaghVamMohaghaghShode, roleTypeId);
                    if (resFile == null || !resFile.Status)
                        return new BaseResult(false, resFile?.Message ?? "به‌روزرسانی وضعیت درخواست انجام نشد.");

                    // تمام مالکیت‌های جاری بسته می‌شوند تا درخواست فقط دست گیرندگان جدید باشد.
                    var activeCartables = _context.Cartables.Where(item => item.FileId == fileId && !item.IsDone).ToList();
                    foreach (var activeCartable in activeCartables) activeCartable.IsDone = true;

                    foreach (var rcvrId in validReceiverIds)
                    {
                        _context.Cartables.Add(new Cartable()
                        {
                            RcvrUserId = rcvrId,
                            SndrUserId = userId,
                            FileId = fileId,
                            StateCd = 0,
                            IsView = false,
                            IsDone = false,
                            RegDate = DateTime.Now
                        });
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return new BaseResult(true, "هامش ثبت شد و درخواست به کارتابل گیرنده منتقل گردید.");


                }
                catch (Exception)
                {
                    // در صورت بروز خطا، تراکنش را لغو می‌کنیم
                    transaction.Rollback();

                    return new BaseResult()
                    {
                        Model = "",
                        Message = "ثبت هامش انجام نشد؛ اطلاعات قبلی بدون تغییر باقی ماند.",
                        Status = false
                    };
                }
            }
        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult RegFileAndAddToCartableAndRegHamesh0(FactPersonalViewModel model, int userId, int roleTypeId, string roleTypeTitle, int roleTypeFinalId, string roleTypeFinalTitle)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var resFile = _fileService.AddFile(model);

                    var resCartable = _cartableService.AddToCartable(userId, resFile.Model, DateTime.Now);

                    var resHamesh = AddToHameshWhenCreateFile(userId, resFile.Model, roleTypeId, roleTypeTitle, roleTypeFinalId, roleTypeFinalTitle);

                    transaction.Commit();

                }
                catch (Exception ex)
                {

                    // در صورت بروز خطا، تراکنش را لغو می‌کنیم
                    transaction.Rollback();

                    return new BaseResult()
                    {
                        Model = "",
                        Message = $"خطایی رخ داد: {ex.Message}",
                        Status = false
                    };
                }

                return new BaseResult()
                {
                    Model = "",
                    Message = "ثبت موفق",
                    Status = true
                };
            }
        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult RegFileAndAddToCartableAndRegHamesh(
    FactPersonalViewModel model,
    int userId,
    int roleTypeId,
    string roleTypeTitle,
    int roleTypeFinalId,
    string roleTypeFinalTitle)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var resFile = _fileService.AddFile(model);

                    if (resFile == null || !resFile.Status || resFile.Model == null)
                    {
                        transaction.Rollback();

                        return new BaseResult
                        {
                            Status = false,
                            Message = resFile?.Message ?? "ثبت درخواست ملاقات ناموفق بود.",
                            Model = null
                        };
                    }

                    var fileId = Convert.ToInt32(resFile.Model);

                    if (fileId <= 0)
                    {
                        transaction.Rollback();

                        return new BaseResult
                        {
                            Status = false,
                            Message = "شناسه درخواست ملاقات معتبر نیست.",
                            Model = null
                        };
                    }

                    var resCartable = _cartableService.AddToCartable(userId, fileId, DateTime.Now);

                    if (resCartable == null || !resCartable.Status)
                    {
                        transaction.Rollback();

                        return new BaseResult
                        {
                            Status = false,
                            Message = resCartable?.Message ?? "ثبت کارتابل ناموفق بود.",
                            Model = null
                        };
                    }

                    var resHamesh = AddToHameshWhenCreateFile(
                        userId,
                        fileId,
                        roleTypeId,
                        roleTypeTitle,
                        roleTypeFinalId,
                        roleTypeFinalTitle
                    );

                    if (resHamesh == null || !resHamesh.Status)
                    {
                        transaction.Rollback();

                        return new BaseResult
                        {
                            Status = false,
                            Message = resHamesh?.Message ?? "ثبت هامش ناموفق بود.",
                            Model = null
                        };
                    }

                    transaction.Commit();

                    return new BaseResult
                    {
                        Status = true,
                        Message = "ثبت درخواست ملاقات با موفقیت انجام شد.",
                        Model = fileId
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return new BaseResult
                    {
                        Status = false,
                        Message = $"خطایی رخ داد: {ex.Message}",
                        Model = null
                    };
                }
            }
        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult RegHameshHeiatRaeise(int fileId, HameshFullInfoFileViewModel hamesh, int roleTypeId, string roleTypeTitle, int roleTypeFinalId, string roleTypeFinalTitle, int userId)
        {
            var file = _fileService.GetFile(fileId);

            if (file == null) return new BaseResult(false, "درخواست ملاقات یافت نشد.");
            if (hamesh == null) return new BaseResult(false, "اطلاعات هامش ارسال نشده است.");
            if (hamesh.ActionTypeId <= 0 || string.IsNullOrWhiteSpace(hamesh.UserDesc))
                return new BaseResult(false, "نوع اقدام و متن هامش الزامی است.");
            if (hamesh.RcvrId == null || !hamesh.RcvrId.Any())
                return new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");

            try
            {
                var resultEnableMeetingHold = _fileService.ActiveFiledMettingHoldFile(file.Id);
                if (resultEnableMeetingHold == null || !resultEnableMeetingHold.Status)
                    return new BaseResult(false, resultEnableMeetingHold?.Message ?? "فعال‌سازی درخواست جلسه انجام نشد.");

                if (hamesh.VoiceRecord != null)
                {
                    var voiceResult = _fileService.AddVoiceRecordToFile(hamesh.VoiceRecord, fileId);
                    if (voiceResult == null || !voiceResult.Status)
                        return new BaseResult(false, voiceResult?.Message ?? "ذخیره فایل صوتی انجام نشد.");

                }

                if (hamesh.AttachDastors != null)
                {
                    //add AttachDastor TO Table File
                    var attachmentResult = _fileService.AddListAttachDastorToFile(hamesh.AttachDastors, fileId);
                    if (attachmentResult == null || !attachmentResult.Status)
                        return new BaseResult(false, attachmentResult?.Message ?? "ذخیره پیوست‌ها انجام نشد.");
                }

                var hameshResult = RegHamesh(hamesh.ActionTypeId, roleTypeId, roleTypeTitle, roleTypeFinalId,
                    roleTypeFinalTitle, hamesh.UserDesc, userId, fileId, hamesh.SumMablaghVamDarkhasti, 0, hamesh.RcvrId);
                if (hameshResult == null || !hameshResult.Status)
                    return new BaseResult(false, hameshResult?.Message ?? "ثبت هامش انجام نشد.");


                if (hamesh.SumMablaghVamDarkhasti != null)
                {
                    var amountResult = _fileService.addMablaghVamDarkhastiVaVamMohaghahShode(fileId, hamesh.SumMablaghVamDarkhasti, 0);
                    if (amountResult == null || !amountResult.Status)
                        return new BaseResult(false, amountResult?.Message ?? "ثبت مبلغ وام انجام نشد.");

                }




                return new BaseResult
                {
                    Message = "هامش جلسه با موفقیت ثبت و به کارتابل مقصد ارسال شد.",
                    Status = true
                };
            }

            catch (Exception ex)
            {


                return new BaseResult
                {
                    Message = $"ثبت هامش جلسه انجام نشد: {ex.Message}",
                    Status = false
                };
            }
        }
    }
}
