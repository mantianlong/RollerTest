using RollerTest.Domain.Abstract;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RollerTest.WebUI.Controllers
{
    public class SampleTestingRecordController : Controller
    {
        private IRecordinfoRepository recordrepo;
        private ISampleinfoRepository samplerepo;
        public SampleTestingRecordController(IRecordinfoRepository recordrepo, ISampleinfoRepository samplerepo)
        {
            this.recordrepo = recordrepo;
            this.samplerepo = samplerepo;
        }
        // GET: SampleTestingRecord
        public ActionResult Index(int RollerSampleInfoId)
        {
            SampleTestingRecordViewModel testingrecordviewModel = new SampleTestingRecordViewModel()
            {
                rollerrecordinfos = recordrepo.RollerRecordInfos.Where(x => x.RollerSampleInfoID == RollerSampleInfoId).Include(x=>x.RollerSampleInfo),
                SampleId=RollerSampleInfoId
            };
            return View(testingrecordviewModel);
        }
        public ViewResult CreateSampleTestingRecord(int RollerSampleInfoID)
        {
            RollerSampleInfo rollersampleinfo = samplerepo.RollerSampleInfos.FirstOrDefault(a => a.RollerSampleInfoID == RollerSampleInfoID);
            return View("EditSampleTestingRecord", new RollerRecordInfo() { RollerSampleInfoID = RollerSampleInfoID,RollerSampleInfo= rollersampleinfo });
        }
        [HttpGet]
        public ViewResult EditSampleTestingRecord(int RollerRecordInfoID)
        {
            RollerRecordInfo rollerrecordinfo = recordrepo.RollerRecordInfos.FirstOrDefault(a => a.RollerRecordInfoID == RollerRecordInfoID);
            return View(rollerrecordinfo);
        }
        [HttpPost]
        public ActionResult EditSampleTestingRecord(RollerRecordInfo rollerrecordinfo)
        {
            if (ModelState.IsValid)
            {
                rollerrecordinfo.CurrentTime = DateTime.Now;
                rollerrecordinfo.TotalTime = TimeSpan.Parse("8:00:00");
                recordrepo.SaveRollerRecordInfo(rollerrecordinfo);
                return RedirectToAction("Index", new { RollerSampleInfoID = rollerrecordinfo.RollerSampleInfoID });
            }
            else
            {
                RollerSampleInfo rollersamleinfo = samplerepo.RollerSampleInfos.FirstOrDefault(a => a.RollerSampleInfoID == rollerrecordinfo.RollerSampleInfoID);
                return View("EditSampleTestingRecord", new RollerRecordInfo() { RollerSampleInfoID = rollerrecordinfo.RollerSampleInfoID, RollerSampleInfo = rollersamleinfo });
            }
        }
        [HttpPost]
        public ActionResult DeleteSampleTestingRecord(int RollerRecordInfoId, int RollerSampleInfoID)
        {
            recordrepo.DeleteRollerRecordInfo(RollerRecordInfoId);
            return RedirectToAction("Index", new { RollerSampleInfoId = RollerSampleInfoID });
        }
    }
}