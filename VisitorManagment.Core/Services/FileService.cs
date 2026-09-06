using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using VisitorManagment.Core.Convertors;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Generator;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class FileService : IFileService
    {

        private readonly VisitorManagmentContext _context;
        private readonly IPersonService _personService;
        private readonly IWebApiService _webApiService;
        /// <summary>
        /// سازنده سرویس فایل و مقداردهی وابستگی‌ها.
        /// </summary>
        public FileService(VisitorManagmentContext context, IPersonService personService, IWebApiService webApiService
            )
        {
            _context = context;
            _personService = personService;
            _webApiService = webApiService;
        }

        /// <summary>
        /// دریافت لیست موضوعات درخواست.
        /// </summary>
        public List<RequestSubject> GetRequestSubject()
        {
            return _context.RequestSubjects.ToList();
        }
        /// <summary>
        /// دریافت لیست انواع درخواست.
        /// </summary>
        public List<FileType> GetListFileType()
        {
            var res = _context.FileTypes.ToList();
            return res;
        }

        /// <summary>
        /// دریافت لیست اوامر صادره.
        /// </summary>
        public List<AvamerSadereh> GetAvamerSadereh()
        {
            return _context.AvamerSaderehs.ToList();
        }
        /// <summary>
        /// دریافت لیست وضعیت‌های درخواست.
        /// </summary>
        public List<FileStatus> GetFileStatus()
        {
            return _context.FileStatuses.ToList();
        }
        /// <summary>
        /// دریافت لیست اولویت‌ها.
        /// </summary>
        public List<Priority> GetPriority()
        {
            return _context.Priorities.ToList();
        }

        #region اضافه کردن پرسنل در پرسنل
        /// <summary>
        /// نسخه قبلی ثبت اطلاعات پرسنل در جدول پرسنل.
        /// </summary>
        public int AddPersonToPersonal0(FactPersonalViewModel person)
        {
            var personInfoApi = _webApiService.GetPersonalByPersonalNo(person.PersonalCode).Data;

            #region پر کردن ویو مدل
            personInfoApi.FPersonalCode = person.FPersonalCode;
            personInfoApi.FPersonalName = person.FPersonalName;
            personInfoApi.Phone = person.Phone;
            personInfoApi.PriorityId = person.PriorityId;
            personInfoApi.FileStatusId = person.FileStatusId;
            personInfoApi.RequestSubjectId = person.RequestSubjectId;
            personInfoApi.Addres = person.Addres;
            personInfoApi.RequestDescription = person.RequestDescription;
            personInfoApi.ProblemDescription = person.ProblemDescription;
            personInfoApi.Attachment = person.Attachment;
            personInfoApi.FishAttachment = person.FishAttachment;
            personInfoApi.IsarStatus = person.IsarStatus;
            personInfoApi.RegDate = DateTime.Now;
            personInfoApi.AddUserId = person.AddUserId;
            personInfoApi.CodGha = person.CodGha;
            personInfoApi.CodGhaTitle = _webApiService.GetGharargah().Data
                .Where(x => x.Id == person.CodGha).Select(x => x.Title).FirstOrDefault();
            #endregion


            var personelExist = _context.Personals.Where(p => p.PersonalCode == person.PersonalCode).SingleOrDefault();


            if (personelExist == null)
            {
                var createperson = new Personal()
                {
                    FirstName = personInfoApi.FirstName,
                    LastName = personInfoApi.LastName,
                    PersonalCode = personInfoApi.PersonalCode,
                    MelliCode = personInfoApi.MelliCode,
                    RankTitle = personInfoApi.RankTitle,
                    RankCode = personInfoApi.RankCode,
                    StatusTitle = personInfoApi.StatuseTitle,
                    BranchCode = personInfoApi.BranchCode,
                    BranchTitle = personInfoApi.BranchTitle,
                    JobDes = personInfoApi.JobDes,
                    UnitDutyCode = personInfoApi.UnitDutyCode,
                    UnitDutyTitle = personInfoApi.UnitDutyTitle,
                    UnitTitle = personInfoApi.UnitTitle,
                    CodGha = personInfoApi.CodGha,
                    CodGhaTitle = personInfoApi.CodGhaTitle,
                    Addres = personInfoApi.Addres,
                    IsarStatus = personInfoApi.IsarStatus,
                    Phone = personInfoApi.Phone,
                    DRSAD_JA = personInfoApi.DRSAD_JA,
                    DRSAD_JB = personInfoApi.DRSAD_JB,
                    TOT_AML2 = personInfoApi.TOT_AML2,
                    TOT_AML = personInfoApi.TOT_AML,
                    FarmandehPersonalCode = personInfoApi.FPersonalCode,
                    FarmandehPersonalName = personInfoApi.FPersonalName,
                    RegUserId = personInfoApi.AddUserId,
                    RegDate = DateTime.Now,
                    EmploymentDate = personInfoApi.EmploymentDate,
                    EmploymentTitle = personInfoApi.EmploymentTitle,
                    BirthPlaceTitle = personInfoApi.BirthPlaceTitle,
                    BirthDate = personInfoApi.BirthDate,
                    BloodTitle = personInfoApi.BloodTitle,
                    ReligoinTitle = personInfoApi.ReligoinTitle,
                    MarridTitle = personInfoApi.MarridTitle,

                };

                #region Save Avatar

                if (person.PersonalAvatar != null)
                {
                    string imagePath = "";
                    createperson.PersonalAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(person.PersonalAvatar.FileName);
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PersonalAvatar", createperson.PersonalAvatar);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        person.PersonalAvatar.CopyTo(stream);
                    }
                }
                else
                {
                    createperson.PersonalAvatar = "Default.png";
                }

                #endregion

                _context.Add(createperson);
                _context.SaveChanges();
                return createperson.Id;
            }

            _context.Update(personelExist);
            _context.SaveChanges();

            return personelExist.Id;
        }

        /// <summary>
        /// ثبت یا به‌روزرسانی اطلاعات پرسنل بر اساس اطلاعات دریافتی از سرویس پرسنلی.
        /// </summary>
        public int AddPersonToPersonal(FactPersonalViewModel person)
        {
            var apiResult = _webApiService.GetPersonalByPersonalNo(person.PersonalCode);

            if (apiResult == null || apiResult.IsSuccess == false || apiResult.Data == null)
            {
                return 0;
            }

            var personInfoApi = apiResult.Data;

            personInfoApi.FPersonalCode = person.FPersonalCode;
            personInfoApi.FPersonalName = person.FPersonalName;
            personInfoApi.Phone = person.Phone;
            personInfoApi.PriorityId = person.PriorityId;
            personInfoApi.FileStatusId = person.FileStatusId;
            personInfoApi.RequestSubjectId = person.RequestSubjectId;
            personInfoApi.Addres = person.Addres;
            personInfoApi.RequestDescription = person.RequestDescription;
            personInfoApi.ProblemDescription = person.ProblemDescription;
            personInfoApi.Attachment = person.Attachment;
            personInfoApi.FishAttachment = person.FishAttachment;
            personInfoApi.IsarStatus = person.IsarStatus;
            personInfoApi.RegDate = DateTime.Now;
            personInfoApi.AddUserId = person.AddUserId;
            personInfoApi.CodGha = person.CodGha;

            var gharargahResult = _webApiService.GetGharargah();
            personInfoApi.CodGhaTitle = gharargahResult?.Data?
                .Where(x => x.Id == person.CodGha)
                .Select(x => x.Title)
                .FirstOrDefault();

            var personalEntity = _context.Personals
                .SingleOrDefault(p => p.PersonalCode == person.PersonalCode);

            if (personalEntity == null)
            {
                personalEntity = new Personal
                {
                    RegDate = DateTime.Now,
                    PersonalAvatar = "Default.png"
                };

                _context.Personals.Add(personalEntity);
            }

            FillPersonalEntity(personalEntity, personInfoApi);

            if (person.PersonalAvatar != null)
            {
                personalEntity.PersonalAvatar = SavePersonalAvatar(person.PersonalAvatar, personalEntity.PersonalAvatar);
            }

            _context.SaveChanges();

            return personalEntity.Id;
        }

        /// <summary>
        /// پر کردن موجودیت پرسنل با اطلاعات ویومدل.
        /// </summary>
        private void FillPersonalEntity(Personal entity, FactPersonalViewModel source)
        {
            entity.FirstName = source.FirstName;
            entity.LastName = source.LastName;
            entity.PersonalCode = source.PersonalCode;
            entity.MelliCode = source.MelliCode;
            entity.RankTitle = source.RankTitle;
            entity.RankCode = source.RankCode;
            entity.StatusTitle = source.StatuseTitle;
            entity.BranchCode = source.BranchCode;
            entity.BranchTitle = source.BranchTitle;
            entity.JobDes = source.JobDes;
            entity.UnitDutyCode = source.UnitDutyCode;
            entity.UnitDutyTitle = source.UnitDutyTitle;
            entity.UnitTitle = source.UnitTitle;
            entity.UnitCode = source.UnitCode;
            entity.CodGha = source.CodGha;
            entity.CodGhaTitle = source.CodGhaTitle;
            entity.Addres = source.Addres;
            entity.IsarStatus = source.IsarStatus;
            entity.Phone = source.Phone;
            entity.DRSAD_JA = source.DRSAD_JA;
            entity.DRSAD_JB = source.DRSAD_JB;
            entity.TOT_AML2 = source.TOT_AML2;
            entity.TOT_AML = source.TOT_AML;
            entity.FarmandehPersonalCode = source.FPersonalCode;
            entity.FarmandehPersonalName = source.FPersonalName;
            entity.RegUserId = source.AddUserId;
            entity.EmploymentDate = source.EmploymentDate;
            entity.EmploymentTitle = source.EmploymentTitle;
            entity.BirthPlaceTitle = source.BirthPlaceTitle;
            entity.BirthDate = source.BirthDate;
            entity.BloodTitle = source.BloodTitle;
            entity.ReligoinTitle = source.ReligoinTitle;
            entity.MarridTitle = source.MarridTitle;
        }

        /// <summary>
        /// ذخیره تصویر پرسنل و بازگرداندن نام فایل ذخیره‌شده.
        /// </summary>
        private string SavePersonalAvatar(IFormFile file, string oldFileName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName)?.ToLower();

            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                return string.IsNullOrWhiteSpace(oldFileName) ? "Default.png" : oldFileName;
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                return string.IsNullOrWhiteSpace(oldFileName) ? "Default.png" : oldFileName;
            }

            var newFileName = NameGenerator.GenerateUniqCode() + extension;
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PersonalAvatar");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var newPath = Path.Combine(directoryPath, newFileName);

            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            if (!string.IsNullOrWhiteSpace(oldFileName) &&
                oldFileName != "Default.png" &&
                oldFileName != "Default.jpg")
            {
                var oldPath = Path.Combine(directoryPath, oldFileName);

                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }

            return newFileName;
        }

        #endregion

        #region add file to table file
        /// <summary>
        /// ثبت اطلاعات درخواست ملاقات در جدول فایل‌ها.
        /// </summary>
        //public BaseResult AddFile0(FactPersonalViewModel person)
        //{
        //    var personInfoApi = _webApiService.GetPersonalByPersonalNo(person.PersonalCode).Data;

        //    #region پر کردن ویو مدل
        //    personInfoApi.FPersonalCode = person.FPersonalCode;
        //    personInfoApi.FPersonalName = person.FPersonalName;
        //    personInfoApi.Phone = person.Phone;
        //    personInfoApi.PriorityId = person.PriorityId;
        //    personInfoApi.FileStatusId = person.FileStatusId;
        //    personInfoApi.RequestSubjectId = person.RequestSubjectId;
        //    personInfoApi.Addres = person.Addres;
        //    personInfoApi.RequestDescription = person.RequestDescription;
        //    personInfoApi.ProblemDescription = person.ProblemDescription;
        //    personInfoApi.Attachment = person.Attachment;
        //    personInfoApi.FishAttachment = person.FishAttachment;
        //    personInfoApi.IsarStatus = person.IsarStatus;
        //    personInfoApi.RegDate = DateTime.Now;
        //    personInfoApi.AddUserId = person.AddUserId;
        //    personInfoApi.CodGha = person.CodGha;
        //    personInfoApi.CodGhaTitle = _webApiService.GetGharargah().Data
        //        .Where(x => x.Id == person.CodGha).Select(x => x.Title).FirstOrDefault();
        //    #endregion
        //    #region MyRegion
        //    var file = new Files();

        //    file.PersonalId = person.PersonalId;
        //    file.FileTypeId = person.FileTypeId;
        //    file.MeetingId = 0;
        //    file.FirstName = personInfoApi.FirstName;
        //    file.LastName = personInfoApi.LastName;
        //    file.MelliCode = personInfoApi.MelliCode;
        //    file.RequestSubjectId = person.RequestSubjectId;
        //    file.PriorityId = person.PriorityId;
        //    file.FileStatusId = person.FileStatusId;
        //    file.RequestDescription = person.RequestDescription;
        //    file.ProblemDescription = person.ProblemDescription;
        //    file.Addres = person.Addres;
        //    file.TOT_AML2 = personInfoApi.TOT_AML2;
        //    file.StatusTitle = person.StatuseTitle;
        //    file.FarmandehPersonalName = person.FPersonalName;
        //    file.FarmandehPersonalCode = person.FPersonalCode;
        //    file.TOT_AML = personInfoApi.TOT_AML;
        //    file.IsarStatus = personInfoApi.IsarStatus;
        //    file.DRSAD_JA = personInfoApi.DRSAD_JA;
        //    file.DRSAD_JB = personInfoApi.DRSAD_JB;
        //    file.JobDes = personInfoApi.JobDes;
        //    file.Phone = person.Phone;
        //    file.RankTitle = personInfoApi.RankTitle;
        //    file.RankCode = personInfoApi.RankCode;
        //    file.BranchTitle = personInfoApi.BranchTitle;
        //    file.BranchCode = personInfoApi.BranchCode;
        //    file.UnitDutyTitle = personInfoApi.UnitDutyTitle;
        //    file.UnitDutyCode = personInfoApi.UnitDutyCode;
        //    file.UnitTitle = personInfoApi.UnitTitle;
        //    file.UnitCode = personInfoApi.UnitCode;
        //    file.CodGhaTitle = personInfoApi.CodGhaTitle;
        //    file.CodGha = person.CodGha.Value;
        //    file.PersonalCode = person.PersonalCode;
        //    file.BranchTitle = personInfoApi.BranchTitle;
        //    file.RegUserId = person.AddUserId;
        //    file.MeetingId = null;
        //    file.RegDate = DateTime.Now;

        //    #region اطلاعات فیش حقوقی
        //    file.CountVam = person.CountVam;
        //    file.ReciveMoney = person.ReciveMoney;
        //    file.SumAghsatVamMahiyaneh = person.SumAghsatVamMahiyaneh;
        //    file.TotalMoney = person.TotalMoney;
        //    #endregion

        //    #region Save File

        //    if (person.Attachment != null)
        //    {
        //        string imagePath = "";
        //        file.Attachment = NameGenerator.GenerateUniqCode() + Path.GetExtension(person.Attachment.FileName);
        //        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FileAttachment", file.Attachment);
        //        using (var stream = new FileStream(imagePath, FileMode.Create))
        //        {
        //            person.Attachment.CopyTo(stream);
        //        }
        //    }
        //    else
        //    {
        //        file.Attachment = "";
        //    }

        //    if (person.FishAttachment != null)
        //    {
        //        string imagePath = "";
        //        file.FishAttachment = NameGenerator.GenerateUniqCode() + Path.GetExtension(person.FishAttachment.FileName);
        //        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FishAttachment", file.FishAttachment);
        //        using (var stream = new FileStream(imagePath, FileMode.Create))
        //        {
        //            person.FishAttachment.CopyTo(stream);
        //        }
        //    }
        //    else
        //    {
        //        file.FishAttachment = "";
        //    }


        //    #endregion
        //    #endregion

        //    return AddToFile(file);

        //}
        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult AddFile(FactPersonalViewModel person)
        {
            #region اعتبارسنجی اولیه

            if (person == null)
            {
                return new BaseResult(false, "اطلاعات درخواست ارسال نشده است.");
            }

            if (string.IsNullOrWhiteSpace(person.PersonalCode))
            {
                return new BaseResult(false, "لطفاً کد پرسنلی را وارد کنید.");
            }

            if (!person.FPersonalCode.HasValue)
            {
                return new BaseResult(false, "لطفاً کد پرسنلی فرمانده را وارد کنید.");
            }

            if (!person.CodGha.HasValue)
            {
                return new BaseResult(false, "لطفاً قرارگاه را انتخاب کنید.");
            }

            if (!person.FileTypeId.HasValue)
            {
                return new BaseResult(false, "لطفاً نوع درخواست را انتخاب کنید.");
            }

            if (!person.RequestSubjectId.HasValue)
            {
                return new BaseResult(false, "لطفاً موضوع درخواست را انتخاب کنید.");
            }

            if (!person.PriorityId.HasValue)
            {
                return new BaseResult(false, "لطفاً اولویت را انتخاب کنید.");
            }

            if (!person.FileStatusId.HasValue)
            {
                return new BaseResult(false, "لطفاً وضعیت درخواست را انتخاب کنید.");
            }

            if (string.IsNullOrWhiteSpace(person.Phone))
            {
                return new BaseResult(false, "لطفاً تلفن همراه را وارد کنید.");
            }

            if (string.IsNullOrWhiteSpace(person.Addres))
            {
                return new BaseResult(false, "لطفاً آدرس را وارد کنید.");
            }

            if (string.IsNullOrWhiteSpace(person.ProblemDescription))
            {
                return new BaseResult(false, "لطفاً شرح مشکل را وارد کنید.");
            }

            if (string.IsNullOrWhiteSpace(person.RequestDescription))
            {
                return new BaseResult(false, "لطفاً شرح درخواست را وارد کنید.");
            }

            if (!person.TotalMoney.HasValue)
            {
                return new BaseResult(false, "لطفاً حقوق کل را وارد کنید.");
            }

            if (!person.ReciveMoney.HasValue)
            {
                return new BaseResult(false, "لطفاً میزان دریافتی را وارد کنید.");
            }

            if (!person.CountVam.HasValue)
            {
                return new BaseResult(false, "لطفاً تعداد وام را وارد کنید.");
            }

            if (!person.SumAghsatVamMahiyaneh.HasValue)
            {
                return new BaseResult(false, "لطفاً مجموع اقساط ماهیانه را وارد کنید.");
            }

            if (person.FishAttachment == null || person.FishAttachment.Length == 0)
            {
                return new BaseResult(false, "لطفاً فایل فیش حقوقی را بارگذاری کنید.");
            }

            #endregion

            #region دریافت اطلاعات پرسنلی از سرویس

            var personalApiResult = _webApiService.GetPersonalByPersonalNo(person.PersonalCode);

            if (personalApiResult == null)
            {
                return new BaseResult(false, "پاسخی از سرویس اطلاعات پرسنلی دریافت نشد.");
            }

            if (personalApiResult.IsSuccess == false || personalApiResult.Data == null)
            {
                return new BaseResult(false, personalApiResult.Message ?? "اطلاعات پرسنلی از سرویس دریافت نشد.");
            }

            var personInfoApi = personalApiResult.Data;

            #endregion

            #region دریافت عنوان قرارگاه

            var gharargahResult = _webApiService.GetGharargah();

            var codGhaTitle = gharargahResult?.Data?
                .Where(x => x.Id == person.CodGha.Value)
                .Select(x => x.Title)
                .FirstOrDefault();

            #endregion

            #region پر کردن اطلاعات تکمیلی مدل دریافتی از سرویس

            personInfoApi.FPersonalCode = person.FPersonalCode;
            personInfoApi.FPersonalName = person.FPersonalName;
            personInfoApi.Phone = person.Phone;
            personInfoApi.PriorityId = person.PriorityId;
            personInfoApi.FileStatusId = person.FileStatusId;
            personInfoApi.RequestSubjectId = person.RequestSubjectId;
            personInfoApi.Addres = person.Addres;
            personInfoApi.RequestDescription = person.RequestDescription;
            personInfoApi.ProblemDescription = person.ProblemDescription;
            personInfoApi.Attachment = person.Attachment;
            personInfoApi.FishAttachment = person.FishAttachment;
            personInfoApi.IsarStatus = person.IsarStatus;
            personInfoApi.RegDate = DateTime.Now;
            personInfoApi.AddUserId = person.AddUserId;
            personInfoApi.CodGha = person.CodGha;
            personInfoApi.CodGhaTitle = codGhaTitle;

            #endregion

            #region ساخت موجودیت فایل

            var file = new Files
            {
                PersonalId = person.PersonalId,

                FileTypeId = person.FileTypeId.Value,
                MeetingId = null,

                FirstName = personInfoApi.FirstName,
                LastName = personInfoApi.LastName,
                MelliCode = personInfoApi.MelliCode,

                RequestSubjectId = person.RequestSubjectId.Value,
                PriorityId = person.PriorityId.Value,
                FileStatusId = person.FileStatusId.Value,

                RequestDescription = person.RequestDescription,
                ProblemDescription = person.ProblemDescription,
                Addres = person.Addres,

                TOT_AML2 = personInfoApi.TOT_AML2,
                TOT_AML = personInfoApi.TOT_AML,

                StatusTitle = !string.IsNullOrWhiteSpace(personInfoApi.StatusTitle)
                    ? personInfoApi.StatusTitle
                    : personInfoApi.StatuseTitle,

                FarmandehPersonalName = person.FPersonalName,
                FarmandehPersonalCode = person.FPersonalCode,

                IsarStatus = personInfoApi.IsarStatus,
                DRSAD_JA = personInfoApi.DRSAD_JA,
                DRSAD_JB = personInfoApi.DRSAD_JB,

                JobDes = personInfoApi.JobDes,
                Phone = person.Phone,

                RankTitle = personInfoApi.RankTitle,
                RankCode = personInfoApi.RankCode,

                BranchTitle = personInfoApi.BranchTitle,
                BranchCode = personInfoApi.BranchCode,

                UnitDutyTitle = personInfoApi.UnitDutyTitle,
                UnitDutyCode = personInfoApi.UnitDutyCode,

                UnitTitle = personInfoApi.UnitTitle,
                UnitCode = personInfoApi.UnitCode,

                CodGha = person.CodGha.Value,
                CodGhaTitle = codGhaTitle,

                PersonalCode = person.PersonalCode,

                RegUserId = person.AddUserId,
                RegDate = DateTime.Now,

                CountVam = person.CountVam.Value,
                ReciveMoney = person.ReciveMoney.Value,
                SumAghsatVamMahiyaneh = person.SumAghsatVamMahiyaneh.Value,
                TotalMoney = person.TotalMoney.Value
            };

            #endregion

            #region ذخیره فایل‌های پیوست

            var attachmentResult = SaveUploadedFile(
                uploadedFile: person.Attachment,
                folderName: "FileAttachment",
                allowedExtensions: new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".rar" },
                maxSizeInBytes: 2 * 1024 * 1024,
                isRequired: false,
                requiredMessage: ""
            );

            if (!attachmentResult.Status)
            {
                return attachmentResult;
            }

            file.Attachment = attachmentResult.Model?.ToString() ?? "";

            var fishAttachmentResult = SaveUploadedFile(
                uploadedFile: person.FishAttachment,
                folderName: "FishAttachment",
                allowedExtensions: new[] { ".jpg", ".jpeg", ".png", ".pdf" },
                maxSizeInBytes: 2 * 1024 * 1024,
                isRequired: true,
                requiredMessage: "لطفاً فایل فیش حقوقی را بارگذاری کنید."
            );

            if (!fishAttachmentResult.Status)
            {
                return fishAttachmentResult;
            }

            file.FishAttachment = fishAttachmentResult.Model?.ToString() ?? "";

            #endregion

            return AddToFile(file);
        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        private BaseResult SaveUploadedFile(
    IFormFile uploadedFile,
    string folderName,
    string[] allowedExtensions,
    long maxSizeInBytes,
    bool isRequired,
    string requiredMessage)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                if (isRequired)
                {
                    return new BaseResult(false, requiredMessage);
                }

                return new BaseResult(true, "فایلی انتخاب نشده است.", "");
            }

            var extension = Path.GetExtension(uploadedFile.FileName)?.ToLower();

            if (string.IsNullOrWhiteSpace(extension))
            {
                return new BaseResult(false, "فرمت فایل انتخاب شده معتبر نیست.");
            }

            if (!allowedExtensions.Contains(extension))
            {
                return new BaseResult(false, "فرمت فایل انتخاب شده مجاز نیست.");
            }

            if (uploadedFile.Length > maxSizeInBytes)
            {
                return new BaseResult(false, "حجم فایل نباید بیشتر از ۲ مگابایت باشد.");
            }

            var fileName = NameGenerator.GenerateUniqCode() + extension;

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName
            );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                uploadedFile.CopyTo(stream);
            }

            return new BaseResult(true, "فایل با موفقیت ذخیره شد.", fileName);
        }

        /// <summary>
        /// افزودن موجودیت فایل به دیتابیس.
        /// </summary>
        public BaseResult AddToFile(Files file)
        {
            _context.Files.Add(file);
            _context.SaveChanges();
            if (file.Id != 0)
            {
                return new BaseResult
                {
                    Message = "ثبت موفق",
                    Model = file.Id,
                    Status = true
                };
            }
            return new BaseResult
            {
                Message = "ثبت ناموفق",
                Model = file.Id,
                Status = false
            };
        }
        #endregion

        #region GetListFile

        /// <summary>
        /// دریافت لیست درخواست‌ها بر اساس نقش، کاربر و فیلترها.
        /// </summary>
        public ListFileViewModel GetListFile(int roleTypeId, int rcvrUserId, int pageId = 1, int requestsubject = 0, int filterAvamerSadereh = 0, string filterGharargah = "", string filterCaption = "")
        {
            const int take = 10; // تعداد فایل‌ها در هر صفحه
            var skip = (pageId - 1) * take; // محاسبه تعداد فایل‌هایی که باید رد شوند

            try
            {
                IQueryable<Cartable> filesQuery = _context.Cartables.Include(f => f.File)
                    .Where(f => f.RcvrUserId == rcvrUserId && !f.File.IsDelete); // دریافت فایل‌ها برای کاربر خاص
                #region کاربر لاگین کرده ادمین باشد
                if (roleTypeId == 100)
                {
                    var files = filesQuery.Select(t => new FactPersonalViewModel
                    {
                        Id = t.File.Id,
                        FileTypeId = t.File.FileTypeId,
                        FileTypeTitle = t.File.FileType.Title,
                        AddUserId = t.File.Personal.RegUserId,
                        FirstName = t.File.Personal.FirstName,
                        LastName = t.File.Personal.LastName,
                        PersonalCode = t.File.Personal.PersonalCode,
                        MelliCode = t.File.Personal.MelliCode,
                        ReqSubTitle = t.File.RequestSubject.Title,
                        FileStatusTitle = t.File.FileStatus.Title,
                        PriorityTitle = t.File.Priority.Title,
                        RankTitle = t.File.RankTitle,
                        JobDes = t.File.JobDes,
                        Phone = t.File.Phone,
                        IsAnswerdMoavenat = t.File.IsMoavenatAnswered,
                        IsMeetingHold = t.File.IsMeetingHold,
                        IsArchived = t.File.IsArchived,
                        StatuseTitle = t.File.StatusTitle,
                        BranchTitle = t.File.BranchTitle,
                        UnitDutyTitle = t.File.UnitDutyTitle,
                        UnitTitle = t.File.UnitTitle,
                        CodGhaTitle = t.File.CodGhaTitle,
                        RegDate = t.RegDate
                    }).OrderBy(u => u.RegDate).Distinct().ToList();

                    return new ListFileViewModel
                    {
                        CurrentPage = pageId,
                        skip = skip,
                        count = files.Count,
                        PageCount = (int)Math.Ceiling(files.Count / (double)take),
                        files = files
                    };
                }
                #endregion

                #region کاربر لاگین کرده رئیس دفتر شماره 3 باشد
                if (rcvrUserId == 120)
                {
                    // اعمال فیلترها
                    if (filterAvamerSadereh == 1)
                    {
                        filesQuery = filesQuery.Where(f => f.File.IsMeetingHold); // برگزاری جلسه
                    }

                    if (!string.IsNullOrEmpty(filterCaption))
                    {
                        filesQuery = filesQuery.Where(u => u.File.PersonalCode.Contains(filterCaption)); // فیلتر بر اساس کد شخصی
                    }

                    if (!string.IsNullOrWhiteSpace(filterGharargah))
                    {
                        filesQuery = filesQuery.Where(t => t.File.CodGha == int.Parse(filterGharargah)); // فیلتر بر اساس کد قرارداد
                    }

                    if (requestsubject != 0)
                    {
                        filesQuery = filesQuery.Where(t => t.File.RequestSubjectId == requestsubject); // فیلتر بر اساس موضوع درخواست
                    }

                    var files = filesQuery.Select(t => new FactPersonalViewModel
                    {
                        Id = t.File.Id,
                        FileTypeId = t.File.FileTypeId,
                        FileTypeTitle = t.File.FileType.Title,
                        AddUserId = t.File.Personal.RegUserId,
                        FirstName = t.File.Personal.FirstName,
                        LastName = t.File.Personal.LastName,
                        PersonalCode = t.File.Personal.PersonalCode,
                        MelliCode = t.File.Personal.MelliCode,
                        ReqSubTitle = t.File.RequestSubject.Title,
                        FileStatusTitle = t.File.FileStatus.Title,
                        PriorityTitle = t.File.Priority.Title,
                        RankTitle = t.File.RankTitle,
                        JobDes = t.File.JobDes,
                        IsAnswerdMoavenat = t.File.IsMoavenatAnswered,
                        IsArchived = t.File.IsArchived,
                        IsMeetingHold = t.File.IsMeetingHold,
                        Phone = t.File.Phone,
                        StatuseTitle = t.File.StatusTitle,
                        BranchTitle = t.File.BranchTitle,
                        UnitDutyTitle = t.File.UnitDutyTitle,
                        UnitTitle = t.File.UnitTitle,
                        CodGhaTitle = t.File.CodGhaTitle,
                        RegDate = t.RegDate
                    }).OrderBy(u => u.RegDate).Distinct().ToList();

                    return new ListFileViewModel
                    {
                        CurrentPage = pageId,
                        skip = skip,
                        count = files.Count,
                        PageCount = (int)Math.Ceiling(files.Count / (double)take),
                        files = files
                    };
                }
                #endregion


                // برای سایر کاربران
                // اعمال فیلترها
                if (filterAvamerSadereh == 1)
                {
                    filesQuery = filesQuery.Where(f => f.File.IsMeetingHold); // برگزاری جلسه
                }

                if (!string.IsNullOrEmpty(filterCaption))
                {
                    filesQuery = filesQuery.Where(u => u.File.PersonalCode.Contains(filterCaption)); // فیلتر بر اساس کد شخصی
                }

                if (!string.IsNullOrWhiteSpace(filterGharargah))
                {
                    filesQuery = filesQuery.Where(t => t.File.CodGha == int.Parse(filterGharargah)); // فیلتر بر اساس کد قرارداد
                }

                if (requestsubject != 0)
                {
                    filesQuery = filesQuery.Where(t => t.File.RequestSubjectId == requestsubject); // فیلتر بر اساس موضوع درخواست
                }

                var defaultFiles = filesQuery.Select(t => new FactPersonalViewModel
                {
                    Id = t.File.Id,
                    FileTypeId = t.File.FileTypeId,
                    FileTypeTitle = t.File.FileType.Title,
                    AddUserId = t.File.Personal.RegUserId,
                    FirstName = t.File.Personal.FirstName,
                    LastName = t.File.Personal.LastName,
                    PersonalCode = t.File.Personal.PersonalCode,
                    MelliCode = t.File.Personal.MelliCode,
                    ReqSubTitle = t.File.RequestSubject.Title,
                    FileStatusTitle = t.File.FileStatus.Title,
                    PriorityTitle = t.File.Priority.Title,
                    RankTitle = t.File.RankTitle,
                    JobDes = t.File.JobDes,
                    IsAnswerdMoavenat = t.File.IsMoavenatAnswered,
                    IsArchived = t.File.IsArchived,
                    IsMeetingHold = t.File.IsMeetingHold,
                    Phone = t.File.Phone,
                    StatuseTitle = t.File.StatusTitle,
                    BranchTitle = t.File.BranchTitle,
                    UnitDutyTitle = t.File.UnitDutyTitle,
                    UnitTitle = t.File.UnitTitle,
                    CodGhaTitle = t.File.CodGhaTitle,
                    RegDate = t.RegDate
                }).OrderBy(u => u.RegDate).Distinct().ToList();

                return new ListFileViewModel
                {
                    CurrentPage = pageId,
                    skip = skip,
                    count = defaultFiles.Count,
                    PageCount = (int)Math.Ceiling(defaultFiles.Count / (double)take),
                    files = defaultFiles
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در دریافت فایل‌ها: {ex.Message}");

                return new ListFileViewModel
                {
                    CurrentPage = pageId,
                    skip = skip,
                    count = 0,
                    PageCount = 0,
                    files = new List<FactPersonalViewModel>(),
                    ErrorMessage = "در هنگام دریافت فایل‌ها خطایی رخ داد."
                };
            }
        }


        /// <summary>
        /// دریافت لیست درخواست‌هایی که برای جلسه ملاقات ثبت شده‌اند.
        /// </summary>
        public ListFileViewModel GetListFileWhenMetingHold(int roleTypeId, int rcvrUserId, int pageId = 1, int requestsubject = 0, string filterGharargah = "", string filterCaption = "")


        {
            // rcvrUserId ===>>>UserId claim
            IQueryable<Cartable> result = _context.Cartables.Include(f => f.File).Where(f => f.RcvrUserId == rcvrUserId && f.File.IsDelete == false && f.File.IsMeetingHold == true);


            #region Search
            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.File.PersonalCode.Contains(filterCaption));
            }

            if (filterGharargah != "" && filterGharargah != null)
            {
                result = result.Where(t => t.File.CodGha == int.Parse(filterGharargah));
            }

            if (requestsubject != 0)
            {
                result = result.Where(t => t.File.RequestSubjectId == requestsubject);
            }
            #endregion


            var take = 10;
            var skip = (pageId - 1) * take;

            ListFileViewModel list = new ListFileViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
            list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);

            list.files = result.Select(t => new FactPersonalViewModel()
            {
                Id = t.File.Id,
                AddUserId = t.File.Personal.RegUserId,
                FirstName = t.File.Personal.FirstName,
                LastName = t.File.Personal.LastName,
                PersonalCode = t.File.Personal.PersonalCode,
                MelliCode = t.File.Personal.MelliCode,
                ReqSubTitle = t.File.RequestSubject.Title,
                FileStatusTitle = t.File.FileStatus.Title,
                PriorityTitle = t.File.Priority.Title,
                RankTitle = t.File.RankTitle,
                JobDes = t.File.JobDes,
                Phone = t.File.Phone,
                StatuseTitle = t.File.StatusTitle,
                BranchTitle = t.File.BranchTitle,
                UnitDutyTitle = t.File.UnitDutyTitle,
                UnitTitle = t.File.UnitTitle,
                CodGhaTitle = t.File.CodGhaTitle,
                RegDate = t.RegDate
            }).OrderByDescending(u => u.RegDate).Skip(skip).Take(take).ToList();
            return list;
        }

        /// <summary>
        /// دریافت لیست درخواست‌ها جهت گردش و مشاهده سوابق.
        /// </summary>
        public ListFileViewModel GetListFileForCirculation(int rcvrUserId, DateTime? startDateSearchFilter, DateTime? endDateSearchFilter, int unitCode, int pageId = 1, int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, string filterCaption = "")
        {
            // rcvrUserId ===>>>UserId claim
            IQueryable<Hamesh> hamesh = _context.Hameshes.Include(f => f.File);
            IQueryable<Cartable> result2 = _context.Cartables.Include(f => f.File).ThenInclude(x => x.Personal);
            IQueryable<Cartable> result1 = _context.Cartables.Include(f => f.File).Where(f => f.RcvrUserId == rcvrUserId);
            IQueryable<Files> result;
            //120-------->رئیس دفتر 3
            //154-------->مدیر سامانه

            if (rcvrUserId == 120 || rcvrUserId == 154)
            {
                result = _context.Files.Where(x => x.IsDelete == false);
            }

            else
            {
                var listFileId = _context.Hameshes
                 .Include(x => x.File)
                 .Where(x => x.UserId == rcvrUserId).Select(x => x.FileId).Distinct().ToList();

                result = _context.Files.Where(x => listFileId.Contains(x.Id));
            }


            #region Search/filter
            if (startDateSearchFilter != null)
            {
                if (endDateSearchFilter != null)
                {
                    result = result.Where(u => u.RegDate >= startDateSearchFilter && u.RegDate <= endDateSearchFilter);
                }
                //var stDate = startDateSearchFilter.ToString();
                //var regDateWithoutTime = result.Select(x=>x.RegDate.ToShortDateString()).SingleOrDefault() ;
                /*result =result.Select(x=>x.RegDate)*/;
                // result = result.Where(u => u.RegDate.Contains(regDateWithoutTime));
                //result = result.Where(u => u.RegDate.ToShamsi()==startDateSearchFilter);
            }

            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.PersonalCode.Contains(filterCaption) || u.Personal.FirstName.Contains(filterCaption) || u.Personal.LastName.Contains(filterCaption));
            }


            if (requestsubject != 0)
            {
                result = result.Where(t => t.RequestSubjectId == requestsubject);
            }

            if (filterGharargah != 0)
            {
                result = result.Where(t => t.CodGha == filterGharargah);
            }

            if (filterYegan != 0)
            {
                result = result.Where(t => t.UnitCode == filterYegan);
            }

            #endregion


            var take = 20;
            var skip = (pageId - 1) * take;

            ListFileViewModel list = new ListFileViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
            list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;
                                                                              //*****************

            //*****************
            list.files = result.Select(t => new FactPersonalViewModel()
            {
                Id = t.Id,
                AddUserId = t.Personal.RegUserId,
                FirstName = t.Personal.FirstName,
                LastName = t.Personal.LastName,
                PersonalCode = t.Personal.PersonalCode,
                MelliCode = t.Personal.MelliCode,
                ReqSubTitle = t.RequestSubject.Title,
                FileStatusTitle = t.FileStatus.Title,
                PriorityTitle = t.Priority.Title,
                RankTitle = t.RankTitle,
                JobDes = t.JobDes,
                Phone = t.Phone,
                StatuseTitle = t.StatusTitle,
                BranchTitle = t.BranchTitle,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                RequestDescription = t.RequestDescription,
                // ReciverUserId =t.Cartables.Where(x=>x.FileId==t.Id).OrderBy(x=>x.RegDate).Select(x=>x.RcvrUserId).Last(),
                ReciverUserRole = _context.Hameshes
                .Include(x => x.User)
                .Where(x => x.FileId == t.Id)
                .OrderBy(x => x.RegDate)
                .Select(x => x.RoleTypeTitle)
                .LastOrDefault(),

                RegDate = t.RegDate
            }).OrderByDescending(u => u.RegDate).ToList();
            return list;
        }


        /// <summary>
        /// دریافت لیست درخواست‌ها جهت گردش در بخش مدیریت.
        /// </summary>
        public ListFileViewModel GetListFileForCirculationAdmin(int userId, DateTime? startDateSearchFilter, DateTime? endDateSearchFilter, int unitCode, int pageId = 1, int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, int filterMoavenat = 0, string filterCaption = "")
        {
            // rcvrUserId ===>>>UserId claim
            //فایل هایی که ف انصار و معاون شماره 3 هامش زده رو میاره برامون
            var result = _context.Hameshes.Include(x => x.File)
                .Where((x => x.UserId == userId && (x.RoleTypeId == 5 || x.RoleTypeId == 7 || x.RoleTypeId == 10))).Select(x => x.File).Distinct();

            var resultForFilterMoavenat = _context.Hameshes.Include(x => x.File).Distinct();


            #region Search/filter

            if (startDateSearchFilter != null)
            {
                if (endDateSearchFilter != null)
                {
                    result = result.Where(u => u.RegDate >= startDateSearchFilter && u.RegDate <= endDateSearchFilter);
                }
                //var stDate = startDateSearchFilter.ToString();
                //var regDateWithoutTime = result.Select(x=>x.RegDate.ToShortDateString()).SingleOrDefault() ;
                /*result =result.Select(x=>x.RegDate)*/;
                // result = result.Where(u => u.RegDate.Contains(regDateWithoutTime));
                //result = result.Where(u => u.RegDate.ToShamsi()==startDateSearchFilter);
            }

            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.PersonalCode.Contains(filterCaption) || u.Personal.FirstName.Contains(filterCaption) || u.Personal.LastName.Contains(filterCaption));
            }


            if (requestsubject != 0)
            {
                result = result.Where(t => t.RequestSubjectId == requestsubject);
            }

            if (filterGharargah != 0)
            {
                result = result.Where(t => t.CodGha == filterGharargah);
            }

            if (filterYegan != 0)
            {
                result = result.Where(t => t.UnitCode == filterYegan);
            }


            //اینجا رو بنویسم
            if (filterMoavenat != 0)
            {
                var role = _context.Roles.Where(x => x.RoleId == filterMoavenat).SingleOrDefault();

                result = _context.Hameshes.Include(x => x.File).Where(x => x.User.UserRoles.Select(x => x.Role.Code).FirstOrDefault() == role.Code).Select(x => x.File).Distinct();

                // result = _context.Hameshes.Include(x => x.File).Where(x=>x.RoleTypeId == filterMoavenat).Select(x => x.File).Distinct();
                // result = _context.Hameshes.Include(x => x.File).Where(x => x.RoleTypeId == filterMoavenat).Select(x => x.File).Distinct();

                //result = resultForFilterMoavenat.Where(x => x.RoleTypeId == filterMoavenat).Select(x => x.File).Distinct();
            }

            #endregion


            //var take = 20;
            //var skip = (pageId - 1) * take;

            ListFileViewModel list = new ListFileViewModel() { };
            list.CurrentPage = pageId;
            //list.skip = skip;
            list.count = result.Count();
            /* list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);*/// result.Count() / take;
                                                                                   //*****************

            //*****************
            list.files = result.Select(t => new FactPersonalViewModel()
            {
                Id = t.Id,
                AddUserId = t.Personal.RegUserId,
                FirstName = t.Personal.FirstName,
                LastName = t.Personal.LastName,
                PersonalCode = t.Personal.PersonalCode,
                MelliCode = t.Personal.MelliCode,
                ReqSubTitle = t.RequestSubject.Title,
                FileStatusTitle = t.FileStatus.Title,
                PriorityTitle = t.Priority.Title,
                RankTitle = t.RankTitle,
                JobDes = t.JobDes,
                Phone = t.Phone,
                StatuseTitle = t.StatusTitle,
                BranchTitle = t.BranchTitle,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                RequestDescription = t.RequestDescription,
                LastMoavenatHamesh = _context.Hameshes.Include(x => x.User).Where(x => x.FileId == t.Id).OrderBy(x => x.RegDate).Select(x => x.RoleTypeTitle).LastOrDefault(),
                // ReciverUserId =t.Cartables.Where(x=>x.FileId==t.Id).OrderBy(x=>x.RegDate).Select(x=>x.RcvrUserId).Last(),
                ReciverUserRole = _context.Hameshes.Include(x => x.User).Where(x => x.FileId == t.Id).OrderBy(x => x.RegDate).Select(x => x.RoleTypeTitle).LastOrDefault(),

                RegDate = t.RegDate
                //}).OrderByDescending(u => u.RegDate).Skip(skip).Take(take).ToList();
            }).OrderByDescending(u => u.RegDate).ToList();
            return list;
        }


        /// <summary>
        /// دریافت لیست درخواست‌های ثبت‌شده توسط کاربر برای نمایش مدیریت.
        /// </summary>
        public ListFileViewModel GetListFileForShowAdmin(int userId, int pageId = 1, int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, string filterCaption = "")
        {
            // rcvrUserId ===>>>UserId claim
            IQueryable<Cartable> result2 = _context.Cartables.Include(f => f.File).ThenInclude(x => x.Personal);
            //IQueryable<Cartable> result1 = _context.Cartables.Include(f => f.File).Where(f => f.RcvrUserId == rcvrUserId);

            IQueryable<Files> result = _context.Files.Include(x => x.Cartables).Where(x => x.RegUserId == userId);

            #region Search/filter
            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.PersonalCode.Contains(filterCaption) || u.Personal.FirstName.Contains(filterCaption) || u.Personal.LastName.Contains(filterCaption));
            }


            if (requestsubject != 0)
            {
                result = result.Where(t => t.RequestSubjectId == requestsubject);
            }

            if (filterGharargah != 0)
            {
                result = result.Where(t => t.CodGha == filterGharargah);
            }

            if (filterYegan != 0)
            {
                result = result.Where(t => t.UnitCode == filterYegan);
            }
            #endregion


            var take = 20;
            var skip = (pageId - 1) * take;

            ListFileViewModel list = new ListFileViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
            list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;
                                                                              //*****************

            //*****************
            list.files = result.Select(t => new FactPersonalViewModel()
            {
                Id = t.Id,
                AddUserId = t.Personal.RegUserId,
                FirstName = t.Personal.FirstName,
                LastName = t.Personal.LastName,
                PersonalCode = t.Personal.PersonalCode,
                MelliCode = t.Personal.MelliCode,
                ReqSubTitle = t.RequestSubject.Title,
                FileStatusTitle = t.FileStatus.Title,
                PriorityTitle = t.Priority.Title,
                RankTitle = t.RankTitle,
                JobDes = t.JobDes,
                Phone = t.Phone,
                StatuseTitle = t.StatusTitle,
                BranchTitle = t.BranchTitle,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                RequestDescription = t.RequestDescription,
                LastMoavenatHamesh = _context.Hameshes.Include(x => x.User).Where(x => x.FileId == t.Id).OrderBy(x => x.RegDate).Select(x => x.RoleTypeTitle).LastOrDefault(),
                // ReciverUserId =t.Cartables.Where(x=>x.FileId==t.Id).OrderBy(x=>x.RegDate).Select(x=>x.RcvrUserId).Last(),
                ReciverUserRole = _context.Hameshes.Include(x => x.User).Where(x => x.FileId == t.Id).OrderBy(x => x.RegDate).Select(x => x.RoleTypeTitle).LastOrDefault(),

                RegDate = t.RegDate
            }).OrderByDescending(u => u.RegDate).Skip(skip).Take(take).ToList();
            return list;
        }
        #endregion

        #region Edit

        /// <summary>
        /// تغییر وضعیت درخواست هنگام ثبت هامش.
        /// </summary>
        public void EditFileStatusIdWhenSabtHamesh(int fileId, int actionTypeId)
        {
            var file = _context.Files.Where(x => x.Id == fileId).SingleOrDefault();
            file.FileStatusId = actionTypeId;
            _context.Update(file);
            _context.SaveChanges();
        }
        /// <summary>
        /// دریافت اطلاعات درخواست برای ویرایش.
        /// </summary>
        public EditFactPersonalViewModel GetFileForEdit(int Id)
        {
            var file = GetFileByFileId(Id);
            var personalAvatar = _personService.GetPersonByPersonalId(file.PersonalId).PersonalAvatar;
            var editfile = new EditFactPersonalViewModel()
            {
                Id = file.Id,
                FileTypeId = file.FileTypeId,
                PersonalCode = file.PersonalCode,
                IsAnswerdMoavenat = file.IsMoavenatAnswered,
                IsArchived = file.IsArchived,
                PersonalId = file.PersonalId,
                RequestSubjectId = file.RequestSubjectId,
                PriorityId = file.PriorityId,
                FileStatusId = file.FileStatusId,
                RequestDescription = file.RequestDescription,
                ProblemDescription = file.ProblemDescription,
                AttachmentFileName = file.Attachment,
                AttachDastor = file.AttachDastor,
                MelliCode = file.MelliCode,
                FirstName = file.FirstName,
                LastName = file.LastName,
                RankTitle = file.RankTitle,
                BranchTitle = file.BranchTitle,
                StatusTitle = file.StatusTitle,
                DRSAD_JA = file.DRSAD_JA,
                DRSAD_JB = file.DRSAD_JB,
                IsarStatus = file.IsarStatus,
                TOT_AML2 = file.TOT_AML2,
                TOT_AML = file.TOT_AML,
                UnitDutyTitle = file.UnitDutyTitle,
                UnitTitle = file.UnitTitle,
                CodGha = file.CodGha,
                CodGhaTitle = file.CodGhaTitle,
                FPersonalCode = file.FarmandehPersonalCode,
                FPersonalName = file.FarmandehPersonalName,
                Addres = file.Addres,
                Phone = file.Phone,
                EditUserId = file.EditUserId,
                EditDate = DateTime.Now.ToShamsi(),
                PersonalAvatarName = personalAvatar,
                AttacmentFileName = file.Attachment,
                FishAttacmentFileName = file.FishAttachment,
                CountVam = file.CountVam,
                ReciveMoney = file.ReciveMoney,
                SumAghsatVamMahiyaneh = file.SumAghsatVamMahiyaneh,
                TotalMoney = file.TotalMoney,
                //Attachment = file.AttachDastor,
            };
            return editfile;
        }

        /// <summary>
        /// دریافت فایل به همراه اطلاعات مرتبط بر اساس شناسه فایل.
        /// </summary>
        public Files GetFileByFileId(int Id)
        {
            var file = _context.Files.Where(f => f.Id == Id).Include(x => x.Personal).Include(f => f.RequestSubject).SingleOrDefault();
            return file;
        }


        /// <summary>
        /// دریافت فایل بر اساس شناسه.
        /// </summary>
        public Files GetFile(int Id)
        {
            var file = _context.Files.Where(f => f.Id == Id).SingleOrDefault();
            return file;
        }


        /// <summary>
        /// دریافت لیست درخواست‌ها بر اساس نقش، کاربر و فیلترها.
        /// </summary>
        public ListFileInfoViewModel GetListFile(int personalId)
        {
            var result = _context.Files.Where(f => f.PersonalId == personalId && f.MeetingId != null);


            ListFileInfoViewModel list = new ListFileInfoViewModel() { };

            list.files = result.Select(t => new FileInfoViewModel()
            {

                Id = t.Id,
                AddUserId = t.Personal.RegUserId,
                FirstName = t.Personal.FirstName,
                LastName = t.Personal.LastName,
                PersonalCode = t.Personal.PersonalCode,
                PersonalAvatar = t.Personal.PersonalAvatar,
                MelliCode = t.Personal.MelliCode,
                ReqSubTitle = t.RequestSubject.Title,
                FileStatusTitle = t.FileStatus.Title,
                PriorityTitle = t.Priority.Title,
                RankTitle = t.RankTitle,
                JobDes = t.JobDes,
                Phone = t.Phone,
                StatuseTitle = t.StatusTitle,
                BranchTitle = t.BranchTitle,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                RegDate = t.RegDate,
                RegDateFa = t.RegDate.ToShamsi(),
                FinalHameshDesc = _context.Hameshes.Where(x => x.FileId == result.Select(x => x.Id).SingleOrDefault() && x.RoleTypeId == 9).Select(x => x.UserDesc).SingleOrDefault(),

            }).OrderByDescending(u => u.RegDate).ToList();

            return list;
        }


        /// <summary>
        /// تعداد فایل های ثبت شده
        /// </summary>
        /// <param name="unitDutyCode"></param>
        /// <param name="unitCode"></param>
        /// <param name="codeGha"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="personalCode"></param>
        /// <returns></returns>
        public int GetFileCount(string unitDutyCode, string unitCode, string codeGha, string roleTypeId, string personalCode)
        {

            var fileCount = 0;

            switch (roleTypeId)
            {
                //کاربر عادی
                case "1":
                    fileCount = _context.Files.Where(f => f.Personal.PersonalCode == personalCode).Count();
                    break;

                //ف یگان مستقیم
                case "2":
                    fileCount = _context.Files.Where(f => f.Personal.UnitDutyCode == int.Parse(unitDutyCode)).Count();
                    break;

                //ف یگان عمده
                case "3":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //ف یگان عمده(هوانیروز)
                case "12":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //ف یگان عمده مراکز آموزشی
                case "601":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //ف بیمارستان یگان
                case "201":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //ف بیمارستان تهران
                case "203":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //کارشناس مراکز آموزشی
                case "600":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //کارشناس بیمارستان یگان
                case "200":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //کارشناس بیمارستان تهران
                case "202":
                    fileCount = _context.Files.Where(f => f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                //ف قرارگاه/ارشد نظامی
                case "4":
                    fileCount = _context.Files.Where(f => f.Personal.CodGha == int.Parse(codeGha)).Count();
                    break;


                default:
                    fileCount = _context.Files.Count();
                    break;
            }


            return fileCount;
        }

        /// <summary>
        /// محاسبه تعداد درخواست‌های اقدام‌شده بر اساس نقش و محدوده دسترسی.
        /// </summary>
        public int GetFileCountEghdamShode(string unitDutyCode, string unitCode, string codeGha, string roleTypeId, string personalCode)
        {
            var fileCount = 0;

            switch (roleTypeId)
            {
                case "1":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 1 && f.Personal.PersonalCode == personalCode).Count();
                    break;

                case "2":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 1 && f.Personal.UnitDutyCode == int.Parse(unitDutyCode)).Count();
                    break;

                case "3":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 1 && f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                case "4":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 1 && f.Personal.CodGha == int.Parse(codeGha)).Count();
                    break;

                //case "5":
                //    fileCount = _context.Files.Where(f => f.FileStatusId == 2).Count();
                //    break;

                default:
                    fileCount = _context.Files.Where(f => f.FileStatusId == 1).Count();
                    break;
            }

            //var fileCount = _context.Files.Where(f => f.FileStatusId == 2).Count();

            return fileCount;
        }
        /// <summary>
        /// محاسبه تعداد درخواست‌های ثبت نظریه بر اساس نقش و محدوده دسترسی.
        /// </summary>
        public int GetFileCountSabteNazariye(string unitDutyCode, string unitCode, string codeGha, string roleTypeId, string personalCode)
        {
            var fileCount = 0;

            switch (roleTypeId)
            {
                case "1":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 2 && f.Personal.PersonalCode == personalCode).Count();
                    break;

                case "2":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 2 && f.Personal.UnitDutyCode == int.Parse(unitDutyCode)).Count();
                    break;

                case "3":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 2 && f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                case "4":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 2 && f.Personal.CodGha == int.Parse(codeGha)).Count();
                    break;

                //case "5":
                //    fileCount = _context.Files.Where(f => f.FileStatusId == 2).Count();
                //    break;

                default:
                    fileCount = _context.Files.Where(f => f.FileStatusId == 2).Count();
                    break;
            }

            //var fileCount = _context.Files.Where(f => f.FileStatusId == 2).Count();

            return fileCount;
        }
        /// <summary>
        /// محاسبه تعداد درخواست‌های رد یا عودت‌شده بر اساس نقش و محدوده دسترسی.
        /// </summary>
        public int GetFileCountRadeDarkhastVaAodat(string unitDutyCode, string unitCode, string codeGha, string roleTypeId, string personalCode)
        {
            var fileCount = 0;

            switch (roleTypeId)
            {
                case "1":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 3 && f.Personal.PersonalCode == personalCode).Count();
                    break;

                case "2":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 3 && f.Personal.UnitDutyCode == int.Parse(unitDutyCode)).Count();
                    break;

                case "3":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 3 && f.Personal.UnitCode == int.Parse(unitCode)).Count();
                    break;

                case "4":
                    fileCount = _context.Files.Where(f => f.FileStatusId == 3 && f.Personal.CodGha == int.Parse(codeGha)).Count();
                    break;

                //case "5":
                //    fileCount = _context.Files.Where(f => f.FileStatusId == 3).Count();
                //    break;

                default:
                    fileCount = _context.Files.Where(f => f.FileStatusId == 3).Count();
                    break;
            }

            //var fileCount = _context.Files.Where(f => f.FileStatusId == 3).Count();

            return fileCount;
        }
        //public int GetFileCountDarEntezar()
        //{
        //    var fileCount = _context.Files.Where(f => f.FileStatusId == 5).Count();
        //    return fileCount;
        //}
        //public int GetFileSabtDarLsitMolaghat()
        //{
        //    var fileCount = _context.Files.Where(f => f.FileStatusId == 5).Count();
        //    return fileCount;
        //}


        #endregion

        #region edit file
        /// <summary>
        /// ویرایش اطلاعات درخواست ملاقات.
        /// </summary>
        public BaseResult EditFile(EditFactPersonalViewModel file)
        {
            var editfile = GetFileByFileId(file.Id);
            var codGhaTitle = _webApiService.GetGharargah().Data
                .Where(x => x.Id == file.CodGha)
                .Select(x => x.Title)
                .FirstOrDefault();


            #region پر کردن ویو مدل
            editfile.FileTypeId = file.FileTypeId;
            editfile.PersonalCode = file.PersonalCode;
            editfile.Personal.MelliCode = file.MelliCode;
            editfile.Personal.FirstName = file.FirstName;
            editfile.Personal.LastName = file.LastName;
            editfile.Personal.RankTitle = file.RankTitle;
            editfile.Personal.BranchTitle = file.BranchTitle;
            editfile.Personal.DRSAD_JA = file.DRSAD_JA;
            editfile.Personal.DRSAD_JB = file.DRSAD_JB;
            editfile.Personal.IsarStatus = file.IsarStatus;
            editfile.Personal.TOT_AML2 = file.TOT_AML2;
            editfile.Personal.TOT_AML = file.TOT_AML;
            editfile.UnitDutyTitle = file.UnitDutyTitle;
            editfile.UnitTitle = file.UnitTitle;
            editfile.CodGha = file.CodGha.Value;
            editfile.CodGhaTitle = codGhaTitle;
            editfile.RequestSubjectId = file.RequestSubjectId;
            editfile.PriorityId = file.PriorityId;
            editfile.FileStatusId = file.FileStatusId;
            editfile.Addres = file.Addres;
            editfile.Phone = file.Phone;
            editfile.Attachment = file.AttachmentFileName;
            editfile.FishAttachment = file.FishAttacmentFileName;
            editfile.RequestDescription = file.RequestDescription;
            editfile.ProblemDescription = file.ProblemDescription;
            editfile.EditDate = DateTime.Now;
            editfile.EditUserId = file.EditUserId;
            #region Save File

            if (file.Attachment != null)
            {
                string imagePath = "";
                editfile.Attachment = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.Attachment.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FileAttachment", editfile.Attachment);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    file.Attachment.CopyTo(stream);
                }
            }
            if (file.Attachment == null)
            {
                editfile.Attachment = "Default.png";
            }

            if (file.FishAttachmnet != null)
            {
                string imagePath = "";
                editfile.FishAttachment = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.FishAttachmnet.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FishAttachment", editfile.FishAttachment);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    file.FishAttachmnet.CopyTo(stream);
                }
            }
            if (file.FishAttachmnet == null)
            {
                editfile.FishAttachment = "Default.png";
            }
            #endregion
            #endregion

            return UpdateFile(editfile);
        }


        /// <summary>
        /// به‌روزرسانی فایل در دیتابیس.
        /// </summary>
        public BaseResult UpdateFile(Files file)
        {
            _context.Update(file);
            var res = _context.SaveChanges();

            if (res == 1)
            {
                return new BaseResult()
                {
                    Message = ",ویرایش درخواست ملاقات با موفقیت انجام شد",
                    Model = file,
                    Status = true
                };
            }

            return new BaseResult()
            {
                Message = ",ویرایش درخواست ملاقات با خطا مواجه شد",
                Model = file,
                Status = false
            };
        }


        #endregion

        #region edit personal
        /// <summary>
        /// ویرایش اطلاعات پرسنل.
        /// </summary>
        public BaseResult EditPersonal(EditFactPersonalViewModel personal)
        {
            var editpersonal = GetPersonalByPersonalId(personal.PersonalId);
            #region پر کردن ویو مدل
            editpersonal.PersonalCode = personal.PersonalCode;
            editpersonal.MelliCode = personal.MelliCode;
            editpersonal.FirstName = personal.FirstName;
            editpersonal.LastName = personal.LastName;
            editpersonal.RankTitle = personal.RankTitle;
            editpersonal.BranchTitle = personal.BranchTitle;
            editpersonal.DRSAD_JA = personal.DRSAD_JA;
            editpersonal.DRSAD_JB = personal.DRSAD_JB;
            editpersonal.IsarStatus = personal.IsarStatus;
            editpersonal.TOT_AML2 = personal.TOT_AML2;
            editpersonal.TOT_AML = personal.TOT_AML;
            editpersonal.UnitDutyTitle = personal.UnitDutyTitle;
            editpersonal.UnitTitle = personal.UnitTitle;
            editpersonal.CodGhaTitle = personal.CodGhaTitle;
            editpersonal.Addres = personal.Addres;
            editpersonal.Phone = personal.Phone;
            editpersonal.EditDate = DateTime.Now;
            editpersonal.EditUserId = personal.EditUserId;
            #region Save Avatar

            if (personal.PersonalAvatar != null)
            {
                string imagePath = "";
                if (personal.PersonalAvatarName != "Default.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PersonalAvatar", personal.PersonalAvatarName);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }

                }

                editpersonal.PersonalAvatar = "Default.png";
                editpersonal.PersonalAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(personal.PersonalAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PersonalAvatar", editpersonal.PersonalAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    personal.PersonalAvatar.CopyTo(stream);
                }


            }


            #endregion
            #endregion

            return UpdatePersonal(editpersonal);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات پرسنل در دیتابیس.
        /// </summary>
        public BaseResult UpdatePersonal(Personal personal)
        {
            _context.Update(personal);
            var res = _context.SaveChanges();

            if (res == 1)
            {
                return new BaseResult()
                {
                    Message = ",ویرایش موفق",
                    Model = personal,
                    Status = true
                };
            }

            return new BaseResult()
            {
                Message = ",ویرایش ناموفق",
                Model = personal,
                Status = false
            };

        }

        /// <summary>
        /// دریافت پرسنل بر اساس شناسه پرسنل.
        /// </summary>
        public Personal GetPersonalByPersonalId(int PersonalId)
        {
            return _context.Personals.Where(p => p.Id == PersonalId).SingleOrDefault();
        }


        #endregion

        #region Delete File

        /// <summary>
        /// دریافت اطلاعات خلاصه درخواست برای عملیات حذف.
        /// </summary>
        public DeleteFactPersonalViewModel GetFileInformation(int FileId)
        {
            var file = GetFileByFileId(FileId);
            DeleteFactPersonalViewModel information = new DeleteFactPersonalViewModel()
            {
                Id = file.Id,
                PersonalCode = file.PersonalCode,
                FirstName = file.FirstName,
                LastName = file.LastName,
                ReqSubTitle = file.RequestSubject.Title,

            };

            return information;
        }


        /// <summary>
        /// حذف منطقی درخواست ملاقات.
        /// </summary>
        public void DeleteFile(int Id)
        {
            var file = GetFileByFileId(Id);

            file.IsDelete = true;

            UpdateFile(file);
        }


        /// <summary>
        /// دریافت کدهای پرسنلی جهت جستجوی AutoComplete.
        /// </summary>
        public List<string> GetFileForAutoCompliteSearch(string term)
        {
            return _context.Files.Where(p => p.Personal.PersonalCode.Contains(term)).Select(p => p.PersonalCode).Distinct().ToList();
        }

        #endregion

        #region UserAccess
        /// <summary>
        /// دریافت لیست نقش‌ها.
        /// </summary>
        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        /// <summary>
        /// دریافت نقش بر اساس نوع نقش.
        /// </summary>
        public Role GetRoleTitleByRoleType(int roleTypeId)
        {
            var res = _context.Roles.Where(x => x.RoleType == roleTypeId).SingleOrDefault();

            return res;
        }

        /// <summary>
        /// دریافت نقش‌های مربوط به معاونت‌ها.
        /// </summary>
        public List<Role> GetRolesJustMooavenatHa()
        {
            return _context.Roles.Where(x => x.RoleType == 5 || x.RoleType == 6 || x.RoleType == 7).OrderBy(x => x.RoleType).ToList();
        }

        /// <summary>
        /// دریافت شناسه فایل بر اساس شناسه فایل.
        /// </summary>
        public int GetFileIdByFileId(int FileId)
        {
            return _context.Files.Where(u => u.Id == FileId).Select(u => u.Id).SingleOrDefault();
        }

        /// <summary>
        /// دریافت شناسه فایل‌ها بر اساس شناسه جلسه.
        /// </summary>
        public List<int> GetFileIdByMeetingId(int meetingId)
        {
            return _context.Files.Where(u => u.MeetingId == meetingId).Select(u => u.Id).ToList();
        }

        /// <summary>
        /// دریافت شناسه پرسنل بر اساس شناسه فایل.
        /// </summary>
        public int GetPersonalIdByFileId(int fileId)
        {
            return _context.Files.Where(u => u.Id == fileId).Select(u => u.PersonalId).SingleOrDefault();
        }

        /// <summary>
        /// ثبت فایل صوتی جلسه برای درخواست.
        /// </summary>
        public BaseResult AddVoiceRecordToFile(IFormFile voiceRecord, int fileId)
        {
            var File = GetFile(fileId);

            #region Save File

            if (voiceRecord != null)
            {
                string imagePath = "";
                File.VoiceRecord = NameGenerator.GenerateUniqCode() + Path.GetExtension(voiceRecord.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/VoiceRecords", File.VoiceRecord);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    voiceRecord.CopyTo(stream);
                }
            }
            else
            {
                File.VoiceRecord = "";
            }


            #endregion

            _context.Update(File);
            var res = _context.SaveChanges();

            if (res > 0)
            {
                return new BaseResult
                {
                    Message = "ویس جلسه با موفقیت ثبت گردید",
                    Status = true
                };
            }
            return new BaseResult
            {
                Message = "ویس جلسه با خطا مواجه گردید",
                Status = false
            };

        }


        #endregion

        #region Update Picture Personel When Create File
        /// <summary>
        /// تغییر تصویر پرسنل هنگام ثبت درخواست.
        /// </summary>
        public void ChangePicturePersonelWhenCreateFile(IFormFile personalAvatar, int personalId)
        {
            var Person = _context.Personals.Where(x => x.Id == personalId).SingleOrDefault();
            #region Save Avatar

            if (personalAvatar != null)
            {
                string imagePath = "";
                Person.PersonalAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(personalAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PersonalAvatar", Person.PersonalAvatar);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    personalAvatar.CopyTo(stream);
                }
            }
            else
            {
                Person.PersonalAvatar = "";
            }
            _context.Update(Person);
            _context.SaveChanges();
            #endregion

        }


        #endregion

        /// <summary>
        /// ثبت لیست پیوست‌های دستور برای درخواست.
        /// </summary>
        public BaseResult AddListAttachDastorToFile(List<IFormFile> listAttachDastor, int fileId)
        {
            // var File = GetFile(fileId);
            var attachDastor = new FileAttachment();

            #region Save File

            if (listAttachDastor != null)
            {

                foreach (var item in listAttachDastor)
                {
                    attachDastor.FileId = fileId;
                    string imagePath = "";
                    attachDastor.FileUplodeAttacmentDastor = NameGenerator.GenerateUniqCode() + Path.GetExtension(item.FileName);
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AttachDastor", attachDastor.FileUplodeAttacmentDastor);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }
                }

            }
            else
            {
                attachDastor.FileUplodeAttacmentDastor = "";
            }
            _context.FileAttachments.Add(attachDastor);
            var res = _context.SaveChanges();

            #endregion

            if (res > 0)
            {
                return new BaseResult
                {
                    Message = "ثبت موفق",
                    Status = true
                };
            }

            return new BaseResult
            {
                Message = "ثبت ناموفق",
                Status = false
            };
        }

        /// <summary>
        /// فعال‌سازی وضعیت برگزاری جلسه برای درخواست.
        /// </summary>
        public BaseResult ActiveFiledMettingHoldFile(int fileId)
        {
            var file = _context.Files.Where(x => x.Id == fileId).FirstOrDefault();
            file.IsMeetingHold = true;
            _context.Update(file);
            var res = _context.SaveChanges();

            if (res > 0)
            {
                return new BaseResult
                {
                    Message = "وضعیت برگزاری جلسه با موفقیت ثبت شد",
                    Status = true
                };
            }
            return new BaseResult
            {
                Message = "وضعیت برگزاری جلسه با خطا مواجه  شد",
                Status = false
            };


        }


        /// <summary>
        /// دریافت لیست درخواست‌های کارتابل معاونت برای پایش.
        /// </summary>
        public ListFileViewModel GetListFileForPayeshMoavenat(int filterMoavenat = 0)
        {
            // دریافت شناسه کاربر بر اساس نقش
            var userId = _context.UserRoles.Include(x => x.User)
                .Where(x => x.RoleId == filterMoavenat && !x.User.IsDelete)
                .Select(x => x.UserId)
                .FirstOrDefault();

            // فیلتر کردن فایل‌ها در کارتابل
            var filesInCartable = _context.Cartables
                .Include(x => x.File)
                    .ThenInclude(x => x.Personal)
                    .Include(x => x.File.RequestSubject)
                    .Include(x => x.File.FileStatus)
                    .Include(x => x.File.Priority)
                .Where(x => x.RcvrUserId == userId && !x.File.IsDelete && !x.File.IsArchived && !x.IsDone)
                .ToList() // تبدیل به لیست برای پردازش در حافظه
                .GroupBy(t => t.FileId) // گروه‌بندی بر اساس FileId
                .Select(g => g.FirstOrDefault()) // انتخاب اولین رکورد از هر گروه
                .Select(t => new FactPersonalViewModel
                {
                    Id = t.File.Id,
                    AddUserId = t.File.Personal.RegUserId,
                    FirstName = t.File.Personal.FirstName,
                    LastName = t.File.Personal.LastName,
                    PersonalCode = t.File.Personal.PersonalCode,
                    MelliCode = t.File.Personal.MelliCode,
                    ReqSubTitle = t.File.RequestSubject.Title,
                    FileStatusTitle = t.File.FileStatus.Title,
                    PriorityTitle = t.File.Priority.Title,
                    RankTitle = t.File.RankTitle,
                    JobDes = t.File.JobDes,
                    Phone = t.File.Phone,
                    StatuseTitle = t.File.StatusTitle,
                    BranchTitle = t.File.BranchTitle,
                    UnitDutyTitle = t.File.UnitDutyTitle,
                    UnitTitle = t.File.UnitTitle,
                    CodGhaTitle = t.File.CodGhaTitle,
                    RegDate = t.RegDate
                })
                .OrderByDescending(u => u.RegDate)
                .ToList();

            // ایجاد مدل نتیجه
            var list1 = new ListFileViewModel
            {
                count = filesInCartable.Count,
                PageCount = (int)Math.Ceiling(filesInCartable.Count / 10.0), // تعداد صفحات
                files = filesInCartable
            };

            return list1;
        }


        /// <summary>
        /// دریافت لیست درخواست‌ها بدون اعمال فیلتر.
        /// </summary>
        public ListFileViewModel GetListFileWithoutFilter()
        {


            //var filesInCartable = _context.Cartables.Where(x => x.RcvrUserId == userId).ToList();

            #region MyRegion
            IQueryable<Cartable> files = _context.Cartables.Include(f => f.File);
            var take1 = 10;
            //var skip1 = (pageId - 1) * take1;

            ListFileViewModel list1 = new ListFileViewModel() { };
            //list1.CurrentPage = pageId;
            //list1.skip = skip1;
            list1.count = files.Count();
            list1.PageCount = (int)Math.Ceiling(files.Count() / (double)take1);// result.Count() / take;

            list1.files = files.Select(t => new FactPersonalViewModel()
            {
                Id = t.File.Id,
                AddUserId = t.File.Personal.RegUserId,
                FirstName = t.File.Personal.FirstName,
                LastName = t.File.Personal.LastName,
                PersonalCode = t.File.Personal.PersonalCode,
                MelliCode = t.File.Personal.MelliCode,
                ReqSubTitle = t.File.RequestSubject.Title,
                FileStatusTitle = t.File.FileStatus.Title,
                PriorityTitle = t.File.Priority.Title,
                RankTitle = t.File.RankTitle,
                JobDes = t.File.JobDes,
                Phone = t.File.Phone,
                StatuseTitle = t.File.StatusTitle,
                BranchTitle = t.File.BranchTitle,
                UnitDutyTitle = t.File.UnitDutyTitle,
                UnitTitle = t.File.UnitTitle,
                CodGhaTitle = t.File.CodGhaTitle,
                RegDate = t.RegDate
            }).OrderByDescending(u => u.RegDate).ToList();
            #endregion

            return list1;
        }

        /// <summary>
        /// تعداد درخواست های داخل کارتابل معاونت ها
        /// </summary>
        /// <returns></returns>
        public ListCountCartableMoavenat GetListCountCartableMoavenat1()
        {
            var model = new ListCountCartableMoavenat();
            ///****************************************************************************
            var KarshenasGharargahAnsarNezajaUserId = _context.Roles.Include(x => x.UserRoles)
                .ThenInclude(x => x.User).Where(x => x.RoleType == 5)
                .Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var countKartableKarshenasGharargahAnsar = _context.Cartables.Include(x => x.File)
                .Where(x => x.RcvrUserId == KarshenasGharargahAnsarNezajaUserId.FirstOrDefault()
                && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false)
                .Count();

            var countKartableKarshenasGharargahAnsar2 = _context.Cartables.Include(x => x.File)
                .Where(x => x.RcvrUserId == KarshenasGharargahAnsarNezajaUserId.FirstOrDefault()
                && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false)
                .ToList();
            //******************************************************************************

            var KarbarNezajaUserId = _context.Roles.Include(x => x.UserRoles)
                .ThenInclude(x => x.User).Where(x => x.RoleType == 7)
                .Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountKarbarNezaja = _context.Cartables.Include(x => x.File)
                .Where(x => x.RcvrUserId == KarbarNezajaUserId.FirstOrDefault()
                && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false)
                .Count();

            //******************************************************************************

            var MNEnsaniUserId = _context.Roles.Include(x => x.UserRoles)
                    .ThenInclude(x => x.User).Where(x => x.RoleId == 8)
                    .Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMNEnsani = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MNEnsaniUserId.FirstOrDefault() &&
                 x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false)
                 .Count();

            //******************************************************************************

            var MMohandesiUserId = _context.Roles.Include(x => x.UserRoles)
        .ThenInclude(x => x.User).Where(x => x.RoleId == 9)
        .Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMMohandesi = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MMohandesiUserId.FirstOrDefault()
                 && x.File.IsDelete == false && x.File.IsArchived == false
                 && x.IsDone == false)
                 .Count();

            //******************************************************************************

            var MTarhVaBarnamehUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 16)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMTarhVaBarnameh = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MTarhVaBarnamehUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var MAmozeshUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 18)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMAmozesh = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MAmozeshUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var MAmadVaPoshUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 20)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMAmadVaPosh = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MAmadVaPoshUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var MHoghoghiVaGhazayiUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 21)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountMHoghoghiVaGhazayi = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == MHoghoghiVaGhazayiUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************


            var BazresiNezajaUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 19)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountBazresiNezaja = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == BazresiNezajaUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var FHavapeymayiUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 32)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountFHavapeymayi = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == FHavapeymayiUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var DarayiUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 35)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountDarayi = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == DarayiUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************


            var IsargaranUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 28)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountIsargaran = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == IsargaranUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            var BehdashtUserId = _context.Roles.Include(x => x.UserRoles)
.ThenInclude(x => x.User).Where(x => x.RoleId == 27)
.Select(x => x.UserRoles.Select(x => x.UserId)).FirstOrDefault();

            var CountBehdasht = _context.Cartables.Include(x => x.File)
                 .Where(x => x.RcvrUserId == BehdashtUserId.FirstOrDefault() && x.File.IsDelete == false && x.File.IsArchived == false && x.IsDone == false).Count();

            //******************************************************************************

            model.CountKarshenasGharargahAnsarNezaja = countKartableKarshenasGharargahAnsar;
            model.CountKarbarNezaja = CountKarbarNezaja;
            model.CountMNEnsani = CountMNEnsani;
            model.CountMMohandesi = CountMMohandesi;
            model.CountMTarhVaBarnameh = CountMTarhVaBarnameh;
            model.CountMAmozesh = CountMAmozesh;
            model.CountMAmadVaPosh = CountMAmadVaPosh;
            model.CountMHoghoghiVaGhazayi = CountMHoghoghiVaGhazayi;
            model.CountFHavapeymayi = CountFHavapeymayi;
            model.CountDarayi = CountDarayi;
            model.CountIsargaran = CountIsargaran;
            model.CountBehdasht = CountBehdasht;
            model.CountBazresiNezaja = CountBazresiNezaja;

            return model;
        }

        /// <summary>
        /// دریافت تعداد درخواست‌های داخل کارتابل معاونت‌ها.
        /// </summary>
        public ListCountCartableMoavenat GetListCountCartableMoavenat()
        {
            var model = new ListCountCartableMoavenat();

            var rolesWithCartableCounts = new Dictionary<int, Action<int>>
    {
        { 132, count => model.CountKarshenasGharargahAnsarNezaja = count },
        { 120, count => model.CountKarbarNezaja = count },//رئیس دفتر 3
        { 416, count => model.CountMNEnsani = count },//م ن انسانی
        { 127, count => model.CountMMohandesi = count },// م مهندسی
        { 413, count => model.CountMTarhVaBarnameh = count },//م طرح
        { 128, count => model.CountMAmozesh = count },//م آموزش
        { 130, count => model.CountMAmadVaPosh = count },//آماد و پش
        { 131, count => model.CountMHoghoghiVaGhazayi = count },//قضایی
        { 4716, count => model.CountFHavapeymayi = count },//هواپیمایی
        { 1616, count => model.CountDarayi = count },//دارایی
        { 147, count => model.CountIsargaran = count },//ایثارگران
       // { 1617, count => model.CountBehdasht = count },//بهداشت
        { 146, count => model.CountBehdasht = count },//بهداشت
        { 526, count => model.CountBazresiNezaja = count }//بازرسی
    };

            foreach (var userId in rolesWithCartableCounts)
            {
                var userIds = _context.Users
                    .Where(x => x.Id == userId.Key)
                    .FirstOrDefault();

                var count = _context.Cartables
                    .Include(x => x.File)
                    .Where(x => x.RcvrUserId == userId.Key && x.File.IsDelete == false && x.File.IsArchived == false)
                    .Select(x => x.FileId)
                    .Distinct()
                    .Count();

                userId.Value(count);
            }

            return model;
        }


        /// <summary>
        /// ویرایش فایل هنگام ثبت پاسخ معاونت.
        /// </summary>
        public void EditFileWhenMoavenatAnswerToFile(int fileId)
        {
            var file = GetFileByFileId(fileId);
            file.IsMoavenatAnswered = true;
            _context.Update(file);
            // _context.SaveChanges();
        }


        #region تغییر وضعیت درخواست ملاقات برای آرشیو
        /// <summary>
        /// آرشیو کردن درخواست و تکمیل کارتابل کاربر.
        /// </summary>
        public void ArchivedFile(int fileId, int userId)
        {
            var file = GetFileByFileId(fileId);

            file.IsArchived = true;
            file.ArchivedRegUserId = userId;

            UpdateFile(file);
        }


        /// <summary>
        /// اضافه کردن میزان مبلغ وام درخواستی و محقق شده به در خواست ملاقات نفر
        /// </summary>
        /// <param name="fileId"></param>
        public BaseResult addMablaghVamDarkhastiVaVamMohaghahShode(int fileId, double? MablaghVamDarkhasti, double? MablaghVamMohaghaghSode)
        {

            var file = GetFileByFileId(fileId);

            file.SumMablaghVamDarkhasti = MablaghVamDarkhasti;
            file.MablaghVamMohaghaghSode = MablaghVamMohaghaghSode;

            var res = UpdateFile(file);

            return new BaseResult
            {
                Message = res.Message,
                Status = res.Status
            };
        }

        /// <summary>
        /// دریافت لیست درخواست‌های آرشیوشده.
        /// </summary>
        public ListFileViewModel GetListArchivedFile(int userId, int requestsubject = 0,
            int filterAvamerSadereh = 0,
            string filterGharargah = "",
            string filterCaption = "")
        {

            var listFileId = _context.Hameshes
                .Include(x => x.File)
                .Where(x => x.UserId == userId && x.File.FileTypeId == 1).Select(x => x.FileId).Distinct().ToList();

            IQueryable<Files> listFile = _context.Files.Where(x => listFileId.Contains(x.Id) && x.IsArchived);

            #region Search

            if (!string.IsNullOrWhiteSpace(filterCaption))
            {
                var term = filterCaption.Trim();
                listFile = listFile.Where(u => u.Personal.PersonalCode.Contains(term) ||
                    u.Personal.FirstName.Contains(term) || u.Personal.LastName.Contains(term));
            }

            if (int.TryParse(filterGharargah, out var gharargahId) && gharargahId > 0)
                listFile = listFile.Where(t => t.CodGha == gharargahId);

            if (requestsubject > 0)
                listFile = listFile.Where(t => t.RequestSubjectId == requestsubject);

            if (filterAvamerSadereh == 1)
                listFile = listFile.Where(t => t.IsMeetingHold);
            #endregion

            ListFileViewModel list = new ListFileViewModel() { };

            list.count = listFile.Count();
            list.files = listFile.Select(t => new FactPersonalViewModel()
            {
                Id = t.Id,
                AddUserId = t.Personal.RegUserId,
                FirstName = t.Personal.FirstName,
                LastName = t.Personal.LastName,
                PersonalCode = t.Personal.PersonalCode,
                MelliCode = t.Personal.MelliCode,
                ReqSubTitle = t.RequestSubject.Title,
                FileStatusTitle = t.FileStatus.Title,
                PriorityTitle = t.Priority.Title,
                RankTitle = t.RankTitle,
                JobDes = t.JobDes,
                Phone = t.Phone,
                StatuseTitle = t.StatusTitle,
                BranchTitle = t.BranchTitle,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.CodGhaTitle,
                RegDate = t.RegDate,
                IsMeetingHold = t.IsMeetingHold,
                IsAnswerdMoavenat = t.IsMoavenatAnswered,
                IsArchived = t.IsArchived,

            })
                .Distinct()
                .OrderByDescending(u => u.RegDate)
                .ToList();

            return list;
        }


        /// <summary>
        /// ویرایش نوع اقدام - ثبت وام محقق شده - فیلد ارسال به معاونت ملاقات
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="actionTypeId"></param>
        /// <returns></returns>
        public BaseResult EditFileWhenSendHamesh(int fileId, int actionTypeId, double? mablaghVamDarkhasti, double? mablaghVamMohaghaghShode, int roleTypeId)
        {
            var file = GetFileByFileId(fileId);

            file.ActionTypeId = actionTypeId;
            file.SumMablaghVamDarkhasti = mablaghVamDarkhasti;
            file.MablaghVamMohaghaghSode = mablaghVamMohaghaghShode;

            if (roleTypeId == 6)
            {
                file.IsMoavenatAnswered = true;
            }

            _context.Update(file);

            var result = _context.SaveChanges();

            if (result != 0)
            {
                return new BaseResult
                {
                    Message = "ویرایش  موفق",
                    Model = file,
                    Status = true
                };

            }

            return new BaseResult
            {
                Message = "ویرایش  ناموفق",
                Model = file,
                Status = false
            };

        }

        #endregion

    }
}
