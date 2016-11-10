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
        public ActionResult Index()
        {
            return View();
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

            model.EditorID = GetUserID();
            await ArticleDa.PostNewArticle(model);

            throw new NotImplementedException();

            return RedirectToAction("Index", "Home");
        }
    }
}
