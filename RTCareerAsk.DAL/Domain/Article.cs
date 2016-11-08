using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class ArticleInfo : UpperArticleBaseDomain
    {
        public ArticleInfo() { }

        public ArticleInfo(AVObject obj)
        {
            GenerateArticleBaseObject(obj);
        }
    }

    public class Article : UpperArticleBaseDomain
    {
        public Article() { }

        public Article(AVObject obj)
        {
            GenerateArticleObject(obj);
        }

        public string Content { get; set; }

        public User Editor { get; set; }

        public Answer Reference { get; set; }

        private void GenerateArticleObject(AVObject obj)
        {
            GenerateArticleBaseObject(obj);
            Content = obj.ContainsKey("content") ? obj.Get<string>("content") : default(string);
            Editor = obj.ContainsKey("editor") ? obj.Get<User>("editor") : default(User);
            Reference = obj.ContainsKey("reference") ? obj.Get<Answer>("reference") : default(Answer);
        }

        public AVObject CreateArticleObjectForSave()
        {
            AVObject atcl = new AVObject("Article");

            atcl.Add("title", Title);
            atcl.Add("cover", Cover);
            atcl.Add("author", Author);
            atcl.Add("index", Index);
            atcl.Add("content", Content);
            atcl.Add("editor", Editor.LoadUserObject());
            atcl.Add("reference", Reference != default(Answer) ? Reference.LoadAnswerObject() : null);

            return atcl;
        }
    }

    public class ArticleReference
    {
        public ArticleReference() { }

        public ArticleReference(AVObject ans)
        {
            GenerateArticleReferenceObject(ans);
        }

        public Answer Reference { get; set; }

        public List<ArticleInfo> TopArticles { get; set; }

        private void GenerateArticleReferenceObject(AVObject ans)
        {
            if (ans.ClassName != "Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是答案类object。对象类型：{1}", ans.ObjectId, ans.ClassName));
            }

            Reference = new Answer(ans);
        }

        public ArticleReference SetTopArticles(IEnumerable<AVObject> atcls)
        {
            if (atcls.First().ClassName != "Article")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是资讯类object。对象类型：{1}", atcls.First().ObjectId, atcls.First().ClassName));
            }

            foreach (AVObject atcl in atcls)
            {
                TopArticles.Add(new ArticleInfo(atcl));
            }

            return this;
        }
    }
}
