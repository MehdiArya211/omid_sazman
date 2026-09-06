using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisitorManagment.Web.Helpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    //[HtmlTargetElement("tag-name")]
    //public class SessionHelper : TagHelper
    //{
    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {

    //    }
    //}

    public static class SessionHelper
    {
        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
