using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class ChartService : IChartService
    {
        private readonly VisitorManagmentContext _context;
        public ChartService(VisitorManagmentContext context)
        {
            _context = context;
        }

        //public Chart ShowReport()
        //{
        //    IQueryable<Files> result = _context.Files;

        //    var chart = new Chart()
        //    {
        //        Files = result
        //    };

        //    return chart;
        //}

        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public ChartBarDto ShowReportBarChartSearch(DateTime? startDateSearch, DateTime? endDateSearch, int requestSubjectId = 0,
                                                 int filterGharargahId = 0, int filterYeganId = 0)

        {
            IQueryable<Files> files = _context.Files.Include(x => x.RequestSubject);

            #region Search
            if (filterGharargahId != 0)
            {
                files = files.Where(x => x.CodGha == filterGharargahId);
            }

            if (filterYeganId != 0)
            {
                files.Where(x => x.UnitCode == filterYeganId);
            }

            if (requestSubjectId != 0)
            {
                files.Where(x => x.RequestSubjectId == requestSubjectId);
            }

            if (requestSubjectId != 0)
            {
                files.Where(x => x.RequestSubjectId == requestSubjectId);
            }

            if (startDateSearch != null)
            {
                if (endDateSearch != null)
                {
                    files = files.Where(u => u.RegDate >= startDateSearch && u.RegDate <= endDateSearch);
                }

            }
            #endregion

            var model = new ChartBarDto()
            {
                //مانده
            };

            return model;

        }

        #region میزان فعالیت فرماندهان
        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public ChartFarmandehActivityDto ShowReportChartFarmandehActivity(string personCode)
        {
            var res = _context.Hameshes.Include(x => x.File);
            var prsnCd = personCode.ToString();
            int userId = _context.Users.FirstOrDefault(_ => _.UserName == prsnCd).Id;
            //ثبت نظریه
            //var TotalCheckRequest = _context.Hameshes.AsSplitQuery().Include(x => x.File).Where(x => x.File.FarmandehPersonalCode == personCode && x.UserDesc != "").Count();
            var TotalNazarieh = _context.Hameshes.Where(_ => _.UserId == userId && _.ActionTypeId == 2).Count();
            //تعداد درخواست های اقدام شده
            //var TotalResolveRequest = _context.Hameshes.Include(x => x.File).Where(x => x.File.FarmandehPersonalCode == personCode && x.ActionTypeId == 1).Count();
            var TotalResolveRequest = _context.Hameshes.Where(_ => _.UserId == userId && _.ActionTypeId == 1).Count();
            //رد درخواست و عودت   
            //var TotalForwardRequest = _context.Hameshes.Include(x => x.File).Where(x => x.File.FarmandehPersonalCode == personCode && x.ActionTypeId == 3).Count();
            var TotalReturnRequest = _context.Hameshes.Where(_ => _.UserId == userId && _.ActionTypeId == 3).Count();

            //در انتظار
            var TotalWaitingRequest = _context.Hameshes.Where(_ => _.UserId == userId && _.ActionTypeId == 1002).Count();


            var result = new ChartFarmandehActivityDto();

            result.Files = new FarmandehActivityDto();

            //fileResult.TotalCheckRequest = TotalCheckRequest;
            //fileResult.TotalResolveRequest = TotalResolveRequest;
            //fileResult.TotalForwardRequest = TotalForwardRequest;

            result.Files.TotalNazarieh = TotalNazarieh;
            result.Files.TotalResolveRequest = TotalResolveRequest;
            result.Files.TotalReturnRequest = TotalReturnRequest;
            result.Files.TotalWaitingRequest = TotalWaitingRequest;
            result.Files.TotalRequest = TotalNazarieh + TotalWaitingRequest + TotalReturnRequest + TotalResolveRequest;


            return result;

        }
        #endregion
        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public Chart ShowReportOne()
        {
            var files = _context.Files.Where(x => x.CodGhaTitle != null);

            var chart = new Chart();
            chart.Files = files.Select(t => new ChartOneDto()
            {

                CodeGha = t.CodGha,
                CodeGhaTitle = t.CodGhaTitle,

            }).ToList();


            return chart;

        }

        //
    }
}
