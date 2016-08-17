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
    public class User2DA : DABase
    {
        public async Task<UserDetailModel> LoadUserDetail(string targetId, string userId = "")
        {
            UserDetailModel udm = await LCDal.LoadUserInfo(targetId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return new UserDetailModel(t.Result);
                });

            #region Code for Test
            //int followerCnt = await LCDal.GetFollowerCount(targetId);
            //int followeeCnt = await LCDal.GetFolloweeCount(targetId);
            //List<QuestionInfoModel> questions = await LCDal.GetRecentQuestions(targetId).ContinueWith(t =>
            //{
            //    if (t.IsFaulted || t.IsCanceled)
            //    {
            //        throw t.Exception;
            //    }

            //    List<QuestionInfoModel> qms = new List<QuestionInfoModel>();

            //    foreach (QuestionInfo q in t.Result)
            //    {
            //        qms.Add(new QuestionInfoModel(q));
            //    }

            //    return qms;
            //});
            //List<AnswerModel> answers = await LCDal.GetRecentAnswers(targetId).ContinueWith(t =>
            //{
            //    if (t.IsFaulted || t.IsCanceled)
            //    {
            //        throw t.Exception;
            //    }

            //    List<AnswerModel> ams = new List<AnswerModel>();

            //    foreach (Answer a in t.Result)
            //    {
            //        ams.Add(new AnswerModel(a));
            //    }

            //    return ams;
            //});

            //return udm.SetDetailInfomation(followerCnt, followeeCnt, hasFollowed, questions, answers);
            #endregion

            bool hasFollowed = string.IsNullOrEmpty(userId) ? false : await LCDal.IfAlreadyFollowed(userId, targetId);

            Task<int> followerCnt = LCDal.GetFollowerCount(targetId);
            Task<int> followeeCnt = LCDal.GetFolloweeCount(targetId);
            Task<List<QuestionInfoModel>> questions = LCDal.GetRecentQuestions(targetId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<QuestionInfoModel> qms = new List<QuestionInfoModel>();

                    foreach (QuestionInfo q in t.Result)
                    {
                        qms.Add(new QuestionInfoModel(q));
                    }

                    return qms;
                });
            Task<List<AnswerModel>> answers = LCDal.GetRecentAnswers(targetId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<AnswerModel> ams = new List<AnswerModel>();

                    foreach (Answer a in t.Result)
                    {
                        ams.Add(new AnswerModel(a));
                    }

                    return ams;
                });

            await Task.WhenAll(followerCnt, followeeCnt, questions, answers);

            return udm.SetDetailInfomation(followerCnt.Result, followeeCnt.Result, hasFollowed, questions.Result, answers.Result);
        }

        public async Task Follow(string userId, string followeeId)
        {
            await LCDal.Follow(userId, followeeId);
        }

        public async Task Unfollow(string userId, string followeeId)
        {
            await LCDal.Unfollow(userId, followeeId);
        }
    }
}