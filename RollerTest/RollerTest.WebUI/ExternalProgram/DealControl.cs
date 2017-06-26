using Hangfire;
using Microsoft.AspNet.SignalR;
using RollerTest.Domain.Abstract;
using RollerTest.Domain.Concrete;
using RollerTest.Domain.Entities;
using RollerTest.WebUI.IniFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;
using System.Windows.Threading;

namespace RollerTest.WebUI.ExternalProgram
{
    public class DealControl
    {
        private static DealControl instance;
        private static readonly object locker = new object();
        private CancellationTokenSource Receivects;
        private CancellationTokenSource Dealcts;
        private CancellationTokenSource Queuects;
        private Task ReceiveTask;
        private Task DealTask;
        private Task QueueTask;
        private bool connectState=false;
        private List<ChannelData> channelList = new List<ChannelData>();
        private List<string> channelNum = new List<string>();
        private RollerLimit rollerLimit = new RollerLimit();
        private FaultData faultdata = new FaultData();
        private EFRecordinfoRepository recordrepo;
        private EFSampleinfoRepository samplerepo;
        private Queue<int> Queue1 = new Queue<int>(5);
        private Queue<int> Queue2 = new Queue<int>(5);
        private Queue<int> Queue3 = new Queue<int>(5);
        private Queue<int> Queue4 = new Queue<int>(5);
        private Queue<int> Queue5 = new Queue<int>(5);
        private Queue<int> Queue6 = new Queue<int>(5);
        private Queue<int> Queue7 = new Queue<int>(5);
        private Queue<int> Queue8 = new Queue<int>(5);
        private Queue<int> Queue9 = new Queue<int>(5);
        private Queue<int> Queue10 = new Queue<int>(5);
        private Queue<int> Queue11 = new Queue<int>(5);
        private Queue<int> Queue12 = new Queue<int>(5);
        private JudgeLimitSwitch judgelimitswitch = new JudgeLimitSwitch();

        private object m_lock = new object();
        private const int ReceiveDataCount = 4048;
        private const int PacketHeadSize = 12;
        private int nNetPos = 0;
        private Socket s;
        //网络缓存数据
        List<byte> m_NetData = new List<byte>();
        //处理缓存数据
        private byte[] m_TmpData;
        //所有待处理的包数据
        private List<PackData> m_lstPackData = new List<PackData>();
        private CdioControl cdioControl = CdioControl.GetInstance();
        private IniFileControl inifileControl = IniFileControl.GetInstance();

        private DealControl()
        {
            recordrepo = new EFRecordinfoRepository();
            samplerepo = new EFSampleinfoRepository();
        }
        public RollerLimit getRollerLimit()
        {
            return rollerLimit;
        }
        public void setRollerLimit(int stationId,int UpLimit,int DnLimit)
        {
            switch (stationId)
            {
                case 1:rollerLimit.DnLimit1 = DnLimit;rollerLimit.UpLimit1 = UpLimit;break;
                case 2: rollerLimit.DnLimit2 = DnLimit; rollerLimit.UpLimit2 = UpLimit; break;
                case 3: rollerLimit.DnLimit3 = DnLimit; rollerLimit.UpLimit3 = UpLimit; break;
                case 4: rollerLimit.DnLimit4 = DnLimit; rollerLimit.UpLimit4 = UpLimit; break;
                case 5: rollerLimit.DnLimit5 = DnLimit; rollerLimit.UpLimit5 = UpLimit; break;
                case 6: rollerLimit.DnLimit6 = DnLimit; rollerLimit.UpLimit6 = UpLimit; break;
                case 7: rollerLimit.DnLimit7 = DnLimit; rollerLimit.UpLimit7 = UpLimit; break;
                case 8: rollerLimit.DnLimit8 = DnLimit; rollerLimit.UpLimit8 = UpLimit; break;
                case 9: rollerLimit.DnLimit9 = DnLimit; rollerLimit.UpLimit9 = UpLimit; break;
                case 10: rollerLimit.DnLimit10 = DnLimit; rollerLimit.UpLimit10 = UpLimit; break;
                case 11: rollerLimit.DnLimit11 = DnLimit; rollerLimit.UpLimit11 = UpLimit; break;
                case 12: rollerLimit.DnLimit12 = DnLimit; rollerLimit.UpLimit12 = UpLimit; break;
                default:break;

            }
        }
        public static DealControl GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new DealControl();
                    }
                }
            }
            return instance;
        }
        public void DealConnect()
        {
            if (connectState == false)
            {
                connectState = this.SocketConnectState();
                Receivects = new CancellationTokenSource();
                Dealcts = new CancellationTokenSource();
                Queuects = new CancellationTokenSource();
                ReceiveTask = new Task(() => ReceiveData(), Receivects.Token);
                DealTask = new Task(() => DealData(), Dealcts.Token);
                QueueTask = new Task(() => QueueData(), Queuects.Token);
                ReceiveTask.Start();
                DealTask.Start();
                QueueTask.Start();
                this.GetSignalInfo();
            }
        }
        public void DealConnectDis()
        {
            if (connectState == true)
            {
                sendExit();
                connectState = false;
            }
        }

        public bool getConnectState()
        {
            return this.connectState;
        }
        //连接用函数
        private bool SocketConnectState()
        {
            string txtIp = "192.168.0.30";
            string txtPort = "5003";
            IPEndPoint removeServer = new IPEndPoint(IPAddress.Parse(txtIp), int.Parse(txtPort));
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(removeServer);
            if (s.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void GetSignalInfo()
        {
            s.Send(DealCmd.GetCmdGetSerialSignal());
            s.Send(DealCmd.GetCmdGetBlockSignal());
            s.Send(DealCmd.GetCmdGetStatSignal());
        }

        //接收数据调用函数

        /// <summary>
        /// 接收数据
        /// </summary>
        private void ReceiveData()
        {
                while (!Receivects.IsCancellationRequested)
                {
                    byte[] recvData = new byte[ReceiveDataCount];
                    try
                    {
                        int nRecvCount = s.Receive(recvData); //接收数据，返回每次接收的字节总数
                        for (int i = 0; i < nRecvCount; i++)
                        {
                            m_NetData.Add(recvData[i]);
                        }
                        ParseBuffer();
                    }
                    catch
                    {
                        Console.WriteLine("ReceiveData Catch>>" + DateTime.Now);
                    }
                }
        }

        /// <summary>
        /// 处理缓存数据
        /// </summary>
        private void ParseBuffer()
        {
            m_TmpData = new byte[m_NetData.Count - nNetPos];
            Array.Copy(m_NetData.ToArray(), nNetPos, m_TmpData, 0, (m_NetData.Count - nNetPos));

            int nAlreadyRecCount = m_TmpData.Length;
            if (nAlreadyRecCount <= 0)
                return;

            // 查找包文头信息
            for (int i = 0; i < m_TmpData.Length - 3; i++)
            {
                if (m_TmpData[i] == 0x55 && m_TmpData[i + 1] == 0xaa && m_TmpData[i + 2] == 0xaa && m_TmpData[i + 3] == 0x55)
                {
                    FindPackHead(nAlreadyRecCount, i);
                }
            }
        }

        /// <summary>
        /// 找到包头标志后，对数据进行处理
        /// </summary>
        /// <param name="nAlreadyRecCount">接收的数据长度</param>
        /// <param name="nDataPointer">第几个字节开始为包头位置</param>
        private void FindPackHead(int nAlreadyRecCount, int nDataPointer)
        {
            // 找到一个数据报头信息
            // 网络包长度小于包头时不是一个完整的包
            if (nAlreadyRecCount - nDataPointer < PacketHeadSize)
                return;

            PackHead m_PackHead = new PackHead();
            SetPackHead(nDataPointer, ref m_PackHead);

            // 判断获得的数据除去表头 是否大于数据长度
            if (nAlreadyRecCount - nDataPointer - PacketHeadSize < m_PackHead.DataLength)
                return;

            //完整的包数据
            byte[] m_ComData = new byte[PacketHeadSize + m_PackHead.DataLength];
            Array.Copy(m_TmpData, nDataPointer, m_ComData, 0, PacketHeadSize + m_PackHead.DataLength);

            PackData pd = new PackData();
            pd.m_Index = m_lstPackData.Count;
            pd.m_Head = m_PackHead;
            pd.m_ByteData = m_ComData.ToList();

            lock (m_lock)
            {
                nNetPos += PacketHeadSize + pd.m_Head.DataLength;
                m_lstPackData.Add(pd);
            }
        }
        /// <summary>
        /// 设置包头
        /// </summary>
        private void SetPackHead(int nDataPointer, ref PackHead m_PackHead)
        {
            m_PackHead.Reset();
            m_PackHead.m_Signature[0] = m_TmpData[nDataPointer];
            m_PackHead.m_Signature[1] = m_TmpData[nDataPointer + 1];
            m_PackHead.m_Signature[2] = m_TmpData[nDataPointer + 2];
            m_PackHead.m_Signature[3] = m_TmpData[nDataPointer + 3];
            m_PackHead.m_Command[0] = m_TmpData[nDataPointer + 4];
            m_PackHead.m_Command[1] = m_TmpData[nDataPointer + 5];
            m_PackHead.m_Command[2] = m_TmpData[nDataPointer + 6];
            m_PackHead.m_Command[3] = m_TmpData[nDataPointer + 7];
            m_PackHead.m_Length[0] = m_TmpData[nDataPointer + 8];
            m_PackHead.m_Length[1] = m_TmpData[nDataPointer + 9];
            m_PackHead.m_Length[2] = m_TmpData[nDataPointer + 10];
            m_PackHead.m_Length[3] = m_TmpData[nDataPointer + 11];

            m_PackHead.DataLength = BitConverter.ToInt32(m_PackHead.m_Length, 0);
            m_PackHead.DataCommand = BitConverter.ToInt32(m_PackHead.m_Command, 0);
        }



        //处理数据调用函数

        /// <summary>
        /// 处理数据
        /// </summary>
        private void DealData()
        {
                while (!Dealcts.IsCancellationRequested)
                {
                    lock (m_lock)
                    {
                        if (m_lstPackData.Count == 0)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            DealComData();
                            //处理完包数据后，将m_NetData中用过的数据清除
                            // 一直到 位置nNetPos 的m_NetData的数据已经用过
                            
                            m_NetData.RemoveRange(0, nNetPos);
                            nNetPos = 0;
                        }
                    }
                }   
        }


        /// <summary>
        /// 处理完整包数据
        /// </summary>
        public void DealComData()
        {
            string txtres = "";
            string res = "";
            List<String> value = new List<string>();
            for (int i = 0; i < m_lstPackData.Count; i++)
            {
                PackData pd = m_lstPackData[i];
                switch (m_lstPackData[i].m_Head.DataCommand)
                {
                    case 128:
                        res = DealCmd.DealGetSignal(pd);
                        if (res != string.Empty)
                        {
                            channelNum = GetChannel(res);
                        }
                        //txtSignalInfo += "Commond 128  信号类型：" + pd.m_SignalType + ";内容:" + res + Environment.NewLine;
                        break;
                    case 123:
                        res = DealCmd.DealTransferDataSignal(pd);
                        txtres += "Commond 123  信号类型：" + pd.m_SignalType + ";信号名称:" + res + Environment.NewLine;
                        break;
                    case 124:
                        res = DealCmd.DealSerialData(pd);
                        string[] sArray = res.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);
                        
                        int j = 0;
                        foreach(var p in channelNum)
                        {
                            ChannelData channeldata = new ChannelData()
                            {
                                channel = p,
                                data =(int) sArray[j].AsFloat()
                            };
                            channelList.Add(channeldata);
                            j++;
                        }
                        break;
                    case 125:
                        res = DealCmd.DealStatData(pd);
                        txtres += "Commond 125Stat  信号数：" + pd.m_SignalCount + ";数据：" + res + Environment.NewLine;
                        break;
                    case 126:
                        res = DealCmd.DealBlockData(pd);
                        txtres += "Commond 126Block  信号字符串长度：" + pd.m_SignalNameLength + ";信号名:" + pd.m_SignalName
                                + ";一个数据中包含几个float：" + pd.m_YCount + ";数据量：" + pd.m_DataCount + ";数据：" + res + Environment.NewLine;
                        break;
                }
            }
            m_lstPackData.Clear();
            if (channelList!=null)
            {
                HandleGetData(channelList);
                channelList.Clear();
            }
        }

        private void HandleGetData(List<ChannelData> info)
        {
            foreach(var p in info)
            {
                SendQueueData(p);
            }
        }

        private void SendQueueData(ChannelData channeldata) {
            switch (channeldata.channel) {
                case "AI1-1-01": if (getLimitSwitch("1#")){ Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("1#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-02": if (getLimitSwitch("2#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("2#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-03": if (getLimitSwitch("3#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("3#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-04": if (getLimitSwitch("4#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("4#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-05": if (getLimitSwitch("5#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("5#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-06": if (getLimitSwitch("6#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("6#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-07": if (getLimitSwitch("7#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("7#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-08": if (getLimitSwitch("8#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("8#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-09": if (getLimitSwitch("9#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("9#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-10": if (getLimitSwitch("10#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("10#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-11": if (getLimitSwitch("11#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("11#")) { Queue1.Enqueue(channeldata.data); }; break;
                case "AI1-1-12": if (getLimitSwitch("12#")) { Send(channeldata.channel, channeldata.data); }; if (getJudgeSwitch("12#")) { Queue1.Enqueue(channeldata.data); }; break;
                default:break;
            }
        }
        private void QueueData() {
            while (!Queuects.IsCancellationRequested)
            {
                if (Queue1.Count > 5){if (JudegLimit(Queue1, "1#")) { setJudgeSwitch("1#", false); };Queue1.Dequeue();}
                if (Queue2.Count > 5) { if (JudegLimit(Queue2, "2#")) { setJudgeSwitch("2#", false); }; Queue2.Dequeue(); }
                if (Queue3.Count > 5) { if (JudegLimit(Queue3, "3#")) { setJudgeSwitch("3#", false); }; Queue3.Dequeue(); }
                if (Queue4.Count > 5) { if (JudegLimit(Queue4, "4#")) { setJudgeSwitch("4#", false); }; Queue4.Dequeue(); }
                if (Queue5.Count > 5) { if (JudegLimit(Queue5, "5#")) { setJudgeSwitch("5#", false); }; Queue5.Dequeue(); }
                if (Queue6.Count > 5) { if (JudegLimit(Queue6, "6#")) { setJudgeSwitch("6#", false); }; Queue6.Dequeue(); }
                if (Queue7.Count > 5) { if (JudegLimit(Queue7, "7#")) { setJudgeSwitch("7#", false); }; Queue7.Dequeue(); }
                if (Queue8.Count > 5) { if (JudegLimit(Queue8, "8#")) { setJudgeSwitch("8#", false); }; Queue8.Dequeue(); }
                if (Queue9.Count > 5) { if (JudegLimit(Queue9, "9#")) { setJudgeSwitch("9#", false); }; Queue9.Dequeue(); }
                if (Queue10.Count > 5) { if (JudegLimit(Queue10, "10#")) { setJudgeSwitch("10#", false); }; Queue10.Dequeue(); }
                if (Queue11.Count > 5) { if (JudegLimit(Queue11, "11#")) { setJudgeSwitch("11#", false); }; Queue11.Dequeue(); }
                if (Queue12.Count > 5) { if (JudegLimit(Queue12, "12#")) { setJudgeSwitch("12#", false); }; Queue12.Dequeue(); }

            }

        }
        private bool JudegLimit(Queue<int> queue,string station)
        {
             if (queue.Max() < rollerLimit.DnLimit1 || queue.Max() > rollerLimit.UpLimit1)
                {
                    faultdata.station = station;
                    faultdata.UpLimit = rollerLimit.UpLimit1.ToString();
                    faultdata.DnLimit = rollerLimit.DnLimit1.ToString();
                    faultdata.Value = queue.Max().ToString();
                    Task faulttask = new Task(() => HandleFaultData());
                    faulttask.Start();
                    faulttask.Wait();
                    return true;
                }   
            return false;
        }

        private void HandleFaultData()
        {
            IniFileControl.GetInstance().CloseRollerTimeSwitch(faultdata.station);
            recordrepo.SaveRollerRecordInfo(new RollerRecordInfo()
            {
                CurrentTime = DateTime.Now,
                SampleStatus = false,
                RollerSampleInfoID = samplerepo.RollerSampleInfos.FirstOrDefault(x => x.RollerBaseStation.Station == faultdata.station && x.State == true).RollerSampleInfoID,
                TotalTime = IniFileControl.GetInstance().getRollerTime(faultdata.station),
                RecordInfo = "上限值：" + faultdata.UpLimit + "|下限值：" + faultdata.DnLimit + "|实际值：" + faultdata.Value
            });
        }
        public void setLimitSwitch(string station,bool state) {
            switch (station)
            {
                case "1#": rollerLimit.LimitSwitch1 = state; break;
                case "2#": rollerLimit.LimitSwitch2 = state; break;
                case "3#": rollerLimit.LimitSwitch3 = state; break;
                case "4#": rollerLimit.LimitSwitch4 = state; break;
                case "5#": rollerLimit.LimitSwitch5 = state; break;
                case "6#": rollerLimit.LimitSwitch6 = state; break;
                case "7#": rollerLimit.LimitSwitch7 = state; break;
                case "8#": rollerLimit.LimitSwitch8 = state; break;
                case "9#": rollerLimit.LimitSwitch9 = state; break;
                case "10#": rollerLimit.LimitSwitch10 = state; break;
                case "11#": rollerLimit.LimitSwitch11 = state; break;
                case "12#": rollerLimit.LimitSwitch12 = state; break;
                default: break;
            }
        }
        public bool getLimitSwitch(string station) {
            bool state=false;
            switch (station)
            {
                case "1#": state=rollerLimit.LimitSwitch1; break;
                case "2#": state = rollerLimit.LimitSwitch2; break;
                case "3#": state = rollerLimit.LimitSwitch3; break;
                case "4#": state = rollerLimit.LimitSwitch4; break;
                case "5#": state = rollerLimit.LimitSwitch5; break;
                case "6#": state = rollerLimit.LimitSwitch6; break;
                case "7#": state = rollerLimit.LimitSwitch7; break;
                case "8#": state = rollerLimit.LimitSwitch8; break;
                case "9#": state = rollerLimit.LimitSwitch9; break;
                case "10#": state = rollerLimit.LimitSwitch10; break;
                case "11#": state = rollerLimit.LimitSwitch11; break;
                case "12#": state = rollerLimit.LimitSwitch12; break;
                default: break;
            }
            return state;
        }
        public void OpenAllLimitSwtich() {
            rollerLimit.LimitSwitch1 = true;
            rollerLimit.LimitSwitch2 = true;
            rollerLimit.LimitSwitch3 = true;
            rollerLimit.LimitSwitch4 = true;
            rollerLimit.LimitSwitch5 = true;
            rollerLimit.LimitSwitch6 = true;
            rollerLimit.LimitSwitch7 = true;
            rollerLimit.LimitSwitch8 = true;
            rollerLimit.LimitSwitch9 = true;
            rollerLimit.LimitSwitch10 = true;
            rollerLimit.LimitSwitch11 = true;
            rollerLimit.LimitSwitch12 = true;
        }
        public void CloseAllLimitSwtich()
        {
            rollerLimit.LimitSwitch1 = false;
            rollerLimit.LimitSwitch2 = false;
            rollerLimit.LimitSwitch3 = false;
            rollerLimit.LimitSwitch4 = false;
            rollerLimit.LimitSwitch5 = false;
            rollerLimit.LimitSwitch6 = false;
            rollerLimit.LimitSwitch7 = false;
            rollerLimit.LimitSwitch8 = false;
            rollerLimit.LimitSwitch9 = false;
            rollerLimit.LimitSwitch10 = false;
            rollerLimit.LimitSwitch11 = false;
            rollerLimit.LimitSwitch12 = false;
        }
        public void setJudgeSwitch(string station, bool state) {
            switch (station)
            {
                case "1#": judgelimitswitch.switch1 = state; break;
                case "2#": judgelimitswitch.switch2 = state; break;
                case "3#": judgelimitswitch.switch3 = state; break;
                case "4#": judgelimitswitch.switch4 = state; break;
                case "5#": judgelimitswitch.switch5 = state; break;
                case "6#": judgelimitswitch.switch6 = state; break;
                case "7#": judgelimitswitch.switch7 = state; break;
                case "8#": judgelimitswitch.switch8 = state; break;
                case "9#": judgelimitswitch.switch9 = state; break;
                case "10#": judgelimitswitch.switch10 = state; break;
                case "11#": judgelimitswitch.switch11 = state; break;
                case "12#": judgelimitswitch.switch12 = state; break;
                default: break;
            }
        }
        public bool getJudgeSwitch(string station)
        {
            bool state = false;
            switch (station)
            {
                case "1#": state = judgelimitswitch.switch1; break;
                case "2#": state = judgelimitswitch.switch2; break;
                case "3#": state = judgelimitswitch.switch3; break;
                case "4#": state = judgelimitswitch.switch4; break;
                case "5#": state = judgelimitswitch.switch5; break;
                case "6#": state = judgelimitswitch.switch6; break;
                case "7#": state = judgelimitswitch.switch7; break;
                case "8#": state = judgelimitswitch.switch8; break;
                case "9#": state = judgelimitswitch.switch9; break;
                case "10#": state = judgelimitswitch.switch10; break;
                case "11#": state = judgelimitswitch.switch11; break;
                case "12#": state = judgelimitswitch.switch12; break;
                default: break;
            }
            return state;
        }


        private List<string> GetChannel(string res)
        {
            List<string> tmpChannel = new List<string>();
            string[] temp = res.Split('|');
            for (int i = 0; i < temp.Length - 1; i++)
            {
                temp[i] = temp[i].Substring(0, 8);
                tmpChannel.Add(temp[i]);
            }
            return tmpChannel;
        }


        // 操作方法

        //发送“服务器退出提示”
        private void sendExit()
        {

            Receivects.Cancel();
            Dealcts.Cancel();
            Queuects.Cancel();
            ClearQueue();
            s.Shutdown(SocketShutdown.Both);
            s.Close();
        }
        private void ClearQueue()
        {
            Queue1.Clear(); Queue2.Clear(); Queue3.Clear(); Queue4.Clear();
            Queue5.Clear(); Queue6.Clear(); Queue7.Clear(); Queue8.Clear();
            Queue9.Clear(); Queue10.Clear(); Queue11.Clear(); Queue12.Clear();
        }

        bool CheckSocket()
        {
            bool res = false;
            if (s == null || s.Connected == false)
            {
                //MessageBox.Show("请点击连接...");
                res = true;
            }
            return res;
        }
        public void Send(string station, int data)
        {
            var dataHub = GlobalHost.ConnectionManager.GetHubContext("dataHub");
            dataHub.Clients.All.addNewDataToPage(station, data);
        }


    }

    public class ChannelData
    {
        public string channel;
        public int data;
    }
    public class FaultData
    {
        public string station;
        public string UpLimit;
        public string DnLimit;
        public string Value;
    }

    public class JudgeLimitSwitch {
        public bool switch1;
        public bool switch2;
        public bool switch3;
        public bool switch4;
        public bool switch5;
        public bool switch6;
        public bool switch7;
        public bool switch8;
        public bool switch9;
        public bool switch10;
        public bool switch11;
        public bool switch12;
    }
    public class PackData
    {
        public int m_Index;
        public PackHead m_Head;
        /// <summary>
        /// 完整包数据
        /// </summary>
        public List<byte> m_ByteData;

        //命令123的相关信息
        public int m_SignalType;
        public List<string> m_lstSignalName;

        //命令124的相关信息
        /// <summary>
        /// 位置
        /// </summary>
        public long m_Position;
        /// <summary>
        /// 数据量 每个通道发过来的数据数量
        /// </summary>
        public int m_DataCount;
        /// <summary>
        /// 通道数
        /// </summary>
        public int m_SignalCount;
        /// <summary>
        /// 发来的时间序列数据
        /// </summary>
        public List<float> m_lstSerialSignalData = new List<float>();

        //命令125的相关信息
        /// <summary>
        /// 发来的统计数据
        /// </summary>
        public List<StatData> m_lstStatSignalData = new List<StatData>();

        //命令126的相关信息
        /// <summary>
        /// 信号字符串长度
        /// </summary>
        public int m_SignalNameLength;
        /// <summary>
        /// 信号名
        /// </summary>
        public string m_SignalName;
        /// <summary>
        /// 信号信息
        /// </summary>
        public string m_SignalInfo = "";
        /// <summary>
        /// 数据量 每个通道发过来的数据数量
        /// </summary>
        public int m_SignalInfoLength;
        /// <summary>
        /// 一个数据中包含几个float
        /// </summary>
        public int m_YCount;
        /// <summary>
        /// 发来的块数据
        /// </summary>
        public List<BlockData> m_lstBlockSignalData = new List<BlockData>();
    }

    public class StatData
    {
        public float m_Time;
        public float m_Data;
    }

    public class BlockData
    {
        public float m_Time;
        /// <summary>
        /// 一个数据中包含1个float 则使用m_Data1,否则用m_Data1，m_Data2
        /// </summary>
        public float m_Data1;
        public float m_Data2;
    }

    public class PackHead
    {
        public byte[] m_Signature;
        public byte[] m_Command;
        public byte[] m_Length;
        public int DataLength;
        public int DataCommand;

        public void Reset()
        {
            m_Signature = new byte[4];
            m_Command = new byte[4];
            m_Length = new byte[4];
        }
    }
}