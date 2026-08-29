using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface ISmsService
    {

        //SMS
        ListSMSInfoViewModel GetFileForSMSInfo(int? meetingId);
        SMSInfoViewModel GetFileIdForSendSmsInEachProcess(int id, int fileId, string roleTypeTitle , int? actionTypeId);
        int AddSMSInfo(SMSInfoViewModel smsInfo);
        /// <summary>
        /// ارسال پیامک به سلسله مراتی یگان نفر وقتی جلسه توسط هیئت رئیسه برگزار شد
        /// </summary>
        /// <returns></returns>
        void SendSmsToSelseleMaratebYeganNafar(int fileId);

        #region ارسال پیامک به لیست نفراتی که در جلسه ملاقات اضافه میشوند
        /// <summary>
        ///  ارسال پیامک به لیست نفراتی که در جلسه ملاقات اضافه میشوند
        /// </summary>
        /// <param name="meetingId"></param>
        void SendSmsToMemberAddToMeeting(int meetingId);
        #endregion
    }
}
