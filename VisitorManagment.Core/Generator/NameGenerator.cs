using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorManagment.Core.Generator
{
    public class NameGenerator
    {
        public static string GenerateUniqCode()
        {
            //==== GUId = Globaly Unique Identifire
            return Guid.NewGuid().ToString().Replace("-","");
        }
    }
}
