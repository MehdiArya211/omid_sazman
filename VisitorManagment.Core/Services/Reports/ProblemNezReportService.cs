using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces.Reports;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Views;

namespace VisitorManagment.Core.Services.Reports
{
    public class ProblemNezReportService : IProblemNezReportService
    {
        private VisitorManagmentContext _context;

        public ProblemNezReportService(VisitorManagmentContext context)
        {
            _context = context;
        }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>

        public List<ProblemAllNez> GetPeroblemAllNez()
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwFiles>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(id) As TCount,RequestSubjectId,RequestSubjectTitle ";

            sqlQuery += $" FROM  { schemaAndTableName} ";


            //sqlQuery += $" Where IsFinaly = 1 ";

            sqlQuery += $"  GROUP BY RequestSubjectId,RequestSubjectTitle";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================



            return _context.ViwFiles.FromSqlRaw(sqlQuery).Select(c => new ProblemAllNez()
            {
                TCount = c.TCount,
                RequestSubjectId = c.RequestSubjectId,
                RequestSubjectTitle = c.RequestSubjectTitle,

            }).ToList();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<ProblemGhaReport> GetPeroblemGhaNez()
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwFiles>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(id) As TCount,CodGha,CodGhaTitle ";

            sqlQuery += $" FROM  { schemaAndTableName} ";


            //sqlQuery += $" Where IsFinaly = 1 ";

            sqlQuery += $"  GROUP BY CodGha,CodGhaTitle";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================



            return _context.ViwFiles.FromSqlRaw(sqlQuery).Select(c => new ProblemGhaReport()
            {
                TCount = c.TCount,
                CodGha = c.CodGha,
                CodGhaTitle = c.CodGhaTitle,
            }).ToList();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<ProblemOmdOrganReport> GetPeroblemOmdOrganNez()
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwFiles>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(id) As TCount,UnitCode,UnitTitle,RequestSubjectId,RequestSubjectTitle";

            sqlQuery += $" FROM  { schemaAndTableName} ";


            //sqlQuery += $" Where IsFinaly = 1 ";

            sqlQuery += $"  GROUP BY RequestSubjectId,RequestSubjectTitle,UnitCode,UnitTitle";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================



            return _context.ViwFiles.FromSqlRaw(sqlQuery).Select(c => new ProblemOmdOrganReport()
            {
                TCount = c.TCount,
                UnitCode = c.CodGha,
                UnitTitle = c.CodGhaTitle,
                RequestSubjectTitle = c.RequestSubjectTitle,
                RequestSubjectId = c.RequestSubjectId,
            }).ToList();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ChartProblemOmdOrgan GetProblemCountOmdOrganInfo(int OmdOrganId)
        {
            var model = new ChartProblemOmdOrgan();
            var OrganInfo = GetPeroblemOmdOrganNez().Where(i => i.UnitCode == OmdOrganId).ToList();
            foreach (var item in OrganInfo)
            {
                if (item.RequestSubjectId == 1)
                    model.VamCount = item.TCount;
                if (item.RequestSubjectId == 2)
                    model.MosaedatCount = item.TCount;
            }

            return model;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<ProblemOmdOrganReport> GetProblemOmdOrgByGhCd_OmdCd(int GhaCd, int OmdCd, DateTime? startDateEnglish, DateTime? endDateEnglish) 
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwFiles>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(id) As TCount,CodGha,UnitDutyCode,UnitTitle,UnitDutyTitle,UnitCode,RequestSubjectId,RequestSubjectTitle,IsDelete ,RegDate";

            sqlQuery += $" FROM  { schemaAndTableName} ";


           

            sqlQuery += $"  GROUP BY RequestSubjectId,RequestSubjectTitle,CodGha,UnitDutyCode,UnitTitle,UnitCode,UnitDutyTitle,IsDelete ,RegDate";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================


            var listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery);

            if (GhaCd != 0 && OmdCd == 0)
            {
                //sqlQuery += $" Where CodGha =" + GhaCd.ToString();
                if (startDateEnglish != null && endDateEnglish == null)
                {
                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.IsDelete == false && a.RegDate>=startDateEnglish);

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }

                if (startDateEnglish == null && endDateEnglish != null) {

                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.IsDelete == false && a.RegDate<=endDateEnglish);

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }

                if (startDateEnglish != null && endDateEnglish != null)
                {
                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.IsDelete == false &&(a.RegDate>=startDateEnglish && a.RegDate<=endDateEnglish));

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }


                listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery).Where(a => a.CodGha == GhaCd && a.IsDelete == false);

                return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                {
                    TCount = c.TCount,
                    UnitCode = c.UnitCode,
                    CodeGha = c.CodGha,
                    UnitTitle = c.UnitTitle,
                    UnitDutyCode = c.UnitDutyCode,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RequestSubjectId = c.RequestSubjectId,
                }).ToList();

            }
            if (GhaCd != 0 && OmdCd != 0)
            {
                //sqlQuery += $" Where UnitCode =" + OmdCd.ToString() ;

                if (startDateEnglish != null && endDateEnglish == null)
                {
                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.UnitCode == OmdCd && a.IsDelete == false && a.RegDate >= startDateEnglish);

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }

                if (startDateEnglish == null && endDateEnglish != null)
                {

                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.UnitCode == OmdCd && a.IsDelete == false && a.RegDate <= endDateEnglish);

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }

                if (startDateEnglish != null && endDateEnglish != null)
                {
                    listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery)
                        .Where(a => a.CodGha == GhaCd && a.UnitCode == OmdCd && a.IsDelete == false && (a.RegDate >= startDateEnglish && a.RegDate <= endDateEnglish));

                    return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                    {
                        TCount = c.TCount,
                        UnitCode = c.UnitCode,
                        CodeGha = c.CodGha,
                        UnitTitle = c.UnitTitle,
                        UnitDutyCode = c.UnitDutyCode,
                        RequestSubjectTitle = c.RequestSubjectTitle,
                        RequestSubjectId = c.RequestSubjectId,
                    }).ToList();

                }

                listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery).Where(a =>a.CodGha == GhaCd && a.UnitCode == OmdCd && a.IsDelete == false);

                return listomdeyegn.Select(c => new ProblemOmdOrganReport()
                {
                    TCount = c.TCount,
                    UnitCode = c.UnitCode,
                    CodeGha = c.CodGha,
                    UnitTitle = c.UnitTitle,
                    UnitDutyCode = c.UnitDutyCode,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RequestSubjectId = c.RequestSubjectId,
                }).ToList();

            }

         
            return listomdeyegn.Select(c => new ProblemOmdOrganReport()
            {
                TCount = c.TCount,
                UnitCode = c.UnitCode,
                CodeGha = c.CodGha,
                UnitTitle = c.UnitTitle,
                UnitDutyCode = c.UnitDutyCode,
                RequestSubjectTitle = c.RequestSubjectTitle,
                RequestSubjectId = c.RequestSubjectId,
            }).ToList();
        }
        /// <summary>
        /// گزارش  فراوانی تعداد مشکلات براساس قرارگاه و یگان های عمده
        /// </summary>
        /// <param name="GhaCd"></param>
        /// <param name="OmdCd"></param>
        /// <returns></returns>
        public ChartProblemOmdOrgan GetProblemCountGhCd_OmdCd(int GhaCd, int OmdCd, DateTime? startDateEnglish, DateTime? endDateEnglish) 
        {
            var model = new ChartProblemOmdOrgan();
            var OrganInfo = GetProblemOmdOrgByGhCd_OmdCd(GhaCd, OmdCd , startDateEnglish , endDateEnglish);
            foreach (var item in OrganInfo)
            {
                if (item.RequestSubjectId == 1)
                    model.VamCount += item.TCount;
                if (item.RequestSubjectId == 2)
                    model.MosaedatCount += item.TCount;
                if (item.RequestSubjectId == 3)
                    model.TransferCount += item.TCount;
                if (item.RequestSubjectId == 4)
                    model.RahaeiCount += item.TCount;
                if (item.RequestSubjectId == 5)
                    model.EbghaCount += item.TCount;
                if (item.RequestSubjectId == 6)
                    model.MaskanCount += item.TCount;
                if (item.RequestSubjectId == 7)
                    model.RankMaskanCount += item.TCount;
                if (item.RequestSubjectId == 8)
                    model.EastekhtamCount += item.TCount;
                if (item.RequestSubjectId == 9)
                    model.EadehBeKhetmatCount += item.TCount;
                if (item.RequestSubjectId == 10)
                    model.ShekaiatCount += item.TCount;
                if (item.RequestSubjectId == 11)
                    model.MahkomiatCount += item.TCount;
                if (item.RequestSubjectId == 12)
                    model.MadrakTahsiliCount += item.TCount;
                if (item.RequestSubjectId == 13)
                    model.CourseCount += item.TCount;
                if (item.RequestSubjectId == 14)
                    model.MorakhasiNoUseCount += item.TCount;
                if (item.RequestSubjectId == 15)
                    model.MoseadatAnyMoneyCount += item.TCount;
                if (item.RequestSubjectId == 16)
                    model.OtherCount += item.TCount;
            }
            return model;
        }



        #endregion
    }
}
