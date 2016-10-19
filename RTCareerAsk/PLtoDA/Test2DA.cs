using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using RTCareerAsk.Models;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PLtoDA
{
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