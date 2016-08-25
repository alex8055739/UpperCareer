using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.BL;
using RTCareerAsk.Models;

namespace RTCareerAsk.Controllers
{
    public class MessageController : UpperBaseController
    {
        public string SessionCopyName { get { return "MessageCopy"; } }

        public async Task<ActionResult> Index(string Id = "")
        {
            try
            {
                if (IsUserAuthorized("User,Admin"))
                {
                    ViewBag.Title = "消息列表";
                    ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                    ViewBag.IsAdmin = IsUserAuthorized("Admin");

                    if (!string.IsNullOrEmpty(Id))
                    {
                        if (await UpperMessageService.MarkMessageAsOpened(GetUserID(), Id))
                        {
                            ViewBag.Message = Id;
                        }
                    }

                    return View(await MessageDa.LoadMessagesByUserID(GetUserID()));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> LoadMessageContent(string Id)
        {
            try
            {
                if (await UpperMessageService.MarkMessageAsOpened(GetUserID(), Id))
                {
                    MessageModel model = await MessageDa.GetMessageByID(Id);

                    if (model.From != null)
                    {
                        ViewBag.IsFromSelf = GetUserID() == model.From.UserID;
                        ViewBag.IsFromSystem = false;
                    }
                    else
                    {
                        ViewBag.IsFromSystem = true;
                    }

                    return PartialView("_MessageContent", await MessageDa.GetMessageByID(Id));
                }

                throw new InvalidOperationException("标记消息为已读失败");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> UpdateMsgCount()
        {
            await UpdateUserInfo(new Dictionary<string, object>() { { "NewMessageCount", null } });

            ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
            ViewBag.IsAdmin = IsUserAuthorized("Admin");

            return PartialView("_NavBar");
        }

        [HttpPost]
        public PartialViewResult CreateLetterForm(string targetId)
        {
            try
            {
                LetterModel model;

                if (HasSessionCopy(SessionCopyName))
                {
                    model = RestoreCopy<LetterModel>(SessionCopyName);
                    model.To = targetId;
                }
                else
                {
                    model = new LetterModel() { To = targetId };
                }

                return PartialView("_LetterModal", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task WritePersonalLetter(LetterModel l)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    l.From = GetUserID();
                    CopyToSave(SessionCopyName, l);

                    await MessageDa.WriteNewMessage(l);
                    ClearCopy(SessionCopyName);
                }
                else
                {
                    throw new InvalidOperationException("邮件格式不符合要求");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task DeleteMessage(string targetId)
        {
            try
            {
                await UpperMessageService.DeleteMessage(GetUserID(), targetId);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }
    }
}
