using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.ReportsAdmin
{
    public class FarmandehReportDTO
    {
        public int TotalNazarieh { get; set; }
        public int TotalResolveRequest { get; set; }
        public int TotalReturnRequest { get; set; }
        public int TotalWaitingRequest { get; set; }
        public int TotalRequest { get; set; }
        public string FullName { get; set; }
        public string Rank { get; set; }
        public string PrsnCd { get; set; }
        public string Job { get; set; }
        public string EntesabDate { get; set; }
        public string BranchTitle { get; set; }
        public string Organ { get; set; }
        public int UnitCode { get; set; }

        //problem 
        public int VamCount { get; set; }
        public int MosaedatCount { get; set; }
        public int TransferCount { get; set; }
        public int RahaeiCount { get; set; }
        public int EbghaCount { get; set; }
        public int MaskanCount { get; set; }
        public int RankMaskanCount { get; set; }
        public int EastekhtamCount { get; set; }
        public int EadehBeKhetmatCount { get; set; }
        public int ShekaiatCount { get; set; }
        public int MahkomiatCount { get; set; }
        public int MadrakTahsiliCount { get; set; }
        public int CourseCount { get; set; }
        public int MorakhasiNoUseCount { get; set; }
        public int MoseadatAnyMoneyCount { get; set; }
        public int OtherCount { get; set; }
      


    }
}
