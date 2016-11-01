using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RTCareerAsk.Models;

namespace RTCareerAsk.Filters
{
    public class UpperResultAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            bool hasUserInfo = filterContext.HttpContext.Session["UserInfo"] != null;
            UserInfoModel userInfo = hasUserInfo ? filterContext.HttpContext.Session["UserInfo"] as UserInfoModel : null;

            filterContext.Controller.ViewBag.IsAuthorized = IsUserAuthorized(userInfo, "User,Admin");
            filterContext.Controller.ViewBag.IsAdmin = IsUserAuthorized(userInfo, "Admin");
        }

        protected bool IsUserAuthorized(UserInfoModel user, string roles)
        {
            if (user != null)
            {
                string[] rolesSplit = SplitString(roles);

                if (user.RoleNames.Contains("Block"))
                {
                    return false;
                }

                if (rolesSplit.Length > 0 && rolesSplit.Intersect(user.RoleNames).Count() == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}
