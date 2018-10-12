using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellerBox.Models;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Restart()
        {
            Common.Services.VkCallbackWorkerService.Restart();

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ShortLink(string key, string id)
        {
            var guid = System.Guid.Parse("89877DEF-CDE0-4CB4-37C2-08D61E2E3F88");

            var encode = Common.Helpers.UrlShortenerHelper.Encode(guid);
            var idSubscriber = Common.Helpers.UrlShortenerHelper.Decode(encode);

            var result = Common.Helpers.UrlShortenerHelper.Convert(idSubscriber);
            return Redirect("http://www.google.ru");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
