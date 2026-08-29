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
        public Chart ShowReportOne();
       // public Chart ShowReport();
        public ChartBarDto ShowReportBarChartSearch(DateTime? startDateSearch, DateTime? endDateSearch,
            int requestSubjectId = 0, int filterGharargahId = 0, int filterYeganId = 0);


        #region میزان فعالیت فرماندهان
        public ChartFarmandehActivityDto ShowReportChartFarmandehActivity(string personCode);
        #endregion

    }
}
