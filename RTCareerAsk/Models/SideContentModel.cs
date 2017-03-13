using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;
using RTCareerAsk.App_DLL;

namespace RTCareerAsk.Models
{
    public class SideContentModel
    {
        public SideContentModel()
        {
            InfoList = new List<UpperInfoBaseModel>();
            ButtonText = "查看更多";
        }

        public SideContentModel(string title, SideContentType type, IEnumerable<UpperInfoBaseDomain> infoList, string btnText = "查看更多")
        {
            InfoList = new List<UpperInfoBaseModel>();
            GenerateSideContent(title, type, infoList, btnText);
        }

        public string Title { get; set; }

        public SideContentType Type { get; set; }

        public List<UpperInfoBaseModel> InfoList { get; set; }

        public string ButtonText { get; set; }

        private void GenerateSideContent(string title, SideContentType type, IEnumerable<UpperInfoBaseDomain> infoList, string btnText = "查看更多")
        {
            Title = title;
            Type = type;

            if (type == SideContentType.Question)
            {
                InfoList.AddRange(infoList.Select(x => new QuestionInfoModel(x as QuestionInfo)));
            }
            else
            {
                InfoList.AddRange(infoList.Select(x => new AnswerInfoModel(x as AnswerInfo)));
            }

            ButtonText = btnText;
        }
    }
}