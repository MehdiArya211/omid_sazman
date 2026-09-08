namespace VisitorManagment.Core.Constants
{
    /// <summary>
    /// کدهای سطح نهایی سازمانی ذخیره‌شده در RoleTypeFinalId.
    /// این مقادیر برای تشخیص سطح دسترسی سازمانی استفاده می‌شوند و RoleId نیستند.
    /// </summary>
    public static class SystemRoleFinalTypes
    {
        public const int Headquarters = 1;
        public const int UnitCommander = 2;
        public const int AnsarExpert = 5;
        public const int AbhadExpert = 6;
        public const int AbhadSecretary = 7;
        public const int AviationCommander = 9;
        public const int RegionalCommander = 11;

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
