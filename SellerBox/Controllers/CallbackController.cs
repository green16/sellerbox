using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    public class CallbackController : Controller
    {
        private readonly DatabaseContext _context;
        //private readonly VkCallbackWorkerService _vkCallbackWorkerService;

        public CallbackController(System.Collections.Generic.IEnumerable<Microsoft.Extensions.Hosting.IHostedService> hostedServices, DatabaseContext context)
        {
            _context = context;
         //   _vkCallbackWorkerService = hostedServices.OfType<VkCallbackWorkerService>().First();
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

            var callbackMessage = new Models.Database.VkCallbackMessages()
            {
                Dt = System.DateTime.UtcNow,
                Type = message.Type,
                IdGroup = message.IdGroup,
                Object = message.ToJSON()
            };
            await _context.VkCallbackMessages.AddAsync(callbackMessage);
            await _context.SaveChangesAsync();
            message.IdVkCallbackMessage = callbackMessage.Id;

            VkCallbackWorkerService.AddCallbackMessage(message);

            return Content("ok");
        }
    }
}