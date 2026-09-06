using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorManagment.Core.Convertors
{
    public class FixedText
    {

        //================ 3
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public static string FixedEmail(string email)
        {
            return email.Trim().ToLower();
        }
    }
}
