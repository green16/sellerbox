using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Models;
using System.Linq;
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
        public async Task<IActionResult> ShortLinkWithoutSubscriber(string key)
        {
            var bi_idShortUrl = Common.Helpers.UrlShortenerHelper.Decode(key);
            var idShortUrl = Common.Helpers.UrlShortenerHelper.Convert(bi_idShortUrl);

            var shortUrl = await _context.ShortUrls.FindAsync(idShortUrl);
            if (shortUrl == null)
                return NotFound();

            await _context.History_ShortUrlClicks.AddAsync(new Models.Database.History_ShortUrlClicks()
            {
                Dt = System.DateTime.UtcNow,
                IdShortUrl = idShortUrl
            });
            await _context.SaveChangesAsync();

            return Redirect(shortUrl.RedirectTo);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ShortLinkWithSubscriber(string key, string id)
        {
            var bi_idShortUrl = Common.Helpers.UrlShortenerHelper.Decode(key);
            var bi_idSubscriber = Common.Helpers.UrlShortenerHelper.Decode(id);
            var idShortUrl = Common.Helpers.UrlShortenerHelper.Convert(bi_idShortUrl);
            var idSubscriber = Common.Helpers.UrlShortenerHelper.Convert(bi_idSubscriber);

            var shortUrl = await _context.ShortUrls.FindAsync(idShortUrl);
            if (shortUrl == null)
                return NotFound();

            if (!shortUrl.IsSingleClick || await _context.History_ShortUrlClicks.Where(x => x.IdShortUrl == idShortUrl).AllAsync(x => x.IdSubscriber != idSubscriber))
            {
                await _context.History_ShortUrlClicks.AddAsync(new Models.Database.History_ShortUrlClicks()
                {
                    Dt = System.DateTime.UtcNow,
                    IdShortUrl = idShortUrl,
                    IdSubscriber = idSubscriber
                });
                await _context.SaveChangesAsync();
            }

            if (shortUrl.IdChain.HasValue)
            {
                var isSubscriberInChain = await _context.SubscribersInChains
                    .Where(x => x.IdSubscriber == idSubscriber)
                    .Include(x => x.ChainStep)
                    .AnyAsync(x => x.ChainStep.IdChain == shortUrl.IdChain.Value);
                if (!isSubscriberInChain)
                    await Common.Helpers.SubscriberHelper.AddSubscriberToChain(_context, shortUrl.IdGroup, idSubscriber, shortUrl.IdChain.Value);
            }

            return Redirect(shortUrl.RedirectTo);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
