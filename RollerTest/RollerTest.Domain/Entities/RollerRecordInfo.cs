using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollerTest.Domain.Entities
{
    public class RollerRecordInfo
    {
        [Key]
        public int RollerRecordInfoID { get; set; }
        [ForeignKey("RollerSampleInfo")]
        public int RollerSampleInfoID { get; set; }
        public virtual RollerSampleInfo RollerSampleInfo { get; set; }
        [Required(ErrorMessage = "请输入样品状态")]
        public bool SampleStatus { get; set; }
        public DateTime CurrentTime { get; set; }
        public string TotalTime { get; set; }
        [Required(ErrorMessage = "请输入样品信息")]
        public string RecordInfo { get; set; }
    }
}
