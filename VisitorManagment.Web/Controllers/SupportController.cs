using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisitorManagment.Web.Controllers
{
    [Authorize]
    public class SupportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
