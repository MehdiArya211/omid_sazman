using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.ReportsAdmin
{

    #region ViewModal درخواست قرارگاه 

    public class HameshRequestGhaModel
    {
        public int Id { get; set; }
        public int TCount { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleTypeTitle { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionTypeTitle { get; set; }
        public int? CodGha { get; set; }
        public int RequestSubjectId { get; set; }
        public string RequestSubjectTitle { get; set; }
        public string CodGhaTitle { get; set; }
        public string DimensionOne { get; set; }
        public int Quantity { get; set; }

    }
    public class ReqoestGhaReportViewModal
    {
        public int CountEghdam { get; set; }
        public int CountRequestRegect { get; set; }
        public int CountHamesh { get; set; }
        public int CountEntezar { get; set; }
        public int CountAll { get; set; }
        public int CountEghdamVam { get; set; }


    }


    #endregion
    #region ویو مودل فراوانی مشکلات

    //public class ProblemAllNez
    //{
    //    public int TCount { get; set; }
    //    public int RequestSubjectId { get; set; }
    //    public string RequestSubjectTitle { get; set; }


    //}
    //public class ProblemGhaReport
    //{
    //    public int CodGha { get; set; }
    //    public string CodGhaTitle { get; set; }
    //    public int TCount { get; set; }


    //}
    //public class ProblemOmdOrganReport
    //{
    //    public int TCount { get; set; }
    //    public int UnitCode { get; set; }
    //    public int UnitDutyCode { get; set; }
    //    public int CodeGha { get; set; }
    //    public string UnitTitle { get; set; }
    //    public string UnitDutyTitle { get; set; }
    //    public int RequestSubjectId { get; set; }
    //    public string RequestSubjectTitle { get; set; }
    //}

    public class ChartProblemOmdRequestGha
    {
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
        public int CountAll { get; set; }

    }

    public class ChartNomrehArzyabiGha
    {
        public int MainCount { get; set; }
        public string Title { get; set; }
        public int CountNezaja { get; set; }
        public int CountMarato { get; set; }
        public int CountOlomFononMekanizeh { get; set; }
        public int CountGhaShomal { get; set; }
        public int CountGhaShomalGharb { get; set; }
        public int CountGhaShomalShargh { get; set; }
        public int CountGhaGharb { get; set; }
        public int CountGhaShargh { get; set; }
        public int CountGhaJonob { get; set; }
        public int CountGhaJonobGharb { get; set; }
        public int CountGhaJonobShargh { get; set; }
        public int CountAll { get; set; }

    }

    public class ChartNomrehArzyabiAllUnitGha
    {
        public int MainCount { get; set; }
        public string Title { get; set; }
        public int CountAll { get; set; }

    }

    public class ProblemReportViewModel
    {
        public string DimensionOne { get; set; }
        public int Quantity { get; set; }


    }

    public class SearchPageReportViewModel {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? ActionTypeId { get; set; }
        public List<int> CodeGha { get; set; }
    }

    public class SearchPageUnitCodeReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? ActionTypeId { get; set; }
        public int CodeGha { get; set; }
        public int UnitCode { get; set; }
    }

    public class SearchPageAllUnitCodeForGhaReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? ActionTypeId { get; set; }
        public int CodeGha { get; set; }
        public List<int> UnitCode { get; set; }
    }

    public class SearchPageReportFarmandehActivityReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? PrsnCd { get; set; }
    }

    public class SearchPageReportKarbarAnsarReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? PrsnCd { get; set; }
    }

    public class SearchPageReportProblemNezajaReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? GharargahId { get; set; }
        public int? YeganId { get; set; }
    }


    public class SearchPageReportRequestGhaReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? GharargahId { get; set; }
        public int? RoleId { get; set; }
    }

    public class SearchPageRankingAllUnitReportViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? ActionTypeId { get; set; }
        public int CodeGha { get; set; }
        public List<int> UnitCode { get; set; }
    }

    #endregion
}
