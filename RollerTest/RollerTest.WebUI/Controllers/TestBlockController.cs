using RollerTest.Domain.Abstract;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.ExternalProgram;
using RollerTest.WebUI.IniFiles;
using RollerTest.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RollerTest.WebUI.Controllers
{
    public class TestBlockController : Controller
    {
        private ISampleinfoRepository samplerepo;
        private IBaseRepository baserepo;
        private IRealtimeinfoRepository realtimerepo;

        public TestBlockController(ISampleinfoRepository repo, IRealtimeinfoRepository rtrepo,IBaseRepository baserepo)
        {
            samplerepo = repo;
            realtimerepo = rtrepo;
            this.baserepo = baserepo;
        }
        // GET: TestBlock
        public ActionResult Index()
        {
            TestListViewModel testlistviewModel = new TestListViewModel()
            {
                rollersampleinfos = samplerepo.RollerSampleInfos.Where(x => x.State == true).Include(x => x.RollerBaseStation).Include(x => x.RollerProjectInfo)
            };
            return View(testlistviewModel);
        }
        public PartialViewResult RealtimeAction(int RollerSampleInfoId)
        {
            RollerRealtimeInfo rollerrealtimeInfo = realtimerepo.RollerRealtimeInfos.FirstOrDefault(x => x.RollerSampleInfoID == RollerSampleInfoId);
            return PartialView(rollerrealtimeInfo);
        }

        public ActionResult OpenTest(int StationId)
        {
            if (baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).State == false)
            {
                baserepo.ChangeStationState(StationId, true);
                IniFileControl.GetInstance().OpenRollerTimeSwitch(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
                RollerSampleInfo rollersampleinfo = samplerepo.RollerSampleInfos.FirstOrDefault(x => x.RollerBaseStationID == StationId && x.State == true);
                samplerepo.setsampleStartTime(rollersampleinfo);
                //DealControl.GetInstance().setRollerLimit(StationId, rollersampleinfo.UpLimit, rollersampleinfo.DnLimit);
                //RedirectToAction("Index", "TestReport", new { RollerSampleInfoId = rollersampleinfo.RollerSampleInfoID });
                return RedirectToAction("Index","TestReport", new { RollerSampleInfoId = rollersampleinfo.RollerSampleInfoID });
            }
            else {
                return RedirectToAction("Index");
            }
        }
        public ActionResult CloseTest(int StationId)
        {
            if (baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).State == true)
            {
                baserepo.ChangeStationState(StationId, false);
                int sampleId = samplerepo.RollerSampleInfos.FirstOrDefault(x => x.State == true && x.RollerBaseStationID == StationId).RollerSampleInfoID;
                samplerepo.setsampleState(sampleId, false);
                samplerepo.setsampleEndTime(sampleId);
                IniFileControl.GetInstance().CloseRollerTimeSwitch(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
                IniFileControl.GetInstance().CleanRollerTime(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
                return RedirectToAction("AfterTest", "TestReport", new { RollerSampleInfoId = sampleId });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public void OpenTimer(int StationId)
        {
            IniFileControl.GetInstance().OpenRollerTimeSwitch(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
            Response.Redirect("/TestBlock/Index");
        }
        public void PauseTimer(int StationId)
        {
            IniFileControl.GetInstance().CloseRollerTimeSwitch(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
            Response.Redirect("/TestBlock/Index");
        }
        public void CleanTimer(int StationId)
        {
            IniFileControl.GetInstance().CleanRollerTime(baserepo.RollerBaseStations.FirstOrDefault(x => x.RollerBaseStationID == StationId).Station);
            Response.Redirect("/TestBlock/Index");
        }


    }
}