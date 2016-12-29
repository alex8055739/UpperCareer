using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Models
{
    public class HistoryModel
    {
        public HistoryModel() { }

        public HistoryModel(History ntfn)
        {
            ConvertHistoryObjectToModel(ntfn);
        }

        public string ID { get; set; }

        public UserModel User { get; set; }

        public UserModel Target { get; set; }

        public NotificationType Type { get; set; }

        public bool IsNew { get; set; }

        public string[] NameStrings { get; set; }

        public string[] InfoStrings { get; set; }

        public string DateCreate { get; set; }

        public string DateUpdate { get; set; }

        private void ConvertHistoryObjectToModel(History ntfn)
        {
            ID = ntfn.ObjectID;
            User = new UserModel(ntfn.FromUser);
            Target = new UserModel(ntfn.ForUser);
            Type = (NotificationType)ntfn.Type;
            IsNew = ntfn.IsNew;
            NameStrings = ntfn.CompoundNameString.Split(';');
            InfoStrings = ntfn.CompoundInfoString.Split(';');
            DateCreate = ntfn.DateCreate.ToString("yyyy/MM/dd");
            DateUpdate = ntfn.DateUpdate.ToString("yyyy/MM/dd");
        }

        public History CreateHistoryForSave()
        {
            if (Target.UserID == User.UserID)
            {
                return null;
            }
            else if (NameStrings.Length != InfoStrings.Length)
            {
                throw new InvalidOperationException("名称变量数与信息变量数不匹配。");
            }

            string compoundedName = "";
            string compoundedInfo = "";

            for (int i = 0; i < NameStrings.Length; i++)
            {
                compoundedName += NameStrings[i] + ";";
                compoundedInfo += InfoStrings[i] + ";";
            }

            return new History()
            {
                FromUser = new User() { ObjectID = User.UserID },
                ForUser = new User() { ObjectID = Target.UserID },
                Type = (int)Type,
                CompoundNameString = compoundedName,
                CompoundInfoString = compoundedInfo
            };
        }
    }
}