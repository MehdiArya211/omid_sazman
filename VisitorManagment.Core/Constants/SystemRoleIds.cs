namespace VisitorManagment.Core.Constants
{
    /// <summary>
    /// شناسه نقش‌های ثابت موجود در دیتابیس فعلی.
    /// استفاده از این کلاس مانع پراکندگی اعداد نامفهوم در سرویس‌های قدیمی می‌شود.
    /// برای نقش‌های جدید باید تا حد امکان از RoleType یا RoleId دریافت‌شده از دیتابیس استفاده شود.
    /// </summary>
    public static class SystemRoleIds
    {
        public const int ExecutiveOffice = 2;
        public const int UnitCommander = 6;
        public const int MajorUnitCommander = 7;
        public const int HumanResourcesDeputy = 8;
        public const int EngineeringDeputy = 9;
        public const int MeetingManager = 10;
        public const int PlanningDeputy = 16;
        public const int TrainingDeputy = 18;
        public const int InspectionOffice = 19;
        public const int LogisticsDeputy = 20;
        public const int LegalDeputy = 21;
        public const int MeetingBoardChairman = 22;
        public const int MeetingBoardFirstMember = 24;
        public const int MeetingBoardSecondMember = 26;
        public const int HealthOffice = 27;
        public const int VeteransOffice = 28;
        public const int AviationCommand = 32;
        public const int FinanceOffice = 35;
        public const int DefaultVisitor = 14;
        public const int MeetingCoordinator = 15;

        /// <summary>
        /// مشخص می‌کند نقش اجازه برگزاری و ثبت نتیجه جلسه هیئت‌رئیسه را دارد یا خیر.
        /// </summary>
        public static bool CanHoldMeeting(int roleId)
        {
            return roleId == MeetingBoardChairman ||
                   roleId == MeetingBoardFirstMember ||
                   roleId == MeetingBoardSecondMember;
        }
    }
}
