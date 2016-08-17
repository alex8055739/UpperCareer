using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Models;

namespace RTCareerAsk.Controllers
{
    /// <summary>
    /// 基础控制器，用于提供通用方法。
    /// </summary>
    public class BaseController : Controller
    {
        #region Private Field

        private string _roles;
        private string[] _rolesSplit = new string[0];

        #endregion

        #region Property

        protected Home2DA HomeDa { get { return new Home2DA(); } }
        protected Account2DA AccountDa { get { return new Account2DA(); } }
        protected Question2DA QuestionDa { get { return new Question2DA(); } }
        protected Message2DA MessageDa { get { return new Message2DA(); } }
        protected User2DA UserDa { get { return new User2DA(); } }
        protected Test2DA TestDa { get { return new Test2DA(); } }

        public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }

        public bool HasUserInfo
        {
            get { return Session["UserInfo"] != null; }
        }

        public UserInfoModel UserInfo
        {
            get { return Session["UserInfo"] != null ? Session["UserInfo"] as UserInfoModel : null; }
        }

        #endregion

        #region Upper Helper

        protected async Task AccountLogin(string userName, string password)
        {
            await AccountDa.LoginWithEmail(userName, password).ContinueWith(t => StoreUserToSession(t.Result));
        }

        protected string GetUserID()
        {
            if (!HasUserInfo)
            {
                throw new InvalidOperationException("错误：未能找到用户信息");
            }

            return UserInfo.UserID;
        }

        protected void StoreUserToSession(UserInfoModel um)
        {
            Session["UserInfo"] = um;
        }

        protected async Task UpdateUserInfo(string portraitUrl = "")
        {
            //Load updated new messages count.
            UserInfo.NewMessageCount = await HomeDa.LoadMessageCount(GetUserID());
            //Load updated new portrait link.
            if (!string.IsNullOrEmpty(portraitUrl))
            {
                UserInfo.Portrait = portraitUrl;
            }

            StoreUserToSession(UserInfo);
        }

        protected void ClearUserFromSession()
        {
            Session["UserInfo"] = null;
        }

        protected string TranslateExceptionMessage(string msg)
        {
            switch (msg)
            {
                case "Could not find user":
                    return "找不到对应的用户";
                case "The username and password mismatch.":
                    return "用户名与密码不匹配";
                case "Email address isn't verified.":
                    return "邮箱地址未验证，请验证后登陆";
                case "Username has already been taken":
                    return "用户名已经被占用";
                default:
                    return msg;
            }
        }

        protected List<FileModel> GetUploadedFiles(HttpFileCollectionBase files)
        {
            List<FileModel> fms = new List<FileModel>();

            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload] != null && Request.Files[upload].ContentLength > 0)
                {
                    string fileName = Path.GetFileName(Request.Files[upload].FileName);
                    Stream fileStream = Request.Files[upload].InputStream;
                    IDictionary<string, object> metaData = new Dictionary<string, object>()
                    {
                        {"类型",Request.Files[upload].ContentType},
                        {"用途","Test functions"}
                    };

                    fms.Add(new FileModel()
                    {
                        FileName = fileName,
                        FileDataStream = fileStream,
                        MetaData = metaData
                    });
                }
            }

            return fms;
        }

        protected FileModel CreateFileModelForUpload(HttpPostedFileBase upload, string fileName = "")
        {
            if (upload != null && upload.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.GetFileName(upload.FileName);
                }

                return new FileModel()
                {
                    FileName = fileName,
                    FileDataStream = upload.InputStream,
                    MetaData = new Dictionary<string, object>()
                    {
                        {"类型",upload.ContentType}
                    }
                };
            }

            throw new NullReferenceException("未能成功获取上传内容");
        }

        protected void CopyToSave(string sessionName, object contentCopy)
        {
            Session[sessionName] = contentCopy;
        }

        protected void ClearCopy(string sessionName)
        {
            Session[sessionName] = null;
        }

        protected bool HasSessionCopy(string sessionName)
        {
            return Session[sessionName] != null ? true : false;
        }

        protected T RestoreCopy<T>(string sessionName) where T : class
        {
            return Session[sessionName] as T;
        }

        #endregion

        #region Authorization

        /// <summary>
        /// 验证用户身份是否符合指定的角色
        /// </summary>
        /// <param name="roles">允许通过验证的身份，多个身份请用","分开</param>
        /// <returns>代表身份验证是否通过的Boolean值</returns>
        protected bool IsUserAuthorized(string roles)
        {
            if (HasUserInfo)
            {
                Roles = roles;

                if (UserInfo.RoleNames.Contains("Block"))
                {
                    return false;
                }

                if (_rolesSplit.Length > 0 && _rolesSplit.Intersect(UserInfo.RoleNames).Count() == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }

        #endregion
    }
}
