using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTCareerAsk.Models
{
    public class FileInfoModel
    {
        public FileInfoModel()
        {
            MetaData = new Dictionary<string, object>();
        }

        public FileInfoModel(DAL.Domain.FileInfo fio)
        {
            MetaData = new Dictionary<string, object>();

            ConvertFileInfoObjectToModel(fio);
        }

        public string FileID { get; set; }

        public string Mime_Type { get; set; }

        public string Key { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public IDictionary<string, object> MetaData { get; set; }

        public DateTime DateCreate { get; set; }

        private void ConvertFileInfoObjectToModel(DAL.Domain.FileInfo fio)
        {
            FileID = fio.ObjectID;
            Mime_Type = fio.Mime_Type;
            Key = fio.Key;
            FileName = fio.FileName;
            Url = fio.Url;
            MetaData = fio.MetaData;
            DateCreate = fio.DateCreate;
        }
    }
}