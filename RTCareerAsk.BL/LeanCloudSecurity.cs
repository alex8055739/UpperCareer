using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.BL
{
    public static class LeanCloudSecurity
    {
        public static LeanCloudAccess LCDal { get { return new LeanCloudAccess(); } }

        public static async Task<bool> ValidateInviteCode(string code)
        {
            return await LCDal.IsInviteCodeValid(code);
        }

        public static async Task<bool> CreateUserAndAccount(User u, string inviteCode)
        {
            if (await LCDal.RegisterUser(u))
            {
                if (await LCDal.DisposeInviteCode(inviteCode))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
