using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class VoteQuestion : UpperVoteBaseDomain
    {
        public VoteQuestion() { }

        public VoteQuestion(AVObject vq)
        {
            GenerateVoteQuestionObject(vq);
        }

        public Question VoteFor { get; set; }

        private void GenerateVoteQuestionObject(AVObject vq)
        {
            if (vq.ClassName != "VotePost")
            {
                throw new InvalidOperationException(string.Format("获取的对象{0}不是问题投票类object。类型：{1}", vq.ObjectId, vq.ClassName));
            }

            GenerateVoteObject(vq);
            VoteFor = vq.ContainsKey("voteFor") ? new Question(vq.Get<AVObject>("voteFor")) : null;
        }
    }
}
