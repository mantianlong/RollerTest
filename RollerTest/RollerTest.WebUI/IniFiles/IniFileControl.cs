using Microsoft.AspNet.SignalR;
using RollerTest.Domain.Abstract;
using RollerTest.Domain.Concrete;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollerTest.WebUI.IniFiles
{
    public class IniFileControl
    {
        private static IniFileControl instance;
        private static readonly object locker = new object();
        public static System.Timers.Timer controlTimer = new System.Timers.Timer(1000);
        public static System.Timers.Timer controlTimer2= new System.Timers.Timer(1000);
        private RollerTime rollerTime = new RollerTime();
        private TimeSpan ts1, ts2, ts3, ts4, ts5, ts6, ts7, ts8, ts9, ts10, ts11, ts12;
        System.TimeSpan duration = new System.TimeSpan(0, 0, 0, 1);
        private EFBaseRepository baserepo = new EFBaseRepository();
        private IniFileControl() {
            controlTimer.Elapsed += ControlTimer_Elapsed;
            controlTimer.Enabled = false;
            controlTimer2.Elapsed += ControlTimer2_Elapsed;
            controlTimer2.Enabled = false;
        }

        public static IniFileControl GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new IniFileControl();
                    }
                }
            }
            return instance;
        }
        private void ControlTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveRollerTime();
        }
        public bool TimerState()
        {
            return controlTimer.Enabled;
        }
        public void OpenTimer()
        {
            controlTimer.Enabled = true;
        }
        public void CloseTimer()
        {
            controlTimer.Enabled = false;
        }
        public ReadIniModel ReadInifile()
        {
            IniFiles.Profile.LoadProfile();
            ReadIniModel readiniModel = new ReadIniModel
            {
                TestTime1 = Profile.G_TestTime1,
                TestTime2 = Profile.G_TestTime2,
                TestTime3 = Profile.G_TestTime3,
                TestTime4 = Profile.G_TestTime4,
                TestTime5 = Profile.G_TestTime5,
                TestTime6 = Profile.G_TestTime6,
                TestTime7 = Profile.G_TestTime7,
                TestTime8 = Profile.G_TestTime8,
                TestTime9 = Profile.G_TestTime9,
                TestTime10 = Profile.G_TestTime10,
                TestTime11 = Profile.G_TestTime11,
                TestTime12 = Profile.G_TestTime12

            };

            return readiniModel ;
        }
        
        private void ReadRollerTime()
        {
            IniFiles.Profile.LoadProfile();
            rollerTime.TimeData1 = Profile.G_TestTime1;
            rollerTime.TimeData2 = Profile.G_TestTime2;
            rollerTime.TimeData3 = Profile.G_TestTime3;
            rollerTime.TimeData4 = Profile.G_TestTime4;
            rollerTime.TimeData5 = Profile.G_TestTime5;
            rollerTime.TimeData6 = Profile.G_TestTime6;
            rollerTime.TimeData7 = Profile.G_TestTime7;
            rollerTime.TimeData8 = Profile.G_TestTime8;
            rollerTime.TimeData9 = Profile.G_TestTime9;
            rollerTime.TimeData10 = Profile.G_TestTime10;
            rollerTime.TimeData11 = Profile.G_TestTime11;
            rollerTime.TimeData12 = Profile.G_TestTime12;

        }
        private void ReadRollerTimeSwitch()
        {
            rollerTime.TimeDataSwitch1 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "1#").State;
            rollerTime.TimeDataSwitch2 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "2#").State;
            rollerTime.TimeDataSwitch3 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "3#").State;
            rollerTime.TimeDataSwitch4 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "4#").State;
            rollerTime.TimeDataSwitch5 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "5#").State;
            rollerTime.TimeDataSwitch6 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "6#").State;
            rollerTime.TimeDataSwitch7 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "7#").State;
            rollerTime.TimeDataSwitch8 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "8#").State;
            rollerTime.TimeDataSwitch9 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "9#").State;
            rollerTime.TimeDataSwitch10 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "10#").State;
            rollerTime.TimeDataSwitch11 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "11#").State;
            rollerTime.TimeDataSwitch12 = baserepo.RollerBaseStations.FirstOrDefault(x => x.Station == "12#").State;


        }
        private void SaveRollerTime()
        {
            Profile.G_TestTime1 = rollerTime.TimeData1;
            Profile.G_TestTime2 = rollerTime.TimeData2;
            Profile.G_TestTime3 = rollerTime.TimeData3;
            Profile.G_TestTime4 = rollerTime.TimeData4;
            Profile.G_TestTime5 = rollerTime.TimeData5;
            Profile.G_TestTime6 = rollerTime.TimeData6;
            Profile.G_TestTime7 = rollerTime.TimeData7;
            Profile.G_TestTime8 = rollerTime.TimeData8;
            Profile.G_TestTime9 = rollerTime.TimeData9;
            Profile.G_TestTime10 = rollerTime.TimeData10;
            Profile.G_TestTime11 = rollerTime.TimeData11;
            Profile.G_TestTime12 = rollerTime.TimeData12;
            IniFiles.Profile.Saveprofile();
        }

        private void ControlTimer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ts1 = TimeSpan.Parse(rollerTime.TimeData1);                 //累计时间的累加
            ts1 = ts1.Add(duration);
            ts2 = TimeSpan.Parse(rollerTime.TimeData2);
            ts2 = ts2.Add(duration);
            ts3 = TimeSpan.Parse(rollerTime.TimeData3);
            ts3 = ts3.Add(duration);
            ts4 = TimeSpan.Parse(rollerTime.TimeData4);
            ts4 = ts4.Add(duration);
            ts5 = TimeSpan.Parse(rollerTime.TimeData5);
            ts5 = ts5.Add(duration);
            ts6 = TimeSpan.Parse(rollerTime.TimeData6);
            ts6 = ts6.Add(duration);
            ts7 = TimeSpan.Parse(rollerTime.TimeData7);
            ts7 = ts7.Add(duration);
            ts8 = TimeSpan.Parse(rollerTime.TimeData8);
            ts8 = ts8.Add(duration);
            ts9 = TimeSpan.Parse(rollerTime.TimeData9);
            ts9 = ts9.Add(duration);
            ts10 = TimeSpan.Parse(rollerTime.TimeData10);
            ts10 = ts10.Add(duration);
            ts11 = TimeSpan.Parse(rollerTime.TimeData11);
            ts11 = ts11.Add(duration);
            ts12 = TimeSpan.Parse(rollerTime.TimeData12);
            ts12 = ts12.Add(duration);
            if (rollerTime.TimeDataSwitch1)
            {
                rollerTime.TimeData1 = ts1.Days.ToString("D1") + "." + ts1.Hours.ToString("D2") + ":" + ts1.Minutes.ToString("D2") + ":" + ts1.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch2)
            {
                rollerTime.TimeData2 = ts2.Days.ToString("D1") + "." + ts2.Hours.ToString("D2") + ":" + ts2.Minutes.ToString("D2") + ":" + ts2.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch3)
            {
                rollerTime.TimeData3 = ts3.Days.ToString("D1") + "." + ts3.Hours.ToString("D2") + ":" + ts3.Minutes.ToString("D2") + ":" + ts3.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch4)
            {
                rollerTime.TimeData4 = ts4.Days.ToString("D1") + "." + ts4.Hours.ToString("D2") + ":" + ts4.Minutes.ToString("D2") + ":" + ts4.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch5)
            {
                rollerTime.TimeData5 = ts5.Days.ToString("D1") + "." + ts5.Hours.ToString("D2") + ":" + ts5.Minutes.ToString("D2") + ":" + ts5.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch6)
            {
                rollerTime.TimeData6 = ts6.Days.ToString("D1") + "." + ts6.Hours.ToString("D2") + ":" + ts6.Minutes.ToString("D2") + ":" + ts6.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch7)
            {
                rollerTime.TimeData7 = ts7.Days.ToString("D1") + "." + ts7.Hours.ToString("D2") + ":" + ts7.Minutes.ToString("D2") + ":" + ts7.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch8)
            {
                rollerTime.TimeData8 = ts8.Days.ToString("D1") + "." + ts8.Hours.ToString("D2") + ":" + ts8.Minutes.ToString("D2") + ":" + ts8.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch9)
            {
                rollerTime.TimeData9 = ts9.Days.ToString("D1") + "." + ts9.Hours.ToString("D2") + ":" + ts9.Minutes.ToString("D2") + ":" + ts9.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch10)
            {
                rollerTime.TimeData10 = ts10.Days.ToString("D1") + "." + ts10.Hours.ToString("D2") + ":" + ts10.Minutes.ToString("D2") + ":" + ts10.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch11)
            {
                rollerTime.TimeData11 = ts11.Days.ToString("D1") + "." + ts11.Hours.ToString("D2") + ":" + ts11.Minutes.ToString("D2") + ":" + ts11.Seconds.ToString("D2");
            }
            if (rollerTime.TimeDataSwitch12)
            {
                rollerTime.TimeData12 = ts12.Days.ToString("D1") + "." + ts12.Hours.ToString("D2") + ":" + ts12.Minutes.ToString("D2") + ":" + ts12.Seconds.ToString("D2");
            }
            string rollertime = rollerTime.TimeData1 + "|" + rollerTime.TimeData2 + "|" + rollerTime.TimeData3 + "|" + rollerTime.TimeData4 + "|" + rollerTime.TimeData5 + "|" + rollerTime.TimeData6 + "|" + rollerTime.TimeData7 + "|" + rollerTime.TimeData8 + "|" + rollerTime.TimeData9 + "|" + rollerTime.TimeData10 + "|" + rollerTime.TimeData11 + "|" + rollerTime.TimeData12;
            Task sendtask = new Task(() => Send(rollertime));
            sendtask.Start();
            sendtask.Wait();
        }
        public bool Timer2State()
        {
            return controlTimer2.Enabled;
        }
        public void OpenTimer2()
        {
            ReadRollerTime();
            ReadRollerTimeSwitch();
            controlTimer2.Enabled = true;
        }
        public void CloseTimer2()
        {
            controlTimer2.Enabled = false;
        }
        public string getRollerTime(string station)
        {
            string timedata;
            switch (station)
            {
                case "1#": timedata=rollerTime.TimeData1; break;
                case "2#": timedata = rollerTime.TimeData2; break;
                case "3#": timedata = rollerTime.TimeData3; break;
                case "4#": timedata = rollerTime.TimeData4; break;
                case "5#": timedata = rollerTime.TimeData5; break;
                case "6#": timedata = rollerTime.TimeData6; break;
                case "7#": timedata = rollerTime.TimeData7; break;
                case "8#": timedata = rollerTime.TimeData8; break;
                case "9#": timedata = rollerTime.TimeData9; break;
                case "10#": timedata = rollerTime.TimeData10; break;
                case "11#": timedata = rollerTime.TimeData11; break;
                case "12#": timedata = rollerTime.TimeData12; break;
                default: timedata = ""; break;
            }
            return timedata;
        }
        public void OpenRollerTimeSwitch(string station)
        {
            switch (station)
            {
                case "1#":rollerTime.TimeDataSwitch1 = true;break;
                case "2#": rollerTime.TimeDataSwitch2 = true; break;
                case "3#": rollerTime.TimeDataSwitch3 = true; break;
                case "4#": rollerTime.TimeDataSwitch4 = true; break;
                case "5#": rollerTime.TimeDataSwitch5 = true; break;
                case "6#": rollerTime.TimeDataSwitch6 = true; break;
                case "7#": rollerTime.TimeDataSwitch7 = true; break;
                case "8#": rollerTime.TimeDataSwitch8 = true; break;
                case "9#": rollerTime.TimeDataSwitch9 = true; break;
                case "10#": rollerTime.TimeDataSwitch10 = true; break;
                case "11#": rollerTime.TimeDataSwitch11 = true; break;
                case "12#": rollerTime.TimeDataSwitch12 = true; break;
                default:break;
            }
        }
        public void CloseRollerTimeSwitch(string station)
        {
            switch (station)
            {
                case "1#": rollerTime.TimeDataSwitch1 = false; break;
                case "2#": rollerTime.TimeDataSwitch2 = false; break;
                case "3#": rollerTime.TimeDataSwitch3 = false; break;
                case "4#": rollerTime.TimeDataSwitch4 = false; break;
                case "5#": rollerTime.TimeDataSwitch5 = false; break;
                case "6#": rollerTime.TimeDataSwitch6 = false; break;
                case "7#": rollerTime.TimeDataSwitch7 = false; break;
                case "8#": rollerTime.TimeDataSwitch8 = false; break;
                case "9#": rollerTime.TimeDataSwitch9 = false; break;
                case "10#": rollerTime.TimeDataSwitch10 = false; break;
                case "11#": rollerTime.TimeDataSwitch11 = false; break;
                case "12#": rollerTime.TimeDataSwitch12 = false; break;
                default: break;
            }
        }
        public void OpenAllTimeSwitch()
        {
            rollerTime.TimeDataSwitch1 = true;
            rollerTime.TimeDataSwitch2 = true;
            rollerTime.TimeDataSwitch3 = true;
            rollerTime.TimeDataSwitch4 = true;
            rollerTime.TimeDataSwitch5 = true;
            rollerTime.TimeDataSwitch6 = true;
            rollerTime.TimeDataSwitch7 = true;
            rollerTime.TimeDataSwitch8 = true;
            rollerTime.TimeDataSwitch9 = true;
            rollerTime.TimeDataSwitch10 = true;
            rollerTime.TimeDataSwitch11 = true;
            rollerTime.TimeDataSwitch12 = true;
        }
        public void CloseAllTimeSwitch()
        {
            rollerTime.TimeDataSwitch1 = false;
            rollerTime.TimeDataSwitch2 = false;
            rollerTime.TimeDataSwitch3 = false;
            rollerTime.TimeDataSwitch4 = false;
            rollerTime.TimeDataSwitch5 = false;
            rollerTime.TimeDataSwitch6 = false;
            rollerTime.TimeDataSwitch7 = false;
            rollerTime.TimeDataSwitch8 = false;
            rollerTime.TimeDataSwitch9 = false;
            rollerTime.TimeDataSwitch10 = false;
            rollerTime.TimeDataSwitch11 = false;
            rollerTime.TimeDataSwitch12 = false;
        }
        public void CleanRollerTime(string station)
        {
            switch (station)
            {
                case "1#": rollerTime.TimeData1 = "0.00:00:00"; break;
                case "2#": rollerTime.TimeData2 = "0.00:00:00"; break;
                case "3#": rollerTime.TimeData3 = "0.00:00:00"; break;
                case "4#": rollerTime.TimeData4 = "0.00:00:00"; break;
                case "5#": rollerTime.TimeData5 = "0.00:00:00"; break;
                case "6#": rollerTime.TimeData6 = "0.00:00:00"; break;
                case "7#": rollerTime.TimeData7 = "0.00:00:00"; break;
                case "8#": rollerTime.TimeData8 = "0.00:00:00"; break;
                case "9#": rollerTime.TimeData9 = "0.00:00:00"; break;
                case "10#": rollerTime.TimeData10 = "0.00:00:00"; break;
                case "11#": rollerTime.TimeData11 = "0.00:00:00"; break;
                case "12#": rollerTime.TimeData12 = "0.00:00:00"; break;
                default: break;
            }
            SaveRollerTime();
        }
        //public void Send(string station,string time)
        //{
        //    var timeHub = GlobalHost.ConnectionManager.GetHubContext("timeHub");
        //    timeHub.Clients.All.addNewTimeToPage(station, time);
        //}
        public void Send(string rollertime)
        {
            var timeHub = GlobalHost.ConnectionManager.GetHubContext("timeHub");
            timeHub.Clients.All.addNewTimeToPage(rollertime);
        }

    }
}
