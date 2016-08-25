using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    abstract public class UpperQACBaseModel
    {
        public string ID { get; set; }

        public UserModel Creator { get; set; }

        public DateTime DateCreate { get; set; }

        public string Content { get; set; }

        public string TimeDisplay { get; set; }

        protected void ConvertQACObjectToModel(UpperQACBaseDomain obj)
        {
            ID = obj.ObjectID;
            Creator = obj.CreatedBy != null ? new UserModel(obj.CreatedBy) : null;
            DateCreate = obj.DateCreate;
            Content = obj.Content;
            //GenerateTimeDisplay();
        }

        protected void GenerateTimeDisplay()
        {
            if (DateCreate == default(DateTime))
            {
                throw new InvalidOperationException("没有找到创建时间");
            }

            TimeSpan diff = DateTime.Now.Subtract(DateCreate);
            TimeDisplay = diff.Days > 365 ? string.Format("{0}年前", diff.Days / 365) : diff.Days > 30 ? string.Format("{0}月前", diff.Days / 30) : diff.Days > 0 ? string.Format("{0}天前", diff.Days) : diff.Hours > 0 ? string.Format("{0}小时前", diff.Hours) : string.Format("{0}分钟前", diff.Minutes);
        }
    }
}