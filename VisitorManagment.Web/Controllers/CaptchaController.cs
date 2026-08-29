using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using VisitorManagment.Web.Pages.Captcha;

namespace Peygiry.Web.Controllers
{
    [AllowAnonymous]
    public class CaptchaController : Controller
    {
        public IActionResult Index()
        {
            int width = 150;
            int height = 40;
            var captchaCode = CaptchaService.GenerateCaptchaCode();
            var result = CaptchaService.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
    }
}
