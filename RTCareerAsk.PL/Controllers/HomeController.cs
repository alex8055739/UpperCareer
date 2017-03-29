using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.PL.App_DLL;
using RTCareerAsk.PL.Models;
using RTCareerAsk.PL.Filters;

namespace RTCareerAsk.PL.Controllers
{
    public class HomeController : UpperBaseController
    {
        #region Action

        [UpperResult]
        public async Task<ActionResult> Index()
        {
            try
            {
                await AutoLogin();

                return HasUserInfo ? RedirectToAction("Feeds") : RedirectToAction("Index", "Article");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> SearchResults(string keyword)
        {
            try
            {
                ViewBag.Title = GenerateTitle(string.Format("“{0}”的搜索结果", keyword));

                keyword = keyword == MasterSearchKey ? "" : keyword;

                SearchResultModel result = await HomeDa.SearchStupid(HasUserInfo ? GetUserID() : null, keyword);
                result.Keyword = string.IsNullOrEmpty(keyword) ? MasterSearchKey : keyword;

                return View(result);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> Feeds()
        {
            await AutoLogin();

            if (!HasUserInfo)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Home/Feeds" });
            }

            Task<IEnumerable<FeedModel>> tModel = HomeDa.LoadFeedsForUser(GetUserID(), 0);
            Task<List<UserRecommandationModel>> tRecommandedUsers = HomeDa.LoadRecommandedUsers(GetUserID());
            Task<SideContentModel> tSideQuestions = LoadSideQuestions();
            Task<SideContentModel> tSideAnswers = LoadSideAnswers();

            await Task.WhenAll(UpdateNewMessageCount(), tModel, tSideQuestions, tSideAnswers, tRecommandedUsers);

            ViewBag.Title = GenerateTitle("动态");
            ViewBag.RecommandedUsers = tRecommandedUsers.Result.Count > 0 ? tRecommandedUsers.Result : null;
            ViewBag.SideQuestions = tSideQuestions.Result;
            ViewBag.SideAnswers = tSideAnswers.Result;

            return View(tModel.Result);
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> ExtendSearchResult(string targetId, SearchModelType contentType, int pageIndex)
        {
            try
            {
                targetId = targetId == MasterSearchKey ? "" : targetId;

                SearchResultModel result = await HomeDa.ExtendSearchResult(HasUserInfo ? GetUserID() : null, targetId, contentType, pageIndex);
                result.Keyword = string.IsNullOrEmpty(targetId) ? MasterSearchKey : targetId;

                switch (contentType)
                {
                    case SearchModelType.Question:
                        return PartialView("_QuestionList", result.QuestionResults);
                    case SearchModelType.User:
                        return PartialView("_UserTagList", result.UserResults);
                    default:
                        throw new IndexOutOfRangeException("错误：不能识别的搜索类型。");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadFeedsByPage(int pageIndex)
        {
            try
            {
                if (!HasUserInfo)
                {
                    throw new TimeoutException("您的登陆已失效，请重新登陆。");
                }

                IEnumerable<FeedModel> model = await HomeDa.LoadFeedsForUser(GetUserID(), pageIndex);

                return PartialView("_FeedsPartial", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> LoadFeedAnswerComments(string ansId)
        {
            try
            {
                AnswerModel model = await HomeDa.LoadCommentsForFeedAnswer(HasUserInfo ? GetUserID() : string.Empty, ansId);

                return PartialView("_FeedCommentList", SetFlagsForActions(model));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> SaveFeedComment(CommentPostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new ArgumentException("评论不能超过500个字符");
                }

                model.UserID = GetUserID();
                model.UserName = GetUserName();

                CommentModel result = await HomeDa.SaveCommentForFeedAnswer(model);

                return PartialView("_FeedCommentDetail", SetFlagsForActions(result));
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

        [HttpPost]
        public async Task<JsonResult> UploadDroppedAndPastedImage(HttpPostedFileBase upload)
        {
            try
            {
                var url = await HomeDa.UploadImageFile(CreateFileModelForUpload(upload));
                return Json(new { uploaded = 1, fileName = "图片", url = url });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                return Json(new { uploaded = 0, error = new { message = e.Message } });
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult CreateImgDisplayModal(string src)
        {
            try
            {
                ViewBag.Src = src;

                return PartialView("_ImgDisplayModal");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult CreateDeleteComfirmationModal(DeleteModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title))
                {
                    switch (model.Type)
                    {
                        case 1:
                            model.Title = "确认删除这条答案？";
                            break;
                        case 2:
                            model.Title = "确认删除这条评论？";
                            break;
                        case 3:
                            model.Title = "确认删除这条私信？";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(string.Format("请求类型超出范围。收到的请求：{0}", model.Type));
                    }
                }

                return PartialView("_ConfirmDel", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
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

        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}