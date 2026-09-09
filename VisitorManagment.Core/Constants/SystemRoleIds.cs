namespace VisitorManagment.Core.Constants
{
    /// <summary>
    /// شناسه نقش‌های ثابت موجود در دیتابیس فعلی.
    /// استفاده از این کلاس مانع پراکندگی اعداد نامفهوم در سرویس‌های قدیمی می‌شود.
    /// برای نقش‌های جدید باید تا حد امکان از RoleType یا RoleId دریافت‌شده از دیتابیس استفاده شود.
    /// </summary>
    public static class SystemRoleIds
    {
        public const int ExecutiveOffice = 2;               // دفتر اجرایی
        public const int UnitCommander = 6;                 // فرمانده یگان
        public const int MajorUnitCommander = 7;            // فرمانده یگان عمده
        public const int HumanResourcesDeputy = 8;          // معاونت نیروی انسانی
        public const int EngineeringDeputy = 9;             // معاونت مهندسی
        public const int MeetingManager = 10;               // مدیر جلسات ملاقات
        public const int DefaultVisitor = 14;               // نقش پیش‌فرض مراجعه‌کننده
        public const int MeetingCoordinator = 15;           // هماهنگ‌کننده جلسات ملاقات
        public const int PlanningDeputy = 16;                // معاونت طرح و برنامه
        public const int TrainingDeputy = 18;                // معاونت آموزش
        public const int InspectionOffice = 19;              // بازرسی نزاجا
        public const int LogisticsDeputy = 20;               // معاونت آماد و پشتیبانی
        public const int LegalDeputy = 21;                   // معاونت حقوقی و قضایی
        public const int MeetingBoardChairman = 22;          // رئیس هیئت‌رئیسه جلسه
        public const int MeetingBoardFirstMember = 24;       // عضو اول هیئت‌رئیسه جلسه
        public const int MeetingBoardSecondMember = 26;      // عضو دوم هیئت‌رئیسه جلسه
        public const int HealthOffice = 27;                  // اداره بهداشت
        public const int VeteransOffice = 28;                // اداره ایثارگران
        public const int AviationCommand = 32;               // فرماندهی هوانیروز
        public const int FinanceOffice = 35;                 // اداره دارایی

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
