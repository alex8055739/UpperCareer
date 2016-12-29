using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class Vote
    {
        public string TargetID { get; set; }

        public VoteType Type { get; set; }

        public string VoterID { get; set; }

        public bool IsLike { get; set; }

        public bool IsUpdate { get; set; }

        public History Notification { get; set; }

        public AVObject CreateVoteObject()
        {
            AVObject vote;

            switch (Type)
            {
                case VoteType.Question:
                    vote = new AVObject("VotePost");
                    vote.Add("voteFor", AVObject.CreateWithoutData("Post", TargetID));
                    break;

                case VoteType.Answer:
                    vote = new AVObject("VoteAnswer");
                    vote.Add("voteFor", AVObject.CreateWithoutData("Answer", TargetID));
                    break;
                default:
                    throw new InvalidCastException("所提供的点赞类型未知。");
            }

            vote.Add("voteBy", AVObject.CreateWithoutData("_User", VoterID) as AVUser);
            vote.Add("isLike", IsLike);

            return vote;
        }

        public AVObject LoadTargetObject()
        {
            switch (Type)
            {
                case VoteType.Question:
                    return AVObject.CreateWithoutData("Post", TargetID);

                case VoteType.Answer:
                    return AVObject.CreateWithoutData("Answer", TargetID);

                default:
                    throw new InvalidCastException("所提供的点赞类型未知。");
            }
        }

        public AVUser LoadVoter()
        {
            return AVObject.CreateWithoutData("_User", VoterID) as AVUser;
        }

        public string LoadClassName()
        {
            switch (Type)
            {
                case VoteType.Question:
                    return "VotePost";

                case VoteType.Answer:
                    return "VoteAnswer";

                default:
                    throw new InvalidCastException("所提供的点赞类型未知。");
            }
        }
    }
}
