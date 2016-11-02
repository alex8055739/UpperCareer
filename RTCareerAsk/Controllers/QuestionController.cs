using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class QuestionController : UpperBaseController
    {
        [UpperResult]
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                return View(SetFlagsForActions(await QuestionDa.GetQuestionModel(HasUserInfo ? GetUserID() : string.Empty, id)));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
        public async Task<ActionResult> AnswerDetail(string id)
        {
            try
            {
                return View(SetFlagsForActions(await QuestionDa.GetAnswerModel(id)));
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
        public PartialViewResult CreateCommentForm(string ansId, string ntfyId, string prefixText = "")
        {
            try
            {
                return PartialView("_CommentForm", new CommentPostModel() { AnswerID = ansId, NotifyUserID = ntfyId, PostContent = prefixText });
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
                System.Threading.Thread.Sleep(2000);

                if (!ModelState.IsValid)
                {
                    throw new InvalidOperationException("用户输入的信息不符合要求");
                }

                p.UserID = GetUserID();

                if (await QuestionDa.PostNewQuestion(p))
                {
                    return PartialView("_QuestionList", await QuestionDa.LoadNewQuestions());
                }

                throw new InvalidOperationException("保存问题失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
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
                    return PartialView("_AnswersDetail", SetFlagsForActions(await QuestionDa.GetAnswerModels(GetUserID(), a.QuestionID)));
                }

                throw new InvalidOperationException("保存答案失败，请再次尝试");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [UpperResult]
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
                c.PostContent = ModifyTextareaData(c.PostContent, true);

                if (await QuestionDa.PostNewComment(c))
                {
                    List<CommentModel> model = await QuestionDa.GetCommentModels(c.AnswerID);

                    return PartialView("_CommentDetail", SetFlagsForActions(model));
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

        [HttpPost]
        public async Task DeleteComment(string cmtId)
        {
            await QuestionDa.DeleteComment(cmtId);
        }

        [HttpPost]
        public async Task SaveVote(VoteModel model)
        {
            try
            {
                model.VoterID = GetUserID();

                await QuestionDa.SaveOrUpdateVote(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
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

                foreach (AnswerModel ans in model.Answers)
                {
                    if (ans.Comments.Count > 0)
                    {
                        ans.Comments = SetFlagsForActions(ans.Comments);
                    }
                }
            }

            return model;
        }

        private AnswerModel SetFlagsForActions(AnswerModel model)
        {
            if (!HasUserInfo)
            {
                return model;
            }

            if (model.Creator.UserID == GetUserID())
            {
                model.IsEditAllowed = true;
            }

            if (model.Comments.Count > 0)
            {
                model.Comments = SetFlagsForActions(model.Comments);
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

                if (ans.Comments.Count > 0)
                {
                    ans.Comments = SetFlagsForActions(ans.Comments);
                }
            }

            return models;
        }

        private List<CommentModel> SetFlagsForActions(List<CommentModel> models)
        {
            foreach (CommentModel cmt in models)
            {
                if (cmt.Creator.UserID != GetUserID())
                {
                    cmt.IsReplyAllowed = true;
                }
            }

            return models;
        }
    }
}
