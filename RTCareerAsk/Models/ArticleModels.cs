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
}