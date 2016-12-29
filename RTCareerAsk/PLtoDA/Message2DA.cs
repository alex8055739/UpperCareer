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
    /// 此页方法仅限于与Message Controller的沟通。
    /// </summary>
    public class Message2DA : DABase
    {
        public async Task<List<HistoryModel>> LoadNotificationsByPage(string userId, int[] types, int pageIndex = 0)
        {
            return await LCDal.LoadNotifications(userId, types, pageIndex).ContinueWith(t =>
                {
                    List<HistoryModel> ntfns = new List<HistoryModel>();

                    foreach (History ntfn in t.Result.OrderByDescending(x => x.IsNew).ThenByDescending(x => x.DateCreate))
                    {
                        ntfns.Add(new HistoryModel(ntfn));
                    }

                    return ntfns;
                });
        }

        public async Task<List<HistoryModel>> LoadNotificationsByPage(int[] types, int pageIndex = 0)
        {
            return await LCDal.LoadNotifications(types, pageIndex).ContinueWith(t =>
            {
                List<HistoryModel> ntfns = new List<HistoryModel>();

                foreach (History ntfn in t.Result.OrderByDescending(x => x.IsNew).ThenByDescending(x => x.DateCreate))
                {
                    ntfns.Add(new HistoryModel(ntfn));
                }

                return ntfns;
            });
        }

        public async Task<bool> MarkNotificationAsRead(string id)
        {
            return await LCDal.MarkNotificationAsRead(id);
        }

        public async Task<List<MessageModel>> LoadMessagesByUserID(string userId)
        {
            return await LCDal.LoadMessagesForUser(userId).ContinueWith(t =>
                {
                    List<MessageModel> msgs = new List<MessageModel>();

                    foreach (Message msg in t.Result)
                    {
                        msgs.Add(new MessageModel(msg));
                    }

                    return msgs;
                });
        }

        public async Task<MessageModel> GetMessageByID(string messageId)
        {
            return await LCDal.GetMessageByID(messageId).ContinueWith(t => new MessageModel(t.Result));
        }

        public async Task WriteNewMessage(LetterModel l)
        {
            await LCDal.WriteNewMessage(l.CreateMessageForSave());
        }
    }
}