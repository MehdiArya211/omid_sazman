using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ActionType> GetActionType()
        {
            return _context.ActionTypes.ToList();
        }

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
        public BaseResult EditHamesh(int actionTypeId, int roleTypeId, string roleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal,
            string userDesc, int userId, int fileId, double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode)
        {
            var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);

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



        public void EditHameshForMeetingViewModel(int actionTypeId, MeetingHoldViewModel meetingHoldViewModel, int userId, int fileId)
        {
            var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);

            editHamesh.ActionTypeId = actionTypeId;
            editHamesh.UserDesc = meetingHoldViewModel.UserDesc;
            editHamesh.RegDate = DateTime.Now;
            UpdateHamesh(editHamesh);
        }
        /// <summary>
        /// زمانیکه یه درخواست ملاقات ثبت میشه باید یه رکورد خالی هم تو هامش بخوره با شناسه نفری که لاگین کرده
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="FileId"></param>
        /// <param name="RoleTypeId"></param>
        /// <param name="RoleTypeTitle"></param>
        /// <returns></returns>
        public BaseResult AddToHameshWhenCreateFile(int userId, int FileId, int RoleTypeId, string RoleTypeTitle, int RoleTypeIdFinal, string RoleTypeTitleFinal)
        {
            var result = new Hamesh()
            {
                FileId = FileId,
                UserId = userId,
                ActionTypeId = 1002,
                ParentId = null,
                RoleTypeId = RoleTypeId,
                RoleTypeTitle = RoleTypeTitle,
                RoleTypeFinalId = RoleTypeIdFinal,
                RoleTypeFinalTitle = RoleTypeTitleFinal,
                UserDesc = "",
                RegDate = DateTime.Now,

            };

            return AddHamesh(result);
        }


        public void AddToHameshWhenSendListFileToFarmandehiNezaja(List<int> rcvrUserId, List<Files> files, int RoleTypeId, string RoleTypeTitle, int roleTypeIdFinal, string roleTypeTitleFinal, int userId)
        {
            foreach (var file in files)
            {
                foreach (var rcvr in rcvrUserId)
                {
                    int hameshId = _context.Hameshes.Where(c => c.UserId == userId && c.FileId == file.Id)
                        .OrderBy(x => x.Id)
                        .Select(c => c.Id)
                        .LastOrDefault();

                    var result = new Hamesh()
                    {
                        FileId = file.Id,
                        UserId = rcvr,
                        ActionTypeId = 1002,
                        ParentId = hameshId,
                        RoleTypeId = RoleTypeId,
                        RoleTypeTitle = RoleTypeTitle,
                        RoleTypeFinalId = roleTypeIdFinal,
                        RoleTypeFinalTitle = roleTypeTitleFinal,
                        UserDesc = "",
                        RegDate = DateTime.Now,

                    };

                    AddHamesh(result);
                }
            }

        }
        public BaseResult AddToHameshWhenSendFileToCartable(int userId, int fileId, List<int> rcvrUserId, int RoleTypeId, string RoleTypeTitle, int RoleTypeIdFinal, string RoleTypeTitleFinal)
        {
            var hamesh = _context.Hameshes
                .Where(c => c.UserId == userId && c.FileId == fileId)
                .OrderByDescending(x => x.RegDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();

            if (hamesh == null)
            {
                return new BaseResult(false, "هامش مبدأ برای ایجاد گردش جدید یافت نشد.");
            }

            var receiverIds = (rcvrUserId ?? new List<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (!receiverIds.Any())
            {
                return new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");
            }

            foreach (var rcvrId in receiverIds)
            {
                var hasPendingHamesh = _context.Hameshes.Any(x =>
                    x.FileId == fileId && x.UserId == rcvrId && x.UserDesc == "");
                if (hasPendingHamesh)
                {
                    continue;
                }

                _context.Hameshes.Add(new Hamesh()
                {
                    FileId = fileId,
                    UserId = rcvrId,
                    ActionTypeId = 1002,
                    ParentId = hamesh.Id,
                    RoleTypeId = RoleTypeId,
                    RoleTypeTitle = RoleTypeTitle,
                    RoleTypeFinalId = RoleTypeIdFinal,
                    RoleTypeFinalTitle = RoleTypeTitleFinal,
                    UserDesc = "",
                    RegDate = DateTime.Now,
                });

            }
            var result = _context.SaveChanges();
            if (result != 0)
            {
                return new BaseResult
                {
                    Message = "عملیات موفق",
                    Model = this,
                    Status = true,
                };
            }

            return new BaseResult
            {
                Message = "عملیات ناموفق",
                Model = this,
                Status = false,
            };

        }


        public void AddToHameshWhenSendListFileToCartable(int userId, List<Files> files, List<int> rcvrUserId, int RoleTypeId, string RoleTypeTitle)
        {
            //******************
            //var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);

            //editHamesh.ActionTypeId = actionTypeId;
            //editHamesh.UserDesc = hamesh.UserDesc;
            //editHamesh.RoleTypeId = hamesh.RoleTypeId;
            //editHamesh.RoleTypeTitle = hamesh.RoleTypeTitle;
            //editHamesh.RegDate = DateTime.Now;

            //UpdateHamesh(editHamesh);



            //******************


            foreach (int rcvrId in rcvrUserId)
            {

                foreach (var file in files)
                {
                    //آیدی هامش قبلی رو میگیرم برای پرنت هامش ها
                    int hameshId = _context.Hameshes.Where(c => c.UserId == userId && c.FileId == file.Id).Select(c => c.Id).FirstOrDefault();

                    var editHamesh = GetHameshByUserIdAndFileId(userId, file.Id);

                    editHamesh.ActionTypeId = 1;
                    editHamesh.UserDesc = "سلام علیکم ، جهت اضافه شدن به جلسه ملاقات معرفی گردید!";
                    editHamesh.RoleTypeId = RoleTypeId;
                    editHamesh.RoleTypeTitle = RoleTypeTitle;
                    editHamesh.ParentId = hameshId;
                    editHamesh.RegDate = DateTime.Now;

                    UpdateHamesh(editHamesh);
                }

                _context.SaveChanges();
            }

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

            int hameshId = _context.Hameshes.Where(c => c.UserId == userId && c.FileId == fileId).Select(c => c.Id).FirstOrDefault();


            foreach (var rcverUser in rcvrUserId)
            {
                _context.Hameshes.Add(new Hamesh()
                {
                    FileId = fileId,
                    UserId = rcverUser,
                    ActionTypeId = 1002,
                    ParentId = hameshId,
                    UserDesc = "",
                    RoleTypeId = roleTypeId,
                    RoleTypeTitle = roleTypeTitle,
                    RegDate = DateTime.Now,
                });
            }

            _context.SaveChanges();
        }




        public void AddToHameshWhenSendFileToCartableInMeetingHold(int userId, int fileId, int rcvrUserId, Hamesh hamehsViewModel)
        {
            var editHamesh = GetHameshByUserIdAndFileId(userId, fileId);
            editHamesh.FileId = fileId;
            editHamesh.UserId = rcvrUserId;
            editHamesh.ActionTypeId = hamehsViewModel.ActionTypeId;
            editHamesh.UserDesc = hamehsViewModel.UserDesc;
            editHamesh.RegDate = DateTime.Now;
            UpdateHamesh(editHamesh);
        }

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


        public int? GetHameshIdByFileId(int fileId)
        {
            return _context.Hameshes.Where(h => h.FileId == fileId).Select(h => h.ParentId).SingleOrDefault();
        }


        public int GetHameshIdByUseerIdAndFileId(int userId, int fileId)
        {
            var hameshId = _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId).Select(h => h.Id).SingleOrDefault();
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
            if (userId == 132 || userId == 3709)
            {
                var userId1 = 132;
                var userId2 = 3709;
                return _context.Hameshes
                        .Where(h => h.FileId == fileId && (h.UserId == userId1 || h.UserId == userId2))
                        .OrderBy(h => h.RegDate).LastOrDefault();
            }
            return _context.Hameshes
                .Where(h => h.UserId == userId && h.FileId == fileId)
                .OrderBy(h => h.RegDate).LastOrDefault();
        }


        public Hamesh GetHameshByUserIdAndFileId2(int userId, int? fileId)
        {
            //return _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId).SingleOrDefault();
            return _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId).OrderBy(h => h.RegDate).Last();
        }

        public int? GetParentIdHameshByUserIdAndFileId(int userId, int fileId)
        {
            return _context.Hameshes.Where(h => h.UserId == userId && h.FileId == fileId).OrderBy(h => h.RegDate).Select(h => h.ParentId).Last();
        }

        public List<Users> GetUserByParentId(int? parentId)
        {
            return _context.Hameshes.Include(u => u.User).Where(h => h.Id == parentId).Select(u => u.User).ToList();
        }

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
        public ListHameshViewModel GetHameshIdByFileId(int fileId, int pageId = 1, int requestsubject = 0, string filterCaption = "")
        {
            int fileid = _context.Files.Where(f => f.Id == fileId).Select(f => f.Id).SingleOrDefault();
            int? meetingId = _context.Files.Where(f => f.Id == fileId).Select(f => f.MeetingId).SingleOrDefault();

            IQueryable<Hamesh> result = _context.Hameshes.Where(r => r.FileId == fileId).OrderBy(x => x.RegDate);

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
                            .Where(r => r.FileId == fileId)
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
            //کسی که داره میفرسته از جدول کارتابل رو میگیره تا بتونیم هامش نفر قبلی رو از روش پیدا کنیم
            var sndrUserIdCartable = _context.Cartables.Where(x => x.FileId == fileId)
                .OrderBy(x => x.Id).Select(x => x.SndrUserId)
                .LastOrDefault();

            return _context.Hameshes.Where(x => x.FileId == fileId && x.UserId == sndrUserIdCartable)
                .OrderBy(x => x.Id)
                .LastOrDefault();
        }

        //get Perv Hamesh
        public List<Hamesh> GetHameshMoavenatForFRadeBalatar(int fileId)
        {
            var result = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                .Where(x => x.FileId == fileId && (x.RoleTypeId == 6 || x.RoleTypeId == 10) && x.UserDesc != "")
                .Distinct().ToList();
            // return _context.Hameshes.Where(x => x.FileId == fileId).Include(x => x.User).ThenInclude(x => x.UserRoles).Where(y=>y.).Where(x=>x.);
            return result;
            //داخل اینکلود میشه ور گزاشت
        }
        #region  Hamesh For Stimul
        //farmandeh Unit Duty
        public string GetHameshFUnitDuty(int fileId)
        {
            //var result1 = _context.Hameshes.Include(x => x.User)
            //    .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x=>x.RoleTypeFinal)
            //    .Where(x => x.FileId == fileId && x.RoleTypeFinalId == 2 && x.UserDesc != "")
            //    .Select(x => x.UserDesc).SingleOrDefault();

            var result = _context.Hameshes.Include(x => x.User)
    .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
    .Where(x => x.FileId == fileId && x.RoleTypeFinalId == 2 && x.UserDesc != "").OrderBy(x => x.RegDate)
    .Select(x => x.UserDesc).LastOrDefault();


            return result;
        }
        //farmandeh Unit 
        public string GetHameshFUnit(int fileId)
        {
            var res = _context.Hameshes.Include(x => x.User)
                .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
                .Where(x => x.FileId == fileId && x.RoleTypeId == 3000)
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
        public string GetHameshFGharargah(int fileId)
        {
            // var res = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).Where(x => x.FileId == fileId && x.RoleTypeId == 4 && x.UserDesc != "").Select(x => x.UserDesc).SingleOrDefault();
            var res = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles)
                .ThenInclude(x => x.Role).ThenInclude(x => x.RoleTypeFinal)
                .Where(x => x.FileId == fileId && x.RoleTypeId == 1000 && x.UserDesc != "")
                .OrderBy(x => x.RegDate)
                .Select(x => x.UserDesc + "\n" + x.User.RankTitle + " " + x.User.FirstName + " " + x.User.LastName + "\n" + x.RegDate.ToShamsi()).LastOrDefault();
            if (string.IsNullOrEmpty(res))
            {
                return res = "! این پرونده در ستاد نیروی زمینی میباشد";
            }
            return res;
        }
        #endregion

        //Get Role Type Person
        public HameshInfoViewModel GetRoleTypePerson(int userId)
        {
            var role = _context.UserRoles.Include(x => x.Role)
                .ThenInclude(r => r.RoleTypeFinal)
                .Where(x => x.UserId == userId).SingleOrDefault();


            var roleType = new HameshInfoViewModel()
            {
                RoleTypeId = role.Role.RoleType,
                RoleTypeTitle = role.Role.Title,
                RoleTypeIdFinal = role.Role.RoleTypeFinalId,
                RoleTypeTitleFinal = role.Role.RoleTypeFinal.Title,
            };

            return roleType;
        }

        //get all hamesh
        public List<HameshInfoViewModel> getAllHameshWithOutMoavenat(int fileId)
        {
            var listhamesh = _context.Hameshes.Include(x => x.File)
                .Where(x => x.FileId == fileId && x.RoleTypeId != 1 && x.RoleTypeId != 5 && x.RoleTypeId != 6 && x.RoleTypeId != 7 && x.RoleTypeId != 9 && x.RoleTypeId != 10 && x.UserDesc != "")
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


        public string GetlastHameshKarshenasgharagahAnsarNezaja(int fileId)
        {
            string result = _context.Hameshes.Where(x => x.FileId == fileId && x.RoleTypeId == 5 && x.UserDesc != "")
               .OrderBy(x => x.RegDate).Select(x => x.UserDesc).LastOrDefault();

            if (result == null)
            {
                return "نظریه ای یافت نشد";
            }

            return result;

        }


        public string GetlastHameshKarbarNezaja(int fileId)
        {
            var result = "";
            try
            {
                result = _context.Hameshes.Where(x => x.FileId == fileId && x.RoleTypeId == 7).Select(x => x.UserDesc).FirstOrDefault();

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
                .Where(x => x.FileId == fileId && (x.RoleTypeId == 6 || x.RoleTypeId == 10) && x.UserDesc != "")
                .Select(x => new HameshInfoViewModel
                {

                    UserDesc = x.UserDesc,
                    RoleTypeId = x.RoleTypeId,
                    RoleTypeTitle = x.RoleTypeTitle,
                    RegDate = x.RegDate
                }).Distinct().ToList();

            return listhamesh;
        }


        public string GetHameshHeiatReeiseByUserIdAndFileId(int userId, int fileId)
        {
            //کسی که داره میفرسته از جدول کارتابل رو میگیره تا بتونیم هامش نفر قبلی رو از روش پیدا کنیم
            var hamesh = _context.Hameshes.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                            .Where(x => x.FileId == fileId && (x.RoleTypeId == 9) && x.UserDesc != "").Select(x => x.UserDesc).FirstOrDefault();

            return hamesh;
        }


        public HameshFullInfoFileViewModel GetFullInfoFileForOnlineConversation(int fileId, int userId)
        {
            var file = _context.Files.Include(x => x.RequestSubject).Where(x => x.Id == fileId).FirstOrDefault();

            var listAttachmnetDastor = _context.FileAttachments.Where(x => x.FileId == fileId).ToList();

            var hamesh = new HameshFullInfoFileViewModel();

            hamesh.file = new FactPersonalViewModel();

            hamesh.file.Id = file.Id;
            hamesh.file.ReqSubTitle = file.RequestSubject.Title;
            hamesh.file.ProblemDescription = file.ProblemDescription;
            hamesh.file.RequestDescription = file.RequestDescription;
            hamesh.file.FishAttachmentFileName = file.FishAttachment;
            hamesh.file.attachDastor = file.AttachDastor;
            hamesh.file.AttachmentFileName = file.Attachment;
            hamesh.hameshKarshenasGharagahAnsarNezaja = GetlastHameshKarshenasgharagahAnsarNezaja(fileId);
            hamesh.hameshAllYegan = getAllHameshWithOutMoavenat(fileId);
            hamesh.HameshMoavenats = getAllHameshMoavenat(fileId);
            hamesh.HameshKarbarNezaja = GetlastHameshKarbarNezaja(fileId);
            hamesh.PervHamesh = GetPervHameshForFRadeBalatar(fileId).UserDesc;
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

        public string GetFirstHameshKarshenasgharagahAnsarNezaja(int fileId)
        {
            string result = _context.Hameshes
                .Where(x => x.FileId == fileId && x.RoleTypeId == 5 && x.UserDesc != "")
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

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // عملیات ویرایش هامش
                    var resultHamesh = EditHamesh(actionTypeId, roleTypeId, roleTypeTitle, roleTypeIdFinal, roleTypeTitleFinal, userDesc.Trim(), userId, fileId, mablaghVamDarkhasti, mablaghVamMohaghaghShode);
                    if (!resultHamesh.Status)
                    {
                        transaction.Rollback();
                        return resultHamesh;
                    }

                    // عملیات ویرایش فایل
                    var resFile = _fileService.EditFileWhenSendHamesh(fileId, actionTypeId, mablaghVamDarkhasti, mablaghVamMohaghaghShode, roleTypeId);

                    // حذف از کارتابل نفرات قدیم
                    var cartable = _context.Cartables
                                           .FirstOrDefault(x => x.FileId == fileId && x.RcvrUserId == userId && x.IsDone == false);

                    if (cartable != null)
                    {
                        _context.Remove(cartable);
                    }

                    // ارسال به کارتابل
                    var file = _context.Files.FirstOrDefault(f => f.Id == fileId);
                    if (file == null)
                    {
                        transaction.Rollback();
                        return new BaseResult(false, "درخواست ملاقات یافت نشد.");
                    }

                    foreach (var rcvrId in receiverIds)
                    {
                        _context.Cartables.Add(new Cartable()
                        {
                            RcvrUserId = rcvrId,
                            SndrUserId = userId,
                            FileId = fileId,
                            StateCd = 0,
                            IsView = false,
                            IsDone = false,
                            RegDate = file.RegDate
                        });
                    }

                    // ذخیره‌سازی تغییرات
                    var resCartable = _context.SaveChanges();

                    //هامش خالی به گیرنده
                    var resEmptyHamesh = AddToHameshWhenSendFileToCartable(userId, fileId, receiverIds, roleTypeId, roleTypeTitle, roleTypeIdFinal, roleTypeTitleFinal);
                    if (resultHamesh.Status && resFile.Status && resCartable > 0 && resEmptyHamesh.Status)
                    {
                        transaction.Commit();

                        return new BaseResult()
                        {
                            Model = "",
                            Message = "عملیات با موفقیت انجام شد",
                            Status = true
                        };
                    }

                    // در صورت بروز خطا، تراکنش را لغو می‌کنیم
                    transaction.Rollback();

                    return new BaseResult()
                    {
                        Model = "",
                        Message = "عملیات با خطا مواجه شد",
                        Status = false
                    };


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
            }
        }


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


        public BaseResult RegHameshHeiatRaeise(int fileId, HameshFullInfoFileViewModel hamesh, int roleTypeId, string roleTypeTitle, int roleTypeFinalId, string roleTypeFinalTitle, int userId)
        {
            var file = _fileService.GetFile(fileId);

            try
            {
                var resultEnableMeetingHold = _fileService.ActiveFiledMettingHoldFile(file.Id);

                if (hamesh.VoiceRecord != null)
                {
                    _fileService.AddVoiceRecordToFile(hamesh.VoiceRecord, fileId);

                }

                if (hamesh.AttachDastors != null)
                {
                    //add AttachDastor TO Table File
                    _fileService.AddListAttachDastorToFile(hamesh.AttachDastors, fileId);
                }

                RegHamesh(hamesh.ActionTypeId, roleTypeId, roleTypeTitle, roleTypeFinalId, roleTypeFinalTitle, hamesh.UserDesc, userId, fileId, hamesh.SumMablaghVamDarkhasti, 0, hamesh.RcvrId);


                if (hamesh.SumMablaghVamDarkhasti != null)
                {
                    _fileService.addMablaghVamDarkhastiVaVamMohaghahShode(fileId, hamesh.SumMablaghVamDarkhasti, 0);

                }




                return new BaseResult
                {
                    Message = " عملیات موفق",
                    Status = true
                };
            }

            catch (Exception)
            {


                return new BaseResult
                {
                    Message = " عملیات ناموفق",
                    Status = false
                };
            }
        }
    }
}
