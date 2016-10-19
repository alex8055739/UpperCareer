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

        public string Content { get; set; }

        public string CreateBefore { get; set; }

        public string UpdateBefore { get; set; }

        protected void ConvertQACObjectToModel(UpperQACBaseDomain obj)
        {
            ID = obj.ObjectID;
            Creator = obj.CreatedBy != null ? new UserModel(obj.CreatedBy) : null;
            Content = obj.Content;
            CreateBefore = GenerateTimeDisplay(obj.DateCreate);
            UpdateBefore = GenerateTimeDisplay(obj.DateUpdate);
        }

        protected string GenerateTimeDisplay(DateTime date)
        {
            if (date != default(DateTime))
            {
                TimeSpan diff = DateTime.Now.Subtract(date);
                return diff.Days > 365 ? string.Format("{0}年前", diff.Days / 365) : diff.Days > 30 ? string.Format("{0}个月前", diff.Days / 30) : diff.Days > 0 ? string.Format("{0}天前", diff.Days) : diff.Hours > 0 ? string.Format("{0}小时前", diff.Hours) : string.Format("{0}分钟前", diff.Minutes);
            }

            return "未知时间";
        }

        protected string ProcessLargeNumDisplay(int num)
        {
            return num > 1000 ? string.Format("{0}K", num / 1000) : num.ToString();
        }
    }
}