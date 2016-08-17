using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTCareerAsk.Models
{
    public class QuestionModel
    {
        public string QuestionID { get; set; }

        public UserModel Creator { get; set; }

        public DateTime DateCreate { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        //Place a group of pictures at this point
    }
}