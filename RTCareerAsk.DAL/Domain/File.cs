using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AVOSCloud;

namespace RTCareerAsk.DAL.Domain
{
    public class File
    {
        public File()
        {
            MetaData = new Dictionary<string, object>();
        }

        public File(AVFile fo)
        {
            GenerateFileObject(fo);
        }

        public string ObjectID { get; set; }

        public string FileName { get; set; }

        public byte[] FileDataByte { get; set; }

        public Stream FileDataStream { get; set; }

        public Uri Url { get; set; }

        public IDictionary<string, object> MetaData { get; set; }

        private void GenerateFileObject(AVFile fo)
        {
            ObjectID = fo.ObjectId;
            FileName = fo.Name;
            FileDataByte = fo.DataByte;
            MetaData = fo.MetaData;
            Url = fo.Url;
        }

        public LeanCloud.AVFile CreateStreamFileObjectForSave()
        {
            return new LeanCloud.AVFile(FileName, FileDataStream, MetaData);
        }

        public AVFile CreateByteFileObjectForSave()
        {
            return new AVFile(FileName, FileDataByte, MetaData);
        }
    }
}
