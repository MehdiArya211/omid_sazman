using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.Convertors;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class PersonService : IPersonService
    {

        private string apiUrl;
        private string apiUrlTashvighat;
        private string apiUrlTanbihat;

        private readonly IConfiguration _configuration;
        private HttpClient _client;
        private readonly VisitorManagmentContext _context;
        public PersonService(VisitorManagmentContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            apiUrl = _configuration.GetSection("ItoWebApiUrl").GetSection("PersonnelUrl").Value;
            apiUrlTashvighat = _configuration.GetSection("ItoWebApiUrl").GetSection("TashvighatUrl").Value;
            apiUrlTanbihat = _configuration.GetSection("ItoWebApiUrl").GetSection("TanbihatUrl").Value;
            _client = new HttpClient();
        }

        public Personal GetPersonalByPersonalCode(string personalCode)
        {
            return _context.Personals.Where(p => p.PersonalCode == personalCode).SingleOrDefault();
        }

        public FactSpecificationPersonalViewModel GetPersonalForEdit(int personalId)
        {
            var person = GetPersonByPersonalId(personalId);
            var editPerson = new FactSpecificationPersonalViewModel()
            {
                Id = person.Id,
                PersonalCode = person.PersonalCode,
                MelliCode = person.MelliCode,
                FirstName = person.FirstName,
                LastName = person.LastName,
                RankTitle = person.RankTitle,
                BranchTitle = person.BranchTitle,
                StatusTitle = person.StatusTitle,
                DRSAD_JA = person.DRSAD_JA,
                DRSAD_JB = person.DRSAD_JB,
                IsarStatus = person.IsarStatus,
                TOT_AML2 = person.TOT_AML2,
                TOT_AML = person.TOT_AML,
                UnitDutyTitle = person.UnitDutyTitle,
                UnitTitle = person.UnitTitle,
                CodGhaTitle = person.CodGhaTitle,
                FPersonalCode = person.FarmandehPersonalCode,
                FPersonalName = person.FarmandehPersonalName,
                Addres = person.Addres,
                Phone = person.Phone,
                JobDes = person.JobDes,
                StatuseTitle = person.StatusTitle,
                PersonalAvatar = person.PersonalAvatar,
                //
                EmploymentDate = person.EmploymentDate,
                EmploymentTitle = person.EmploymentTitle,
                BirthPlaceTitle = person.BirthPlaceTitle,
                BirthDate = person.BirthDate,
                BloodTitle = person.BloodTitle,
                MarridTitle = person.MarridTitle,
                ReligoinTitle = person.ReligoinTitle
            };
            return editPerson;
        }

        public Personal GetPersonByPersonalId(int personsalId)
        {
            return _context.Personals.Where(p => p.Id == personsalId).SingleOrDefault();
        }

        //*************************************************************************************


        public FactSpecificationPersonalViewModel GetPersonalByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrl + "/GetPersonalByPersonalCode/" + personalCode).Result;
            //convert
            var person = JsonConvert.DeserializeObject<FactSpecificationPersonalViewModel>(result);

            return person;
        }

        public List<TashvighatInfoViewModel> GetTashvighatByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrlTashvighat + "/TashvighatInfo/GetTashvighatInfoByPersonalCode/" + personalCode).Result;
            //convert
            var Tashvighat = JsonConvert.DeserializeObject<List<TashvighatInfoViewModel>>(result);

            return Tashvighat;
        }



        public List<TanbihatInfoViewModel> GetTanbihatByPersonalNo(int fileId)
        {
            var personalCode = _context.Files.Find(fileId).PersonalCode.ToString();

            var result = _client.GetStringAsync(apiUrlTanbihat + "/TanbihatInfo/GetTanbihatInfoByPersonalCode/" + personalCode).Result;
            //convert
            var Tanbihat = JsonConvert.DeserializeObject<List<TanbihatInfoViewModel>>(result);

            return Tanbihat;
        }
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

        public List<FarmandehInfo> GetFarmandehInfos()
        {
            var result = _context.UserRoles.Include(x => x.Role).Include(x => x.User)
                .Where(x => x.Role.IsDelete == false)
                .Select(_ => new FarmandehInfo
                {
                    Id = _.User.UserName,
                    Title = _.User.RankTitle + " " + _.User.FirstName + " " + _.User.LastName + " " + _.User.JobDes + " (" + _.User.UserName + ") ",
                }).ToList();

            return result;
        }

        public List<FarmandehInfo> GetKarshenashAnsarInfos()
        {

            var result= _context.UserRoles.Include(x => x.Role).Include(x=>x.User)
                .Where(x=>x.Role.IsDelete==false)
                .Select(_ => new FarmandehInfo
                {
                    Id =_.User.UserName,
                    Title = _.User.RankTitle + " " + _.User.FirstName + " " + _.User.LastName + " " + _.User.JobDes + " (" + _.User.UserName + ") ",
                }).ToList();

            return result;
        }

        public List<FarmandehInfo> GetFarmandehAndKarshenasInfos()
        {
            var result = _context.UserRoles.Include(x => x.Role).Include(x => x.User)
                .Where(x => x.Role.IsDelete == false)
                .Select(_ => new FarmandehInfo
                {
                    Id = _.User.UserName,
                    Title = _.User.RankTitle + " " + _.User.FirstName + " " + _.User.LastName + " " + _.User.JobDes + " (" + _.User.UserName + ") ",
                }).ToList();

            return result;
        }



        public string GetAvatarUserByPrsnCd(int prsnCd)
        {
            var prsnCode = prsnCd.ToString();
            return _context.Users.Where(_ => _.UserName == prsnCode).Select(_ => _.UserAvatar).FirstOrDefault();
        }

        public FarmandehReportDTO GetPrsnInfoByPrsnId(string personalCode)
        {
            return _context.UserRoles.AsSplitQuery().Include(_ => _.User)
             .Where(_ => _.User.UserName == personalCode).Select(_ => new FarmandehReportDTO
             {
                 FullName = _.User.FirstName + " " + _.User.LastName,
                 Job = _.User.JobDes,
                 BranchTitle = _.User.BranchTitle,
                 PrsnCd = _.User.UserName,
                 Rank = _.User.RankTitle,
                 Organ = _.User.UnitDutyTitle,
                 UnitCode = _.User.UnitCode,
             }).FirstOrDefault();
        }


    }



}
