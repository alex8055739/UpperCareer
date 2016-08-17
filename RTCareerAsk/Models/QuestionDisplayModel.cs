using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTCareerAsk.Models
{
    public class QuestionDisplayModel
    {
        public QuestionModel Question { get; set; }

        public List<AnswerModel> Answers { get; set; }
    }
}