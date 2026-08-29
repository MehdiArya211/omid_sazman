using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.DTOs
{

    public class Chart
    {
        [JsonProperty(PropertyName = "label")] public string Label { get; set; }

        [JsonProperty(PropertyName = "data")] public List<int> Data { get; set; }

        [JsonProperty(PropertyName = "backgroundColor")]
        public string[] BackgroundColor { get; set; }

        [JsonProperty(PropertyName = "borderColor")]
        public string BorderColor { get; set; }
        public List<ChartOneDto> Files { get; set; }
    }

    public class ChartOneDto
    {
        public int? CodeGha { get; set; }
        public string CodeGhaTitle { get; set; }
    }
    public class ChartBarDto
    {
        public List<string> GharagahTitle { get; set; }
        public List<string> OrganTitle { get; set; }
        public List<string> RequestSubjectTitle { get; set; }
    }

    #region مدل گزارش فعالیت فرماندهان 
    public class ChartFarmandehActivityDto
    {
        [JsonProperty(PropertyName = "label")] public string Label { get; set; }

        [JsonProperty(PropertyName = "data")] public int Data { get; set; }

        [JsonProperty(PropertyName = "backgroundColor")]
        public string[] BackgroundColor { get; set; }

        [JsonProperty(PropertyName = "borderColor")]
        public string BorderColor { get; set; }

        public FarmandehActivityDto Files { get; set; }
    }

    public class FarmandehActivityDto
    {
        /// <summary>
        ///  //ثبت نظریه
        /// </summary>
        public int TotalNazarieh { get; set; }

        /// <summary>
        /// تعداد درخواست های اقدام شده
        /// </summary>
        public int TotalResolveRequest { get; set; }

        /// <summary>
        /// رد درخواست و عودت
        /// </summary>
        public int TotalReturnRequest { get; set; }

        /// <summary>
        /// در انتظار  
        /// </summary>
        public int TotalWaitingRequest { get; set; }


        public int TotalRequest { get; set; }

    }
    #endregion
}
