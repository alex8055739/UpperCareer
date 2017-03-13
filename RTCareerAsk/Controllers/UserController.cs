﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;
using RTCareerAsk.Models;
using RTCareerAsk.PLtoDA;
using RTCareerAsk.Filters;

namespace RTCareerAsk.Controllers
{
    public class UserController : UpperBaseController
    {
        [UpperResult]
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                await Task.WhenAll(AutoLogin(), UpdateNewMessageCount());

                UserDetailModel model = await UserDa.LoadUserDetail(id, HasUserInfo ? GetUserID() : "");

                ViewBag.Title = GenerateTitle(model.Name);
                ViewBag.IsSelf = HasUserInfo ? GetUserID() == id : false;

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task FollowUser(string id)
        {
            try
            {
                if (IsUserAuthorized("User,Admin"))
                {
                    await UserDa.Follow(GetUserID(), id);
                }
                else
                {
                    throw new OperationCanceledException("您还未登录，不能关注");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task UnfollowUser(string id)
        {
            try
            {
                if (IsUserAuthorized("User,Admin"))
                {
                    await UserDa.Unfollow(GetUserID(), id);
                }
                else
                {
                    throw new OperationCanceledException("您还未登录，不能关注");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> RecentRecord(int contentType, string targetId, int pageIndex)
        {
            try
            {
                if (contentType == 1)
                {
                    return PartialView("_RecentQuestions", await UserDa.GetRecentQuestions(targetId, pageIndex));
                }
                else if (contentType == 2)
                {
                    return PartialView("_RecentAnswers", await UserDa.GetRecentAnswers(targetId, pageIndex));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("所提供的操作代码不符合要求。");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperResult]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> FollowersOrFollowees(bool contentType, string targetId, int pageIndex)
        {
            try
            {
                ViewBag.TargetId = targetId;
                ViewBag.ContentType = contentType;

                return PartialView("_UserTagList", await UserDa.LoadFollowersOrFollowees(HasUserInfo ? GetUserID() : null, targetId, contentType, pageIndex));
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> LoadUserIntro(string userId)
        {
            return PartialView("_UserInfoTooltip", await UserDa.LoadUserTag(GetUserID(), userId));
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> CreateUserRecommandModal(string id)
        {
            try
            {
                bool hasExisted = await UserDa.CheckIfUserRecommanded(id);

                ViewBag.Message = hasExisted ? "该用户已被推荐，请输入要更新用户介绍：" : "该用户还未被推荐过，请给出给出用户介绍：";
                ViewBag.UserId = id;
                ViewBag.IsUpdate = hasExisted;

                return PartialView("_RecommandUser");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public async Task RecommandUser(bool isUpdate, string id, string intro)
        {
            try
            {
                if (!await UserDa.SaveOrUpdateRecommandation(isUpdate, id, intro))
                {
                    throw new InvalidOperationException("操作未能成功，请重新尝试");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }
    }
}
