namespace VisitorManagment.Core.Constants
{
    /// <summary>
    /// کدهای سطح نهایی سازمانی ذخیره‌شده در RoleTypeFinalId.
    /// این مقادیر برای تشخیص سطح دسترسی سازمانی استفاده می‌شوند و RoleId نیستند.
    /// </summary>
    public static class SystemRoleFinalTypes
    {
        public const int Headquarters = 1;           // سطح ستاد یا قرارگاه
        public const int UnitCommander = 2;          // سطح فرمانده یگان
        public const int AnsarExpert = 5;            // سطح کارشناس قرارگاه انصار
        public const int AbhadExpert = 6;             // سطح کارشناس ابهاد
        public const int AbhadSecretary = 7;          // سطح دبیر ابهاد
        public const int AviationCommander = 9;      // سطح فرمانده هوانیروز
        public const int RegionalCommander = 11;     // سطح فرمانده قرارگاه منطقه‌ای

        /// <summary>
        /// سطوح نهایی قابل انتخاب در فرم نقش را برمی‌گرداند.
        /// </summary>
        public static System.Collections.Generic.IReadOnlyDictionary<int, string> GetManagementOptions() =>
            new System.Collections.Generic.Dictionary<int, string>
            {
                [Headquarters] = "ستاد و قرارگاه",
                [UnitCommander] = "فرمانده یگان",
                [AnsarExpert] = "کارشناس انصار",
                [AbhadExpert] = "کارشناس ابهاد",
                [AbhadSecretary] = "دبیر ابهاد",
                [AviationCommander] = "فرمانده هوانیروز",
                [RegionalCommander] = "فرمانده قرارگاه منطقه‌ای"
            };

        /// <summary>
        /// اعتبار کد سطح نهایی را پیش از ذخیره نقش بررسی می‌کند.
        /// </summary>
        public static bool IsKnown(int? roleFinalType) =>
            !roleFinalType.HasValue || GetManagementOptions().ContainsKey(roleFinalType.Value);
    }
}
