using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
    public class Article2DA : DABase
    {
        public async Task<ArticlePostModel> CreatePostModelWithReference(string id)
        {
            return await LCDal.LoadReference(id).ContinueWith(t =>
                {
                    return new ArticlePostModel(t.Result);
                });
        }

        public async Task<bool> PostNewArticle(ArticlePostModel model)
        {
            return await LCDal.SaveNewArticle(model.CreatePostForSave());
        }

        public async Task<ArticleModel> LoadArticleDetail(string id)
        {
            return await LCDal.LoadArticle(id).ContinueWith(t =>
            {
                return new ArticleModel(t.Result);
            });
        }
    }
}