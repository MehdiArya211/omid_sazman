using System.Collections.Generic;
using VisitorManagment.Core.DTOs.Inspection;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IInspectionService
    {
        InspectionPersonLookupViewModel LookupPerson(string personalCode);
        InspectionOperationResult RegisterPerson(string personalCode, int userId);
        InspectionOperationResult DeletePerson(int personId);
        IReadOnlyList<InspectionPerson> GetPeople();
        IReadOnlyList<InspectionUnitViewModel> GetRegisteredUnits();
        InspectionOperationResult CreateMeeting(InspectionMeetingCreateViewModel model, int userId);
        InspectionOperationResult UpdateMeetingPeople(int meetingId, IEnumerable<int> personIds, int userId);
        IReadOnlyList<InspectionMeeting> GetMeetings();
        IReadOnlyList<InspectionMeeting> GetAvailableMeetings(int unitCode);
        bool CanUnitEnterMeeting(int meetingId, int unitCode);
        IReadOnlyList<InspectionParticipationSummaryViewModel> GetParticipationHistory(int? meetingId = null, string search = null);
    }
}
