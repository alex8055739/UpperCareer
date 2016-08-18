using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class User
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public User(AVUser uo)
        {
            Roles = new List<Role>();

            GenerateUserObject(uo);
        }

        public string ObjectID { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Portrait { get; set; }

        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        public bool MobileVerified { get; set; }

        public DateTime DateCreate { get; set; }

        public List<Role> Roles { get; set; }

        private void GenerateUserObject(AVUser uo)
        {
            if (uo != null)
            {
                ObjectID = uo.ObjectId;
                Name = uo.ContainsKey("nickname") ? uo.Get<string>("nickname") : null;
                Portrait = uo.ContainsKey("portrait") ? uo.Get<string>("portrait") : null;
                Email = uo.Email;
                EmailVerified = uo.ContainsKey("emailVerified") ? uo.Get<bool>("emailVerified") : false;
                MobileVerified = uo.MobilePhoneVerified;
                DateCreate = Convert.ToDateTime(uo.CreatedAt);
            }
        }

        public User SetRoles(IEnumerable<AVRole> ros)
        {
            if (ros.Count() > 0)
            {
                foreach (AVRole ro in ros)
                {
                    Roles.Add(new Role(ro));
                }
            }

            return this;
        }

        public AVUser LoadUserObject()
        {
            if (string.IsNullOrEmpty(ObjectID))
            {
                throw new NullReferenceException("没有可用的用户ID来获取信息");
            }

            return AVUser.CreateWithoutData("_User", ObjectID) as AVUser;
        }

        public AVUser CreateUserObjectForRegister()
        {
            AVUser user = new AVUser()
            {
                Username = Email,
                Password = Password,
                Email = Email,
            };

            user.Add("nickname", Name);

            return user;
        }
    }

    public class Role
    {
        public Role() { }

        public Role(AVRole ro)
        {
            GenerateRoleObject(ro);
        }

        public string ObjectID { get; set; }

        public string RoleName { get; set; }

        private void GenerateRoleObject(AVRole ro)
        {
            ObjectID = ro.ObjectId;
            RoleName = ro.Get<string>("name");
        }
    }

    public class UserDetail
    {
        public UserDetail() { }

        public UserDetail(AVUser u)
        {
            CreateNewUserDetailObject(u);
        }

        public UserDetail(AVObject udo)
        {
            GenerateUserDetailObject(udo);
        }

        public string ObjectId { get; set; }

        public User ForUser { get; set; }

        public string Title { get; set; }

        public int Gender { get; set; }

        public string Company { get; set; }

        public string SelfDescription { get; set; }

        public int FieldIndex { get; set; }

        private void CreateNewUserDetailObject(AVUser uo)
        {
            ForUser = new User(uo);
        }

        private void GenerateUserDetailObject(AVObject udo)
        {
            if (udo.ClassName != "UserDetail")
            {
                throw new InvalidOperationException("获取的对象不是用户信息类object。");
            }
            else if (!udo.ContainsKey("forUser") || udo.Get<AVUser>("forUser") == null)
            {
                throw new NullReferenceException("未能获取用户基本信息。");
            }

            ObjectId = udo.ObjectId;
            ForUser = new User(udo.Get<AVUser>("forUser"));
            Title = udo.Get<string>("title");
            Gender = udo.Get<int>("gender");
            Company = udo.Get<string>("company");
            SelfDescription = udo.Get<string>("selfDescription");
            FieldIndex = udo.Get<int>("fieldIndex");
        }

        public AVObject CreateUserDetailObjectForSave()
        {
            AVObject userDetail = new AVObject("UserDetail");

            userDetail.Add("forUser", ForUser.LoadUserObject());
            userDetail.Add("title", Title);
            userDetail.Add("gender", Gender);
            userDetail.Add("company", Company);
            userDetail.Add("selfDescription", SelfDescription);
            userDetail.Add("fieldIndex", FieldIndex);

            return userDetail;
        }

        public AVObject UpdateUserDetailObject(AVObject udo)
        {
            if (udo.ClassName != "UserDetail")
            {
                throw new InvalidOperationException("获取的对象不是用户信息类object。");
            }
            //else if (!udo.ContainsKey("forUser") || udo.Get<AVUser>("forUser") == null)
            //{
            //    throw new NullReferenceException("未能获取用户基本信息。");
            //}

            //AVUser uo = udo.Get<AVUser>("forUser");
            //uo["nickname"] = ForUser.Name;
            //udo["forUser"] = uo;
            udo["title"] = Title;
            udo["gender"] = Gender;
            udo["company"] = Company;
            udo["selfDescription"] = SelfDescription;
            udo["fieldIndex"] = FieldIndex;

            return udo;
        }

    }
}
