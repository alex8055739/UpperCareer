using System.Web;
using System.Web.Optimization;

namespace RTCareerAsk.PL
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                        "~/Scripts/bootstrap*",
                        "~/Scripts/Upper/upper-base*",
                        "~/Scripts/Upper/new-post.js"));

            bundles.Add(new ScriptBundle("~/bundles/content-list").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/question-list.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js"));

            bundles.Add(new ScriptBundle("~/bundles/feeds").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/side-bar.js",
                        "~/Scripts/Upper/feeds.js"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                        "~/Scripts/Upper/login.js")); ;


            bundles.Add(new ScriptBundle("~/bundles/question").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/question-answer-edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/answer").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/side-bar.js",
                        "~/Scripts/Upper/question-answer-edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/user").Include(
                        "~/Scripts/Upper/user-page.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-shorten.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/search-result*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/message").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/notification-index.js",
                        "~/Scripts/Upper/message-index.js"));

            bundles.Add(new ScriptBundle("~/bundles/notification").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/notification-all.js"));

            bundles.Add(new ScriptBundle("~/bundles/manage").Include(
                        "~/Scripts/Upper/user-manage.js",
                        "~/Scripts/cropper*",
                        "~/Scripts/canvas-to-blob*"));

            bundles.Add(new ScriptBundle("~/bundles/article").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-article-detail.js"));

            bundles.Add(new ScriptBundle("~/bundles/article-list").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js"));

            bundles.Add(new ScriptBundle("~/bundles/compose").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/article-compose.js",
                        "~/Scripts/canvas-to-blob*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.upper.css",
                        "~/Content/Upper.css",
                        "~/Content/preloader.css"));

            bundles.Add(new StyleBundle("~/Content/cropper").Include(
                        "~/Content/cropper*"));
        }
    }
}
