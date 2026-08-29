using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorManagment.Core.Convertors
{
    public class FixedText
    {

        //================ 3
        public static string FixedEmail(string email)
        {
            return email.Trim().ToLower();
        }
    }
}
