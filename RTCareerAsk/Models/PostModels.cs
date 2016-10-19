using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class QuestionPostModel
    {
        [Required(ErrorMessage = "请输入标题")]
        [StringLength(30,ErrorMessage="标题请不要超过30字")]
        [DisplayName("标题：")]
        public string PostTitle { get; set; }
        [DisplayName("正文：")]
        [StringLength(1500, ErrorMessage ="超过字数上限，正文请不要超过1000字")]
        public string PostContent { get; set; }

        public string UserID { get; set; }

        public Question CreatePostForSave()
        {
            return new Question()
            {
                Title = PostTitle,
                Content = PostContent,
                CreatedBy = new User() { ObjectID = UserID }
            };
        }
    }

    public class AnswerPostModel
    {
        [Required(ErrorMessage="请您输入答案正文")]
        [DisplayName("答案内容：")]
        [StringLength(6000, ErrorMessage = "超过字数上限，答案请不要超过5000字")]
        public string PostContent { get; set; }

        public string QuestionID { get; set; }

        public string UserID { get; set; }

        public string NotifyUserID { get; set; }

        public Answer CreatePostForSave()
        {
            return new Answer()
            {
                Content = PostContent,
                ForQuestion = new Question() { ObjectID = QuestionID },
                CreatedBy = new User() { ObjectID = UserID }
            };
        }
    }

    public class CommentPostModel
    {
        [Required(ErrorMessage = "请您输入评论正文")]
        [DisplayName("评论内容：")]
        [StringLength(140, ErrorMessage = "超过字数上限，答案请不要超过140字")]
        public string PostContent { get; set; }

        public string AnswerID { get; set; }

        public string UserID { get; set; }

        public string NotifyUserID { get; set; }

        public Comment CreatePostForSave()
        {
            return new Comment()
            {
                Content = PostContent,
                ForAnswer = new Answer() { ObjectID = AnswerID },
                CreatedBy = new User() { ObjectID = UserID },
            };
        }
    }
}