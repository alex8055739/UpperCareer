using System.Collections.Generic;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PL.Models
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
        public ArticleModel()
        {
            Comments = new List<ArticleCommentModel>();
        }

        public ArticleModel(Article atcl)
        {
            Comments = new List<ArticleCommentModel>();

            ConvertArticleObjectToModel(atcl);
        }

        public AnswerModel Reference { get; set; }

        public bool HasReference { get; set; }

        public string Content { get; set; }

        public UserModel Editor { get; set; }

        public List<ArticleCommentModel> Comments { get; set; }

        private void ConvertArticleObjectToModel(Article atcl)
        {
            base.ConvertArticleObjectToModel(atcl);

            Content = atcl.Content;
            HasReference = atcl.HasReference;
            Reference = atcl.HasReference ? new AnswerModel(atcl.Reference) : default(AnswerModel);
            Editor = atcl.HasReference ? new UserModel(atcl.Editor) : default(UserModel);

            foreach (ArticleComment cmt in atcl.Comments)
            {
                Comments.Add(new ArticleCommentModel(cmt));
            }
        }
    }

    public class ArticleCommentModel
    {
        public ArticleCommentModel() { }

        public ArticleCommentModel(ArticleComment acmt)
        {
            ConvertArticleCommentObjectToModel(acmt);
        }

        public string ID { get; set; }

        public UserModel Creator { get; set; }

        public string ForArticleID { get; set; }

        public string Content { get; set; }

        public bool IsDeleteAllowed { get; set; }

        public string DateCreate { get; set; }

        public string DateUpdate { get; set; }

        private void ConvertArticleCommentObjectToModel(ArticleComment acmt)
        {
            ID = acmt.ObjectID;
            Content = acmt.Content;
            Creator = new UserModel(acmt.Creator);
            ForArticleID = acmt.ForArticle.ObjectID;
            DateCreate = acmt.DateCreate.ToString("yyyy-M-d");
            DateUpdate = acmt.DateUpdate.ToString("yyyy-M-d");
        }
    }
}