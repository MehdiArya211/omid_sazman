using System;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IMeetingService
    {
        void CreateMeeting(int userId,Meeting meetingViewModel);
        List<MeetingPlace> GetMeetingPlace();
        List<BoseMeeting> GetBoseMeeting();
        List<ClerkMeeting> GetClerkMeeting();
        List<MeetingStatus> GetMeetingStatus();
        ListMeetingViewModel GetListMeeting(int pageId=1 ,int filterMeetingStatus=1, string filterCaption="");

        ListMeetingForHameshFormViewModel GetListMeetingForFormHamesh();
        void AddToMeeting(Meeting meeting);
        Meeting GetMeetingByMeetingId(int Id);
        int? GetMeetingIdByFileId(int Id);

        #region edit
        EditMeetingViewModel GetMeetingForEdit(int Id);
        void EditMeeting(EditMeetingViewModel meeting);

        void UpdateMeeting(Meeting meeting);

        #endregion

        #region Delete
        DeleteMeetingViewModel GetMeetingInformation(int meetingId);
        void DeleteMeeting(int meetingId);

        #endregion

        #region AutoCompleteSearch
        List<string> GetMeetingForAutoCompliteSearch(string term);
        #endregion

        #region Meeting Hold
        List<Meeting> GetMeetingList(int id);

        //اعضای جلسه
        ListMeetingHoldViewModel GetPersonalMemberForMeetingList(int meetingId , int pageId=1 , string filterCaption = "");

        List<Hamesh> getHmaeshByFileId(int personId, int meetingId);

        int GetFileIdByMeetingIdAndPersonId(int meetingId, int personId);


        #endregion

        #region ReferenceToMeeting
        List<FactPersonalReferencViewModel> GetCodGhaTitle();
        List<ListFileReferenceViewModel> GetMeetingTitle();
        ListFileReferenceViewModel GetListFileForReference(int rcvrUserId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0 , string filterGharargah = "");
        ListFileReferenceViewModel GetListFileForAddPersonToMeeting(int rcvrUserId,int meetingId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0);
        ListFileReferenceViewModel GetListFileForEditReference(int meetingId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0);
        ListFileReferenceViewModel GetFileForFinalApprovalMeeting(int meetingId);
        List<RequestSubject> GetRequestSubjects();
        void AddMeetingIdToFile(List<int> fileId, int MeetingId);
        /// <summary>
        /// اضافه کردن شناسه جلسه به جدول درخواست ملاقات ها
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="MeetingId"></param>
        BaseResult AddSingleMeetingIdToFile(int fileId, int MeetingId);
        /// <summary>
        /// لیست تمام ارگان های جلسه
        /// </summary>
        /// <returns></returns>
        List<OrganViewModelDto> GetListOrganMemberMeeting(int meetingId);
        void DeletePersonInMeeting(int fileId);
        void AddFinalApprovalMeeting(int meetingId, int userid);
        /// <summary>
        /// اضافه کردن نفرات به لیست جلسات
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="fileId"></param>
        BaseResult AddPersonToMemberMeeting(int meetingId, int fileId, int UnitCode, string unitTitle);



        #endregion

        #region get List File When MeetingId==Id(Meeting)
        List<Files> GetListFileByMeetingId(int meetingId);
        #endregion


        /// <summary>
        /// نمایش تمام جلسات فعال برای ارتباط تصویری
        /// </summary>
        /// <returns></returns>
        ListMeetingViewModel GetListMeetingForOnlineConversation();
        /// <summary>
        /// لیست شرکت کنندگان در جلسه برای ارتباط تصویری
        /// </summary>
        ListFileReferenceViewModel GetListFileForEditReferenceForOnlineConversation(int meetingId);

        #region تغییر وضعیت جلسه ملاقات 
        public BaseResult ChangeStatusMeeting(int meetingId);
        #endregion


        #region تغییر وضعیت نفر در لیست اعضای جلسه که جلسه ش رگزار شده است
        public BaseResult ChangeStatusPersonInMeeting(int fileId);

        #endregion


    }
}
