using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;

namespace RTCareerAsk.Controllers
{
    public class QuestionController : BaseController
    {
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                ViewBag.IsAdmin = IsUserAuthorized("Admin");

                return View(await QuestionDa.GetQuestionModel(id));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public PartialViewResult CreateQuestionForm()
        {
            try
            {
                return PartialView("_PostModal", new QuestionPostModel());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<PartialViewResult> PostQuestion(QuestionPostModel p)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                p.UserID = GetUserID();

                if (await QuestionDa.PostNewQuestion(p))
                {
                    return PartialView("_QuestionList", await QuestionDa.GetQuestionInfoModels());
                }

                throw new InvalidOperationException("保存问题失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<PartialViewResult> PostAnswer(AnswerPostModel a)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                a.UserID = GetUserID();

                if (await QuestionDa.PostNewAnswer(a))
                {
                    ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                    ViewBag.IsAdmin = IsUserAuthorized("Admin");

                    //Placeholder: Send a notify message to original poster, a.NotifyUserID.
                    return PartialView("_AnswersDetail", await QuestionDa.GetAnswerModels(a.QuestionID));
                }

                throw new InvalidOperationException("保存答案失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> PostComment(CommentPostModel c)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                c.UserID = GetUserID();

                if (await QuestionDa.PostNewComment(c))
                {
                    ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                    ViewBag.IsAdmin = IsUserAuthorized("Admin");

                    //Placeholder: Send a notify message to original poster, a.NotifyUserID.
                    List<CommentModel> model = await QuestionDa.GetCommentModels(c.AnswerID);
                    return PartialView("_CommentDetail", model);
                    //return PartialView("_CommentDetail", await QuestionDa.GetCommentModels(c.AnswerID));
                }

                throw new InvalidOperationException("保存评论失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }
    }
}
