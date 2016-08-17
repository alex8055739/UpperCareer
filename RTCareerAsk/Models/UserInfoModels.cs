using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    /// <summary>
    /// 用于保存用户登陆后返回的信息，并保存于Session中。
    /// </summary>
    public class UserInfoModel
    {
        public UserInfoModel()
        {
            RoleNames = new List<string>();
        }

        public UserInfoModel(User u)
        {
            RoleNames = new List<string>();
            ConvertUserObjectToUserInfoModel(u);
        }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Portrait { get; set; }

        public bool EmailVerified { get; set; }

        public int NewMessageCount { get; set; }

        public List<string> RoleNames { get; set; }

        private void ConvertUserObjectToUserInfoModel(User u)
        {
            UserID = u.ObjectID;
            Name = u.Name;
            Portrait = u.Portrait;
            EmailVerified = u.EmailVerified;

            if (u.Roles.Count > 0)
            {
                foreach (Role r in u.Roles)
                {
                    RoleNames.Add(r.RoleName);
                }
            }
        }

        public void SetNewMessageCount(int count)
        {
            NewMessageCount = count;
        }
    }

    /// <summary>
    /// 用于获取用户信息页面的具体信息。
    /// </summary>
    public class UserDetailModel
    {
        public UserDetailModel()
        {
            RecentQuestions = new List<QuestionInfoModel>();
            RecentAnswers = new List<AnswerModel>();
        }

        public UserDetailModel(User u)
        {
            RecentQuestions = new List<QuestionInfoModel>();
            RecentAnswers = new List<AnswerModel>();

            ConvertUserObjectToModel(u);
        }

        public UserDetailModel(UserDetail ud)
        {
            ConvertUserDetailObjectToModel(ud);
        }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Portrait { get; set; }

        public string Title { get; set; }

        public string SelfDescription { get; set; }

        public int FieldIndex { get; set; }

        public DateTime DateCreate { get; set; }

        public bool HasFollowed { get; set; }

        public int FollowerCount { get; set; }

        public int FolloweeCount { get; set; }

        public List<QuestionInfoModel> RecentQuestions { get; set; }

        public List<AnswerModel> RecentAnswers { get; set; }

        //Will be removed after complete.
        private void ConvertUserObjectToModel(User u)
        {
            if (u != null)
            {
                UserID = u.ObjectID;
                Name = u.Name;
                DateCreate = u.DateCreate;
            }
        }

        private void ConvertUserDetailObjectToModel(UserDetail ud)
        {
            if (ud != null)
            {
                UserID = ud.ForUser.ObjectID;
                Name = ud.ForUser.Name;
                Portrait = ud.ForUser.Portrait;
                Title = ud.Title;
                SelfDescription = ud.SelfDescription;
                FieldIndex = ud.FieldIndex;
            }
        }

        public UserDetailModel SetDetailInfomation(int followerCnt, int followeeCnt, bool hasFollowed, IEnumerable<QuestionInfoModel> questions, IEnumerable<AnswerModel> answers)
        {
            FollowerCount = followerCnt;
            FolloweeCount = followeeCnt;
            HasFollowed = hasFollowed;
            RecentQuestions.AddRange(questions);
            RecentAnswers.AddRange(answers);

            return this;
        }

        public UserDetail CreateUserDetailObjectForSave()
        {
            throw new NotImplementedException();
        }
    }
}