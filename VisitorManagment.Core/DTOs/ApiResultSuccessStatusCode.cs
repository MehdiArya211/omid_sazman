using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{
    public enum ApiResultSuccessStatusCode
    {
        [Display(Name="عملیات با موفقیت انجام شد")]
        Success=0,

        [Display(Name = " خطایی در سرور رخ داده است")]
        ServerError = 1,

        //[Display(Name = " خطایی در سرور رخ داده است")]
        //ServerError = 1,

    }
}
