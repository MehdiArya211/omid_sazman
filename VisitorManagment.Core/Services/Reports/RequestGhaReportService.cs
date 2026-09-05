using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces.Reports;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Views;

namespace VisitorManagment.Core.Services.Reports
{
    public class RequestGhaReportService : IRequestGhaReportService
    {
        private VisitorManagmentContext _context;

        public RequestGhaReportService(VisitorManagmentContext context)
        {
            _context = context;
        }

        #region گزارش عملکرد قرارگاه و یگان

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<HameshRequestGhaModel> GetTotalHelpAmountForServiceType(int GharargahId, int RoleId, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {

            #region کوئری

            var schemaAndTableName = _context.GetSqlServerTableName<ViwHamesh>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(Id) as TCount, RoleTypeId,RoleTypeTitle,RequestSubjectId,RequestSubjectTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle , RegDate ";

            sqlQuery += $" FROM  { schemaAndTableName} ";

            //sqlQuery += $" Where IsFinaly = 1 ";

            sqlQuery += $"  GROUP BY RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,RequestSubjectId,RequestSubjectTitle,CodGha,CodGhaTitle, RegDate";

            sqlQuery += $"  ORDER BY CodGha offset 0 rows";
            #endregion


            //======================================
            var ListViwHamesh = _context.ViwHamesh.FromSqlRaw(sqlQuery);

            #region جستجو براساس تاریخ
            if (startDateEnglish != null && endDateEnglish == null)
            {
                ListViwHamesh = _context.ViwHamesh.FromSqlRaw(sqlQuery)
                    .Where(d =>  d.RegDate >= startDateEnglish);

                return ListViwHamesh.Select(c => new HameshRequestGhaModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,

                }).ToList();
            }

            if (startDateEnglish == null && endDateEnglish != null)
            {
                ListViwHamesh = _context.ViwHamesh.FromSqlRaw(sqlQuery).Where(d => d.RegDate <= endDateEnglish);

                return ListViwHamesh.Select(c => new HameshRequestGhaModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,

                }).ToList();
            }

            if (startDateEnglish != null && startDateEnglish != null)
            {
                ListViwHamesh = _context.ViwHamesh.FromSqlRaw(sqlQuery).Where(d =>  (d.RegDate >= startDateEnglish && d.RegDate <= endDateEnglish));

                return ListViwHamesh.Select(c => new HameshRequestGhaModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,

                }).ToList();
            }
            #endregion
            #region Search
            if (GharargahId != 0)
            {


                ListViwHamesh = ListViwHamesh.Where(d => d.CodGha == GharargahId);



            }
            if (RoleId != 0)
            {

                ListViwHamesh = ListViwHamesh.Where(d => d.RoleTypeId == RoleId);

            }
            if (RoleId != 0 && GharargahId != 0)
            {


                ListViwHamesh = ListViwHamesh
                    .Where(d => d.CodGha == GharargahId && d.RoleTypeId == RoleId && d.RoleTypeId == RoleId);

            }
            #endregion
            return ListViwHamesh.Select(c => new HameshRequestGhaModel()
            {

                TCount = c.TCount,
                RoleTypeId = c.RoleTypeId,
                RequestSubjectId = c.RequestSubjectId,
                RequestSubjectTitle = c.RequestSubjectTitle,
                RoleTypeTitle = c.RoleTypeTitle,
                ActionTypeId = c.ActionTypeId,
                ActionTypeTitle = c.ActionTypeTitle,
                CodGha = c.CodGha,
                CodGhaTitle = c.CodGhaTitle,

            }).ToList();
        }


        #endregion
        #region سرویس فراوانی مشکلات ViwFiles
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
        public List<ProblemOmdOrganReport> GetPeroblemOmdOrganNez(int OmdOrganId, int YeganId)
        {
            var strAnd = " AND ";


            var schemaAndTableName = _context.GetSqlServerTableName<ViwFiles>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(id) As TCount,CodGha,UnitDutyCode,UnitTitle,UnitDutyTitle,UnitCode,RequestSubjectId,RequestSubjectTitle";

            sqlQuery += $" FROM  { schemaAndTableName} ";


            //sqlQuery += $" Where IsFinaly = 1 ";

            sqlQuery += $"  GROUP BY RequestSubjectId,RequestSubjectTitle,CodGha,UnitDutyCode,UnitTitle,UnitCode,UnitDutyTitle";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================

            var listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery);
            if (OmdOrganId != 0)
            {
                listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery).Where(a => a.CodGha == OmdOrganId);

            }
            if (OmdOrganId != 0 && YeganId != 0)
            {
                listomdeyegn = _context.ViwFiles.FromSqlRaw(sqlQuery).Where(a => a.CodGha == OmdOrganId && a.UnitCode == YeganId);

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
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ChartProblemOmdOrgan GetProblemCountOmdOrganInfo(int OmdOrganId, int YeganId)
        {
            var model = new ChartProblemOmdOrgan();
            var OrganInfo = GetPeroblemOmdOrganNez(OmdOrganId, YeganId);
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


        /// <summary>
        /// نمایش درخواست  قرارگاه بصورت لیستی
        /// </summary>
        /// <param name="GharargahId"></param>
        /// <param name="RoleId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        public List<HameshRequestGhaModel> GetListTotalHelpAmountForServiceType(int GharargahId, int RoleId, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<HameshActionTypeModel> GetNomrehArzyabiGharargah(int actionTypeId, DateTime? startDateEnglish, DateTime? endDateEnglish)
        {
            var strAnd = " AND ";

            var schemaAndTableName = _context.GetSqlServerTableName<ViwHamesh>();
            //--- Default = rptByProvince = true 
            // ستون‌های عددی View در بعضی داده‌های قدیمی NULL هستند؛ تبدیل صریح از
            // بروز SqlBuffer.get_Int32 هنگام ساخت مدل گزارش جلوگیری می‌کند.
            var sqlQuery = $"SELECT count(Id) as TCount, ISNULL(RoleTypeId,0) AS RoleTypeId,ISNULL(RoleTypeTitle,N'') AS RoleTypeTitle,ISNULL(ActionTypeId,0) AS ActionTypeId,ISNULL(ActionTypeTitle,N'') AS ActionTypeTitle,CodGha,ISNULL(CodGhaTitle,N'') AS CodGhaTitle,ISNULL(UnitCode,0) AS UnitCode,ISNULL(UnitTitle,N'') AS UnitTitle,ISNULL(UserId,0) AS UserId,ISNULL(RequestSubjectId,0) AS RequestSubjectId,ISNULL(RequestSubjectTitle,N'') AS RequestSubjectTitle,RegDate ";

            sqlQuery += $" FROM  { schemaAndTableName} ";

            //sqlQuery += $" Where UserId = " + userId;

            sqlQuery += $"  GROUP BY RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle, UnitCode , UnitTitle,RequestSubjectId,RequestSubjectTitle,UserId,RegDate";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================


            #region جستجو براساس تاریخ
            if (startDateEnglish != null && endDateEnglish == null)
            {

                return _context.ViwHamesh.FromSqlRaw(sqlQuery)
                     .Where(d => d.RegDate >= startDateEnglish && d.ActionTypeId == actionTypeId)
                    .Select(c => new HameshActionTypeModel()
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

                    }).OrderBy(x => x.TCount).ToList();
            }

            if (startDateEnglish == null && endDateEnglish != null)
            {


                return _context.ViwHamesh.FromSqlRaw(sqlQuery)
                      .Where(d => d.RegDate <= endDateEnglish && d.ActionTypeId == actionTypeId)
                     .Select(c => new HameshActionTypeModel()
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

                     }).OrderBy(x => x.TCount).ToList();
            }

            if (startDateEnglish != null && endDateEnglish != null)
            {

                return _context.ViwHamesh.FromSqlRaw(sqlQuery)
                          .Where(d => d.RegDate >= startDateEnglish && d.RegDate <= endDateEnglish && d.ActionTypeId == actionTypeId)
                         .Select(c => new HameshActionTypeModel()
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

                         }).OrderBy(x => x.TCount).ToList();
            }
            #endregion

            return _context.ViwHamesh.FromSqlRaw(sqlQuery).Where(x => x.ActionTypeId == actionTypeId)
                .Select(c => new HameshActionTypeModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,
                    UnitCode = c.UnitCode,
                    UnitCodeTitle = c.UnitTitle,
                    UserId = c.UserId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RegDate = c.RegDate,

                }).ToList();



        }


        /// <summary>
        /// گزارش نموداری رتبه بندی یگان های قرارگاه در صفحه اول سایت
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        public List<HameshActionTypeModel> GetArzyabiAllYeganForGharargah(int actionTypeId, int unitCode, int codeGha, int roleTypeId)
        {
            var strAnd = " AND ";

            var schemaAndTableName = _context.GetSqlServerTableName<ViwHamesh>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(Id) as TCount, RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle , UnitCode , UnitTitle,UserId,RequestSubjectId,RequestSubjectTitle,RegDate ";

            sqlQuery += $" FROM  { schemaAndTableName} ";

            //sqlQuery += $" Where UserId = " + userId;

            sqlQuery += $"  GROUP BY RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle, UnitCode , UnitTitle,RequestSubjectId,RequestSubjectTitle,UserId,RegDate";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================

            return _context.ViwHamesh.FromSqlRaw(sqlQuery)
                .Where(x => x.ActionTypeId == actionTypeId && x.CodGha == codeGha && x.UnitCode == unitCode)
                .Select(c => new HameshActionTypeModel()
                {

                    TCount = c.TCount,
                    RoleTypeId = c.RoleTypeId,
                    RoleTypeTitle = c.RoleTypeTitle,
                    ActionTypeId = c.ActionTypeId,
                    ActionTypeTitle = c.ActionTypeTitle,
                    CodGha = c.CodGha,
                    CodGhaTitle = c.CodGhaTitle,
                    UnitCode = c.UnitCode,
                    UnitCodeTitle = c.UnitTitle,
                    UserId = c.UserId,
                    RequestSubjectId = c.RequestSubjectId,
                    RequestSubjectTitle = c.RequestSubjectTitle,
                    RegDate = c.RegDate,

                }).ToList();
        }



        /// <summary>
        ///   رتبه  یگان در صفحه اول سایت
        /// </summary>
        /// <param name="actionTypeId"></param>
        /// <param name="startDateEnglish"></param>
        /// <param name="endDateEnglish"></param>
        /// <returns></returns>
        public int GradYegan(int actionTypeId, int unitCode, int codeGha, int roleTypeId)
        {
            var schemaAndTableName = _context.GetSqlServerTableName<ViwHamesh>();
            //--- Default = rptByProvince = true 
            var sqlQuery = $"SELECT count(Id) as TCount, RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle , UnitCode , UnitTitle,UserId,RequestSubjectId,RequestSubjectTitle,RegDate ";

            sqlQuery += $" FROM  { schemaAndTableName} ";

            //sqlQuery += $" Where UserId = " + userId;

            sqlQuery += $"  GROUP BY RoleTypeId,RoleTypeTitle,ActionTypeId,ActionTypeTitle,CodGha,CodGhaTitle, UnitCode , UnitTitle,RequestSubjectId,RequestSubjectTitle,UserId,RegDate";

            //sqlQuery += $"  ORDER BY CodGha offset 0 rows";


            //======================================

            return _context.ViwHamesh.FromSqlRaw(sqlQuery)
                .Where(x => x.ActionTypeId == actionTypeId && x.CodGha == codeGha && x.UnitCode == unitCode)
                .Select(x => x.TCount).FirstOrDefault();

        }
    }
}
