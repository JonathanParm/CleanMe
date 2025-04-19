using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CleanMe.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IHostEnvironment _env;

        public ErrorController(IHostEnvironment env)
        {
            _env = env;
        }

        [Route("Error")]
        public IActionResult HandleError()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandlerFeature != null)
            {
                ViewBag.ErrorMessage = _env.IsDevelopment()
                    ? exceptionHandlerFeature.Error.Message
                    : "An unexpected error occurred. Please try again later.";
            }

            return View("Error");
        }
    }
}