using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Services;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserHelperService _userHelperService;

        public HomeController(UserHelperService userHelperService)
        {
            _userHelperService = userHelperService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
