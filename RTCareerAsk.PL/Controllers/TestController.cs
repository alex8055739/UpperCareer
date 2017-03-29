using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using RTCareerAsk.PL.Models;
using RTCareerAsk.PL.Filters;

namespace RTCareerAsk.PL.Controllers
{
    public class TestController : UpperBaseController
    {
        public string SessionCopyName { get { return "BugReportCopy"; } }

        [UpperResult]
        public async Task<ActionResult> Index()
        {
            if (!IsUserAuthorized("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Title = "浏览错误报告";
            ViewBag.Priorities = GetPriorities();
            ViewBag.StatusCodes = GetStatusCodes();

            List<BugModel> model = await TestDa.LoadBugReports();

            return View(model);
        }

        [HttpPost]
        public PartialViewResult CreateBugReportForm()
        {
            try
            {
                ViewBag.Priorities = GetPriorities();
                CatchModel model;

                if (HasSessionCopy(SessionCopyName))
                {
                    model = RestoreCopy<CatchModel>(SessionCopyName);
                }
                else
                {
                    model = new CatchModel();
                }

                return PartialView("_BugReportModal", model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewBugReport(CatchModel cm)
        {
            try
            {
                List<FileModel> files = GetUploadedFiles(Request.Files);

                cm.Attachment = files.Count > 0 ? files.First() : null;
                cm.ReporterID = GetUserID();

                CopyToSave(SessionCopyName, cm);

                await TestDa.SaveNewBugReport(cm);
                ClearCopy(SessionCopyName);

                return RedirectToAction("Index", "Test");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task UpdateBugReport(BugModel bm)
        {
            try
            {
                await TestDa.UpdateBugReport(bm);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        #region Helper
        private List<SelectListItem> GetPriorities()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "高", Value = "0" },
                new SelectListItem() { Text = "中", Value = "1" },
                new SelectListItem() { Text = "低", Value = "2" }
            };
        }

        private List<SelectListItem> GetStatusCodes()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "待修复", Value = "0" },
                new SelectListItem() { Text = "待测试", Value = "1" },
                new SelectListItem() { Text = "已解决", Value = "2" }
            };
        }
        #endregion
    }
}