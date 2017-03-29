using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.PL.App_DLL;
using RTCareerAsk.PL.Models;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PL.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Home Controller的沟通。
    /// </summary>
    public class Home2DA : DABase
    {
        public async Task<List<FileInfoModel>> GetFileInfoModels()
        {
            return await LCDal.FindAllFiles().ContinueWith(t => ConvertFileInfoObjectsToModels(t.Result));
        }

        public async Task<string> LoadAlertInfo(string key)
        {
            return await LCDal.LoadAlertInfo(key);
        }

        public async Task<FileModel> DownloadImageFiles(string fileId)
        {
            return await LCDal.DownloadFileByID(fileId).ContinueWith(t => new FileModel(t.Result));
        }

        public async Task<SearchResultModel> SearchStupid(string userId, string keyword)
        {
            SearchResult result = await LCDal.SearchByKeywordStupid(keyword);

            result.UserResults = await UpdateUserSearchResults(userId, result.UserResults);

            return new SearchResultModel(result);
        }

        public async Task<SearchResultModel> ExtendSearchResult(string userId, string keyword, SearchModelType type, int pageIndex)
        {
            SearchResult result = await LCDal.ExtendedSearchByKeywordStupid(keyword, (SearchType)type, pageIndex);

            result.UserResults = await UpdateUserSearchResults(userId, result.UserResults);

            return new SearchResultModel(result);
        }

        public async Task<List<UserTag>> UpdateUserSearchResults(string userId, IEnumerable<UserTag> userSearchResults)
        {
            if (userSearchResults.Count() > 0)
            {
                List<Task<UserTag>> tasks = userSearchResults.Select(x => LCDal.BuildUserTag(userId, x)).ToList();

                await Task.WhenAll(tasks.ToArray());

                return tasks.Select(x => x.Result).OrderByDescending(x => x.AnswerCount).ThenByDescending(x => x.FollowerCount).ToList();
            }

            return new List<UserTag>();
        }

        public async Task<IEnumerable<FeedModel>> LoadFeedsForUser(string userId, int pageIndex)
        {
            IEnumerable<History> feeds = await LCDal.LoadNewFeeds(userId, pageIndex);

            List<Task<FeedModel>> tasks = feeds.Select(x => FetchFeedContent(x)).ToList();

            await Task.WhenAll(tasks);

            return tasks.Where(x => x.Result != null).Select(x => x.Result);
        }

        public async Task<FeedModel> FetchFeedContent(History hsty)
        {
            FeedModel result = new FeedModel(hsty);

            switch (hsty.Type)
            {
                case 1:
                case 8:
                case 80:
                    result.Content = await LCDal.LoadQuestionForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new QuestionInfoModel(t.Result));
                    break;
                case 2:
                case 5:
                case 50:
                    result.Content = await LCDal.LoadAnswerForFeed(hsty.ReadInfoStringByIndex(0)).ContinueWith(t => new AnswerInfoModel(t.Result));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("输入数据非动态类型，输入类型：" + hsty.Type.ToString());
            }

            return result;
        }

        public async Task<AnswerModel> LoadCommentsForFeedAnswer(string userId, string answerId, int pageIndex = 0)
        {
            return await LCDal.GetAnswerWithComments(userId, answerId).ContinueWith(t => new AnswerModel(t.Result));
        }

        public async Task<CommentModel> SaveCommentForFeedAnswer(CommentPostModel model)
        {
            return await LCDal.SaveNewFeedComment(model.CreatePostForSave()).ContinueWith(t => new CommentModel(t.Result));
        }

        public async Task<List<UserRecommandationModel>> LoadRecommandedUsers(string userId)
        {
            List<UserRecommandationModel> results = new List<UserRecommandationModel>();
            IEnumerable<UserRecommand> userRecommanded = await LCDal.LoadRecommandedUsers(userId, 5);

            List<Task> tUpdateResult = userRecommanded.Select(x => LCDal.BuildUserTag(userId, x.ForUser).ContinueWith(t =>
            {
                x.ForUser = t.Result;
                results.Add(new UserRecommandationModel(x));
            })).ToList();

            await Task.WhenAll(tUpdateResult.ToArray());

            return results;
        }
    }
}