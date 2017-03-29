using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RTCareerAsk.PL.Models;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PL.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与User Controller的沟通。
    /// </summary>
    public class User2DA : DABase
    {
        public async Task<UserDetailModel> LoadUserDetail(string targetId, string userId = "")
        {
            Task<UserDetailModel> tUserDetail = LCDal.LoadUserDetail(targetId).ContinueWith(t => new UserDetailModel(t.Result));

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

            Task<bool?> tHasFollowed = LCDal.IfAlreadyFollowed(userId, targetId);

            Task<List<QuestionInfoModel>> tRecentQuestions = GetRecentQuestions(targetId, 0);

            await Task.WhenAll(tUserDetail, tRecentQuestions, tHasFollowed);

            tUserDetail.Result.HasFollowed = tHasFollowed.Result;
            tUserDetail.Result.RecentQuestions = tRecentQuestions.Result;

            return tUserDetail.Result;
        }

        public async Task<List<UserTagModel>> LoadFollowersOrFollowees(string userId, string targetId, bool isForFollowers, int pageIndex)
        {
            IEnumerable<UserTag> targets = isForFollowers ? await LCDal.GetFollowers(userId, targetId, pageIndex) : await LCDal.GetFollowees(userId, targetId, pageIndex);

            return targets.Select(x => new UserTagModel(x)).ToList();
        }

        public async Task<UserTagModel> LoadUserTag(string userId, string targetId)
        {
            Task<UserTagModel> tag = LCDal.LoadUserInfo(targetId).ContinueWith(t =>
            {
                return new UserTagModel(t.Result);
            });

            Task<bool?> hasFollowed = LCDal.IfAlreadyFollowed(userId, targetId);

            Task<int> followerCnt = LCDal.GetFollowerCount(targetId);

            Task<int> answerCnt = LCDal.GetAnswerCount(targetId);

            await Task.WhenAll(tag, hasFollowed, followerCnt, answerCnt);

            return tag.Result.SetFollowerAndAnswerCount(hasFollowed.Result, followerCnt.Result, answerCnt.Result);
        }

        public async Task Follow(string userId, string followeeId)
        {
            await LCDal.Follow(userId, followeeId);
        }

        public async Task Unfollow(string userId, string followeeId)
        {
            await LCDal.Unfollow(userId, followeeId);
        }

        public async Task<List<QuestionInfoModel>> GetRecentQuestions(string targetId, int pageIndex)
        {
            return await LCDal.LoadQuestionListByUser(targetId, pageIndex).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new QuestionInfoModel(x)).ToList() : new List<QuestionInfoModel>();
            });
        }

        public async Task<List<AnswerInfoModel>> GetRecentAnswers(string targetId, int pageIndex)
        {
            return await LCDal.LoadAnswerListByUser(targetId, pageIndex).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return t.Result != null && t.Result.Count() > 0 ? t.Result.Select(x => new AnswerInfoModel(x)).ToList() : new List<AnswerInfoModel>();
            });
        }

        public async Task<bool> CheckIfUserRecommanded(string userId)
        {
            return await LCDal.CheckIfUserRecommanded(userId);
        }

        public async Task<bool> SaveOrUpdateRecommandation(bool isUpdate, string userId, string intro)
        {
            return await LCDal.SaveOrUpdateUserRecommandation(isUpdate, userId, intro);
        }
    }
}