using System;
using System.Collections.Generic;

namespace VisitorManagment.Core.DTOs
{
    /// <summary>
    /// نتیجه جست‌وجوی مدیریتی درخواست‌های یک مراجعه‌کننده را نگهداری می‌کند.
    /// هر درخواست فقط یک‌بار نمایش داده می‌شود و گردش هامش‌های آن در مجموعه داخلی قرار می‌گیرد.
    /// </summary>
    public class AdminVisitorRequestSearchViewModel
    {
        public string PersonalCode { get; set; }
        public string FullName { get; set; }
        public string RankTitle { get; set; }
        public string UnitTitle { get; set; }
        public string UnitDutyTitle { get; set; }
        public string Phone { get; set; }
        public List<AdminVisitorRequestItemViewModel> Requests { get; set; } = new List<AdminVisitorRequestItemViewModel>();
    }

    public class AdminVisitorRequestItemViewModel
    {
        public int Id { get; set; }
        public string RequestSubjectTitle { get; set; }
        public string FileTypeTitle { get; set; }
        public string PriorityTitle { get; set; }
        public string FileStatusTitle { get; set; }
        public string RequestDescription { get; set; }
        public string ProblemDescription { get; set; }
        public bool IsArchived { get; set; }
        public bool IsFinished { get; set; }
        public DateTime RegDate { get; set; }
        public int EmptyHameshCount { get; set; }
        public List<string> CurrentOwners { get; set; } = new List<string>();
        public List<AdminVisitorHameshItemViewModel> Hameshes { get; set; } = new List<AdminVisitorHameshItemViewModel>();
    }

    public class AdminVisitorHameshItemViewModel
    {
        public int Id { get; set; }
        public string RegistrarFullName { get; set; }
        public string RegistrarPersonalCode { get; set; }
        public string RoleTitle { get; set; }
        public string ActionTitle { get; set; }
        public string Description { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class WorkflowReceiverViewModel
    {
        public int Id { get; set; }
        public string DisplayTitle { get; set; }
    }
}
