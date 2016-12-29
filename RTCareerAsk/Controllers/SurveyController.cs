using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RTCareerAsk.Controllers
{
    public class SurveyController : UpperBaseController
    {
        public ActionResult Index()
        {
            if (!HasUserInfo || GetUserID() != "57d4d1b079bc44005e5125f8")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = GenerateTitle("调查问卷");

            return View();
        }

    }
}
