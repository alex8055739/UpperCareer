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

        public UserDetail(AVObject udo)
        {
            GenerateUserDetailObject(udo);
        }

        public User ForUser { get; set; }

        public string Title { get; set; }

        public string SelfDescription { get; set; }

        public int FieldIndex { get; set; }

        private void GenerateUserDetailObject(AVObject udo)
        {
            if (udo.ClassName != "UserDetail")
            {
                throw new InvalidOperationException("获取的对象不是用户信息类object。");
            }

            ForUser = udo.ContainsKey("forUser") && udo.Get<AVUser>("forUser") != null ? new User(udo.Get<AVUser>("forUser")) : null;
            Title = udo.Get<string>("title");
            SelfDescription = udo.Get<string>("selfDescription");
            FieldIndex = udo.Get<int>("fieldIndex");
        }

        public AVObject CreateUserDetailObjectForSave()
        {
            AVObject userDetail = new AVObject("UserDetail");


            userDetail.Add("selfDescription", SelfDescription);
            userDetail.Add("fieldIndex", FieldIndex);

            return userDetail;
        }
    }
}
