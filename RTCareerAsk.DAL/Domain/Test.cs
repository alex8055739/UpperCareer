using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class Bug
    {
        public Bug() { }

        public Bug(AVObject bo)
        {
            GenerateBugObject(bo);
        }

        public string ObjectID { get; set; }

        public int BugIndex { get; set; }

        public User Reporter { get; set; }

        public int Priority { get; set; }

        public int StatusCode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Attachment { get; set; }

        public File AttachmentFile { get; set; }

        private void GenerateBugObject(AVObject bo)
        {
            if (bo.ClassName != "Bug")
            {
                throw new InvalidOperationException("获取的对象不是错误报告类object。");
            }

            ObjectID = bo.ObjectId;
            BugIndex = bo.Get<int>("bugIndex");
            Reporter = bo.ContainsKey("reporter") ? new User(bo.Get<AVUser>("reporter")) : null;
            Priority = bo.Get<int>("priority");
            StatusCode = bo.Get<int>("status");
            Title = bo.Get<string>("title");
            Description = bo.Get<string>("description");
            Attachment = bo.ContainsKey("attachment") ? bo.Get<string>("attachment") : null;//bo.ContainsKey("attachment") && bo.Get<AVFile>("attachment") != null ? new File(bo.Get<AVFile>("attachment")) : null;
        }

        public AVObject CreateBugObjectForSave()
        {
            AVObject bug = new AVObject("Bug");

            bug.Add("bugIndex", BugIndex);
            bug.Add("reporter", Reporter.LoadUserObject());
            bug.Add("priority", Priority);
            bug.Add("status", StatusCode);
            bug.Add("title", Title);
            bug.Add("description", Description);
            bug.Add("attachment", Attachment);

            return bug;
        }
    }
}
