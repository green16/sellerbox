using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Shared;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MessagingController : Controller
    {
        public const long MaxRequestBodySize = 400 * 1024 * 1024;

        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;
        private readonly VkPoolService _vkPoolService;

        public MessagingController(DatabaseContext context, UserHelperService userHelperService, VkPoolService vkPoolService)
        {
            _context = context;
            _userHelperService = userHelperService;
            _vkPoolService = vkPoolService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            MessagingViewModel model = new MessagingViewModel()
            {
                IdGroup = _userHelperService.GetSelectedGroup(User).Key,
            };

            ViewBag.IdGroup = model.IdGroup;
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(MessagingViewModel data)
        {
            ViewBag.IdGroup = data.IdGroup;
            return View(nameof(Index), data);
        }

        [HttpPost, RequestSizeLimit(MaxRequestBodySize)]
        public async Task<IActionResult> UploadFileAjax([FromQuery]uint idx, string prefix)
        {
            IFormFile formFile = Request.Form.Files.FirstOrDefault();
            if (formFile == null)
                throw new FileNotFoundException();

            string fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            string groupAccessToken = await _context.Groups.Where(x => x.IdVk == selectedGroup.Key).Select(x => x.AccessToken).FirstOrDefaultAsync();

            Files file = new Files()
            {
                DtAdd = DateTime.UtcNow,
                Name = fileName,
                Size = formFile.Length,
                Content = new byte[formFile.Length],
            };

            using (MemoryStream ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                ms.Flush();
                file.Content = ms.ToArray();
            }

            var vkApi = await _vkPoolService.GetGroupVkApi(selectedGroup.Key);

            var uploadServerInfo = await vkApi.Photo.GetMessagesUploadServerAsync(selectedGroup.Key);

            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            System.IO.File.WriteAllBytes(tempFilePath, file.Content);
            string responseJson;
            using (var wc = new System.Net.WebClient())
                responseJson = System.Text.Encoding.ASCII.GetString(wc.UploadFile(uploadServerInfo.UploadUrl, tempFilePath));

            var photo = (await vkApi.Photo.SaveMessagesPhotoAsync(responseJson)).FirstOrDefault();

            file.VkUrl = Newtonsoft.Json.JsonConvert.SerializeObject(photo);

            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();

            FileModel fileModel = new FileModel()
            {
                Id = file.Id,
                Name = file.Name,
                Index = idx,
                PropertiesPrefix = prefix
            };
            return PartialView("FileRow", fileModel);
        }

        [HttpPost]
        public IActionResult AddKeyboardButton(string prefix, byte row, byte column)
        {
            var model = new MessageButton
            {
                Row = row,
                Column = column,
                ButtonColor = "positive",
                PropertiesPrefix = prefix,
                CanDelete = true
            };
            return PartialView("KeyboardButton", model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete([FromBody]Guid? idFile)
        {
            if (!idFile.HasValue)
                return Json(new { State = -1 });

            _context.Files.RemoveRange(_context.Files.Where(x => x.Id == idFile));
            _context.FilesInMessage.RemoveRange(_context.FilesInMessage.Where(x => x.Id == idFile));
            await _context.SaveChangesAsync();

            return Json(new { State = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessages(MessagingViewModel data)
        {
            if (!ModelState.IsValid)
                return Index(data);

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var message = await DbHelper.AddMessage(_context, groupInfo.Key, data.Message, data.GetVkKeyboard(), data.IsImageFirst, data.Files.Select(x => x.Id));
            long[] userIds = null;
            if (data.IsSelfSend)
                userIds = new long[] { await _userHelperService.GetUserIdVk(User) };
            else
                userIds = await _context.Subscribers
                    .Where(x => x.IdGroup == data.IdGroup)
                    .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                    .Select(x => x.IdVkUser)
                    .ToArrayAsync();

            return Json(new { idMessage = message.Id, ids = userIds });
        }

        public IActionResult MessagesSent()
        {
            return Ok("Сообщение отправлено");
        }
    }
}