using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCareerAsk.DAL.Domain
{
    abstract public class UpperQACBaseDomain
    {
        public string ObjectID { get; set; }

        public string Content { get; set; }

        public DateTime DateCreate { get; set; }

        public User CreatedBy { get; set; }
    }
}
