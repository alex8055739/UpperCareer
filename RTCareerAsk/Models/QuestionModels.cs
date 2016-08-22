using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionModel
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

        public string QuestionID { get; set; }

        public UserModel Creator { get; set; }

        public DateTime DateCreate { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsEditAllowed { get; set; }

        public bool IsAnswerAllowed { get; set; }

        public List<AnswerModel> Answers { get; set; }

        private void ConvertQuestionObjectToModel(Question po)
        {
            QuestionID = po.ObjectID;
            Creator = po.CreatedBy != null ? new UserModel(po.CreatedBy) : null;
            DateCreate = po.DateCreate;
            Title = po.Title;
            Content = po.Content;

            if (po.Answers != null)
            {
                foreach (Answer a in po.Answers)
                {
                    Answers.Add(new AnswerModel(a));
                }
            }
        }
    }

    public class AnswerModel
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

        public string AnswerID { get; set; }

        public string Content { get; set; }

        public UserModel Creator { get; set; }

        public QuestionModel ForQuestion { get; set; }

        public DateTime DateCreate { get; set; }

        public List<CommentModel> Comments { get; set; }

        public bool IsEditAllowed { get; set; }

        private void ConvertAnswerObjectToModel(Answer ao)
        {
            AnswerID = ao.ObjectID;
            Content = ao.Content;
            Creator = ao.CreatedBy != null ? new UserModel(ao.CreatedBy) : null;
            ForQuestion = ao.ForQuestion != null ? new QuestionModel(ao.ForQuestion) : null;
            DateCreate = ao.DateCreate;

            foreach (Comment c in ao.Comments)
            {
                Comments.Add(new CommentModel(c));
            }
        }
        //Place a group of pictures
    }

    public class CommentModel
    {
        public CommentModel() { }

        public CommentModel(Comment co)
        {
            ConvertCommentObjectToModel(co);
        }

        public string CommentID { get; set; }

        public string Content { get; set; }

        public UserModel Creator { get; set; }

        public DateTime DateCreate { get; set; }

        private void ConvertCommentObjectToModel(Comment co)
        {
            CommentID = co.ObjectID;
            Content = co.Content;
            Creator = co.CreatedBy != null ? new UserModel(co.CreatedBy) : null;
            DateCreate = co.DateCreate;
        }
    }
}