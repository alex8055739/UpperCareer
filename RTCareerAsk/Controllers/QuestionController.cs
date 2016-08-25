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
    public class QuestionController : UpperBaseController
    {
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                ViewBag.IsAuthorized = IsUserAuthorized("User,Admin");
                ViewBag.IsAdmin = IsUserAuthorized("Admin");

                return View(SetFlagsForActions(await QuestionDa.GetQuestionModel(id)));
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
                    return PartialView("_AnswersDetail", SetFlagsForActions(await QuestionDa.GetAnswerModels(a.QuestionID)));
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

        [HttpPost]
        public async Task UpdateContent(bool isQuestion, string id, string content)
        {
            await QuestionDa.UpdateContent(isQuestion, id, content);
        }

        [HttpPost]
        public async Task DeleteAnswer(string ansId)
        {
            await QuestionDa.DeleteAnswerWithComments(ansId);
        }

        private QuestionModel SetFlagsForActions(QuestionModel model)
        {
            if (HasUserInfo)
            {
                bool createdByUser = model.Creator.UserID == GetUserID();
                IEnumerable<AnswerModel> answerByUser = model.Answers.Where(x => x.Creator.UserID == GetUserID());

                model.IsEditAllowed = createdByUser;
                model.IsAnswerAllowed = !createdByUser && answerByUser.Count() == 0;

                if (answerByUser.Count() > 0)
                {
                    foreach (AnswerModel ans in answerByUser)
                    {
                        model.Answers.Where(x => x.ID == ans.ID).First().IsEditAllowed = true;
                    }
                }
            }

            return model;
        }

        private List<AnswerModel> SetFlagsForActions(List<AnswerModel> models)
        {
            foreach (AnswerModel ans in models)
            {
                if (ans.Creator.UserID == GetUserID())
                {
                    ans.IsEditAllowed = true;
                }
            }

            return models;
        }
    }
}
