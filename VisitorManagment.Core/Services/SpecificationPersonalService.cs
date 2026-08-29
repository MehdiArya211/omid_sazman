using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class SpecificationPersonalService : ISpecificationPersonal
    {
        private string apiUrl;
        private string apiUrlTashvighat;
        private string apiUrlTanbihat;

        private readonly IConfiguration _configuration;
        private HttpClient _client;
        private readonly VisitorManagmentContext _context;
        public SpecificationPersonalService(VisitorManagmentContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            apiUrl = _configuration.GetSection("ItoWebApiUrl").GetSection("PersonnelUrl").Value;
            apiUrlTashvighat = _configuration.GetSection("ItoWebApiUrl").GetSection("TashvighatUrl").Value;
            apiUrlTanbihat = _configuration.GetSection("ItoWebApiUrl").GetSection("TanbihatUrl").Value;
            _client = new HttpClient();
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public FactSpecificationPersonalViewModel GetPersonalByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrl + "/GetPersonalByPersonalCode/" + personalCode).Result;
            //convert
            var person = JsonConvert.DeserializeObject<FactSpecificationPersonalViewModel>(result);

            return person;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<TashvighatInfoViewModel> GetTashvighatByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrlTashvighat + "/TashvighatInfo/GetTashvighatInfoByPersonalCode/" + personalCode).Result;
            //convert
            var Tashvighat = JsonConvert.DeserializeObject<List<TashvighatInfoViewModel>>(result);

            return Tashvighat;
        }



        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<TanbihatInfoViewModel> GetTanbihatByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrlTanbihat + "/TanbihatInfo/GetTanbihatInfoByPersonalCode/" + personalCode).Result;
            //convert
            var Tanbihat = JsonConvert.DeserializeObject<List<TanbihatInfoViewModel>>(result);

            return Tanbihat;
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ListFileViewModel GetFile(int FileId)
        {
            var personalCode = _context.Files.Find(FileId).PersonalCode.ToString();

            IQueryable<Files> result = _context.Files.Where(f => f.PersonalCode == personalCode);

            ListFileViewModel list = new ListFileViewModel() { };

            list.files = result.Select(t => new FactPersonalViewModel()
            {
                Id = t.Id,
                ReqSubTitle = t.RequestSubject.Title,
                RegDate = t.RegDate
            }).ToList();
            return list;
        }


        #endregion
    }
}
