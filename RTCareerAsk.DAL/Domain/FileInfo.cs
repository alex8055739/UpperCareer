using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCareerAsk.DAL.Domain
{
    public class FileInfo
    {
        public string ObjectID { get; set; }

        public string Mime_Type { get; set; }

        public string Key { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public IDictionary<string,object> MetaData { get; set; }

        public DateTime DateCreate { get; set; }
    }
}
