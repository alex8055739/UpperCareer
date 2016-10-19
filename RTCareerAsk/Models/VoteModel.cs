using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class VoteModel
    {
        public string TargetID { get; set; }

        public int Type { get; set; }

        public string VoterID { get; set; }

        public bool IsLike { get; set; }

        public bool IsUpdate { get; set; }

        public Vote CreateVote()
        {
            return new Vote()
            {
                TargetID = TargetID,
                Type = (VoteType)Type,
                VoterID = VoterID,
                IsLike = IsLike,
                IsUpdate = IsUpdate
            };
        }
    }
}