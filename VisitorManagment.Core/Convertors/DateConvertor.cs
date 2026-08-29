using System;
using System.Globalization;

namespace VisitorManagment.Core.Convertors
{
    public static class DateConvertor
    {
        #region اعضا و متدهای کلاس

        /// <summary>
        /// مقدار ورودی را به قالب موردنظر تبدیل می‌کند.
        /// </summary>

        /// <summary>
        /// مقدار ورودی را به قالب موردنظر تبدیل می‌کند.
        /// </summary>
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            return pc.GetYear(value) + "/" + pc.GetMonth(value).ToString("00") + "/" + pc.GetDayOfMonth(value) + " " +pc.GetHour(value).ToString("00") + ":" + pc.GetMinute(value).ToString("00") + ":" + pc.GetSecond(value).ToString("00");
        }
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public static string StringToDate(this string value)
        {
            if (value==null || value=="")
            {
                return value;
            }
            string Year = value.Substring(0, 4);
            string month = value.Substring(4, 2);
            string day = value.Substring(6, 2);
            string date = Year + "/" + month + "/" + day;
            return date;
        }

        #endregion
    }




}
