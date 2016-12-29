using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class Message
    {
        public Message() { }

        public Message(AVObject mo)
        {
            GenerateMessageObject(mo);
        }

        public string ObjectID { get; set; }

        public MessageBody Content { get; set; }

        public bool IsNew { get; set; }

        public User From { get; set; }

        public User To { get; set; }

        public DateTime DateCreate { get; set; }

        private void GenerateMessageObject(AVObject mo)
        {
            if (mo.ClassName != "Message")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是消息类object。对象类型：{1}", mo.ObjectId, mo.ClassName));
            }

            ObjectID = mo.ObjectId;
            Content = new MessageBody(mo.Get<AVObject>("content"));
            IsNew = mo.Get<bool>("isNew");
            From = mo.ContainsKey("from") ? new User(mo.Get<AVUser>("from")) : null;
            To = mo.ContainsKey("to") ? new User(mo.Get<AVUser>("to")) : null;
            DateCreate = Convert.ToDateTime(mo.CreatedAt);
        }

        public AVObject CreateMessageObjectForWrite()
        {
            AVObject message = new AVObject("Message");

            message.Add("content", Content.RestoreMessageBodyObject());
            message.Add("isNew", true);
            message.Add("from", !string.IsNullOrEmpty(From.ObjectID) ? From.LoadUserObject() : null);
            message.Add("to", !string.IsNullOrEmpty(To.ObjectID) ? To.LoadUserObject() : null);

            return message;
        }
    }

    public class MessageBody
    {
        public MessageBody() { }

        public MessageBody(AVObject mbo)
        {
            GenerateMessageBodyObject(mbo);
        }

        public string ObjectID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsSystem { get; set; }

        public DateTime DateCreate { get; set; }

        private void GenerateMessageBodyObject(AVObject mbo)
        {
            if (mbo.ClassName != "Message_Body")
            {
                throw new InvalidOperationException("获取的对象不是消息内容类object。");
            }

            ObjectID = mbo.ObjectId;
            Title = mbo.Get<string>("title");
            Content = mbo.Get<string>("content");
            IsSystem = mbo.Get<bool>("isSystem");
            DateCreate = Convert.ToDateTime(mbo.CreatedAt);
        }

        public AVObject RestoreMessageBodyObject()
        {
            AVObject messageBody = new AVObject("Message_Body");

            messageBody.Add("title", Title);
            messageBody.Add("content", Content);
            messageBody.Add("isSystem", IsSystem);

            return messageBody;
        }
    }
}
