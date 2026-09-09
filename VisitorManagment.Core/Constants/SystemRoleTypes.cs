using System.Collections.Generic;

namespace VisitorManagment.Core.Constants
{
    /// <summary>
    /// کدهای نوع سمت سازمانی ذخیره‌شده در Role.RoleType و Hamesh.RoleTypeId.
    /// این مقادیر شناسه رکورد نقش نیستند و نباید به‌جای RoleId استفاده شوند.
    /// </summary>
    public static class SystemRoleTypes
    {
        #region User administration levels

        public const int OrdinaryUser = 1;                  // کاربر عادی سامانه
        public const int DirectUnitCommander = 2;           // فرمانده یگان مستقیم
        public const int MajorUnitCommander = 3;            // فرمانده یگان عمده
        public const int SystemAdministrator = 100;         // مدیر کل سامانه
        public const int HeadquartersAdministrator = 101;  // مدیر قرارگاه
        public const int UnitAdministrator = 102;           // مدیر یگان

        #endregion

        #region Workflow positions

        public const int AnsarHeadquartersExpert = 5;       // کارشناس قرارگاه انصار نزاجا
        public const int UnitCommand = 6;                   // فرماندهی یگان
        public const int NezajaOperator = 7;                // کاربر نزاجا
        public const int PresidingBoard = 9;                // عضو هیئت‌رئیسه
        public const int DeputyOffice = 10;                 // معاونت یا دفتر تخصصی

        // این دو مقدار قدیمی در سوابق هامش ذخیره شده‌اند و برای سازگاری نگهداری می‌شوند.
        public const int GharargahCommanderLegacy = 1000;   // فرمانده قرارگاه در سوابق قدیمی هامش
        public const int UnitCommanderLegacy = 3000;        // فرمانده یگان در سوابق قدیمی هامش

        #endregion

        /// <summary>
        /// مشخص می‌کند کاربر اختیار مدیریت کل سامانه را دارد یا خیر.
        /// </summary>
        public static bool IsSystemAdministrator(int roleType) => roleType == SystemAdministrator;

        /// <summary>
        /// انواع نقشی را برمی‌گرداند که مدیر یگان مجاز است به کاربران اختصاص دهد.
        /// </summary>
        public static IReadOnlyCollection<int> GetUnitAdministratorAssignableTypes() => new[]
        {
            OrdinaryUser,
            DirectUnitCommander,
            MajorUnitCommander
        };

        /// <summary>
        /// انواع نقشی را برمی‌گرداند که مدیر قرارگاه مجاز است به کاربران اختصاص دهد.
        /// </summary>
        public static IReadOnlyCollection<int> GetHeadquartersAdministratorAssignableTypes() => new[]
        {
            OrdinaryUser,
            DirectUnitCommander,
            MajorUnitCommander,
            UnitAdministrator
        };

        /// <summary>
        /// انواع سمت‌هایی را برمی‌گرداند که هامش آن‌ها در بخش «سلسله‌مراتب سایر یگان‌ها» نمایش داده نمی‌شود.
        /// </summary>
        public static IReadOnlyCollection<int> GetMainWorkflowTypes() => new[]
        {
            OrdinaryUser,
            AnsarHeadquartersExpert,
            UnitCommand,
            NezajaOperator,
            PresidingBoard,
            DeputyOffice
        };

        /// <summary>
        /// سمت‌های قابل انتخاب در فرم مدیریت نقش را همراه با عنوان خوانای فارسی برمی‌گرداند.
        /// </summary>
        public static IReadOnlyDictionary<int, string> GetManagementOptions() => new Dictionary<int, string>
        {
            [OrdinaryUser] = "کاربر عادی",
            [DirectUnitCommander] = "فرمانده یگان مستقیم",
            [MajorUnitCommander] = "فرمانده یگان عمده",
            [AnsarHeadquartersExpert] = "کارشناس قرارگاه انصار",
            [UnitCommand] = "فرماندهی یگان",
            [NezajaOperator] = "کاربر نزاجا",
            [PresidingBoard] = "هیئت‌رئیسه",
            [DeputyOffice] = "معاونت",
            [SystemAdministrator] = "مدیر سامانه",
            [HeadquartersAdministrator] = "مدیر قرارگاه",
            [UnitAdministrator] = "مدیر یگان"
        };

        /// <summary>
        /// اعتبار کد نوع نقش را پیش از ثبت یا ویرایش بررسی می‌کند.
        /// </summary>
        public static bool IsKnown(int roleType) => GetManagementOptions().ContainsKey(roleType);
    }
}
