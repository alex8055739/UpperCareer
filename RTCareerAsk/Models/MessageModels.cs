using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class MessageModel
    {
        public MessageModel() { }

        public MessageModel(Message mo)
        {
            ConvertMessgeObjectToModel(mo);
        }

        public string MessageID { get; set; }

        public MessageBodyModel Content { get; set; }

        public bool IsNew { get; set; }

        public UserModel From { get; set; }

        public UserModel To { get; set; }

        public DateTime DateCreate { get; set; }

        private void ConvertMessgeObjectToModel(Message mo)
        {
            MessageID = mo.ObjectID;
            Content = new MessageBodyModel(mo.Content);
            IsNew = mo.IsNew;
            From = mo.From != null ? new UserModel(mo.From) : null;
            To = mo.To != null ? new UserModel(mo.To) : null;
            DateCreate = mo.DateCreate;
        }
    }

    public class MessageBodyModel
    {
        public MessageBodyModel() { }

        public MessageBodyModel(MessageBody mbo)
        {
            ConvertMessageBodyObjectToModel(mbo);
        }

        public string MessageBodyID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsSystem { get; set; }

        public DateTime DateCreate { get; set; }

        private void ConvertMessageBodyObjectToModel(MessageBody mbo)
        {
            MessageBodyID = mbo.ObjectID;
            Title = mbo.Title;
            Content = mbo.Content;
            IsSystem = mbo.IsSystem;
            DateCreate = mbo.DateCreate;
        }
    }

    public class LetterModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool IsSystem { get; set; }

        [Required(ErrorMessage = "请输入信件标题")]
        [Display(Name = "标题")]
        [StringLength(15, ErrorMessage = "标题请不要超过15个字")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入正文")]
        [Display(Name = "正文")]
        public string Content { get; set; }

        public Message CreateMessageForSave()
        {
            return new Message()
            {
                From = new User() { ObjectID = From },
                To = new User() { ObjectID = To },
                Content = new MessageBody() { Title = Title, Content = Content, IsSystem = IsSystem }
            };
        }
    }
}