using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.Views
{
    public class ViwFiles
    {
        public int Id { get; set; }

        public int PersonalId { get; set; }

        public int RequestSubjectId { get; set; }

        public int PriorityId { get; set; }

        public int FileStatusId { get; set; }

        public string RequestDescription { get; set; }

        public string Attachment { get; set; }

        public bool IsDelete { get; set; }
        public int RegUserId { get; set; }

        public DateTime RegDate { get; set; }

        public int EditUserId { get; set; }

        public int MeetingId { get; set; }

        public string Addres { get; set; }

        public int BranchCode { get; set; }

        public string BranchTitle { get; set; }
        public int? CodGha { get; set; }

        public string CodGhaTitle { get; set; }

        public DateTime EzamDate { get; set; }

        public int FarmandehPersonalCode { get; set; }

        public string FarmandehPersonalName { get; set; }

        public string FirstName { get; set; }

        public int IsarStatus { get; set; }

        public int TOT_AML { get; set; }
        public int TOT_AML2 { get; set; }

        public string JobDes { get; set; }

        public string LastName { get; set; }

        public int MelliCode { get; set; }

        public int PersonalCode { get; set; }

        public string Phone { get; set; }

        public int RankCode { get; set; }

        public string RankTitle { get; set; }

        public int StatusTitle { get; set; }

        public int? UnitCode { get; set; }

        public int? UnitDutyCode { get; set; }

        public string UnitDutyTitle { get; set; }

        public string UnitTitle { get; set; }

        public string VoiceRecord { get; set; }

        public string AttachDastor { get; set; }

        public float DRSAD_JA { get; set; }

        public float DRSAD_JB { get; set; }

        public string ProblemDescription { get; set; }

        public DateTime EmploymentDate { get; set; }

        public string FishAttachment { get; set; }

        public int CountVam { get; set; }

        public long ReciveMoney { get; set; }

        public long SumAghsatVamMahiyaneh { get; set; }

        public long TotalMoney { get; set; }

        public bool IsMeetingHold { get; set; }

        public string RequestSubjectTitle { get; set; }
        public int TCount { get; set; }
    }
}
