using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IChartService
    {
        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public Chart ShowReportOne();
       // public Chart ShowReport();
        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public ChartBarDto ShowReportBarChartSearch(DateTime? startDateSearch, DateTime? endDateSearch,
            int requestSubjectId = 0, int filterGharargahId = 0, int filterYeganId = 0);


        #region میزان فعالیت فرماندهان
        /// <summary>
        /// اطلاعات موردنیاز برای نمایش را آماده می‌کند.
        /// </summary>
        public ChartFarmandehActivityDto ShowReportChartFarmandehActivity(string personCode);
        #endregion

    }
}
