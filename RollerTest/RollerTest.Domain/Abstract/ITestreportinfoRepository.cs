using RollerTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollerTest.Domain.Abstract
{
    public interface ITestreportinfoRepository
    {
        IQueryable<RollerTestreportInfo> RollerTestreportInfos { get; }
        void SaveRollerTestreportInfo(RollerTestreportInfo rollertestreportinfo);
        //RollerTestreportInfo DeleteRollerTestreportInfo(int rollertestreportinfoID);

    }
}
