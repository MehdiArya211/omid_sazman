using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisitorManagment.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
    }
}
