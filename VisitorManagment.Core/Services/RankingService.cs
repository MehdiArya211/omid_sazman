using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.DTOs.Ranking;
using VisitorManagment.Core.Services.Interfaces.Ranking;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Ranking;

namespace VisitorManagment.Core.Services
{
    public class RankingService : IRankingService
    {

        private readonly VisitorManagmentContext _context;

        public RankingService(VisitorManagmentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// بدست آوردن تعداد درخواست ملاقات هایی که اقدام شده است براساس کد یگان
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public int GetCountEghdamWithUnitCode(int unitCode)
        {
            var count = _context.Hameshes.Include(x => x.File)
                .Where(x => x.File.UnitCode == unitCode && x.ActionTypeId == 1).Count();

            return count;
        }


        /// <summary>
        /// بدست آوردن تعداد درخواست ملاقات هایی که رد درخواست شده است براساس کد یگان
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public int GetCountRejectWithUnitCode(int unitCode)
        {
            var count = _context.Hameshes.Include(x => x.File)
                    .Where(x => x.File.UnitCode == unitCode && x.ActionTypeId == 3).Count();

            return count;
        }



        /// <summary>
        /// فرمول کلی محاسبه ی نمره
        /// </summary>
        /// <param name="pointEghdam"></param>
        /// <param name="pointReject"></param>
        /// <param name="pointHeatReeise"></param>
        /// <returns></returns>
        public int CalculateFinalPoint(int pointEghdam, int pointReject, int pointHeatReeise = 1)
        {
            #region ضرایب
            var zaribEghdamShode = _context.ZaribRankings.Where(x => x.Code == 101).Select(x => x.Zarib).FirstOrDefault();
            var zaribReject = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();
            var zaribHeatReese = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();
            #endregion



            var reject = 300 - (pointReject * 10);

            if (reject == 0 || reject < 0)
            {
                reject = 0;
            }

            var finalPoint = ((pointEghdam * zaribEghdamShode) - (reject * zaribReject)) + (0 * zaribHeatReese);



            return finalPoint;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int GetRanking(int unitCode)
        {
            
            var eshrafPeriodDefId = GetEshrafPeriodDef();

            var department = _context.TblDepartments.Where(x => x.UnitCode == unitCode).FirstOrDefault();

            var departmentType = _context.TblDepartmentTypes.Where(x=>x.Id==department.DepartmentTypeId).FirstOrDefault();

            var listDepartment = _context.TblDepartments.Where(x => x.DepartmentCode == departmentType.Code).ToList();

            List<Point> points = new List<Point>();

            foreach (var item in listDepartment)
            {
                var res = _context.Points.Where(x => x.UnitCode == item.UnitCode).FirstOrDefault();

                points.Add(res);
            }

            var i = 0;
            foreach (var item in points)
            {
                i++;
                if (item.UnitCode==unitCode)
                {
                    item.Rank = i;
                    _context.Points.Update(item);
                    _context.SaveChanges();
                    return i;
                }
            }


            return i;

            //if (department==null)
            //{
            //    return 1000;
            //}


            //var listPoint = _context.Points
            //    .Where(x => x.EshrafPeriodDefId == eshrafPeriodDefId && x.Department.TblDepartmentType.Code== departmentType.Code)
            //    .OrderByDescending(x => x.FinalPoint)
            //    .ToList();

            //int i = 0;

            //foreach (var item in listPoint)
            //{
            //    i++;

            //    if (item.UnitCode == unitCode)
            //    {
            //        item.Rank = i;
            //        _context.Points.Update(item);
            //        _context.SaveChanges();
            //        return i;

            //    }
            //}

        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public int GetEshrafDefId(System.DateTime date)
        {
            var res = _context.EshrafPeriodDefs.Where(x => x.StartDate <= date && x.EndDate <= date).Select
                (x => x.Id).FirstOrDefault();

            return res;
        }



        /// <summary>
        /// پیداکردن نوع قسمت با کد یگان
        /// </summary>
        /// <returns></returns>
        public int GetDepartmentTypeWithUnitCode(int unitCode)
        {
            var res = _context.TblDepartments.Where(x => x.UnitCode == unitCode)
                .Select(x => x.DepartmentTypeId).FirstOrDefault();

            return res;
        }


        /// <summary>
        /// پیدا کردن لیست یگان های هم وند با شناسه قسمت
        /// </summary>
        /// <param name="departmentTypeId"></param>
        /// <returns></returns>
        public List<int> GetListUnitWithDepartmentTypeId(int departmentTypeId)
        {
            var listUnitCode = _context.TblDepartments.Where(x => x.DepartmentTypeId == departmentTypeId)
                 .Select(x => x.UnitCode).ToList();

            return listUnitCode;
        }


        /// <summary>
        /// گرفتن شناسه بازه ماهانه برای رتبه بندی
        /// </summary>
        /// <returns></returns>
        public int GetEshrafPeriodDef()
        {
            var date = DateTime.Now.Date;

            var res = _context.EshrafPeriodDefs.Where(x => x.StartDate.Date <= date && date <= x.EndDate)
                .Select(x => x.Id).FirstOrDefault();

            return res;
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public void FinalCalculate()
        {
            var eshrafDefId = GetEshrafPeriodDef();

            var allRecordAgo = _context.Points.Where(x => x.EshrafPeriodDefId == eshrafDefId).ToList();

            if (allRecordAgo.Count != 0)
            {
                foreach (var item in allRecordAgo)
                {
                    Remove(item);
                }

                //حالا دوباره ثبت میکنیم
                var listAllUnit = GetListAllUnitForCalculateRanking();

                var zaribEghdamShode = _context.ZaribRankings.Where(x => x.Code == 101).Select(x => x.Zarib).FirstOrDefault();//10
                var zaribReject = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();
                var zaribHeatReese = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();

                foreach (var item in listAllUnit)
                {

                    var countEghdam = GetCountEghdamWithUnitCode(item.UnitCode);

                    var pointEghdam = countEghdam * zaribEghdamShode;


                    #region Reject
                    //var countReject = GetCountRejectWithUnitCode(item);

                    ////var pointReject = (300 - ((countReject) * 10)) * zaribReject;
                    //var pointReject = (300 - (countReject * 10)) * zaribReject;

                    //if (pointReject == 0 || pointReject < 0)
                    //{
                    //    pointReject = 0;
                    //}
                    #endregion

                    var pointzaribHeatReese = 0 * zaribHeatReese;

                    //var finalPoint = ((pointEghdam) - (pointReject)) + (pointzaribHeatReese);
                    var finalPoint = (pointEghdam) + (pointzaribHeatReese);

                    var pointModel = new PointViewModel()
                    {
                        EshrafPeriodDefId = eshrafDefId,
                        DepartmentId = item.DepartmentTypeId,
                        UnitCode = item.UnitCode,
                        PointEghdam = pointEghdam,
                        // PointReject = pointReject,
                        PointNezaja = pointzaribHeatReese,
                        FinalPoint = finalPoint,
                    };

                    AddToPoint(pointModel);


                }
            }
            else
            {
                var listAllUnit = GetListAllUnitForCalculateRanking();

                var zaribEghdamShode = _context.ZaribRankings.Where(x => x.Code == 101).Select(x => x.Zarib).FirstOrDefault();//10
                var zaribReject = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();
                var zaribHeatReese = _context.ZaribRankings.Where(x => x.Code == 102).Select(x => x.Zarib).FirstOrDefault();

                foreach (var item in listAllUnit)
                {

                    var countEghdam = GetCountEghdamWithUnitCode(item.UnitCode);

                    var pointEghdam = countEghdam * zaribEghdamShode;


                    #region Reject
                    //var countReject = GetCountRejectWithUnitCode(item);

                    ////var pointReject = (300 - ((countReject) * 10)) * zaribReject;
                    //var pointReject = (300 - (countReject * 10)) * zaribReject;

                    //if (pointReject == 0 || pointReject < 0)
                    //{
                    //    pointReject = 0;
                    //}
                    #endregion

                    var pointzaribHeatReese = 0 * zaribHeatReese;

                    //var finalPoint = ((pointEghdam) - (pointReject)) + (pointzaribHeatReese);
                    var finalPoint = (pointEghdam) + (pointzaribHeatReese);

                    var pointModel = new PointViewModel()
                    {
                        EshrafPeriodDefId = eshrafDefId,
                        DepartmentId = item.Id,
                        UnitCode = item.UnitCode,
                        PointEghdam = pointEghdam,
                        // PointReject = pointReject,
                        PointNezaja = pointzaribHeatReese,
                        FinalPoint = finalPoint,
                    };

                    AddToPoint(pointModel);


                }
            }







        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddToPoint(PointViewModel model)
        {
            var point = new Point()
            {
                EshrafPeriodDefId = model.EshrafPeriodDefId,
                DepartmentId = model.DepartmentId,
                UnitCode = model.UnitCode,
                PointEghdam = model.PointEghdam,
                PointReject = model.PointReject,
                PointNezaja = model.PointNezaja,
                FinalPoint = model.FinalPoint,
                Rank = model.Rank,
            };

            _context.Points.Add(point);
            _context.SaveChanges();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<TblDepartment> GetListAllUnitForCalculateRanking()
        {
            return _context.TblDepartments.ToList();
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public BaseResult Remove(Point point)
        {
            try
            {
                _context.Points.Remove(point);

                _context.SaveChanges();

                return new BaseResult
                {
                    Status = true,
                    Message = "عملیات حذف با موفقیت انجام شد"
                };
            }
            catch (Exception)
            {

                return new BaseResult
                {
                    Status = true,
                    Message = "عملیات حذف با خطا مواجه شد"
                };
            }

        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ListPointViewModel GetAll()
        {
            var list = new ListPointViewModel();

            IQueryable<Point> points = _context.Points
                .Include(x=>x.EshrafPeriodDef)
                .Include(x=>x.Department)
                .OrderByDescending(x=>x.FinalPoint);

            list.Points = points
               .Select(x => new PointViewModel()
               {
                   Id = x.Id,
                   EshrafPeriodDefId = x.EshrafPeriodDefId,
                   EshrafPeriodDefTitle = x.EshrafPeriodDef.Title,
                   DepartmentTitle = x.Department.DepartmentName,
                   UnitCode = x.UnitCode,
                   UnitTitle = x.UnitTitle,
                   CodeGha = x.CodeGha,
                   GhaTitle = x.GhaTitle,
                   PointEghdam = x.PointEghdam,
                   PointReject = x.PointReject,
                   PointNezaja = x.PointNezaja,
                   FinalPoint = x.FinalPoint,
                   Rank = x.Rank,

               }).ToList();


            return list;
        }


        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public List<Point> pointListHamvandUnit(List<int> units)
        {
            var eshrafdefId = GetEshrafPeriodDef();

            var allPoint = _context.Points.Where(x => x.EshrafPeriodDefId == eshrafdefId);

            List<Point> points = new List<Point>();

            foreach (var item in units)
            {
                var res = allPoint.Where(x => x.UnitCode == item).FirstOrDefault();

                points.Add(res);

            }

            return points.OrderBy(x => x.PointEghdam).ToList();
        }
    }
}
