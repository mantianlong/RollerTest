using RollerTest.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RollerTest.Domain.Entities;
using RollerTest.Domain.Context;

namespace RollerTest.Domain.Concrete
{
    public class EFTestreportinfoRepository : ITestreportinfoRepository
    {
        //private EFDbContext context = ContextControl.GetInstance().getContext();
        private EFDbContext context = new EFDbContext();

        public IQueryable<RollerTestreportInfo> RollerTestreportInfos
        {
            get
            {
                return context.RollerTestreportInfos;
            }
        }
        public void SaveRollerTestreportInfo(RollerTestreportInfo rollertestreportinfo)
        {
            if (rollertestreportinfo.RollerTestReportInfoID == 0)
            {
                context.RollerTestreportInfos.Add(rollertestreportinfo);
            }
            else
            {
                RollerTestreportInfo dbEntry = context.RollerTestreportInfos.Find(rollertestreportinfo.RollerTestReportInfoID);
                if (dbEntry != null)
                {
                    dbEntry.RollerTestReportInfoID = rollertestreportinfo.RollerTestReportInfoID;
                    dbEntry.RollerSampleInfoID = rollertestreportinfo.RollerSampleInfoID;
                    dbEntry.StartStatus = rollertestreportinfo.StartStatus;
                    dbEntry.StartText = rollertestreportinfo.StartText;
                    dbEntry.EndText = rollertestreportinfo.EndText;
                    dbEntry.EndStatus = rollertestreportinfo.EndStatus;
                    dbEntry.FinalStatus = rollertestreportinfo.FinalStatus;
                    dbEntry.StartTime = rollertestreportinfo.StartTime;
                    dbEntry.EndTime = rollertestreportinfo.EndTime;
                }
            }
            context.SaveChanges();
        }
    }
}
