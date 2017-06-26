using RollerTest.Domain.Abstract;
using RollerTest.Domain.Concrete;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.IniFiles;
using RollerTest.WebUI.ExternalProgram;
using RollerTest.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace RollerTest.WebUI.Controllers
{
    public class ControlBlockController : Controller
    {
        // GET: ControlBlock
        public ActionResult Index()
        {
            IniFileControl inifileControl = IniFileControl.GetInstance();
            CdioControl cdioControl = CdioControl.GetInstance();
            DealControl dealControl = DealControl.GetInstance();
            ViewData["FileTimerState"] = inifileControl.TimerState()==false?"关闭":"开启";
            ViewData["TimerTimerState"] = inifileControl.Timer2State() == false ? "关闭" : "开启";
            ViewData["DealState"] = dealControl.getConnectState()== false ? "断开" : "连接";
            if (cdioControl.InitMoto().Equals("Success"))
            {
                ViewData["CdioInitState"] = "连接";
            }
            else
            {
                CdioCloseTimer();
                ViewData["CdioInitState"] = cdioControl.InitMoto();
            }
            ViewData["CdioTimerState"] = cdioControl.TimerState() == false ? "关闭" : "开启";
            ControlBlockViewModel controlblockviewModel = new ControlBlockViewModel
            {
                readiniModel = inifileControl.ReadInifile()
            };
            return View(controlblockviewModel);
        }
        public void FileOpenTimer()
        {
            IniFileControl inifileControl = IniFileControl.GetInstance();
            inifileControl.OpenTimer();
            Response.Redirect("/ControlBlock/Index");
        }
        public void FileCloseTimer()
        {
            IniFileControl inifileControl = IniFileControl.GetInstance();
            inifileControl.CloseTimer();
            Response.Redirect("/ControlBlock/Index");
        }
        public void CdioOpenTimer()
        {
            CdioControl cdioControl = CdioControl.GetInstance();
            cdioControl.OpenTimer();
            Response.Redirect("/ControlBlock/Index");
        }
        public void CdioCloseTimer()
        {
            CdioControl cdioControl = CdioControl.GetInstance();
            cdioControl.CloseTimer();
            Response.Redirect("/ControlBlock/Index");
        }
        public void TimerOpenTimer()
        {
            IniFileControl inifileControl = IniFileControl.GetInstance();
            inifileControl.OpenTimer2();
            Response.Redirect("/ControlBlock/Index");
        }
        public void TimerCloseTimer()
        {
            IniFileControl inifileControl = IniFileControl.GetInstance();
            inifileControl.CloseTimer2();
            Response.Redirect("/ControlBlock/Index");
        }
        public void MotoRunning()
        {
            CdioControl cdioControl = CdioControl.GetInstance();
            cdioControl.setMotoRunning();
            Response.Redirect("/ControlBlock/Index");
        }
        public void MotoRunningREV()
        {
            CdioControl cdioControl = CdioControl.GetInstance();
            cdioControl.setMotoRunningREV();
            Response.Redirect("/ControlBlock/Index");
        }
        public void MotoStopping()
        {
            CdioControl cdioControl = CdioControl.GetInstance();
            cdioControl.setMotoStopping();
            Response.Redirect("/ControlBlock/Index");
        }
        public void ConnectDeal()
        {
            DealControl dealControl = DealControl.GetInstance();
            dealControl.DealConnect();
            Response.Redirect("/ControlBlock/Index");
        }
        public void CancelDeal()
        {
            DealControl dealControl = DealControl.GetInstance();
            dealControl.DealConnectDis();
            Response.Redirect("/ControlBlock/Index");
        }
        public void OpenTimePort()
        {
            Task task = new Task(()=> {
                IniFileControl.GetInstance().OpenAllTimeSwitch();
            });
            task.Start();
            task.Wait();
            Response.Redirect("/ControlBlock/Index");
        }
        public void CloseTimePort()
        {
            Task task = new Task(() => {
                IniFileControl.GetInstance().CloseAllTimeSwitch();
            });
            task.Start();
            task.Wait();
            Response.Redirect("/ControlBlock/Index");
        }
        public void OpenForcePort()
        {
            DealControl.GetInstance().OpenAllLimitSwtich();
            Response.Redirect("/ControlBlock/Index");
        }
        public void CloseForcePort()
        {
            DealControl.GetInstance().CloseAllLimitSwtich();
            Response.Redirect("/ControlBlock/Index");
        }

    }
}