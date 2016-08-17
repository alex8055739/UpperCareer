using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionInfoModel
    {
        public QuestionInfoModel()
        {

        }

        public QuestionInfoModel(QuestionInfo qio)
        {
            ConvertQuestionObjectsToQuestionInfoModels(qio);
        }

        public string QuestionID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreate { get; set; }

        public UserModel Creator { get; set; }

        public int AnswerCount { get; set; }

        private void ConvertQuestionObjectsToQuestionInfoModels(QuestionInfo qio)
        {
            QuestionID = qio.ObjectID;
            Title = qio.Title;
            Content = qio.Content;
            DateCreate = qio.DateCreate;
            AnswerCount = qio.AnswerCount;
            Creator = qio != null ? new UserModel(qio.CreatedBy) : null;
        }
    }
}