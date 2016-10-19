using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Roles = new List<RoleModel>();
        }

        public UserModel(User u)
        {
            Roles = new List<RoleModel>();

            ConvertUserObjectToModel(u);
        }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public int Gender { get; set; }

        public string Portrait { get; set; }

        public string Company { get; set; }

        public int FieldIndex { get; set; }

        public List<RoleModel> Roles { get; set; }

        private void ConvertUserObjectToModel(User u)
        {
            if (u != null)
            {
                UserID = u.ObjectID;
                Name = u.Name;
                Title = u.Title;
                Gender = u.Gender;
                Portrait = u.Portrait;
                Company = u.Company;
                FieldIndex = u.FieldIndex;

                if (u.Roles.Count > 0)
                {
                    foreach (Role r in u.Roles)
                    {
                        Roles.Add(new RoleModel(r));
                    }
                }
            }
        }
    }

    public class RoleModel
    {
        public RoleModel() { }

        public RoleModel(Role r)
        {
            ConvertRoleObjectToModel(r);
        }

        public string RoleID { get; set; }

        public string RoleName { get; set; }

        private void ConvertRoleObjectToModel(Role r)
        {
            RoleID = r.ObjectID;
            RoleName = r.RoleName;
        }
    }
}