using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RTCareerAsk.BL;
using RTCareerAsk.PL.Filters;
using RTCareerAsk.PL.Models;

namespace RTCareerAsk.PL.Controllers
{
    //[Authorize]
    public class AccountController : UpperBaseController
    {
        #region Upper Included

        //
        // GET: /Account/Login

        [UpperResult]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool fromRegister = false)
        {
            ViewBag.Title = GeneralTitle;

            if (fromRegister)
            {
                ViewBag.FromRegister = true;
                ViewBag.ReturnUrl = returnUrl;
                bool hasTempData = (TempData["errorMsg"] != null) && (TempData["regiModel"] != null);

                if (hasTempData)
                {
                    ModelState.AddModelError("", TempData["errorMsg"].ToString());
                }

                return View(new LoginRegiModel() { Register = hasTempData ? TempData["regiModel"] as RegisterModel : new RegisterModel() });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginRegiModel());
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {

            #region Login Method Predefined
            //if (ModelState.IsValid && WebSecurity.Login(model.Login.UserName, model.Login.Password, persistCookie: model.Login.RememberMe))
            //{
            //    return RedirectToLocal(returnUrl);
            //}
            #endregion

            #region Login Method Original
            //if (ModelState.IsValid && await LeanCloudSecurity.Login(model.Login.UserName, model.Login.Password))
            //{
            //    if (model.Login.RememberMe)
            //    {
            //        FormsAuthentication.SetAuthCookie(model.Login.UserName, model.Login.RememberMe);
            //    }

            //    await AccountDa.GetUserModel(model.Login.UserName).ContinueWith(t => StoreUserToSession(t.Result));

            //    return RedirectToLocal(returnUrl);
            //}

            //// If we got this far, something failed, redisplay form
            //ModelState.AddModelError("", "用户名或密码不正确");
            //return View(model);
            #endregion

            #region Login Method Updated
            if (ModelState.IsValid)
            {
                try
                {
                    await AccountLogin(model);

                    return Redirect(returnUrl);
                }
                catch (Exception e)
                {
                    while (e.InnerException != null) e = e.InnerException;
                    ModelState.AddModelError("", TranslateExceptionMessage(e.Message));
                    return View(new LoginRegiModel() { Login = model });
                }
            }

            ModelState.AddModelError("", "用户名或密码不正确");
            return View(new LoginRegiModel() { Login = model });
            #endregion
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous] //Placeholder: Self added code.
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();

            ClearUserFromSession();
            RemoveLoginCookie();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model, string returnUrl)
        {
            string eMsg = "注册出现了问题: ";

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    //WebSecurity.Login(model.UserName, model.Password);
                    //if (await LeanCloudSecurity.ValidateInviteCode(model.InviteCode))
                    //{
                    if (await LeanCloudSecurity.CreateUserAndAccountWithoutInviteCode(model.RestoreRegisterModelToUserObject()))
                    {
                        //await AccountLogin(model.UserName, model.Password);//.ContinueWith(t => StoreUserToSession(t.Result));

                        return RedirectToAction("RegisterSuccess", new { returnUrl = returnUrl, email = model.Email });
                    }
                    //}
                    else
                    {
                        throw new NullReferenceException("您输入的邀请码无效，请查看");
                    }
                    //if (await LeanCloudSecurity.Login(model.Register.UserName, model.Register.Password))
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //    return RedirectToLocal(returnUrl);
                    //};
                }
                catch (Exception e)
                {
                    //ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    while (e.InnerException != null) e = e.InnerException;
                    eMsg += TranslateExceptionMessage(e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            TempData["regiModel"] = model;
            TempData["errorMsg"] = eMsg;

            return RedirectToAction("Login", new { returnUrl = returnUrl, fromRegister = true });
            //return View(model);
        }

        #endregion

        #region Upper Exclusive

        [UpperResult]
        [AllowAnonymous]
        public async Task<ActionResult> UserManage()
        {
            try
            {
                await AutoLogin();

                if (!IsUserAuthorized("User,Admin"))
                {
                    throw new InvalidOperationException("未登录不能访问此页面");
                }

                await UpdateNewMessageCount();

                UserManageModel model = await AccountDa.LoadUserManageInfo(GetUserID());
                model.SelfDescription = ModifyTextareaData(model.SelfDescription, false);

                ViewBag.Title = GenerateTitle("个人设置");
                ViewBag.FieldList = GetFieldList();

                return View(model);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        public ActionResult RegisterSuccess(string returnUrl, string email)
        {
            ViewBag.IsAdmin = IsUserAuthorized("Admin");
            ViewBag.Title = GenerateTitle("注册成功");
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Email = email;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [UpperJsonExceptionFilter]
        public async Task<PartialViewResult> UserManage(UserManageModel model)
        {
            try
            {
                model.SelfDescription = ModifyTextareaData(model.SelfDescription, true);

                if (await AccountDa.UpdateProfile(model))
                {
                    UpdateUserInfo(new Dictionary<string, object>() { { "Name", model.Name } });

                    return PartialView("_NavBar");
                }

                throw new InvalidOperationException("未能成功保存信息");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult QuickLoginForm(string returnUrl)
        {
            try
            {
                ViewBag.returnUrl = returnUrl;

                return PartialView("_QuickLoginModal", new LoginModel());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        [UpperJsonExceptionFilter]
        public PartialViewResult ForgetPasswordForm()
        {
            try
            {
                return PartialView("_ForgetPassword", new ForgetPasswordModel());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ForgetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AccountDa.ResetPasswordByEmail(model.Email);
                    ViewBag.Title = GenerateTitle("重置邮件成功");
                    ViewBag.Email = model.Email;

                    return View("ResetPasswordSuccess");
                }
                else
                {
                    throw new InvalidOperationException();
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
        public async Task<PartialViewResult> ChangePortrait(HttpPostedFileBase portrait)
        {
            try
            {
                string url = await UploadImageFile(portrait, string.Format("portrait{0}", GetUserID()));

                if (await AccountDa.ChangeUserPortrait(GetUserID(), url))
                {
                    UpdateUserInfo(new Dictionary<string, object>() { { "Portrait", url } });
                }
                else
                {
                    await HomeDa.DeleteFileWithUrl(url);
                }

                return PartialView("_NavBar");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                throw e;
            }
        }

        #endregion

        #region MVC Predefined

        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        //public AccountController()
        //{
        //}

        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        ////
        //// GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        ////
        //// POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // 这不会计入到为执行帐户锁定而统计的登录失败次数中
        //    // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "无效的登录尝试。");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // 要求用户已通过使用用户名/密码或外部登录名登录
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // 以下代码可以防范双重身份验证代码遭到暴力破解攻击。
        //    // 如果用户输入错误代码的次数达到指定的次数，则会将
        //    // 该用户帐户锁定指定的时间。
        //    // 可以在 IdentityConfig 中配置帐户锁定设置
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "代码无效。");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
        //            // 发送包含此链接的电子邮件
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //    return View(model);
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // 请不要显示该用户不存在或者未经确认
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
        //        // 发送包含此链接的电子邮件
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a>来重置你的密码");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // 请不要显示该用户不存在
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // 请求重定向到外部登录提供程序
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // 生成令牌并发送该令牌
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // 如果用户没有帐户，则提示该用户创建帐户
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // 从外部登录提供程序获取有关用户的信息
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}

        #endregion

        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}