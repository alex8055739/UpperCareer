using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Question Controller的沟通。
    /// </summary>
    public class Question2DA : DABase
    {
        public async Task<QuestionModel> GetQuestionModel(string questionId)
        {
            return await LCDal.GetQuestionAndAnswersWithComments(questionId).ContinueWith(t => new QuestionModel(t.Result));
        }

        public async Task<List<QuestionInfoModel>> GetQuestionInfoModels()
        {
            return await LCDal.FindPostQuestions().ContinueWith(t =>
                {
                    List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

                    foreach (QuestionInfo q in t.Result)
                    {
                        qiList.Add(new QuestionInfoModel(q));
                    }

                    return qiList;
                });
        }

        public async Task<List<AnswerModel>> GetAnswerModels(string questionId)
        {
            return await LCDal.FindAnswersByQuestion(questionId).ContinueWith(t =>
                {
                    List<AnswerModel> ansList = new List<AnswerModel>();

                    foreach (Answer a in t.Result)
                    {
                        ansList.Add(new AnswerModel(a));
                    }

                    return ansList;
                });
        }

        public async Task<List<CommentModel>> GetCommentModels(string answerId)
        {
            return await LCDal.FindCommentsByAnswer(answerId).ContinueWith(t =>
                {
                    List<CommentModel> cmtList = new List<CommentModel>();

                    foreach (Comment c in t.Result)
                    {
                        cmtList.Add(new CommentModel(c));
                    }

                    return cmtList;
                });
        }

        public async Task<bool> PostNewQuestion(QuestionPostModel p)
        {
            return await LCDal.SaveNewQuestion(p.CreatePostForSave());
        }

        public async Task<bool> PostNewAnswer(AnswerPostModel a)
        {
            return await LCDal.SaveNewAnswer(a.CreatePostForSave());
        }

        public async Task<bool> PostNewComment(CommentPostModel c)
        {
            return await LCDal.SaveNewComment(c.CreatePostForSave());
        }
    }
}