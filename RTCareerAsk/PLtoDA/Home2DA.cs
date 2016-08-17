using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.DAL;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Home Controller的沟通。
    /// </summary>
    public class Home2DA : DABase
    {
        public async Task<List<QuestionInfoModel>> GetQuestionInfoModels()
        {
            return await LCDal.FindPostQuestions().ContinueWith(t =>
            {
                List<QuestionInfoModel> qiList = new List<QuestionInfoModel>();

                foreach (QuestionInfo q in t.Result)
                {
                    qiList.Add(new QuestionInfoModel(q));
                }

                return qiList;
            });
        }

        public async Task<UserInfoModel> LoadUserInfo(string userId)
        {
            return await LCDal.LoadUserInfo(userId).ContinueWith(t =>
                {
                    return new UserInfoModel(t.Result);
                });
        }

        public async Task<string> UploadImageFile(FileModel f)
        {
            return await LCDal.SaveNewStreamFile(f.RestoreFileModelToObject());
        }

        public async Task<bool> ChangeUserPortrait(string userId, string portraitUrl)
        {
            try
            {
                return await LCDal.ChangeUserPortrait(userId, portraitUrl);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<FileInfoModel>> GetFileInfoModels()
        {
            return await LCDal.FindAllFiles().ContinueWith(t => ConvertFileInfoObjectsToModels(t.Result));
        }

        public async Task<FileModel> DownloadImageFiles(string fileId)
        {
            return await LCDal.DownloadFileByID(fileId).ContinueWith(t => new FileModel(t.Result));
        }
    }
}