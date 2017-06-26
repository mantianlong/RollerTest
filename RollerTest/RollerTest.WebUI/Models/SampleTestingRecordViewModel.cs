using RollerTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RollerTest.WebUI.Models
{
    public class SampleTestingRecordViewModel
    {
        public IEnumerable<RollerRecordInfo> rollerrecordinfos { get; set; }
        public int SampleId { get; set; }

    }
}