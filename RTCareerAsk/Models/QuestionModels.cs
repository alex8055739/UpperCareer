using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionInfoModel : UpperQACBaseModel
    {
        public QuestionInfoModel() { }

        public QuestionInfoModel(QuestionInfo qio)
        {
            ConvertQuestionInfoObjectToQuestionInfoModel(qio);
        }

        public string Title { get; set; }

        public string VoteDiff { get; set; }

        public string AnswerCount { get; set; }

        private void ConvertQuestionInfoObjectToQuestionInfoModel(QuestionInfo qio)
        {
            ConvertQACObjectToModel(qio);
            Title = qio.Title;
            VoteDiff = ProcessLargeNumDisplay(qio.VoteDiff);
            AnswerCount = ProcessLargeNumDisplay(qio.AnswerCount);
        }
    }

    public class AnswerInfoModel : UpperQACBaseModel
    {
        public AnswerInfoModel() { }

        public AnswerInfoModel(AnswerInfo aio)
        {
            ConvertAnswerInfoObjectToAnswerInfoModel(aio);
        }

        public QuestionModel ForQuestion { get; set; }

        public string VoteDiff { get; set; }

        public string CommentCount { get; set; }

        private void ConvertAnswerInfoObjectToAnswerInfoModel(AnswerInfo aio)
        {
            ConvertQACObjectToModel(aio);
            ForQuestion = aio.ForQuestion != null ? new QuestionModel(aio.ForQuestion) : null;
            VoteDiff = ProcessLargeNumDisplay(aio.VoteDiff);
            CommentCount = ProcessLargeNumDisplay(aio.CommentCount);
        }
    }

    public class QuestionModel : UpperQACBaseModel
    {
        public QuestionModel()
        {
            Answers = new List<AnswerModel>();
        }

        public QuestionModel(Question po)
        {
            Answers = new List<AnswerModel>();

            ConvertQuestionObjectToModel(po);
        }

        public string Title { get; set; }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public bool IsEditAllowed { get; set; }

        public bool IsAnswerAllowed { get; set; }

        public List<AnswerModel> Answers { get; set; }

        private void ConvertQuestionObjectToModel(Question po)
        {
            ConvertQACObjectToModel(po);
            Title = po.Title;
            VoteDiff = po.VoteDiff;
            IsLike = po.IsLike;

            if (po.Answers != null)
            {
                foreach (Answer a in po.Answers)
                {
                    Answers.Add(new AnswerModel(a));
                }
            }
        }
    }

    public class AnswerModel : UpperQACBaseModel
    {
        public AnswerModel()
        {
            Comments = new List<CommentModel>();
        }

        public AnswerModel(Answer ao)
        {
            Comments = new List<CommentModel>();

            ConvertAnswerObjectToModel(ao);
        }

        public int VoteDiff { get; set; }

        public bool? IsLike { get; set; }

        public QuestionModel ForQuestion { get; set; }

        public List<CommentModel> Comments { get; set; }

        public bool IsEditAllowed { get; set; }

        private void ConvertAnswerObjectToModel(Answer ao)
        {
            ConvertQACObjectToModel(ao);
            ForQuestion = ao.ForQuestion != null ? new QuestionModel(ao.ForQuestion) : null;
            VoteDiff = ao.VoteDiff;
            IsLike = ao.IsLike;

            foreach (Comment c in ao.Comments)
            {
                Comments.Add(new CommentModel(c));
            }
        }
    }

    public class CommentModel : UpperQACBaseModel
    {
        public CommentModel() { }

        public CommentModel(Comment co)
        {
            ConvertCommentObjectToModel(co);
        }

        public bool IsReplyAllowed { get; set; }

        private void ConvertCommentObjectToModel(Comment co)
        {
            ConvertQACObjectToModel(co);
        }
    }
}