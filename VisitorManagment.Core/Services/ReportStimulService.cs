using Microsoft.EntityFrameworkCore;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Convertors;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using System.Collections.Generic;

namespace VisitorManagment.Core.Services
{
    public class ReportStimulService : IReportStimulService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IPersonService _personService;
        private readonly IWebApiService _webApiService;
        private readonly IHameshService _hameshService;
        public ReportStimulService(VisitorManagmentContext context, IPersonService personService, IHameshService hameshService,
            IWebApiService webApiService)
        {
            _context = context;
            _personService = personService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }
        public ReportTestInfoViewModel GetReportFullPersonal(int fileId)
        {
            var result = _context.Files.Include(x => x.Personal)
                .Include(x => x.Hameshes).Where(x => x.Id == fileId)
                .SingleOrDefault();

            var meeting = _context.Meetings.Where(x => x.Id == result.MeetingId).FirstOrDefault();

            #region All Hamesh

            var hameshUnitDuty = _hameshService.GetHameshFUnitDuty(fileId);

            if (hameshUnitDuty == null)
            {
                hameshUnitDuty = "";
            }

            var hameshUnit = _hameshService.GetHameshFUnit(fileId);

            if (hameshUnit == null)
            {
                hameshUnit = "";
            }

            var hameshGharargah = _hameshService.GetHameshFGharargah(fileId);

            if (hameshGharargah == null)
            {
                hameshGharargah = "";
            }
            #endregion

            var personalCode = result.PersonalCode;

            #region Tashilate

            // string resultTashilat = "";
            var tashilat = _webApiService.GetTashilatDastorForStimul(personalCode);

            var rowBeginDatestring = "";
            var maliTitlestring = "";
            var mablagheVamstring = "";


            if (tashilat != null)
            {
                var rowBeginDate = tashilat.Select(x => x.RowBeginDate).ToList();
                var maliTitle = tashilat.Select(x => x.MaliTitle).ToList();
                var mablagheVam = tashilat.Select(x => x.MablagheVam).ToList();

                foreach (var item in rowBeginDate)
                {
                    rowBeginDatestring += item + "\n";
                }

                foreach (var item in maliTitle)
                {
                    maliTitlestring += item + "\n";
                }

                foreach (var item in mablagheVam)
                {
                    mablagheVamstring += item.Value.ToString("N0") + "\n";
                }
            }
            #endregion


            #region job location

            var jobLocation = _webApiService.GetJobLocation(personalCode);

            var fromOrgan = jobLocation?.Data?.Select(x => x.FromOrgan).ToList() ?? new List<string>();

            //var fromOrgan1 = jobLocation.Data.Select(x => x.FromOrgan).ToList();
            //var wentDate1 = jobLocation.Data.Select(x => x.WentDate).ToList();

            var wentDate = jobLocation?.Data?.Select(x => x.WentDate).ToList() ?? new List<string>();

            // var toOrgan = jobLocation.Data.Select(x => x.ToOrgan).ToList();

            var toOrgan = jobLocation?.Data?.Select(x => x.ToOrgan).ToList() ?? new List<string>();


            var fromOrganstring = "";
            var wentDatestring = "";
            var toOrganstring = "";

            foreach (var item in fromOrgan)
            {
                fromOrganstring += item + "\n";
            }
            foreach (var item in wentDate)
            {
                wentDatestring += item + "\n";
            }
            foreach (var item in toOrgan)
            {
                toOrganstring += item + "\n";
            }
            #endregion

            var countTashvighat = _webApiService.GetCountTashvighat(personalCode);
            var countTanbihat = _webApiService.GetCountTanbihat(personalCode);

            #region fill model
            var person = new ReportTestInfoViewModel() { };
            person.MeetingDate = meeting.StartMeetingDate;
            person.PersonalCode = result.PersonalCode;
            person.FirstName = result.FirstName;
            person.LastName = result.LastName;
            person.RankTitle = result.RankTitle;
            person.BranchTitle = result.BranchTitle;
            person.JobDes = result.JobDes;
            person.UnitDutyTitle = result.UnitDutyTitle;
            person.UnitTitle = result.UnitTitle;
            person.CodGhaTitle = result.CodGhaTitle;
            person.RequestDescription = result.RequestDescription;
            person.MarridTitle = _webApiService.GetStatusMarrid(personalCode);
            person.Address = result.Addres;
            person.Phone = result.Phone;
            person.DrsadJa = result.DRSAD_JA;
            person.DrsadJb = result.DRSAD_JB;
            person.TotAml2 = result.TOT_AML2;
            person.TotAml = result.TOT_AML;
            person.EmployDate = _webApiService.GetEmployDate(personalCode);
            person.RegDate = result.RegDate.ToShamsi();
            person.ProblemDescription = result.ProblemDescription;
            person.HameshUnitDuty = hameshUnitDuty;
            person.HameshUnit = hameshUnit;
            person.HameshGharagah = hameshGharargah;
            person.TotalMoney = result.TotalMoney;
            person.ReciveMoney = result.ReciveMoney;
            person.CountVam = result.CountVam;
            person.SumAghsatVamMahiyaneh = result.SumAghsatVamMahiyaneh;
            //person.HameshMoavenat = _hameshService.GetHameshMoavenatForFRadeBalatar(fileId)
            //    .Select(x => x.RoleTypeTitle + "  :  " + x.UserDesc)
            //    .FirstOrDefault();
            person.HameshMoavenat = _hameshService.getAllHameshMoavenat(fileId);
            person.TashvighatCount = countTashvighat;
            //person.FNameUnitWithRank = result.us;
            person.TanbihatCount = countTanbihat;
            person.RolePersonLogin = "";
            person.HameshKarshenasGharagahAnsarNezaja = _hameshService.GetFirstHameshKarshenasgharagahAnsarNezaja(fileId);
            #endregion

            #region Tashilate
            if (tashilat != null)
            {
                person.TashilatBelaavazMaliTitle = maliTitlestring;
                person.TashilatBelaavazMablagheVam = mablagheVamstring;
                person.TashilatBelaavazRowBeginDate = rowBeginDatestring;
            }
            else
            {
                person.TashilatBelaavazMaliTitle = "-";
                person.TashilatBelaavazMablagheVam = "-";
                person.TashilatBelaavazRowBeginDate = "-";
            }

            #endregion

            #region location job

            person.FromOrgan = fromOrganstring;
            person.WentDate = wentDatestring;
            person.ToOrgan = toOrganstring;

            #endregion

            return person;
        }


        public ReportTestInfoViewModelV2 GetReportFullPersonalV2(int fileId)
        {
            var file = _context.Files
                .Include(f => f.Personal)
                .FirstOrDefault(f => f.Id == fileId);

            if (file?.Personal == null)
                return new ReportTestInfoViewModelV2();

            var personalCode = file.Personal.PersonalCode;

            var user = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserName == personalCode);

            if (user == null || user.UserRoles == null || !user.UserRoles.Any())
                return new ReportTestInfoViewModelV2();

            var userRole = user.UserRoles.First();
            var role = userRole.Role;
            var roleType = role.RoleType;
            var roleTypeFinalId = role.RoleTypeFinalId;

            string hameshUnit = "-";
            string hameshGharagah = "-";

            if (roleType == 3000 || roleType == 7000)
            {
                hameshUnit = GetHameshByRoleTypeFinal(fileId, 2); // فرمانده یگان

                if (roleTypeFinalId == 13) // لشگر
                {
                   // bool isMostaghel = role.IsMostaghel;
                    //hameshGharagah = isMostaghel
                    //    ? GetHameshByRoleTypeFinal(fileId, 5) // کارشناس ق انصار نزاجا
                    //    : GetHameshByRoleTypeFinal(fileId, 11); // ف ق منطقه‌ای
                }
                else if (roleType == 7000)
                {
                    hameshGharagah = GetHameshByRoleTypeFinal(fileId, 9); // ف هوانیروز
                }
                else
                {
                    hameshGharagah = GetHameshByRoleTypeFinal(fileId, 11); // ف ق منطقه‌ای
                }
            }
            else if (roleType == 2000)
            {
                hameshUnit = GetHameshByRoleTypeFinal(fileId, 6); // کارشناس ابهاد
                hameshGharagah = GetHameshByRoleTypeFinal(fileId, 7); // دبیر ابهاد
            }
            else if (roleType == 5000)
            {
                hameshUnit = GetHameshByRoleTypeFinal(fileId, 2); // فرمانده مرکز
                hameshGharagah = GetHameshByRoleTypeFinal(fileId, 5); // کارشناس انصار نزاجا
            }

            return new ReportTestInfoViewModelV2
            {
                HameshUnit = hameshUnit ?? "-",
                HameshGharagah = hameshGharagah ?? "-"
            };
        }



        public string GetHameshByRoleTypeFinal(int fileId, int roleTypeFinalId)
        {
            var hamesh = _context.Hameshes.Include(x => x.User)
                .ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                .Where(x => x.FileId == fileId
                            && x.User.UserRoles.Any(ur => ur.Role.RoleTypeFinalId == roleTypeFinalId))
                .OrderByDescending(x => x.Id)
                .Select(x => x.UserDesc + "<br>" + x.User.RankTitle + " " + x.User.FirstName + " " + x.User.LastName + "<br>" + x.RegDate.ToShamsi())
                .LastOrDefault();

            return hamesh ?? "";
        }


    }
}
