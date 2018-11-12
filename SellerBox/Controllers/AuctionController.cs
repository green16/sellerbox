using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace SellerBox.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;
        private readonly IConfiguration _configuration;

        public AuctionController(IConfiguration configuration, DatabaseContext context, UserHelperService userHelperService, VkPoolService vkPoolService)
        {
            _configuration = configuration;
            _context = context;
            _userHelperService = userHelperService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Auctions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AppId = _configuration.GetValue<ulong>("VkApplicationId");
            ViewBag.OwnerId = _userHelperService.GetSelectedGroup(User).Key * -1;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
                return RedirectToAction(nameof(Index));

            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind] Auction auction)
        {
            if (id != auction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Auctions.Update(auction);
                return RedirectToAction(nameof(Index));
            }

            return View(auction);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
                return NotFound();

            return View(auction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction != null)
            {

                _context.Auctions.Remove(auction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();

        }

    }
}