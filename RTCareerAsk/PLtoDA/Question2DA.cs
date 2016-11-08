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
        public async Task<List<QuestionInfoModel>> LoadQuestionListByPage(int pageIndex, int id = 1)
        {
            bool isHottestFirst = true;

            switch (id)
            {
                case 1:
                    break;
                case 2:
                    isHottestFirst = false;
                    break;
                default:
                    throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
            }

            List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

            foreach (QuestionInfo q in await LCDal.LoadQuestionList(pageIndex, isHottestFirst))
            {
                qiList.Add(new QuestionInfoModel(q));
            }

            return qiList;
        }

        public async Task<List<AnswerInfoModel>> LoadAnswerListByPage(int pageIndex, int id = 3)
        {
            bool isHottestFirst = true;

            switch (id)
            {
                case 3:
                    break;
                case 4:
                    isHottestFirst = false;
                    break;
                default:
                    throw new IndexOutOfRangeException(string.Format("请求代码出错：{0}", id));
            }

            List<AnswerInfoModel> aiList = new List<AnswerInfoModel>();

            foreach (AnswerInfo a in await LCDal.LoadAnswerList(pageIndex, isHottestFirst))
            {
                aiList.Add(new AnswerInfoModel(a));
            }

            return aiList;

        }

        public async Task<QuestionModel> GetQuestionModel(string userId, string questionId)
        {
            return await LCDal.GetQuestionAndAnswersWithComments(userId, questionId).ContinueWith(t => new QuestionModel(t.Result));
        }

        public async Task<List<QuestionInfoModel>> LoadNewQuestions()
        {
            return await LCDal.LoadQuestionList(0, false).ContinueWith(t =>
                {
                    List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

                    foreach (QuestionInfo q in t.Result)
                    {
                        qiList.Add(new QuestionInfoModel(q));
                    }

                    return qiList;
                });
        }

        public async Task<AnswerModel> GetAnswerModel(string answerId)
        {
            return await LCDal.GetAnswerWithComments(answerId).ContinueWith(t => new AnswerModel(t.Result));
        }

        public async Task<List<AnswerModel>> GetAnswerModels(string userId, string questionId, int pageIndex, bool isHottestFirst)
        {
            return await LCDal.LoadAnswersByQuestion(userId, questionId, pageIndex, isHottestFirst).ContinueWith(t =>
                {
                    List<AnswerModel> ansList = new List<AnswerModel>();

                    if (t.Result != null && t.Result.Count() > 0)
                    {
                        foreach (Answer a in t.Result)
                        {
                            ansList.Add(new AnswerModel(a));
                        }
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

        public async Task UpdateContent(bool isQuestion, string id, string content)
        {
            await LCDal.UpdateContent(isQuestion, id, content);
        }

        public async Task DeleteAnswerWithComments(string ansId)
        {
            await LCDal.DeleteAnswerWithComments(ansId);
        }

        public async Task DeleteComment(string cmtId)
        {
            await LCDal.DeleteComment(cmtId);
        }

        public async Task SaveOrUpdateVote(VoteModel v)
        {
            await LCDal.PerformVote(v.CreateVote());
        }
    }
}