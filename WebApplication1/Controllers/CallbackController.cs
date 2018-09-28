﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Schedulers;

namespace WebApplication1.Controllers
{
    public class CallbackController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly VkCallbackWorkerService _vkCallbackWorkerService;

        public CallbackController(System.Collections.Generic.IEnumerable<Microsoft.Extensions.Hosting.IHostedService> hostedServices, DatabaseContext context)
        {
            _context = context;
            _vkCallbackWorkerService = hostedServices.OfType<VkCallbackWorkerService>().First();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Vk([FromBody]Models.Vk.CallbackMessage message)
        {
            if (await _context.GroupAdmins.AllAsync(x => x.IdGroup != message.IdGroup))
                return Content("Ok");

            if (message.Type == "confirmation")
            {
                string callbackConfirmationCode = await _context.Groups
                            .Where(x => x.IdVk == message.IdGroup)
                            .Select(x => x.CallbackConfirmationCode)
                            .FirstOrDefaultAsync();

                return Ok(callbackConfirmationCode);
            }

            _vkCallbackWorkerService.AddCallbackMessage(message);

            return Content("ok");
        }
    }
}