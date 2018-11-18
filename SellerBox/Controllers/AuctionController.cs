using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SellerBox.ViewModels.Auction;

namespace SellerBox.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;
        private readonly IConfiguration _configuration;

        public AuctionController(IConfiguration configuration, DatabaseContext context, UserHelperService userHelperService)
        {
            _configuration = configuration;
            _context = context;
            _userHelperService = userHelperService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var auctionList = _context.Auctions.ToList();
            List<AuctionViewModel> auctionViewModels = new List<AuctionViewModel>();
            foreach (var auction in auctionList)
            {
                AuctionViewModel auctionVM = new AuctionViewModel
                {
                    Name = auction.Name,
                    Description = auction.Description,
                    StartPrice = auction.StartPrice,
                    PriceStep = auction.PriceStep,
                    MaxPrice = auction.MaxPrice,
                    StartDate = auction.StartDate,
                    EndDate = auction.EndDate,
                    IdPost = auction.IdPost,
                    MaxCommentsCount = auction.MaxCommentsCount,
                    Id = auction.Id
                };
                auctionViewModels.Add(auctionVM);
            }
            return View(auctionViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AppId = _configuration.GetValue<ulong>("VkApplicationId");
            ViewBag.OwnerId = _userHelperService.GetSelectedGroup(User).Key * -1;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(AuctionViewModel auctionView)
        {
            Auction auction = new Auction
            {
                CurrentPrice = auctionView.StartPrice,
                Description = auctionView.Description,
                EndDate = auctionView.EndDate,
                IdPost = auctionView.IdPost,
                MaxCommentsCount = auctionView.MaxCommentsCount,
                MaxPrice = auctionView.MaxPrice,
                Name = auctionView.Name,
                PriceStep = auctionView.PriceStep,
                StartDate = auctionView.StartDate,
                StartPrice = auctionView.StartPrice,
                IsActive = auctionView.StartDate >= DateTime.Now && DateTime.Now < auctionView.EndDate,
                IdCreator = await _context.GroupAdmins.FirstOrDefaultAsync(a => a.IdUser == _userHelperService.GetUserId(User))
            };
            
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

            AuctionViewModel auctionViewModel = new AuctionViewModel
            {
                Description = auction.Description,
                EndDate = auction.EndDate,
                IdPost = auction.IdPost,
                MaxCommentsCount = auction.MaxCommentsCount,
                MaxPrice = auction.MaxPrice,
                Name = auction.Name,
                PriceStep = auction.PriceStep,
                StartDate = auction.StartDate,
                StartPrice = auction.StartPrice,
                Id = auction.Id
            };
            
            ViewBag.AppId = _configuration.GetValue<ulong>("VkApplicationId");
            ViewBag.OwnerId = _userHelperService.GetSelectedGroup(User).Key * -1;
            
            return View(auctionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind] AuctionViewModel auctionViewModel)
        {
            if (id != auctionViewModel.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                Auction auction = new Auction
                {
                    Id =  auctionViewModel.Id,
                    Name = auctionViewModel.Name,
                    Description = auctionViewModel.Description,
                    StartPrice = auctionViewModel.StartPrice,
                    CurrentPrice = auctionViewModel.StartPrice,
                    PriceStep = auctionViewModel.PriceStep,
                    MaxPrice = auctionViewModel.MaxPrice,
                    MaxCommentsCount = auctionViewModel.MaxCommentsCount,
                    StartDate = auctionViewModel.StartDate,
                    EndDate = auctionViewModel.EndDate,
                    IsActive = auctionViewModel.StartDate >= DateTime.Now && DateTime.Now < auctionViewModel.EndDate,
                    IdPost = auctionViewModel.IdPost,
                    IdCreator = await _context.GroupAdmins.FirstOrDefaultAsync(a =>
                        a.IdUser == _userHelperService.GetUserId(User))
                };
                _context.Auctions.Update(auction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(auctionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
                return NotFound();

            AuctionViewModel auctionViewModel = new AuctionViewModel
            {
                Description = auction.Description,
                EndDate = auction.EndDate,
                IdPost = auction.IdPost,
                MaxCommentsCount = auction.MaxCommentsCount,
                MaxPrice = auction.MaxPrice,
                Name = auction.Name,
                PriceStep = auction.PriceStep,
                StartDate = auction.StartDate,
                StartPrice = auction.StartPrice,
                Id = auction.Id
            };
            
            ViewBag.AppId = _configuration.GetValue<ulong>("VkApplicationId");
            ViewBag.OwnerId = _userHelperService.GetSelectedGroup(User).Key * -1;
            
            return View(auctionViewModel);
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