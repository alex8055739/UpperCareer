using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.Models
{
    public class FileModel
    {
        public FileModel()
        {
            MetaData = new Dictionary<string, object>();
        }

        public FileModel(DAL.Domain.File fo)
        {
            MetaData = new Dictionary<string, object>();

            ConvertFileObjectToModel(fo);
        }

        public string FileID { get; set; }

        public string FileName { get; set; }

        public byte[] FileDataByte { get; set; }

        public Stream FileDataStream { get; set; }

        public Uri Url { get; set; }

        public IDictionary<string, object> MetaData { get; set; }

        private void ConvertFileObjectToModel(DAL.Domain.File fo)
        {
            FileID = fo.ObjectID;
            FileName = fo.FileName;
            FileDataByte = fo.FileDataByte;
            FileDataStream = fo.FileDataStream;
            Url = fo.Url;
            MetaData = fo.MetaData;
        }

        public DAL.Domain.File RestoreFileModelToObject()
        {
            return new RTCareerAsk.DAL.Domain.File()
            {
                ObjectID = FileID,
                FileName = FileName,
                FileDataByte = FileDataByte,
                FileDataStream = FileDataStream,
                Url = Url,
                MetaData = MetaData
            };
        }
    }
}