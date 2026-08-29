using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorManagment.Core.Generator
{
    public class NameGenerator
    {
        /// <summary>
        /// خروجی موردنیاز را تولید می‌کند.
        /// </summary>
        public static string GenerateUniqCode()
        {
            //==== GUId = Globaly Unique Identifire
            return Guid.NewGuid().ToString().Replace("-","");
        }
    }
}
