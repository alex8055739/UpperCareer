using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTCareerAsk.Models
{
    public class AnswerModel
    {
        public string AnswerID { get; set; }

        public string Content { get; set; }

        public UserModel User { get; set; }

        public DateTime DateCreate { get; set; }

        //Place a group of pictures
    }
}