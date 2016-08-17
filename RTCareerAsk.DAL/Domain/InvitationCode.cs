using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class InvitationCode
    {
        public InvitationCode() { }

        public InvitationCode(AVObject ico)
        {
            GenerateInviteCodeObject(ico);
        }

        public string ObjectID { get; set; }

        public string InviteCode { get; set; }

        public bool IsMaster { get; set; }

        public bool IsValid { get; set; }

        private void GenerateInviteCodeObject(AVObject ico)
        {
            if (ico.ClassName != "Invitation_Code")
            {
                throw new InvalidOperationException("获取的对象不是邀请码类object。");
            }

            ObjectID = ico.ObjectId;
            InviteCode = ico.Get<string>("inviteCode");
            IsMaster = ico.Get<bool>("isMaster");
            IsValid = ico.Get<bool>("isValid");
        }

    }
}
