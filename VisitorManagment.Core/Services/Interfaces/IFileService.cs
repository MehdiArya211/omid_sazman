using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IFileService
    {
        #region اطلاعات پایه و کمبوباکس‌ها

        /// <summary>
        /// دریافت لیست نوع درخواست‌ها
        /// </summary>
        List<FileType> GetListFileType();

        /// <summary>
        /// دریافت لیست موضوعات درخواست
        /// </summary>
        List<RequestSubject> GetRequestSubject();

        /// <summary>
        /// دریافت لیست اوامر صادره
        /// </summary>
        List<AvamerSadereh> GetAvamerSadereh();

        /// <summary>
        /// دریافت لیست وضعیت‌های درخواست
        /// </summary>
        List<FileStatus> GetFileStatus();

        /// <summary>
        /// دریافت لیست اولویت‌ها
        /// </summary>
        List<Priority> GetPriority();

        #endregion

        #region ثبت پرسنل و درخواست ملاقات

        /// <summary>
        /// افزودن یا به‌روزرسانی اطلاعات پرسنل هنگام ثبت درخواست
        /// </summary>
        int AddPersonToPersonal(FactPersonalViewModel person);

        /// <summary>
        /// دریافت اطلاعات پرسنل با شناسه پرسنل
        /// </summary>
        Personal GetPersonalByPersonalId(int PersonalId);

        /// <summary>
        /// ثبت درخواست ملاقات در جدول فایل
        /// </summary>
        BaseResult AddFile(FactPersonalViewModel person);

        /// <summary>
        /// افزودن مستقیم فایل به جدول فایل‌ها
        /// </summary>
        BaseResult AddToFile(Files file);

        #endregion

        #region فایل‌ها و پیوست‌ها

        /// <summary>
        /// ثبت فایل صوتی جلسه برای درخواست
        /// </summary>
        BaseResult AddVoiceRecordToFile(IFormFile voiceRecord, int fileId);

        /// <summary>
        /// فعال کردن وضعیت برگزاری جلسه در جدول فایل
        /// </summary>
        BaseResult ActiveFiledMettingHoldFile(int fileId);

        // void AddFileAttachDastorToFile(List<IFormFile> fileAttachDastor, int fileId);

        /// <summary>
        /// افزودن لیست پیوست‌های دستور به درخواست ملاقات
        /// </summary>
        BaseResult AddListAttachDastorToFile(List<IFormFile> listAttachDastor, int fileId);

        /// <summary>
        /// تغییر عکس پرسنل هنگام ایجاد درخواست
        /// </summary>
        void ChangePicturePersonelWhenCreateFile(IFormFile personalAvatar, int personalId);

        #endregion

        #region لیست درخواست‌ها

        /// <summary>
        /// دریافت لیست درخواست‌ها بر اساس نقش و کاربر گیرنده
        /// </summary>
        ListFileViewModel GetListFile(
            int roleTypeId,
            int rcvrUserId,
            int pageId = 1,
            int requestsubject = 0,
            int filterAvamerSadereh = 0,
            string filterGharargah = "",
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست همه درخواست‌ها بدون فیلتر
        /// </summary>
        ListFileViewModel GetListFileWithoutFilter();

        /// <summary>
        /// دریافت لیست درخواست‌هایی که جلسه آن‌ها برگزار شده است
        /// </summary>
        ListFileViewModel GetListFileWhenMetingHold(
            int roleTypeId,
            int rcvrUserId,
            int pageId = 1,
            int requestsubject = 0,
            string filterGharargah = "",
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست درخواست‌ها برای گردش پرونده
        /// </summary>
        ListFileViewModel GetListFileForCirculation(
            int userId,
            DateTime? startDateSearchFilter,
            DateTime? endDateSearchFilter,
            int unitCode,
            int pageId = 1,
            int requestsubject = 0,
            int filterGharargah = 0,
            int filterYegan = 0,
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست درخواست‌ها برای گردش پرونده در بخش ادمین
        /// </summary>
        ListFileViewModel GetListFileForCirculationAdmin(
            int rcvrUserId,
            DateTime? startDateSearchFilter,
            DateTime? endDateSearchFilter,
            int unitCode,
            int pageId = 1,
            int requestsubject = 0,
            int filterGharargah = 0,
            int filterYegan = 0,
            int filterMoavenat = 0,
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست درخواست‌های پایش معاونت
        /// </summary>
        ListFileViewModel GetListFileForPayeshMoavenat(int filterMoavenat = 0);

        /// <summary>
        /// دریافت لیست درخواست‌ها برای نمایش در پنل ادمین
        /// </summary>
        ListFileViewModel GetListFileForShowAdmin(
            int userId,
            int pageId = 1,
            int requestsubject = 0,
            int filterGharargah = 0,
            int filterYegan = 0,
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست فایل‌های آرشیو شده
        /// </summary>
        ListFileViewModel GetListArchivedFile(
            int userId,
            int requestsubject = 0,
            int filterAvamerSadereh = 0,
            string filterGharargah = "",
            string filterCaption = "");

        /// <summary>
        /// دریافت لیست درخواست‌های ثبت شده برای یک پرسنل
        /// </summary>
        ListFileInfoViewModel GetListFile(int personalId);

        #endregion

        #region دریافت اطلاعات درخواست

        /// <summary>
        /// دریافت درخواست با شناسه فایل همراه با اطلاعات مرتبط
        /// </summary>
        Files GetFileByFileId(int Id);

        /// <summary>
        /// دریافت درخواست با شناسه فایل
        /// </summary>
        Files GetFile(int Id);

        /// <summary>
        /// دریافت شناسه فایل بر اساس شناسه فایل
        /// </summary>
        int GetFileIdByFileId(int FileId);

        /// <summary>
        /// دریافت شناسه پرسنل بر اساس شناسه فایل
        /// </summary>
        int GetPersonalIdByFileId(int fileId);

        /// <summary>
        /// دریافت لیست شناسه فایل‌ها بر اساس شناسه جلسه
        /// </summary>
        List<int> GetFileIdByMeetingId(int meetingId);

        #endregion

        #region شمارش درخواست‌ها

        /// <summary>
        /// تعداد کل درخواست‌های ثبت شده بر اساس سطح دسترسی کاربر
        /// </summary>
        int GetFileCount(
            string unitDutyCode,
            string unitCode,
            string codeGha,
            string roleTypeId,
            string personalCode);

        /// <summary>
        /// تعداد درخواست‌های اقدام شده
        /// </summary>
        int GetFileCountEghdamShode(
            string unitDutyCode,
            string unitCode,
            string codeGha,
            string roleTypeId,
            string personalCode);

        /// <summary>
        /// تعداد درخواست‌های ثبت نظریه
        /// </summary>
        int GetFileCountSabteNazariye(
            string unitDutyCode,
            string unitCode,
            string codeGha,
            string roleTypeId,
            string personalCode);

        /// <summary>
        /// تعداد درخواست‌های رد درخواست و عودت
        /// </summary>
        int GetFileCountRadeDarkhastVaAodat(
            string unitDutyCode,
            string unitCode,
            string codeGha,
            string roleTypeId,
            string personalCode);

        // int GetFileCountDarEntezar();
        // int GetFileSabtDarLsitMolaghat();

        /// <summary>
        /// دریافت تعداد درخواست‌های داخل کارتابل معاونت‌ها
        /// </summary>
        ListCountCartableMoavenat GetListCountCartableMoavenat();

        #endregion

        #region ویرایش درخواست

        /// <summary>
        /// دریافت اطلاعات درخواست برای ویرایش
        /// </summary>
        EditFactPersonalViewModel GetFileForEdit(int Id);

        /// <summary>
        /// ویرایش وضعیت درخواست هنگام ثبت هامش
        /// </summary>
        void EditFileStatusIdWhenSabtHamesh(int fileId, int actionTypeId);

        /// <summary>
        /// ویرایش درخواست هنگام پاسخ معاونت
        /// </summary>
        void EditFileWhenMoavenatAnswerToFile(int fileId);

        /// <summary>
        /// ویرایش اطلاعات درخواست ملاقات
        /// </summary>
        BaseResult EditFile(EditFactPersonalViewModel file);

        /// <summary>
        /// به‌روزرسانی مستقیم موجودیت درخواست
        /// </summary>
        BaseResult UpdateFile(Files file);

        /// <summary>
        /// ویرایش نوع اقدام، ثبت وام محقق شده و فیلد ارسال به معاونت ملاقات
        /// </summary>
        BaseResult EditFileWhenSendHamesh(
            int fileId,
            int actionTypeId,
            double? mablaghVamDarkhasti,
            double? mablaghVamMohaghaghShode,
            int roleTypeId);

        #endregion

        #region ویرایش پرسنل

        /// <summary>
        /// ویرایش اطلاعات پرسنل مرتبط با درخواست
        /// </summary>
        BaseResult EditPersonal(EditFactPersonalViewModel personal);

        /// <summary>
        /// به‌روزرسانی مستقیم موجودیت پرسنل
        /// </summary>
        BaseResult UpdatePersonal(Personal personal);

        #endregion

        #region حذف و آرشیو درخواست

        /// <summary>
        /// دریافت اطلاعات درخواست برای حذف
        /// </summary>
        DeleteFactPersonalViewModel GetFileInformation(int FileId);

        /// <summary>
        /// حذف منطقی درخواست ملاقات
        /// </summary>
        void DeleteFile(int Id);

        /// <summary>
        /// آرشیو کردن درخواست ملاقات
        /// </summary>
        void ArchivedFile(int fileId, int userId);

        #endregion

        #region جستجو و تکمیل خودکار

        /// <summary>
        /// دریافت لیست کدهای پرسنلی برای تکمیل خودکار جستجو
        /// </summary>
        List<string> GetFileForAutoCompliteSearch(string term);

        #endregion

        #region دسترسی کاربران و نقش‌ها

        /// <summary>
        /// دریافت لیست نقش‌ها
        /// </summary>
        List<Role> GetRoles();

        /// <summary>
        /// دریافت نقش بر اساس نوع نقش
        /// </summary>
        Role GetRoleTitleByRoleType(int roleTypeId);

        /// <summary>
        /// دریافت لیست معاونت‌ها برای جستجوی پیشرفته
        /// </summary>
        List<Role> GetRolesJustMooavenatHa();

        #endregion

        #region وام

        /// <summary>
        /// افزودن مبلغ وام درخواستی و مبلغ وام محقق شده به درخواست ملاقات
        /// </summary>
        BaseResult addMablaghVamDarkhastiVaVamMohaghahShode(
            int fileId,
            double? MablaghVamDarkhasti,
            double? MablaghVamMohaghaghSode);

        #endregion
    }
}