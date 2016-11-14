using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class ArticleInfoModel : UpperArticleBaseModel
    {
        public ArticleInfoModel() { }

        public ArticleInfoModel(ArticleInfo atcli)
        {
            ConvertArticleObjectToModel(atcli);
        }
    }

    public class ArticleModel : UpperArticleBaseModel
    {
        public ArticleModel() { }

        public ArticleModel(Article atcl)
        {
            ConvertArticleObjectToModel(atcl);
        }

        public AnswerModel Reference { get; set; }

        public bool HasReference { get; set; }

        public string Content { get; set; }

        public UserModel Editor { get; set; }

        private void ConvertArticleObjectToModel(Article atcl)
        {
            base.ConvertArticleObjectToModel(atcl);

            Content = atcl.Content;
            HasReference = atcl.HasReference;
            Reference = atcl.HasReference ? new AnswerModel(atcl.Reference) : default(AnswerModel);
            Editor = atcl.HasReference ? new UserModel(atcl.Editor) : default(UserModel);
        }
    }
}