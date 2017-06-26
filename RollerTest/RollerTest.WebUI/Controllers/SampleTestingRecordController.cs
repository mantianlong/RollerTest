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
                rollerrecordinfos = recordrepo.RollerRecordInfos.Where(x => x.RollerSampleInfoID == RollerSampleInfoId),
            };
            ViewData["RollerSampleInfoId"] = RollerSampleInfoId;
            return View(testingrecordviewModel);
        }
        public ViewResult CreateSampleTestingRecord(int RollerSampleInfoID)
        {
            RollerSampleInfo rollersampleinfo = samplerepo.RollerSampleInfos.FirstOrDefault(a => a.RollerSampleInfoID == RollerSampleInfoID);
            ViewData["SampleID"] = rollersampleinfo.SampleID;
            return View("EditSampleTestingRecord", new RollerRecordInfo() { RollerSampleInfoID = RollerSampleInfoID });
        }
        [HttpGet]
        public ViewResult EditSampleTestingRecord(int RollerRecordInfoID)
        {
            RollerRecordInfo rollerrecordinfo = recordrepo.RollerRecordInfos.FirstOrDefault(a => a.RollerRecordInfoID == RollerRecordInfoID);
            ViewData["RollerSampleInfoID"] = rollerrecordinfo.RollerSampleInfoID;
            ViewData["SampleID"] = rollerrecordinfo.RollerSampleInfo.SampleID;
            return View(rollerrecordinfo);
        }
        [HttpPost]
        public ActionResult EditSampleTestingRecord(RollerRecordInfo rollerrecordinfo)
        {
            if (ModelState.IsValid)
            {
                rollerrecordinfo.CurrentTime = DateTime.Now;
                rollerrecordinfo.TotalTime = "ddd";
                recordrepo.SaveRollerRecordInfo(rollerrecordinfo);
                return RedirectToAction("Index", new { RollerSampleInfoID = rollerrecordinfo.RollerSampleInfoID });
            }
            else
            {
                RollerSampleInfo rollersamleinfo = samplerepo.RollerSampleInfos.FirstOrDefault(a => a.RollerSampleInfoID == rollerrecordinfo.RollerSampleInfoID);
                ViewData["SampleID"] = rollersamleinfo.SampleID;
                return View("EditSampleTestingRecord", new RollerRecordInfo() { RollerSampleInfoID = rollerrecordinfo.RollerSampleInfoID });
            }
        }
        [HttpPost]
        public ActionResult DeleteSampleTestingRecord(int RollerRecordInfoId, int RollerSampleInfoID)
        {
            recordrepo.DeleteRollerRecordInfo(RollerRecordInfoId);
            return RedirectToAction("Index", new { RollerSampleInfoID = RollerSampleInfoID });
        }
    }
}