using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class HomeController : BaseController
    {
        #region Action

        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "欢迎来到Upper";
                ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                ViewBag.IsAdmin = IsUserAuthorized("Admin");

                return View("QuestionList", await HomeDa.GetQuestionInfoModels());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public ActionResult RegisterSuccess(string returnUrl, string email)
        {
            ViewBag.IsAdmin = IsUserAuthorized("Admin");
            ViewBag.Title = "注册成功";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Email = email;

            return View();
        }

        //[HttpPost]
        //public async Task<PartialViewResult> UpdateMsgCount()
        //{
        //    await UpdateUserInfo();

        //    ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
        //    ViewBag.IsAdmin = IsUserAuthorized("Admin");

        //    return PartialView("_NavBar");
        //}

        [HttpPost]
        public async Task<PartialViewResult> ChangePortrait(HttpPostedFileBase portrait)
        {
            try
            {
                string url = await HomeDa.UploadImageFile(CreateFileModelForUpload(portrait, string.Format("portrait{0}", GetUserID())));

                if (await HomeDa.ChangeUserPortrait(GetUserID(), url))
                {
                    await UpdateUserInfo(new Dictionary<string, object>() { { "Portrait", url } });
                }
                else
                {
                    await HomeDa.DeleteFileWithUrl(url);
                }

                return PartialView("_NavBar");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<ContentResult> UploadImage(HttpPostedFileBase upload)
        {
            var url = await HomeDa.UploadImageFile(CreateFileModelForUpload(upload));
            var CKEditorFuncNum = System.Web.HttpContext.Current.Request["CKEditorFuncNum"];

            //上传成功后，我们还需要通过以下的一个脚本把图片返回到第一个tab选项
            return Content("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\");</script>");
        }

        public async Task<ActionResult> FileListPage()
        {
            try
            {
                IEnumerable<FileInfoModel> model = await HomeDa.GetFileInfoModels();

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> DisplayTestResult(string fileId)
        {
            try
            {
                ViewBag.Title = "测试结果显示页面";

                FileModel model = await HomeDa.DownloadImageFiles(fileId);

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FileContentResult> DownloadFileForPage(string fileId)
        {
            try
            {
                FileModel fm = await HomeDa.DownloadImageFiles(fileId);

                return File(fm.FileDataByte, fm.MetaData["类型"].ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Trunk

        //public ActionResult View1()
        //{
        //    return View(new PostModel());
        //}

        //[HttpPost]
        //public void View1(PostModel p)
        //{

        //}

        #endregion
    }
}