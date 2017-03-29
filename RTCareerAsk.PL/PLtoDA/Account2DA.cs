using System;
using System.Threading.Tasks;
using RTCareerAsk.PL.Models;

namespace RTCareerAsk.PL.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Account Controller的沟通。
    /// </summary>
    public class Account2DA : DABase
    {
        public async Task<UserInfoModel> LoginWithEmail(string email, string password)
        {
            return await LCDal.LoginWithEmail(email, password).ContinueWith(t =>
            {
                return new UserInfoModel(t.Result);
            });
        }

        public async Task<bool> ResetPasswordByEmail(string email)
        {
            return await LCDal.ResetPassword(email);
        }

        public async Task<UserManageModel> LoadUserManageInfo(string userId)
        {
            return await LCDal.LoadUserDetail(userId).ContinueWith(t =>
            {
                return new UserManageModel(t.Result);
            });
        }

        public async Task<bool> ChangeUserPortrait(string userId, string portraitUrl)
        {
            try
            {
                return await LCDal.ChangeUserPortrait(userId, portraitUrl);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfile(UserManageModel umm)
        {
            return await LCDal.SaveUserDetail(umm.RestoreUserManageModelToUserDetailObject());
        }
    }
}