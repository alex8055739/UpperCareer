using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    abstract public class UpperQACBaseDomain
    {
        public string ObjectID { get; set; }

        public string Content { get; set; }

        public DateTime DateCreate { get; set; }

        public User CreatedBy { get; set; }

        protected void GenerateQACObject(AVObject obj)
        {
            ObjectID = obj.ObjectId;
            Content = obj.ContainsKey("content") ? obj.Get<string>("content") : null;
            DateCreate = Convert.ToDateTime(obj.CreatedAt);
            CreatedBy = obj.ContainsKey("createdBy") ? new User(obj.Get<AVUser>("createdBy")) : null;
        }
    }
}
