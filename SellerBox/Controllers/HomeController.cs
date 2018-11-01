using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Models;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ShortLink(string key, string id)
        {
            var bi_idShortUrl = Common.Helpers.UrlShortenerHelper.Decode(key);
            var bi_idSubscriber = Common.Helpers.UrlShortenerHelper.Decode(id);
            var idShortUrl = Common.Helpers.UrlShortenerHelper.Convert(bi_idShortUrl);
            var idSubscriber = Common.Helpers.UrlShortenerHelper.Convert(bi_idSubscriber);

            var shortUrl = await _context.ShortUrls.FindAsync(idShortUrl);
            if (shortUrl == null)
                return NotFound();

            if (!shortUrl.IsSingleClick)
            {
                if (await _context.Subscribers.AnyAsync(x => x.Id == idSubscriber))
                {
                    await _context.History_ShortUrlClicks.AddAsync(new Models.Database.History_ShortUrlClicks()
                    {
                        Dt = System.DateTime.UtcNow,
                        IdShortUrl = idShortUrl,
                        IdSubscriber = idSubscriber
                    });
                    await _context.SaveChangesAsync();
                }
            }

            return Redirect(shortUrl.RedirectTo);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
