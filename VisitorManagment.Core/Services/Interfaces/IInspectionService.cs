using System.Collections.Generic;
using VisitorManagment.Core.DTOs.Inspection;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IInspectionService
    {
        InspectionPersonLookupViewModel LookupPerson(string personalCode);
        InspectionOperationResult RegisterPerson(string personalCode, int userId);
        IReadOnlyList<InspectionPerson> GetPeople();
        IReadOnlyList<InspectionUnitViewModel> GetRegisteredUnits();
        InspectionOperationResult CreateMeeting(InspectionMeetingCreateViewModel model, int userId);
        IReadOnlyList<InspectionMeeting> GetMeetings();
        IReadOnlyList<InspectionMeeting> GetAvailableMeetings(int unitCode);
        bool CanUnitEnterMeeting(int meetingId, int unitCode);
        InspectionOperationResult StartAttendance(int meetingId, string personalCode, string fullName, string rankTitle, int unitCode, string unitTitle, string connectionId);
        void EndAttendance(string connectionId);
        IReadOnlyList<InspectionAttendanceSummaryViewModel> GetAttendanceHistory(int? meetingId = null, string search = null);
    }
}
