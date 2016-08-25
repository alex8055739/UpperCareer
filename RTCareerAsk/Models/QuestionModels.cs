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
            ConvertQuestionObjectsToQuestionInfoModels(qio);
        }

        public string Title { get; set; }

        public int AnswerCount { get; set; }

        private void ConvertQuestionObjectsToQuestionInfoModels(QuestionInfo qio)
        {
            ConvertQACObjectToModel(qio);
            Title = qio.Title;
            AnswerCount = qio.AnswerCount;
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

        public bool IsEditAllowed { get; set; }

        public bool IsAnswerAllowed { get; set; }

        public List<AnswerModel> Answers { get; set; }

        private void ConvertQuestionObjectToModel(Question po)
        {
            ConvertQACObjectToModel(po);
            Title = po.Title;

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

        public QuestionModel ForQuestion { get; set; }

        public List<CommentModel> Comments { get; set; }

        public bool IsEditAllowed { get; set; }

        private void ConvertAnswerObjectToModel(Answer ao)
        {
            ConvertQACObjectToModel(ao);
            ForQuestion = ao.ForQuestion != null ? new QuestionModel(ao.ForQuestion) : null;

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

        private void ConvertCommentObjectToModel(Comment co)
        {
            ConvertQACObjectToModel(co);
        }
    }
}