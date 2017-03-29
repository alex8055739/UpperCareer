using System.Collections.Generic;
using System.Threading.Tasks;
using RTCareerAsk.PL.Models;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PL.PLtoDA
{
    /// <summary>
    /// 此目录下所有方法都可以直接读取数据库，不需经过逻辑层。所有方法仅为了模型转换用。
    /// 
    /// 此页方法仅限于与Test Controller的沟通。
    /// </summary>
    public class Test2DA : DABase
    {
        public async Task<bool> SaveNewBugReport(CatchModel cm)
        {
            return await LCDal.SaveNewBugReport(cm.CreateReportForSave());
        }

        public async Task<List<BugModel>> LoadBugReports()
        {
            return await LCDal.LoadBugReports().ContinueWith(t =>
            {
                List<BugModel> bms = new List<BugModel>();

                foreach (Bug b in t.Result)
                {
                    bms.Add(new BugModel(b));
                }

                return bms;
            });
        }

        public async Task<bool> UpdateBugReport(BugModel bm)
        {
            return await LCDal.UpdateBugReport(bm.CreateBugUpdateModel());
        }
    }
}