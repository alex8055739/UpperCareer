using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class VoteAnswer : UpperVoteBaseDomain
    {
        public VoteAnswer() { }

        public VoteAnswer(AVObject va)
        {
            GenerateVoteAnswerObject(va);
        }

        public Answer VoteFor { get; set; }

        private void GenerateVoteAnswerObject(AVObject va)
        {
            if (va.ClassName != "VoteAnswer")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是答案投票类object。类型：{1}", va.ObjectId, va.ClassName));
            }

            GenerateVoteObject(va);
            VoteFor = va.ContainsKey("voteFor") ? new Answer(va.Get<AVObject>("voteFor")) : null;
        }
    }
}
