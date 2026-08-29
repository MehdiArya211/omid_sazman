using System;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.DTOs.Ranking;
using VisitorManagment.DataLayer.Entities.Ranking;

namespace VisitorManagment.Core.Services.Interfaces.Ranking
{
    public interface IRankingService
    {
        /// <summary>
        /// بدست آوردن تعداد درخواست ملاقات هایی که اقدام شده است براساس کد یگان
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        int GetCountEghdamWithUnitCode(int unitCode);

        /// <summary>
        /// بدست آوردن تعداد درخواست ملاقات هایی که رد درخواست شده است براساس کد یگان
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        int GetCountRejectWithUnitCode(int unitCode);

        /// <summary>
        /// فرمول کلی محاسبه ی نمره
        /// </summary>
        /// <param name="pointEghdam"></param>
        /// <param name="pointReject"></param>
        /// <param name="pointHeatReeise"></param>
        /// <returns></returns>
        int CalculateFinalPoint(int pointEghdam , int pointReject , int pointHeatReeise);

        /// <summary>
        /// بدست آوردن رتبه نفر
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        int GetRanking(int unitCode);
        int GetEshrafDefId(DateTime date);

        /// <summary>
        /// پیداکردن نوع قسمت با کد یگان
        /// </summary>
        /// <returns></returns>
        int GetDepartmentTypeWithUnitCode(int unitCode);

        /// <summary>
        /// پیدا کردن لیست یگان های هم وند با شناسه قسمت
        /// </summary>
        /// <param name="departmentTypeId"></param>
        /// <returns></returns>
        List<int> GetListUnitWithDepartmentTypeId(int departmentTypeId);

        /// <summary>
        /// گرفتن شناسه بازه ماهانه برای رتبه بندی
        /// </summary>
        /// <returns></returns>
        int GetEshrafPeriodDef();
        void FinalCalculate();
        void AddToPoint(PointViewModel model);
        /// <summary>
        /// لیست تمام یگان ها برای محاسبه رتبه
        /// </summary>
        /// <returns></returns>
        List<TblDepartment> GetListAllUnitForCalculateRanking();

        BaseResult Remove(Point point);
        /// <summary>
        /// لیست نمرات
        /// </summary>
        /// <returns></returns>
        ListPointViewModel GetAll();


        /// <summary>
        /// بدست آوردن لیست نمرات تمام یگان های هم وند
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        List<Point> pointListHamvandUnit(List<int> units);


    }
}
