using ITOWebApiClient.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class MeetingService : IMeetingService
    {

        private string apiUrl = string.Empty;//"http://localhost:65504/SMSInfo";
        private string apiUrlCardIsar = string.Empty;
        private HttpClient _client;
        //******************************************************

        private readonly VisitorManagmentContext _context;

        public int GetHameshIdByFileIdAndUserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MeetingService(VisitorManagmentContext context)
        {
            _context = context;
            _client = new HttpClient();

            apiUrl = CustomSettings.Instance.ApiSmsUrl;

            apiUrlCardIsar = CustomSettings.Instance.ApiCardIsarUrl;
        }

        public List<MeetingPlace> GetMeetingPlace()
        {
            return _context.MeetingPlaces.ToList();
        }

        public void CreateMeeting(int userId, Meeting meetingViewModel)
        {
            var meeting = new Meeting();

            meeting.Title = meetingViewModel.Title;
            meeting.Caption = meetingViewModel.Caption;
            meeting.SortName = null;
            meeting.MeetingStatusId = meetingViewModel.MeetingStatusId;
            meeting.MeetingPlaceId = meetingViewModel.MeetingPlaceId;
            meeting.BoseMeetingId = meetingViewModel.BoseMeetingId;
            meeting.ClerkMeetingId = meetingViewModel.ClerkMeetingId;
            meeting.MeetingStatusId = meetingViewModel.MeetingStatusId;
            meeting.StartMeetingDate = meetingViewModel.StartMeetingDate;
            meeting.Description = meetingViewModel.Description;
            meeting.regUserId = userId;
            meeting.RegDate = DateTime.Now;
            meeting.StartMeetingTime = meetingViewModel.StartMeetingTime;

            AddToMeeting(meeting);
        }

        public List<ClerkMeeting> GetClerkMeeting()
        {
            return _context.ClerkMeetings.ToList();
        }

        public List<BoseMeeting> GetBoseMeeting()
        {
            return _context.BoseMeetings.ToList();
        }

        public void AddToMeeting(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            _context.SaveChanges();
        }

        public List<MeetingStatus> GetMeetingStatus()
        {
            return _context.MeetingStaus.ToList();
        }

        public ListMeetingViewModel GetListMeeting(int pageId = 1, int filterMeetingStatus = 1, string filterCaption = "")
        {

            IQueryable<Meeting> result = _context.Meetings;

            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.Title.Contains(filterCaption));
            }

            //if (filterMeetingStatus != 0)
            //{
            //    result = result.Where(u => u.MeetingStatusId == filterMeetingStatus);
            //}

            var take = 10;
            var skip = (pageId - 1) * take;

            ListMeetingViewModel list = new ListMeetingViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
                list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;

            list.Meetings = result.Select(t => new MeetingInfoViewModel()
            {
                Id = t.Id,
                Title = t.Title,
                Caption = t.Caption,
                MeetingStatusTitle = t.MeetingStaus.Title,
                MeetingStatusId = t.MeetingStaus.Id,
                StartMeetingDate = t.StartMeetingDate,
                MeetingPlaceTitle = t.MeetingPlace.Title,
                BoseMeetingTitle = t.BoseMeeting.FullName,
                ClerkMeetingTitle = t.ClerkMeeting.FullName,
                IsOkay = t.IsOkay,
                IsSend = t.IsSend,
                RegDate = t.RegDate

            }).OrderByDescending(u => u.RegDate).Skip(skip).Take(take).ToList();
            return list;
        }

        public int? GetMeetingIdByFileId(int Id)
        {
            return _context.Files.Where(f => f.Id == Id).Select(f => f.MeetingId).SingleOrDefault();
        }
        #region Edit Meeting
        public EditMeetingViewModel GetMeetingForEdit(int Id)
        {
            var meeting = GetMeetingByMeetingId(Id);
            var editfile = new EditMeetingViewModel()
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Caption = meeting.Caption,
                MeetingStatusId = meeting.MeetingStatusId,
                StartMeetingDate = meeting.StartMeetingDate,
                StartMeetingTime = meeting.StartMeetingTime,
                MeetingPlaceId = meeting.MeetingPlaceId,
                BoseMeetingId = meeting.BoseMeetingId,
                ClerkMeetingId = meeting.ClerkMeetingId,
            };
            return editfile;
        }

        public Meeting GetMeetingByMeetingId(int Id)
        {
            var meeting = _context.Meetings.Where(f => f.Id == Id).SingleOrDefault();

            return meeting;
        }

        public void EditMeeting(EditMeetingViewModel meeting)
        {
            var editmeeting = GetMeetingByMeetingId(meeting.Id);

            editmeeting.Title = meeting.Title;
            editmeeting.Caption = meeting.Caption;
            editmeeting.MeetingStatusId = meeting.MeetingStatusId;
            editmeeting.StartMeetingDate = meeting.StartMeetingDate;
            editmeeting.MeetingPlaceId = meeting.MeetingPlaceId;
            editmeeting.BoseMeetingId = meeting.BoseMeetingId;
            editmeeting.ClerkMeetingId = meeting.ClerkMeetingId;
            editmeeting.Description = meeting.Description;
            editmeeting.EditUserId = meeting.EditUserId;
            editmeeting.EditDate = DateTime.Now;
            editmeeting.StartMeetingDate = meeting.StartMeetingDate;
            editmeeting.StartMeetingTime = meeting.StartMeetingTime;
            UpdateMeeting(editmeeting);
        }

        public void UpdateMeeting(Meeting meeting)
        {
            _context.Update(meeting);
            _context.SaveChanges();
        }

        #endregion

        #region Delete
        public DeleteMeetingViewModel GetMeetingInformation(int meetingId)
        {
            var meeting = GetMeetingByMeetingId(meetingId);

            DeleteMeetingViewModel information = new DeleteMeetingViewModel()
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Caption = meeting.Caption,
            };

            return information;
        }

        public void DeleteMeeting(int MeetingId)
        {
            var meeting = GetMeetingByMeetingId(MeetingId);

            meeting.IsDelete = true;

            UpdateMeeting(meeting);
        }


        #endregion

        #region AutoCompleteSerach

        public List<string> GetMeetingForAutoCompliteSearch(string term)
        {
            return _context.Meetings.Where(p => p.Title.Contains(term)).Select(p => p.Title).ToList();
        }

        public List<Meeting> GetMeetingList(int id)
        {
            return _context.Meetings.Where(m => m.Id == id).ToList();
        }
        #endregion

        #region Meeting Hold
        //گرفتن اعضای جلسه
        public ListMeetingHoldViewModel GetPersonalMemberForMeetingList(int meetingId, int pageId = 1, string filterCaption = "")
        {
            //list files
            var fileList = _context.Files.Where(t => t.MeetingId == meetingId).Select(f => f.Id).ToList();
            IQueryable<Files> result = _context.Files.Where(t => fileList.Contains(t.Id));

            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.PersonalCode.Contains(filterCaption));
            }

            var take = 100;
            var skip = (pageId - 1) * take;

            ListMeetingHoldViewModel list = new ListMeetingHoldViewModel() { };
            list.CurrentPage = pageId;
            list.skip = skip;
            list.count = result.Count();
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;

            list.MeetingHolds = result.Select(t => new MeetingHoldInfoViewModel()
            {
                Id = meetingId,
                FileId = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                RankTitle = t.RankTitle,
                PersonalCode = t.PersonalCode,
                UnitDutyTitle = t.UnitDutyTitle,
                UnitTitle = t.UnitTitle,
                Phone = t.Phone,
                RegDate = t.RegDate,
                IsMeetingHold = t.IsMeetingHold

            }).OrderBy(u => u.LastName).Skip(skip).Take(take).ToList();

            return list;

        }

        public List<Hamesh> getHmaeshByFileId(int personId, int meetingId)
        {
            var fileId = _context.Files.Where(f => f.PersonalId == personId && f.MeetingId == meetingId).Select(f => f.Id).SingleOrDefault();
            var res = _context.Hameshes.Where(h => h.FileId == fileId).ToList();
            return res;
        }

        public int GetFileIdByMeetingIdAndPersonId(int meetingId, int personId)
        {
            return _context.Files.Where(f => f.PersonalId == personId && f.MeetingId == meetingId).Select(f => f.Id).SingleOrDefault();
        }

        #endregion

        #region ReferenceToMeeting

        public ListFileReferenceViewModel GetListFileForReference(int rcvrUserId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0, string filterGharargah = "")
        {
            IQueryable<Cartable> result = _context.Cartables.Include(f => f.File).Where(f => f.RcvrUserId == rcvrUserId && f.File.MeetingId == null);
            if (!string.IsNullOrEmpty(filterCaption))
            {
                result = result.Where(u => u.File.PersonalCode.Contains(filterCaption));
            }
            if (filterCodGhaTitle != 0)
            {
                result = result.Where(u => u.File.CodGha == filterCodGhaTitle);
            }
            if (filterGharargah != "" && filterGharargah != null)
            {
                result = result.Where(t => t.File.CodGha == int.Parse(filterGharargah));
            }
            if (SubjectId != 0)
            {
                result = result.Where(u => u.File.RequestSubjectId == SubjectId);
            }
            var take = 10;
            var skip = (pageId - 1) * take;

            ListFileReferenceViewModel list = new ListFileReferenceViewModel() { };
            list.CurrentPage = pageId;
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;

            list.files = result.Select(t => new FactPersonalReferencViewModel()
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
            }).OrderBy(u => u.LastName).Skip(skip).Take(take).ToList();
            return list;
        }

        public List<FactPersonalReferencViewModel> GetCodGhaTitle()
        {
            return _context.Files.Select(f => new FactPersonalReferencViewModel()
            {
                CodGha = f.CodGha,
                CodGhaTitle = f.CodGhaTitle
            }).Distinct().ToList();
        }

        public List<ListFileReferenceViewModel> GetMeetingTitle()
        {
            return _context.Meetings.Where(m => m.MeetingStatusId == 1).Select(f => new ListFileReferenceViewModel()
            {
                MeetingId = f.Id,
                MeetingTitle = f.Title
            }).ToList();
        }

        public List<RequestSubject> GetRequestSubjects()
        {
            return _context.RequestSubjects.ToList();
        }
        public void AddMeetingIdToFile(List<int> fileId, int MeetingId)
        {
            foreach (var item in fileId)
            {
                var result = _context.Files.SingleOrDefault(u => u.Id == item);
                result.MeetingId = MeetingId;
                _context.Update(result);
            }

            _context.SaveChanges();
        }

        public ListFileReferenceViewModel GetListFileForEditReference(int meetingId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0)
        {
            // rcvrUserId ===>>>UserId claim
            IQueryable<Files> result = _context.Files.Where(f => f.MeetingId == meetingId);
            //var take = 10;
            //var skip = (pageId - 1) * take;

            ListFileReferenceViewModel list = new ListFileReferenceViewModel() { };
            list.IsOkayMeeting = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).IsOkay;
            list.CurrentPage = pageId;
            list.MeetingId = meetingId;
            list.IsOkayDate = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).IsOkayDate;

            var IsOkayRegUserId = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).IsOkayRegUserId;
            if (IsOkayRegUserId != null)
            {
                var FullName = _context.Users.Where(u => u.Id == IsOkayRegUserId)
                    .Select(t => new ListFileReferenceViewModel() { FirstName = t.FirstName, LastName = t.LastName })
                    .FirstOrDefault();

                list.FullName = FullName.FirstName + " " + FullName.LastName;
            }


            //  list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;


            list.files = result.Select(t => new FactPersonalReferencViewModel()
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
                MeetingId = t.MeetingId
            }).OrderBy(u => u.LastName).ToList();
            return list;
        }

        public void DeletePersonInMeeting(int fileId)
        {
            var result = _context.Files.SingleOrDefault(f => f.Id == fileId);
            result.MeetingId = null;
            _context.Update(result);
            _context.SaveChanges();
        }

        public ListFileReferenceViewModel GetListFileForAddPersonToMeeting(int rcvrUserId, int meetingId, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGhaTitle = 0)
        {

            //IQueryable<Files> result = _context.Files.Where(f => f.MeetingId != meetingId && f.MeetingId == null);

            //
            //کسایی رو که پرونده شون رسیده به کاربر نزاجا رو نشون میده
            // rcvrUserId ===>>>UserId claim
            IQueryable<Cartable> result = _context.Cartables.Include(f => f.File).Where(f => f.RcvrUserId == rcvrUserId && f.File.MeetingId == null);

            //

            if (!string.IsNullOrEmpty(filterCaption))
            {
                //result = result.Where(u => u.PersonalCode.Contains(filterCaption));
                result = result.Where(u => u.File.Personal.PersonalCode.Contains(filterCaption));
            }
            if (filterCodGhaTitle != 0)
            {
                //result = result.Where(u => u.CodGha == filterCodGhaTitle);
                result = result.Where(u => u.File.Personal.CodGha == filterCodGhaTitle);
            }
            if (SubjectId != 0)
            {
                // result = result.Where(u => u.RequestSubjectId == SubjectId);
                result = result.Where(u => u.File.RequestSubjectId == SubjectId);
            }
            var take = 10;
            var skip = (pageId - 1) * take;

            ListFileReferenceViewModel list = new ListFileReferenceViewModel() { };
            list.CurrentPage = pageId;
            list.MeetingId = meetingId;
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)take);// result.Count() / take;

            list.files = result.Select(t => new FactPersonalReferencViewModel()
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
                RankTitle = t.File.Personal.RankTitle,
                JobDes = t.File.Personal.JobDes,
                Phone = t.File.Personal.Phone,
                StatuseTitle = t.File.Personal.StatusTitle,
                BranchTitle = t.File.Personal.BranchTitle,
                UnitDutyTitle = t.File.Personal.UnitDutyTitle,
                UnitTitle = t.File.Personal.UnitTitle,
                CodGhaTitle = t.File.Personal.CodGhaTitle,
                RegDate = t.RegDate
            }).OrderBy(u => u.LastName).Skip(skip).Take(take).ToList();
            return list;
        }

        public ListFileReferenceViewModel GetFileForFinalApprovalMeeting(int meetingId)
        {
            IQueryable<Files> result = _context.Files.Where(f => f.MeetingId == meetingId);
            ListFileReferenceViewModel list = new ListFileReferenceViewModel() { };
            string meetingTitle = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).Title;
            list.MeetingId = meetingId;
            list.MeetingTitle = meetingTitle;
            list.files = result.Select(t => new FactPersonalReferencViewModel()
            {
                Id = t.Id,
                MeetingId = t.MeetingId,
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
            }).OrderBy(u => u.LastName).ToList();
            return list;
        }

        public void AddFinalApprovalMeeting(int meetingId, int userid)
        {
            var result = _context.Meetings.SingleOrDefault(f => f.Id == meetingId);
            result.IsOkay = true;
            result.IsOkayRegUserId = userid;
            result.IsOkayDate = DateTime.Now;
            _context.Update(result);
            _context.SaveChanges();
        }


        #endregion

        #region SMS
        public int AddSMSInfo(SMSInfoViewModel smsInfo, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string jsonSmsInfo = JsonConvert.SerializeObject(smsInfo);
            StringContent content = new StringContent(jsonSmsInfo, Encoding.UTF8, "application/json");
            var res = _client.PostAsync(apiUrl, content).Result;
            if (res.IsSuccessStatusCode)
                return ((int)res.StatusCode);
            else return 0;

        }

        public ListSMSInfoViewModel GetFileForSMSInfo(int? meetingId)
        {
            IQueryable<Files> result = _context.Files.Where(f => f.MeetingId == meetingId);
            ListSMSInfoViewModel list = new ListSMSInfoViewModel() { };
            string meetingTitle = _context.Meetings.SingleOrDefault(f => f.Id == meetingId).Title;

            list.sMSInfos = result.Select(t => new SMSInfoViewModel()
            {
                CreateDate = DateTime.Now,
                SmsBody = _context.SMS.SingleOrDefault(f => f.Id == 1).TitleFa,
                Mobile = t.Phone,
                PrsnNo = t.PersonalCode,
                NationalNo = t.MelliCode,
                //دو خط زیر ثابته
                SMSTypeId = 1,
                SystemTypeId = 1,
                FullName = t.FirstName + t.LastName,

            }).OrderBy(u => u.PrsnNo).ToList();
            return list;

        }


        #endregion

        #region get List File When MeetingId==Id(Meeting)
        public List<Files> GetListFileByMeetingId(int meetingId)
        {
            var files = _context.Files.Where(x => x.MeetingId == meetingId && x.IsDelete == false).ToList();

            return files;
        }


        #endregion

        //لیست جلسات ملاقات برای فرم هامش
        public ListMeetingForHameshFormViewModel GetListMeetingForFormHamesh()
        {
            var result = _context.Meetings.Where(x =>x.MeetingStatusId==1 && x.IsDelete == false );
            var list = new ListMeetingForHameshFormViewModel() { };

            list.Meetings = result.Select(t => new MeetingInfoViewModel()
            {
                Id = t.Id,
                Title = t.Title,
                Caption = t.Caption,
                MeetingStatusTitle = t.MeetingStaus.Title,
                StartMeetingDate = t.StartMeetingDate,
                MeetingPlaceTitle = t.MeetingPlace.Title,
                BoseMeetingTitle = t.BoseMeeting.FullName,
                ClerkMeetingTitle = t.ClerkMeeting.FullName,
                IsOkay = t.IsOkay,
                IsSend = t.IsSend,
                RegDate = t.RegDate

            }).OrderByDescending(u => u.RegDate).ToList();

            return list;
        }

        public ListMeetingViewModel GetListMeetingForOnlineConversation()
        {
            IQueryable<Meeting> result = _context.Meetings.Where(x => x.IsDelete == false && x.IsSend && x.MeetingStatusId == 1 && x.IsFinished == false);

            ListMeetingViewModel list = new ListMeetingViewModel() { };


            list.Meetings = result.Select(t => new MeetingInfoViewModel()
            {
                Id = t.Id,
                Title = t.Title,
                Caption = t.Caption,
                MeetingStatusTitle = t.MeetingStaus.Title,
                StartMeetingDate = t.StartMeetingDate,
                MeetingPlaceTitle = t.MeetingPlace.Title,
                BoseMeetingTitle = t.BoseMeeting.FullName,
                ClerkMeetingTitle = t.ClerkMeeting.FullName,
                IsOkay = t.IsOkay,
                IsSend = t.IsSend,
                RegDate = t.RegDate

            }).OrderByDescending(u => u.RegDate).Distinct().ToList();
            return list;
        }


        ListFileReferenceViewModel IMeetingService.GetListFileForEditReferenceForOnlineConversation(int meetingId)
        {
            IQueryable<MemberMeeting> result = _context.MemberMeetings
                .Where(f => f.MeetingId == meetingId && f.IsMeetingHold == false)
                .Include(x => x.File)
                .OrderBy(x => x.UnitCode);


            //  IQueryable<Files> result = _context.Files.Where(f => f.MeetingId == meetingId).OrderBy(x => x.UnitCode);


            ListFileReferenceViewModel list = new ListFileReferenceViewModel() { };


            list.files = result.Select(t => new FactPersonalReferencViewModel()
            {
                Id = t.Id,
                FileId = t.FileId,
                AddUserId = t.File.Personal.RegUserId,
                FirstName = t.File.Personal.FirstName,
                LastName = t.File.Personal.LastName,
                PersonalCode = t.File.Personal.PersonalCode,
                FPersonalCode = t.File.Personal.FarmandehPersonalCode,
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
                UnitCode = t.UnitCode,
                UnitTitle = t.UnitTitle,
                CodGhaTitle = t.File.CodGhaTitle,
                RegDate = t.File.RegDate,
                MeetingId = t.MeetingId
            }).ToList();
            return list;
        }

        public BaseResult AddPersonToMemberMeeting(int meetingId, int fileId, int UnitCode, string unitTitle)
        {
            var MemberMeeting = new MemberMeeting
            {
                FileId = fileId,
                MeetingId = meetingId,
                UnitCode = UnitCode,
                UnitTitle = unitTitle,
                SortNum = 1,
                IsMeetingHold = false,
                IsActive = true,
                RegDate = DateTime.Now,
                IsDelete = false,
            };

            _context.MemberMeetings.Add(MemberMeeting);

            var res = _context.SaveChanges();

            if (res==1)
            {
                return new BaseResult()
                {
                    Status = true
                };

            }
            return new BaseResult()
            {
                Status=false
            };

        }


        /// <summary>
        /// اضافه کردن شناسه جلسه به جدول درخواست ملاقات ها
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="MeetingId"></param>
        public BaseResult AddSingleMeetingIdToFile(int fileId, int MeetingId)
        {

            var result = _context.Files.SingleOrDefault(u => u.Id == fileId);

            result.MeetingId = MeetingId;

           var resupdate= _context.Update(result);

          var resSaveChange=  _context.SaveChanges();

            if (resSaveChange==1)
            {
                return new BaseResult()
                {
                    Status = true,

                };
            }

            return new BaseResult()
            {
                Status=false,

            };
        }

        public List<OrganViewModelDto> GetListOrganMemberMeeting(int meetingId)
        {
            var memberMeeting = _context.MemberMeetings.Where(x => x.MeetingId == meetingId)
                .Select(t => new OrganViewModelDto()
                {
                    Id = t.UnitCode,
                    UnitCode = t.UnitCode,
                    Title = t.UnitTitle,
                }).Distinct()
                .ToList();



            return memberMeeting;
        }

        public BaseResult ChangeStatusMeeting(int meetingId)
        {
            var meeting = GetMeetingByMeetingId(meetingId);
            var res = new BaseResult();
            if (meeting.MeetingStatusId == 1)
            {
                meeting.MeetingStatusId = 4;
                _context.Meetings.Update(meeting);

                _context.SaveChanges();

                res.Status = true;

                return res;

            }
            if (meeting.MeetingStatusId == 4)
            {
                meeting.MeetingStatusId = 1;
                _context.Meetings.Update(meeting);

                _context.SaveChanges();

                res.Status = true;

                return res;
            }

            _context.Meetings.Update(meeting);

            _context.SaveChanges();

            res.Status = true;

            return res;
        }

        public BaseResult ChangeStatusPersonInMeeting(int fileId)
        {
            var memberMeeting = _context.MemberMeetings.Where(x => x.FileId == fileId).FirstOrDefault();

            memberMeeting.IsMeetingHold = true;

            _context.MemberMeetings.Update(memberMeeting);
            _context.SaveChanges();

            return new BaseResult()
            {
                Status = true,
                Model = memberMeeting.MeetingId
            };
        }
    }
}
