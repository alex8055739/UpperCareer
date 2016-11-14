using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RTCareerAsk.Models;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class ArticleController : UpperBaseController
    {
        [UpperResult]
        public ActionResult Index()
        {
            return View();
        }

        [UpperResult]
        public async Task<ActionResult> Detail(string id = default(string))
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = "582970508ac247005974b089";
                }

                ArticleModel model = await ArticleDa.LoadArticleDetail(id);

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> Compose(string id = null)
        {
            if (!IsUserAuthorized("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            ArticlePostModel model = await ArticleDa.CreatePostModelWithReference(id);
            ViewBag.Title = "撰写新文章";

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Compose(ArticlePostModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("您输入的内容不符合格式");
            }
            else if (!HasUserInfo)
            {
                throw new InvalidOperationException("请您先登录进行操作");
            }

            if (model.HasReference)
            {
                model.Content = ModifyTextareaData(model.Content, true);
            }

            model.EditorID = GetUserID();

            await ArticleDa.PostNewArticle(model);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> UploadCoverPic(HttpPostedFileBase cover)
        {
            try
            {
                string url = await UploadImageFile(cover, string.Format("ArticleCover{0}", GetUserID()));

                return Json(new
                {
                    url = url
                });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }
    }
}
