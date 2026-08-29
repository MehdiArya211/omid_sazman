using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces.Reports;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Views;

namespace VisitorManagment.Core.Services.Reports
{
    public class HamishReportService : IHamishReportService
    {
        private VisitorManagmentContext _context;

        public HamishReportService(VisitorManagmentContext context)
        {
            _context = context;
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// گرفتن تعداد اقدام های انجام شده فرمانده براساس نوع اقدام
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public List<HameshActionTypeModel> GetTotalHelpAmountForServiceType(int userId, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwHamesh>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(Id) as TCount, RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle,UserId,RequestSubjectId,RequestSubjectTitle,RegDate ";

            sqlQuery += $" FROM  { schemaAndTableName} ";

            sqlQuery += $" Where UserId = " + userId;

            sqlQuery += $"  GROUP BY RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle,RequestSubjectId,RequestSubjectTitle,UserId,RegDate";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================



            var result1 = _context.ViwHamesh.FromSqlRaw(sqlQuery).Select(c => new HameshActionTypeModel()
            {

                TCount = c.TCount,
                RoleTypeId = c.RoleTypeId,
                RoleTypeTitle = c.RoleTypeTitle,
                ActionTypeId = c.ActionTypeId,
                ActionTypeTitle = c.ActionTypeTitle,
                CodGha = c.CodGha,
                CodGhaTitle = c.CodGhaTitle,
                UserId = c.UserId,
                RequestSubjectId = c.RequestSubjectId,
                RequestSubjectTitle = c.RequestSubjectTitle,
                RegDate = c.RegDate,

            }).ToList();

            if (startDateEnglish != null && endDateEnglish == null)
            {

                var result = _context.ViwHamesh.FromSqlRaw(sqlQuery).Select(c => new HameshActionTypeModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,
                    UserId = c.UserId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RegDate = c.RegDate,

                }).Where(x => x.RegDate >= startDateEnglish).ToList();

                return result;
            }

            if (startDateEnglish==null && endDateEnglish != null)
            {
                var result = _context.ViwHamesh.FromSqlRaw(sqlQuery).Select(c => new HameshActionTypeModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,
                    UserId = c.UserId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RegDate = c.RegDate,

                }).Where(x => x.RegDate <= endDateEnglish).ToList();
                //result.Where(x => x.RegDate <= endDateEnglish).ToList();
                return result;
            }

            if (startDateEnglish != null && endDateEnglish!=null)
            {
                var result = _context.ViwHamesh.FromSqlRaw(sqlQuery).Select(c => new HameshActionTypeModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,
                    UserId = c.UserId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RegDate = c.RegDate,

                }).Where(x => x.RegDate >= startDateEnglish && x.RegDate <= endDateEnglish).ToList();
                //result.Where(x => x.RegDate >= startDateEnglish && x.RegDate <= endDateEnglish).ToList();
                return result;
            }

            return result1;

        }

        /// <summary>
        /// گزارش فرماندهان براساس نوع اقدام انجام شده
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public ChartFarmandehActivityDto ReportFarmandehByActionCode(string personCode, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {
            var prsnCd = personCode.ToString();
            int userId = _context.Users.FirstOrDefault(_ => _.UserName == prsnCd).Id;

            var query = GetTotalHelpAmountForServiceType(userId, startDateEnglish, endDateEnglish);
            var TotalNazarieh = 0;
            var TotalResolveRequest = 0;
            var TotalReturnRequest = 0;
            var TotalWaitingRequest = 0;

            foreach (var item in query)
            {
                if (item.ActionTypeId == 2)
                    TotalNazarieh += item.TCount;
                if (item.ActionTypeId == 3)
                    TotalReturnRequest += item.TCount;
                if (item.ActionTypeId == 1)
                    TotalResolveRequest += item.TCount;
                if (item.ActionTypeId == 1002)
                    TotalWaitingRequest += item.TCount;
            }

            var result = new ChartFarmandehActivityDto();

            result.Files = new FarmandehActivityDto();

            result.Files.TotalNazarieh = TotalNazarieh;
            result.Files.TotalResolveRequest = TotalResolveRequest;
            result.Files.TotalReturnRequest = TotalReturnRequest;
            result.Files.TotalWaitingRequest = TotalWaitingRequest;
            result.Files.TotalRequest = TotalNazarieh + TotalWaitingRequest + TotalReturnRequest + TotalResolveRequest;
            return result;
        }

        /// <summary>
        /// گزارش فرماندهان براساس مشکلات
        /// </summary>
        /// <param name="prsnCd"></param>
        /// <returns></returns>
        public ChartProblemOmdOrgan ReportProblemFarmandehInfo(string prsnCd, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {
            int userId = _context.Users.FirstOrDefault(_ => _.UserName == prsnCd).Id;
            var query = GetTotalHelpAmountForServiceType(userId, startDateEnglish, endDateEnglish);
            var chartProblem = new ChartProblemOmdOrgan();
            foreach (var item in query)
            {
                if (item.RequestSubjectId == 1)
                    chartProblem.VamCount += item.TCount;
                if (item.RequestSubjectId == 2)
                    chartProblem.MosaedatCount += item.TCount;
                if (item.RequestSubjectId == 3)
                    chartProblem.TransferCount += item.TCount;
                if (item.RequestSubjectId == 4)
                    chartProblem.RahaeiCount += item.TCount;
                if (item.RequestSubjectId == 5)
                    chartProblem.EbghaCount += item.TCount;
                if (item.RequestSubjectId == 6)
                    chartProblem.MaskanCount += item.TCount;
                if (item.RequestSubjectId == 7)
                    chartProblem.RankMaskanCount += item.TCount;
                if (item.RequestSubjectId == 8)
                    chartProblem.EastekhtamCount += item.TCount;
                if (item.RequestSubjectId == 9)
                    chartProblem.EadehBeKhetmatCount += item.TCount;
                if (item.RequestSubjectId == 10)
                    chartProblem.ShekaiatCount += item.TCount;
                if (item.RequestSubjectId == 11)
                    chartProblem.MahkomiatCount += item.TCount;
                if (item.RequestSubjectId == 12)
                    chartProblem.MadrakTahsiliCount += item.TCount;
                if (item.RequestSubjectId == 13)
                    chartProblem.CourseCount += item.TCount;
                if (item.RequestSubjectId == 14)
                    chartProblem.MorakhasiNoUseCount += item.TCount;
                if (item.RequestSubjectId == 15)
                    chartProblem.MoseadatAnyMoneyCount += item.TCount;
                if (item.RequestSubjectId == 16)
                    chartProblem.OtherCount += item.TCount;

            }
            return chartProblem;
        }

        /// <summary>
        /// تعداد درخواست های ملاقات های قرارگاه براساس کد های قرارگاه ، نوع اقدام و تاریخ
        /// </summary>
        /// <param name="codeGha"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
     



        #endregion
    }
}

