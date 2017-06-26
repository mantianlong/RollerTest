﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RollerTest.Domain.Abstract;
using RollerTest.WebUI.IniFiles;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.ExternalProgram;
using System.Threading.Tasks;

namespace RollerTest.WebUI.Controllers
{
    public class TestReportBeforeTestController : Controller
    {

        private IBaseRepository baserepo;
        private ISampleinfoRepository samplerepo;
        private ITestreportinfoRepository testreportrepo;
        public TestReportBeforeTestController(ISampleinfoRepository samplerepo, IBaseRepository baserepo, ITestreportinfoRepository testreportrepo)
        {
            this.samplerepo = samplerepo;
            this.baserepo = baserepo;
            this.testreportrepo = testreportrepo;
        }
        // GET: TestReportBeforeTest
        public ActionResult Index(int RollerSampleInfoId)
        {
            //RollerTestreportInfo rollertestreport = testreportrepo.RollerTestreportInfos.FirstOrDefault(x => x.RollerSampleInfoID == RollerSampleInfoId);
            //return View(rollertestreport);
            return View();
        }
        public ActionResult EditTestReportBeforeTest(RollerTestreportInfo rollertestreportinfo)
        {
            rollertestreportinfo.StartTime = DateTime.Now;
            rollertestreportinfo.EndTime = Convert.ToDateTime("2001/1/1 0:00:00");
            testreportrepo.SaveRollerTestreportInfo(rollertestreportinfo);
            return RedirectToAction("Index", "TestBlock");
        }
    }
}