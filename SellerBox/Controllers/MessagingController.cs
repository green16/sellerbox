using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SellerBox.Common;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels.Messaging;
using SellerBox.ViewModels.Shared;

namespace SellerBox.Controllers
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
        public async Task<IActionResult> Index()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var model = await _context.Scheduler_Messaging
                .Where(x => x.Message.IdGroup == groupInfo.Key)
                .Where(x => x.Status != Models.Database.Common.MessagingStatus.Finished)
                .Select(x => new MessagingViewModel()
                {
                    DtAdd = x.DtAdd,
                    DtStart = x.DtStart,
                    HasImages = x.Message.Files.Any(),
                    Message = x.Message.Text,
                    Name = x.Name,
                    HasKeyboard = !string.IsNullOrEmpty(x.Message.Keyboard),
                    IdMessaging = x.Id,
                    RecipientsCount = x.RecipientsCount,
                    Status = x.Status
                }).ToArrayAsync();

            ViewBag.IdGroup = groupInfo.Key;
            return View(nameof(Index), model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var model = new EditMessagingViewModel()
            {
                IdGroup = groupInfo.Key,
            };

            ViewBag.IdGroup = groupInfo.Key;
            return View(nameof(Edit), model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? idMessaging)
        {
            if (!idMessaging.HasValue)
                return RedirectToAction(nameof(Index));
            
            var messaging = await _context.Scheduler_Messaging
                .FirstOrDefaultAsync(x => x.Id == idMessaging);
            if (messaging == null)
                return RedirectToAction(nameof(Index));

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var model = new EditMessagingViewModel()
            {
                IdMessage = messaging.IdMessage,
                DtStart = messaging.DtStart.ToString("dd.MM.yyyy HH:mm"),
                Name = messaging.Name,
                IdGroup = groupInfo.Key
            };

            if (model != null && model.IdMessage.HasValue)
            {
                Messages message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == model.IdMessage);
                uint idx = 0;

                model.Message = message.Text;
                model.IsImageFirst = message.IsImageFirst;
                model.Files = message.Files.Select(x => new FileModel()
                {
                    Id = x.IdFile,
                    Name = _context.Files.Where(y => y.Id == x.IdFile).Select(y => y.Name).FirstOrDefault(),
                    Index = idx++
                }).ToList();

                var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);
                if (keyboard != null)
                {
                    model.Keyboard = new List<List<MessageButton>>();
                    byte currentRowIdx = 0;
                    foreach (var currentRow in keyboard.Buttons)
                    {
                        byte colIdx = 0;
                        model.Keyboard.Add(currentRow.Select(x => new MessageButton()
                        {
                            ButtonColor = x.Color.ToString(),
                            Column = colIdx++,
                            CanDelete = colIdx == currentRow.Count(),
                            Row = currentRowIdx,
                            Text = x.Action.Label
                        }).ToList());
                        currentRowIdx++;
                    }
                }

            }

            ViewBag.IdGroup = groupInfo.Key;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditMessagingViewModel data)
        {
            ViewBag.IdGroup = data.IdGroup;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessaging([FromQuery]Guid? idMessaging)
        {
            if (!idMessaging.HasValue)
                return NotFound(idMessaging);

            _context.Scheduler_Messaging.RemoveRange(_context.Scheduler_Messaging.Where(x => x.Id == idMessaging));
            await _context.SaveChangesAsync();

            return Ok();
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
        public async Task<IActionResult> SendMessages(EditMessagingViewModel data)
        {
            if (!ModelState.IsValid)
                return Edit(data);

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var message = await DbHelper.AddMessage(_context, groupInfo.Key, data.Message, data.GetVkKeyboard(), data.IsImageFirst, data.Files.Select(x => x.Id));

            long[] userIds = null;

            if (data.IsSelfSend)
                userIds = new long[] { await _userHelperService.GetUserIdVk(User) };
            //return Json(new { idMessage = message.Id, ids = new long[] { await _userHelperService.GetUserIdVk(User) } });
            else
                userIds = await _context.Subscribers
                    .Where(x => x.IdGroup == groupInfo.Key)
                    .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                    .Select(x => x.IdVkUser)
                    .ToArrayAsync();

            await _context.Scheduler_Messaging.AddAsync(new Scheduler_Messaging()
            {
                DtAdd = DateTime.UtcNow,
                IdMessage = message.Id,
                Name = data.Name,
                RecipientsCount = userIds.LongLength,
                Status = Models.Database.Common.MessagingStatus.Added,
                DtStart = DateTime.SpecifyKind(DateTime.ParseExact(data.DtStart, "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture), DateTimeKind.Local).ToUniversalTime(),
                VkUserIds = string.Join(',', userIds)
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}