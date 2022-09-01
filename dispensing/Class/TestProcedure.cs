using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using dispensingMachine.Infrastracture.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Speech.Synthesis;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.IO;
using System.Diagnostics;

namespace dispensing
{
    class TestProcedure
    {
        public delegate void OutMedicEventHandler(TaskList taskList);
        public event OutMedicEventHandler TaskGetted;
        public static bool[] uIConfirmState = { false, false };

        public delegate void MessageEventHandler(string msg);
        public event MessageEventHandler ShowMsg;

        public delegate void CloseMessageEventHandler();
        public event CloseMessageEventHandler CloseMsg;

        public delegate void LoadingEventHandler(string msg);
        public event LoadingEventHandler StartLoding;

        public delegate void StopLoadingEventHandler();
        public event StopLoadingEventHandler StopLoading;

        public delegate void WaitingEventHandler(string msg);
        public event LoadingEventHandler StartWaiting;

        public delegate void StopWaitingEventHandler();
        public event StopLoadingEventHandler StopWaiting;

        public delegate void HideTakeMedicDetailEventHandler();
        public event HideTakeMedicDetailEventHandler HideTakeMedicDetail;

        public delegate void ShowWaitMessageEventHandler(string msg);
        public event ShowWaitMessageEventHandler ShowWaitMessage;
        #region 变量定义
        private IniFile m_ini = new IniFile("");
        //private sqlite m_sql = new sqlite();
        private serial m_ser = new serial();
        private serial m_ser2 = new serial();
        private serial m_ser3 = new serial();
        private u3Camera m_mvs = new u3Camera();
        //public webCamera m_webcamera = new webCamera();
        PrintService m_print = new PrintService();
        RGPrinter m_rgPrint = new RGPrinter();
        socket m_socket;
        int pcNo = 1;
        string socketIp;
        int socketPort;
        static bool bRunning = false;
        static bool bIsCallBack = false;
        static int nowState;
        static bool bWaiting = false;
        //static bool bConfirm = false;
        static bool bCallBackSuc = false;
        //static bool bSelfEnd = false;
        static bool bOpenDoor = false;
        static bool bFinishCallBack = false;
        IpcChannel tcc = new IpcChannel();
        public dispensing.CustomControls.TakeMedicinePage medicPage;

        //主控板发送指令
        static byte[] send ={0x26,0x26,0x50,0x10,0x00,0x0E,0x01, 0x00,0x00, 0x00, 0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
                     0xcc,0x0D};
        //ini参数内容
        int[] nfactY = new int[15];
        int nTotalRow;
        int nxMax;
        int nFinishSelfLevel = 0;
        int nFinishDoubleY;
        int nFailedY;
        public string printtext;
        string currentPrescription;
        int[] qty;// = new int[split.Length];
        string[] name;// = new string[split.Length];
        int[] level;// = new int[split.Length];
        //int[] qty_last;// = new int[split.Length];
        string[] boxid;// = new string[split.Length];
        string[] medid;// = new string[split.Length];
        int[] provideSucQty;
        TaskList nowTaskList;
        public int nConfirmTimeOut;

        //public EventHandler<GetCodeEventArgs> ScanCodeEventHandler;
        //public bool bNoStock;
        public bool bHavePrescription;
        public bool bProviding;

        SpeechSynthesizer synth = new SpeechSynthesizer();
        string printEndStr = "";
        #endregion
        public void LoadEvent()
        {
            medicPage.SendCode += ManualSendCode;
        }
        public class GetCodeEventArgs : EventArgs
        {
            public String code { get; set; }
        }
        public void SocketOperate(string msg)
        {
            try
            {
                log4net.log.Info("接收到tcp数据：\n" + msg);
                TaskList taskList = new TaskList();
                taskList = JsonConvert.DeserializeObject<TaskList>(msg);//反序列化json
                //if (bRunning && (nowState == 0 || nowState == 1)) return;
                //if (bRunning && !bWaiting)
                //{
                //    return;
                //}
                Task th;
                if (bRunning)
                {
                    if (taskList.type == 7)
                    {
                        log4net.log.Info("接收到另一台机器的发药结果，bIsCallBack置True");
                        bIsCallBack = true;
                        bCallBackSuc = taskList.getSuc;
                        //bWaiting = false;
                        return;
                    }
                }
                else
                {
                    if (taskList.type == 7)
                    {
                        th = new Task(() => ForegroundWait(false));
                        th.Start();
                    }
                }
                if (bWaiting)
                {
                    if (taskList.type == 0 || taskList.type == 1)
                    {
                        bWaiting = false;
                        ForegroundWait(false);
                    }
                    else
                    {
                        return;
                    }
                }




                //if (bRunning && (nowState == 0 || nowState == 1))
                //{
                //    if (taskList.type == 0 || taskList.type == 1)
                //    {
                //        bIsCallBack = true;
                //    }
                //}


                switch (taskList.type)
                {
                    case 0:
                        nowState = 0;
                        bRunning = true;
                        bIsCallBack = false;
                        nowTaskList = taskList;
                        log4net.log.Info("启动前端发药线程");
                        //m_mvs.ReConnect();
                        th = new Task(() => GetForeground(taskList));
                        th.Start();
                        break;
                    case 1:
                        nowState = 1;
                        bRunning = true;
                        bIsCallBack = false;
                        nowTaskList = taskList;
                        //m_mvs.ReConnect();
                        th = new Task(() => GetBackground(taskList));
                        th.Start();
                        break;
                    case 3:
                        nowState = 3;
                        bWaiting = true;
                        th = new Task(() => ForegroundWait(true, taskList.strState));
                        th.Start();
                        break;
                    case 6:
                        log4net.log.Error("接收到报错信息，开始弹窗");
                        th = new Task(() => ShowErrorMsg(taskList.strState));
                        th.Start();
                        break;
                    case 8:
                        log4net.log.Info("bFinishCallBack置true");
                        bFinishCallBack = true;
                        break;
                    case 9:
                        th = new Task(() => AddMedic(taskList));
                        th.Start();
                        break;
                    case 10:
                        AllMoveZero();
                        break;
                    case 11:
                        YControl(taskList);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                log4net.log.Error("Socket解析错误" + e.ToString());
                //throw;
            }
        }
        public void AllMoveZero()
        {
            Task th1 = new Task(YMoveZero);
            Task th2 = new Task(XMoveZero);
            th1.Start();
            th2.Start();
        }
        public void YControl(TaskList taskList)
        {
            if (taskList.yPosition == 0)
            {
                Task th = new Task(YMoveZero);
                th.Start();
            }
            else
            {
                YMoveLevel(taskList.yPosition, false);
            }
        }
        public void AddMedic(TaskList taskList)
        {
            string strState;
            DrugTaskDetail drugTaskDetail = taskList.drugTaskDetailList[0];
            string boxId = drugTaskDetail.boxId;
            string medicId = drugTaskDetail.medicineId;
            int x = drugTaskDetail.x;
            int y = drugTaskDetail.y;
            int z = drugTaskDetail.z;
            int qty = drugTaskDetail.quantity;
            bool bRet = m_ser3.updateboxinfo(boxId, medicId, x, y, z, qty, out strState);
            taskList.getEnd = true;
            if (bRet)
            {
                taskList.getSuc = true;
            }
            else
            {
                taskList.strState = strState;
                taskList.getSuc = false;
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
            string socketStr = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
            m_socket.Send(socketStr);
        }
        public void GetForeground(TaskList taskListIn)
        {
            try
            {
                //============================界面开放=================================
                StopLoading();
                //=====================================================================
                bool endSuc = false;
                bool suc = false;
                string str;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
                bool flag = false;
                DateTime timeOut;
                bool bConfirm = ConfirmMedicine(taskListIn);
                DateTime? TimeNull = null;
                //返回确认结果
                TaskList taskList = new TaskList()
                {
                    code = taskListIn.code,
                    type = 5,
                    strState = String.Empty,
                    taskTime = taskListIn.taskTime,
                    getSuc = false,
                    getEnd = false,
                    confirmState = bConfirm,
                    drugTaskDetailList = taskListIn.drugTaskDetailList,
                    endTime = bConfirm ? TimeNull : DateTime.Now
                };
                str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                m_socket.Send(str);
                if (!bConfirm) return;
                //StartRecord();
                //synth.SpeakAsync("正在准备药品");
                //====================开始发药，屏幕显示正在发药并不允许操作======================
                //StartLoding("正在发药中...");
                //================================================================================
                StartWaiting("正在提取药品...\n若药品较多，可能等待1-2分钟\n请在出药完成后，再扫下个处方码");
                synth.SpeakAsync("正在提取药品，请稍候");
                int i = 0;
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo) i++;
                    if (drug.machine != pcNo) flag = true;
                }
                bOpenDoor = false;
                //Thread.Sleep(1000);
                //synth.SpeakAsync("药品提取中");
                if (i == 0)
                {
                    suc = true;
                    Thread.Sleep(2000);
                    taskList = new TaskList()
                    {
                        code = taskListIn.code,
                        type = taskListIn.type,
                        strState = String.Empty,
                        taskTime = taskListIn.taskTime,
                        getSuc = true,
                        getEnd = true,
                        confirmState = true,
                        drugTaskDetailList = taskListIn.drugTaskDetailList,
                        endTime = DateTime.Now
                    };
                    str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                    goto WaitingOther;
                }
                qty = new int[i];
                name = new string[i];
                level = new int[i];
                //qty_last = new int[i];
                boxid = new string[i];
                medid = new string[i];
                provideSucQty = new int[i];

                i = 0;
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo)
                    {
                        qty[i] = drug.quantity;
                        name[i] = drug.name;
                        level[i] = drug.level;
                        boxid[i] = drug.boxId;
                        medid[i] = drug.medicineId;
                        provideSucQty[i] = 0;
                        i++;
                    }
                }

                //发药
                log4net.log.Info("进入ProvideMedicine发药流程");
                //synth.SpeakAsync("正在出药，请稍候");
                suc = ProvideMedicine(flag);
                int j = 0;
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo)
                    {
                        drug.provideSucQty = provideSucQty[j];
                        j++;
                    }
                }
                taskList = new TaskList()
                {
                    code = taskListIn.code,
                    type = taskListIn.type,
                    strState = suc ? String.Empty : pcNo.ToString() + "号机发药失败",
                    taskTime = taskListIn.taskTime,
                    getSuc = suc,
                    getEnd = true,
                    confirmState = true,
                    drugTaskDetailList = taskListIn.drugTaskDetailList,
                    endTime = DateTime.Now
                };

                str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
            WaitingOther:
                if (flag)
                {
                    log4net.log.Info("另一台机器有药，进入等待 当前状态" + bIsCallBack.ToString());
                    //bWaiting = true;
                    timeOut = DateTime.Now;
                    while (!bIsCallBack)
                    {
                        if (DateTime.Now.Subtract(timeOut).TotalMilliseconds > 300000)
                        {
                            log4net.log.Error("等待另一台机器状态超时，作失败处理");
                            bCallBackSuc = false;
                            break;
                        }
                        Thread.Sleep(200);
                    }
                    log4net.log.Info("已确认另一台机器的发药结果");
                    endSuc = suc && bCallBackSuc;
                }
                else
                {
                    endSuc = suc;
                }

                m_socket.Send(str);


                //==========================进入结束流程并等待========================
                if (endSuc)//成功
                {
                    StopWaiting();
                    Thread.Sleep(10);
                    StartWaiting("正在出药，请稍候");
                    synth.SpeakAsync("正在出药，请稍候");
                    Task<bool> task1 = new Task<bool>(conveyorbelt);
                    if (i != 0)
                    {
                        YMoveLevel(nFinishSelfLevel, false);
                        task1.Start();
                        //conveyorbelt();
                        //YMoveZero();
                        //Task task2 = new Task(YMoveZero);
                        //task2.Start();
                    }
                    //==========================屏幕显示请取药==================================
                    Thread.Sleep(3000);
                    StopWaiting();
                    //StopLoading();
                    synth.SpeakAsync("出药完成，请取走凭条和药品");
                    CloseMsg();
                    ShowMsg("出药完成，请取走药品和凭条\n请核对药品种类和数量，如有\n疑问，请联系护士站人工帮助");
                    HideTakeMedicDetail();
                    GetRecipeByCode("");
                    log4net.log.Info("打印单子内容：\n" + printtext);
                    PrintPaper();

                    taskList.type = 8;
                    str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                    bFinishCallBack = false;
                    log4net.log.Info("反馈皮带开始运动数据");
                    m_socket.Send(str);
                    if (i != 0)
                    {
                        task1.Wait();
                        YMoveZero();
                    }
                    //Thread.Sleep(6000);
                    timeOut = DateTime.Now;
                    while (!bFinishCallBack)
                    {
                        if (DateTime.Now.Subtract(timeOut).TotalMilliseconds > 120000)
                        {
                            log4net.log.Error("等待主机结束消息超时");
                            break;
                        }
                        Thread.Sleep(10);
                    }

                    taskList.type = 12;
                    str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                    log4net.log.Info("反馈运行完毕数据");
                    m_socket.Send(str);
                    bRunning = false;
                    //MessageBox.Show("请取药1");

                    //==========================================================================
                }
                else//失败
                {
                    if (i != 0)
                    {
                        YMoveFail();
                        conveyorbelt();
                        YMoveZero();
                        YMoveZero();
                    }
                    taskList.type = 8;
                    str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                    log4net.log.Info("反馈皮带运动完毕数据");
                    m_socket.Send(str);
                    //StopLoading();
                    StopWaiting();
                    synth.SpeakAsync("发药失败，请联系药房");
                    CloseMsg();
                    ShowMsg("发药失败\n请联系药房");
                    HideTakeMedicDetail();
                    taskList.type = 12;
                    str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                    log4net.log.Info("反馈运行完毕数据");
                    m_socket.Send(str);
                    bRunning = false;
                }
                //====================================================================
            }
            catch (Exception e)
            {

                log4net.log.Error(e.ToString());
            }
            finally
            {
                //StopRecord();
                ClearMemory();
            }

        }
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        void GetBackground(TaskList taskListIn)
        {
            try
            {
                DateTime timeOut;
                bool suc;
                string str;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
                int i = 0;
                //====================开始发药，屏幕显示正在发药并不允许操作======================
                StartLoding("此机器正在后台发药，请稍后");
                //================================================================================
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo) i++;
                }
                qty = new int[i];
                name = new string[i];
                level = new int[i];
                boxid = new string[i];
                medid = new string[i];
                provideSucQty = new int[i];
                i = 0;
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo)
                    {
                        qty[i] = drug.quantity;
                        name[i] = drug.name;
                        level[i] = drug.level;
                        boxid[i] = drug.boxId;
                        medid[i] = drug.medicineId;
                        provideSucQty[i] = 0;
                        i++;
                    }
                }
                suc = ProvideMedicine(true);
                int j = 0;
                foreach (var drug in taskListIn.drugTaskDetailList)
                {
                    if (drug.machine == pcNo)
                    {
                        drug.provideSucQty = provideSucQty[j];
                        j++;
                    }
                }
                TaskList taskList = new TaskList()
                {
                    code = taskListIn.code,
                    type = taskListIn.type,
                    strState = suc ? String.Empty : pcNo.ToString() + "号机发药失败",
                    taskTime = taskListIn.taskTime,
                    getSuc = suc,
                    getEnd = true,
                    confirmState = true,
                    drugTaskDetailList = taskListIn.drugTaskDetailList,
                    endTime = DateTime.Now
                };
                //bIsCallBack = false;
                str = string.Empty;
                str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                log4net.log.Info("后台发药完毕，发送数据" + str);
                m_socket.Send(str);
                timeOut = DateTime.Now;
                while (!bIsCallBack)
                {
                    if (DateTime.Now.Subtract(timeOut).TotalMilliseconds > 300000)
                    {
                        log4net.log.Error("等待另一台机器状态超时，作失败处理");
                        bCallBackSuc = false;
                        break;
                    }
                    Thread.Sleep(200);
                }
                Task<bool> task1 = new Task<bool>(conveyorbelt);
                if (suc && bCallBackSuc)
                {
                    log4net.log.Info("双机后台发药成功");
                    YMoveLevel(nFinishSelfLevel, true);
                    task1.Start();
                    //conveyorbelt();
                    //YMoveZero();
                    //Task task2 = new Task(YMoveZero);
                    //task2.Start();
                }
                else
                {
                    log4net.log.Info("双机后台发药失败");
                    YMoveFail();
                    task1.Start();
                }
                taskList.type = 8;
                str = string.Empty;
                str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                log4net.log.Info("bFinishCallBack置false");
                bFinishCallBack = false;
                log4net.log.Info("反馈完成数据\n" + str);
                m_socket.Send(str);
                if (i != 0)
                {
                    log4net.log.Info("等待皮带运动完毕");
                    task1.Wait();
                    log4net.log.Info("Y轴开始回0");
                    YMoveZero();
                }
                taskList.type = 12;
                str = string.Empty;
                str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                log4net.log.Info("反馈后台发药结束消息\n" + str);
                m_socket.Send(str);
                log4net.log.Info("等待主机返回完成消息");
                timeOut = DateTime.Now;
                while (!bFinishCallBack)
                {
                    if (DateTime.Now.Subtract(timeOut).TotalMilliseconds > 120000)
                    {
                        log4net.log.Error("等待主机结束消息超时");
                        break;
                    }
                    Thread.Sleep(10);
                }
                log4net.log.Info("主机已返回完成消息");
                bRunning = false;
            }
            catch (Exception e)
            {

                log4net.log.Error(e.ToString());
            }
            finally
            {
                //GetMedicEnd(suc, taskListIn.type);
                //====================结束发药，界面允许操作======================
                log4net.log.Info("后台发药结束，关闭loading界面");
                StopLoading();
                ClearMemory();
                //================================================================
            }


        }
        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="msg">信息文本</param>
        void ShowErrorMsg(string msg)
        {
            StopLoading();
            //==================屏幕显示错误信息=====================
            CloseMsg();
            ShowMsg(msg);
            //如无操作n秒后自动消失
            //=======================================================
        }
        /// <summary>
        /// 用户手动输入条码
        /// </summary>
        /// <param name="code">条码</param>
        void ManualSendCode(string code)
        {
            //=============等待主机返回数据并不允许操作==============
            StartLoding("正在通讯");
            //=======================================================
            TaskList taskList = new TaskList()
            {
                code = code,
                type = 4,
                strState = String.Empty,
                taskTime = DateTime.Now,
                getSuc = false,
                getEnd = false,
                confirmState = true,
                drugTaskDetailList = null,
                endTime = null
            };
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
            string str = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
            m_socket.Send(str);
        }
        bool ConfirmMedicine(TaskList taskList)
        {
            bool bConfirm = false;
            for (int i = 0; i < 2; i++)
            {
                uIConfirmState[i] = false;
            }
            //发布事件
            TaskGetted(taskList);
            //log4net.log.Info("开始检测UI确认变量状态");
            DateTime dt1 = DateTime.Now;
            bool timeOut = false;
            while (!uIConfirmState[0])
            {
                Thread.Sleep(10);
                if ((DateTime.Now.Subtract(dt1).TotalMilliseconds >= nConfirmTimeOut)) { timeOut = true; break; }
            }
            //log4net.log.Info("状态已确认");
            if (timeOut)
            {
                bConfirm = false;
                HideTakeMedicDetail();
                ShowWaitMessage("确认超时");
            }
            else
            {
                bConfirm = uIConfirmState[1];
            }
            return bConfirm;
        }
        /// <summary>
        /// 前台等待
        /// </summary>
        /// <param name="state">开始/结束等待</param>
        /// <param name="msg">屏幕上显示的信息</param>
        void ForegroundWait(bool state, string msg = "")
        {
            if (state)
            {
                //显示msg并禁止操作
                StartLoding("机器被占用，请稍后");
                //DateTime dt = DateTime.Now;
                //while (true)
                //{
                //    if (DateTime.Now.Subtract(dt).TotalMilliseconds > 600000)
                //    {
                //        StopLoading();
                //        break;
                //    }
                //}
            }
            else
            {
                StopLoading();
                //结束等待，允许操作
            }
        }
        public void InitAll()
        {
            try
            {
                socketIp = m_ini.IniReadStringValue("Config", "SocketIP");
                socketPort = m_ini.IniReadIntValue("Config", "SocketPort");
                nFinishDoubleY = m_ini.IniReadIntValue("case", "factY0_D");
                nFailedY = m_ini.IniReadIntValue("case", "factY0_F");
                nConfirmTimeOut = m_ini.IniReadIntValue("Config", "ConfirmTimeOut") * 1000;
                printEndStr = StringUtility.BreakLongString(m_ini.IniReadStringValue("Config", "PrintEndStr"), 34);
                MessageBox.Show(socketIp + ":" + socketPort);
                m_socket = new socket(socketIp, socketPort);
                m_socket.MsgReceived += SocketOperate;
                synth.SetOutputToDefaultAudioDevice();
                log4net.log.Info("程序打开初始化设备");
                System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\images");
                System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\log");
                System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\shm");
                System.IO.Directory.CreateDirectory(@"C:\temp");
                m_mvs.camID = m_ini.IniReadStringValue("Config", "CamID");
                m_mvs.bSaveImage = Convert.ToBoolean(m_ini.IniReadIntValue("Config", "SaveImage"));

                //KillRecordApp();
                //Thread.Sleep(10);
                //启动录像附属程序
                //ChannelServices.RegisterChannel(tcc, false);
                //Thread checkRecordAppTh = new Thread(CheckRecordApp);
                //checkRecordAppTh.Start();


                bProviding = false;

                pcNo = m_ini.IniReadIntValue("Config", "PcNo");

                //初始化打印机
                if (m_rgPrint.RG_OpenPrinter() == 0)
                {
                    MessageBox.Show("处方打印机RG-K532打开失败！");
                }

                //初始化数据库
                //if (false == m_sql.Init(0))
                //{
                //    MessageBox.Show("数据库打开失败！");
                //}

                //初始化读取处方用扫码枪端口
                //int nPort = m_ini.IniReadIntValue("Config", "PortScan");
                //m_ser.strPortName = "COM" + Convert.ToString(nPort);
                //m_ser.OpenPort();
                //if (false == m_ser.sp1.IsOpen)
                //{
                //    //MessageBox.Show("扫码枪端口打开失败：" + m_ser.strPortName);
                //    log4net.log.Info("扫码枪端口打开失败：" + m_ser.strPortName);
                //}
                //else
                //{
                //    Thread th = new Thread(ReadPrescriptionCode);
                //    th.IsBackground = true;
                //    th.Start();
                //}

                //初始化主控制板端口
                int nPortMain = m_ini.IniReadIntValue("Config", "PortMain");
                m_ser2.strPortName = "COM" + Convert.ToString(nPortMain);
                m_ser2.OpenPort();
                if (false == m_ser2.sp1.IsOpen)
                {
                    MessageBox.Show("主控制板端口打开失败：" + m_ser2.strPortName + "\n请检查串口和是否有残留进程后重启程序");
                    log4net.log.Info("主控制板端口打开失败：" + m_ser2.strPortName);
                    System.Environment.Exit(0);
                }
                //初始化药盒控制串口
                int nPort3 = m_ini.IniReadIntValue("Config", "PortCase");
                m_ser3.strPortName = "COM" + Convert.ToString(nPort3);
                m_ser3.iPortBaudRate = 2400;
                m_ser3.OpenPort();
                if (false == m_ser3.sp1.IsOpen)
                {
                    MessageBox.Show("药盒控制端口打开失败：" + m_ser3.strPortName + "\n请检查串口和是否有残留进程后重启程序");
                    log4net.log.Info("药盒控制端口打开失败：" + m_ser3.strPortName);
                    System.Environment.Exit(0);
                }

                //初始化usb3.0工业相机
                if (0 == m_mvs.MvsEmulateCamera(out int camIndex))
                {
                    if (camIndex == -1)
                    {
                        MessageBox.Show("未找到匹配的相机，请核对配置文件相机ID");
                        System.Environment.Exit(0);
                        return;
                    }
                    m_mvs.MvsOpenCamera(camIndex);
                }
                else
                {
                    MessageBox.Show("未找到任何mvs相机");
                    System.Environment.Exit(0);
                    this.InsertWarningRecord("核对相机异常");
                }

                //初始化网络摄像头
                //if (false == m_webcamera.OpenCamera())
                //{
                //    MessageBox.Show("未发现任何网络摄像头");
                //    //this.InsertWarningRecord("药品有无网络相机异常");
                //}
                //m_webcamera.sDeviceName = m_ini.IniReadStringValue("Config", "CameraName");
                //m_webcamera.nAreaJudge = m_ini.IniReadIntValue("Config", "AreaJudge");

                //ini参数内容
                m_mvs.bSaveLast = m_ini.IniReadIntValue("Config", "SaveImage");
                m_mvs.nQRPosition = m_ini.IniReadIntValue("Config", "QRPosition");
                m_mvs.nModelPosition = m_ini.IniReadIntValue("Config", "ModelPosition");
                nxMax = m_ini.IniReadIntValue("Config", "xMax");
                nTotalRow = m_ini.IniReadIntValue("case", "row");
                for (int i = 0; i < nTotalRow + 1; i++)
                {
                    nfactY[i] = m_ini.IniReadIntValue("case", "factY" + i.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                log4net.log.Error(e.ToString());
                //throw;
            }

        }
        public void StartRecord()
        {
            try
            {
                MyProcessSendObject myObj = (MyProcessSendObject)Activator.GetObject(typeof(MyProcessSendObject), "ipc://RecordChannal/RecordObj");
                myObj.Add("StartRecord|" + nowTaskList.code + "_" + nowTaskList.taskTime.ToString("yyyyMMdd-HHmmss") + ".mp4");
            }
            catch (Exception ex)
            {
                log4net.log.Error("录像指令发送失败：" + ex.Message);
            }

        }
        public void StopRecord()
        {
            try
            {
                MyProcessSendObject myObj = (MyProcessSendObject)Activator.GetObject(typeof(MyProcessSendObject), "ipc://RecordChannal/RecordObj");
                myObj.Add("StopRecord");
            }
            catch (Exception ex)
            {
                log4net.log.Error("停止录像指令发送失败");
            }
        }

        public void ReleaseAll()
        {
            ClosePorts();
            m_rgPrint.RG_ClosePrinter();
            //m_sql.Close();
            m_mvs.MvsCloseCamera();
            //m_webcamera.CloseWebCamera();
            //m_webcamera.CloseCamera();
        }
        public void ClosePorts()
        {
            //m_ser.ClosePort();
            m_ser2.ClosePort();
            m_ser3.ClosePort();
        }
        //void ReadPrescriptionCode()
        //{
        //    bool _keepReading = true;
        //    string prescription = "";
        //    while (_keepReading)
        //    {
        //        if (m_ser.sp1.IsOpen)
        //        {
        //            int count = m_ser.sp1.BytesToRead;
        //            if (count > 0)
        //            {
        //                byte[] readBuffer = new byte[count];
        //                try
        //                {
        //                    m_ser.sp1.Read(readBuffer, 0, count);
        //                    prescription = prescription + System.Text.Encoding.Default.GetString(readBuffer);
        //                    bool end = false;
        //                    foreach (var item in readBuffer)
        //                    {
        //                        if (item == 0x0D)
        //                        {
        //                            end = true;
        //                            prescription = prescription.Trim();

        //                            break;
        //                        }
        //                    }
        //                    if (true == end)
        //                    {
        //                        //读完处方信息

        //                        if (ScanCodeEventHandler != null)
        //                            ScanCodeEventHandler.Invoke(this, new GetCodeEventArgs()
        //                            {
        //                                code = prescription
        //                            });

        //                        //开始跑流程

        //                        //重置变量
        //                        prescription = "";
        //                        m_ser.sp1.DiscardOutBuffer();
        //                        m_ser.sp1.DiscardInBuffer();
        //                    }
        //                }
        //                catch (TimeoutException)
        //                {
        //                }
        //            }
        //        }
        //        Thread.Sleep(100);
        //    }
        //}

        public int CheckPrintStatus()
        {
            return m_rgPrint.RG_QueryPrintStatus();
        }
        public Recipe GetRecipeByCode(string code)
        {
            Recipe recipe = new Recipe()
            {
                Patient = "张三",
                Code = nowTaskList.code,
                Age = 30,
                Gender = Gender.male,
                Hospital = "北京大学深圳医院",
                Date = nowTaskList.taskTime,
                Medicines = new List<MedicineWithQty>()
            };


            log4net.log.Info("扫到处方名称: " + nowTaskList.code);
            currentPrescription = nowTaskList.code;
            printtext = "";
            printtext += "     自助药房用药指导单\n";
            printtext += "姓名：" + nowTaskList.patName + "\n" + "处方号：" + nowTaskList.code + "\n";
            printtext += "处方时间：" + nowTaskList.createTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + "取药时间：" + nowTaskList.taskTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            printtext += "----------------------------------------------------\n";
            printtext += "药品名\t\t" + "数量(盒)\n";
            printtext += "----------------------------------------------------\n";


            bHavePrescription = false;
            //string content = m_ini.IniReadStringValue("Prescription", code);
            //if (content.Length < 5)
            //{
            //    return recipe;
            //}
            //else
            //{
            //    bHavePrescription = true;
            //}

            //string[] split = content.Split(';');

            //qty = new int[split.Length];
            //name = new string[split.Length];
            //level = new int[split.Length];
            //qty_last = new int[split.Length];
            //boxid = new string[split.Length];
            //medid = new string[split.Length];

            //int i = 0;
            string medcinetext = "";
            foreach (var item in nowTaskList.drugTaskDetailList)  //所有的处方信息
            {
                medcinetext = medcinetext + item.name + "\t\t" + item.quantity + "\n";
                medcinetext += "用法：" + item.administration + "，" + item.frequency + "，" + "每次：" + Convert.ToDouble(item.dosage).ToString() + item.dosageUnit + "\n";
                recipe.Medicines.Add(
                    new MedicineWithQty()
                    {
                        Id = item.medicineId,
                        Name = item.name,
                        Quantity = item.quantity,
                        Unit = item.unit,
                        Image = System.Environment.CurrentDirectory + "\\shm\\" + item.medicineId + ".jpg"
                    });
            }
            printtext += medcinetext;
            printtext += "----------------------------------------------------\n";
            //处方排序
            //冒泡排序
            //int k, j, temp;
            //string sTemp;
            //for (j = 0; j < i - 1; j++)
            //{
            //    for (k = 0; k < i - 1 - j; k++)
            //    {
            //        if (level[k] < level[k + 1])  //从大到小排序
            //        {
            //            //level
            //            temp = level[k];
            //            level[k] = level[k + 1];
            //            level[k + 1] = temp;
            //            //qty
            //            temp = qty[k];
            //            qty[k] = qty[k + 1];
            //            qty[k + 1] = temp;
            //            //qty_last
            //            temp = qty_last[k];
            //            qty_last[k] = qty_last[k + 1];
            //            qty_last[k + 1] = temp;
            //            //
            //            sTemp = boxid[k];
            //            boxid[k] = boxid[k + 1];
            //            boxid[k + 1] = sTemp;
            //            //
            //            sTemp = medid[k];
            //            medid[k] = medid[k + 1];
            //            medid[k + 1] = sTemp;
            //            //
            //            sTemp = name[k];
            //            name[k] = name[k + 1];
            //            name[k + 1] = sTemp;
            //        }
            //    }
            //}
            printtext += printEndStr;

            return recipe;
        }

        public void InsertWarningRecord(string reason)
        {
            //m_sql.InsertAbnormalRecord(currentPrescription, reason, "未完成");
        }
        //public void ProvideMedcProcedure2(bool bDouble)
        //{

        //    DateTime startTime = DateTime.Now;
        //    string printtext = "";
        //    printtext += "     24小时药房取药单\n";
        //    printtext += "" + "\t" + DateTime.Now.ToString("yyyy-MM-dd") + "\n";
        //    printtext += "----------------------------------------------------\n";
        //    printtext += "药品名\t\t" + "数量\n";
        //    printtext += "----------------------------------------------------\n";
        //    //printtext += medcinetext;
        //    //控制电机移动
        //    byte[] rcv;
        //    int i = 0;
        //    bool success = false;
        //    //开始发药
        //    log4net.log.Info("开始发药");
        //    //这里提示“正在出药，请稍后”

        //    foreach (var item in level)
        //    {
        //        //控制Y轴电机运动到第几层
        //        log4net.log.Info("Y轴准备运动");
        //        YMoveLevel(item, false);
        //        log4net.log.Info("Y轴运动到第" + item.ToString() + "层");
        //        //控制X轴电机开始找药
        //        m_mvs.ClearAllShapeModel();
        //        m_mvs.ReadAllShapeModel(medid[i]);
        //        m_mvs.strQrcode = boxid[i];

        //        send[10] = Convert.ToByte(nxMax / 256);
        //        send[11] = Convert.ToByte(nxMax % 256);

        //        //X轴运动
        //        log4net.log.Info("X轴开始运动");
        //        send[12] = 0x01;
        //        send[20] = m_ser2.crc_xor(send, 20);
        //        m_ser2.SendCommandWithReceive(send, out rcv);

        //        bool bfind = false;
        //        int timeout = 5000;
        //        DateTime dt1 = DateTime.Now;
        //        while (DateTime.Now.Subtract(dt1).TotalMilliseconds < timeout)
        //        {
        //            Thread.Sleep(100);
        //            if (true == m_mvs.FindMedicineProcedure())
        //            {
        //                //找到药退出循环
        //                bfind = true;
        //                break;
        //            }
        //        }
        //        //找到药停止X运动
        //        send[12] = 0x00;
        //        send[20] = m_ser2.crc_xor(send, 20);
        //        log4net.log.Info("发送X轴停止指令");
        //        m_ser2.SendCommandWithReceive(send, out rcv);
        //        //判断是否超时
        //        if ((DateTime.Now.Subtract(dt1).TotalMilliseconds >= timeout))
        //        {
        //            //mvs.m_bGrabbing = false;
        //            log4net.log.Info("未找到匹配药槽二维码：" + medid[i] + boxid[i]);
        //            goto End1;
        //        }
        //        else
        //        {
        //            if (bfind == false)
        //            {
        //                log4net.log.Info("未找到匹配药品：" + medid[i]);
        //                //mvs.m_bGrabbing = false;
        //                goto End1;
        //            }
        //            else
        //            {
        //                //开始发药,一个一个发
        //                for (int kk = 0; kk < qty[i]; kk++)
        //                {

        //                    bool bCanProvide = false;
        //                    //不是第一个，拍照重新发
        //                    if (kk == 0)
        //                    {
        //                        bCanProvide = true;
        //                    }
        //                    else
        //                    {
        //                        Thread.Sleep(500);
        //                        dt1 = DateTime.Now;

        //                        //fm1.mvs.bFindMatch = false;
        //                        //fm1.mvs.bTesting = true;
        //                        //fm1.mvs.m_bGrabbing = true;
        //                        while (DateTime.Now.Subtract(dt1).TotalMilliseconds < 5000)
        //                        {
        //                            if (true == m_mvs.FindMedicineProcedure())
        //                            {
        //                                //找到药退出循环
        //                                bCanProvide = true;
        //                                break;
        //                            }
        //                            Thread.Sleep(200);
        //                        }
        //                        //fm1.mvs.bTesting = false;
        //                        //fm1.mvs.m_bGrabbing = false;
        //                    }
        //                    //第一个，直接发药
        //                    if (bCanProvide == true)
        //                    {
        //                        log4net.log.Info($"开始发药品{medid[i]}：第{(kk + 1)}盒，共{qty[i]}盒");
        //                        if (false == StartProvideMedc(qty[i], boxid[i], medid[i], "", "", i))
        //                        {
        //                            log4net.log.Info("药品发放失败：" + medid[i]);
        //                            goto End1;
        //                        }
        //                        else
        //                        {
        //                            //qty_last[i]--;
        //                            log4net.log.Info("药品发放成功：" + medid[i]);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        log4net.log.Info("未找到匹配药品：" + medid[i]);
        //                        //fm1.mvs.m_bGrabbing = false;
        //                        goto End1;
        //                    }
        //                }

        //            }
        //        }
        //        //X轴回零
        //        XMoveZero();
        //        //下一个
        //        i++;
        //    }
        //    success = true;
        //End1:
        //    //fm1.mvs.bTesting = false;
        //    //fm1.mvs.m_bGrabbing = false;
        //    printtext = printtext + "----------------------------------------------------\n";

        //    //关闭发药名称显示
        //    log4net.log.Info("处方发药结束");
        //    //发药结束，Y到第0层
        //    YMoveLevel(nFinishSelfLevel, bDouble);
        //    //控制皮带转动3秒
        //    conveyorbelt();
        //    //成功则取药
        //    if (true == success)
        //    {
        //        //提示取药

        //        //打开取药门
        //        send[15] = m_ser2.set_bit(send[15], 4, true);
        //        send[20] = m_ser2.crc_xor(send, 20);
        //        m_ser2.SendCommandWithReceive(send, out rcv);

        //        //等待取药
        //        startTime = DateTime.Now;
        //        bool removed = false;
        //        m_webcamera.nFailedLoop = 0;
        //        while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 30000)
        //        {
        //            Thread.Sleep(1000);
        //            if (false == m_webcamera.CheckIfhaveBox())
        //            {
        //                removed = true;
        //                break;
        //            }

        //        }
        //        //关闭药门
        //        send[15] = m_ser2.set_bit(send[15], 4, false);
        //        send[20] = m_ser2.crc_xor(send, 20);
        //        m_ser2.SendCommandWithReceive(send, out rcv);
        //        if (removed == false)
        //        {
        //            log4net.log.Info("取药超时回收");
        //            printtext = printtext + "取药品失败，有药品未取，请联系医护人员。";
        //            //取药超时回收
        //            StartReserve();
        //        }
        //        else
        //        {
        //            //补充打印内容
        //            printtext = printtext + "实际出药数量与处方预期出\n药数量不符，请联系医院药房\n如您所取药品与目录不符或\n有疑问，请暂勿使用";
        //        }
        //    }
        //    else
        //    {
        //        //不成功则回收
        //        log4net.log.Info("药品未全部发放成功，回收");
        //        StartReserve();
        //        printtext = printtext + "发药品失败！";
        //        //X轴回零
        //        XMoveZero();
        //    }

        //    //打印内容
        //    printtext = printtext + "\n\n" + ".";

        //    m_print.StartPrint(printtext, "txt");
        //    //m_rgPrint.RG_PrintText(printtext);

        //    //Y轴回零
        //    YMoveZero();

        //    log4net.log.Info("整个处方流程处理完成");
        //    //显示发放结束
        //}
        public bool ProvideMedicine(bool bDouble)
        {
            //控制电机移动
            byte[] rcv;
            int i = 0;
            bool success = false;
            //开始发药
            log4net.log.Info("开始发药");
            bProviding = true;

            foreach (var item in level)
            {
                //StartWaiting("正在准备药品，请稍候");
                //synth.SpeakAsync("正在准备药品，请稍候");
                //控制Y轴电机运动到第几层
                log4net.log.Info("Y轴开始运动到第" + item.ToString() + "层");
                YMoveLevel(item, false);
                //控制X轴电机开始找药

                m_mvs.ClearAllShapeModel();
                log4net.log.Info("读取模板文件：" + medid[i]);
                m_mvs.ReadAllShapeModel(medid[i]);
                log4net.log.Info("识别药盒编号：" + boxid[i]);
                m_mvs.strQrcode = boxid[i];

                send[10] = Convert.ToByte(nxMax / 256);
                send[11] = Convert.ToByte(nxMax % 256);

                //X轴运动
                log4net.log.Info("X轴开始运动");
                send[12] = 0x01;
                send[20] = m_ser2.crc_xor(send, 20);


                bool bfind = false;
                int timeout = 10000;
                DateTime dt1 = DateTime.Now;
                bool bFirst = true;
                while (DateTime.Now.Subtract(dt1).TotalMilliseconds < timeout)//timeout
                {
                    Thread.Sleep(1);
                    if (true == m_mvs.FindMedicineProcedure())
                    {
                        //找到药退出循环
                        bfind = true;
                        break;
                    }
                    if (bFirst)
                    {
                        m_ser2.SendCommandWithReceive(send, out rcv);
                        bFirst = false;
                    }
                }
                //找到药停止X运动
                send[12] = 0x00;
                send[20] = m_ser2.crc_xor(send, 20);
                log4net.log.Info("发送X轴停止指令");
                m_ser2.SendCommandWithReceive(send, out rcv);
                //判断是否超时
                if ((DateTime.Now.Subtract(dt1).TotalMilliseconds >= timeout))
                {
                    if (m_mvs.bFindQr == false)
                    {
                        log4net.log.Info("药槽二维码未找到：" + medid[i] + boxid[i]);
                        this.InsertWarningRecord("药槽二维码未找到" + boxid[i]);
                    }
                    else
                    {
                        this.InsertWarningRecord("药槽二维码找到，药品未核对成功");
                    }
                    goto End1;
                }
                else
                {
                    if (bfind == false)
                    {
                        log4net.log.Info("未找到匹配药品：" + medid[i]);
                        goto End1;
                    }
                    else
                    {
                        //开始发药,一个一个发
                        for (int kk = 0; kk < qty[i]; kk++)
                        {

                            bool bCanProvide = false;
                            //不是第一个，拍照重新发
                            if (kk == 0)
                            {
                                bCanProvide = true;
                            }
                            else
                            {
                                Thread.Sleep(500);
                                dt1 = DateTime.Now;

                                while (DateTime.Now.Subtract(dt1).TotalMilliseconds < 5000)
                                {
                                    if (true == m_mvs.FindMedicineProcedure())
                                    {
                                        //找到药退出循环
                                        bCanProvide = true;
                                        break;
                                    }
                                    Thread.Sleep(100);//200
                                }

                            }
                            //第一个，直接发药
                            if (bCanProvide == true)
                            {
                                log4net.log.Info($"开始发药品{medid[i]}：第{(kk + 1)}盒，共{qty[i]}盒");
                                if (false == StartProvideMedc(1, boxid[i], medid[i], currentPrescription, name[i]))
                                {
                                    this.InsertWarningRecord("药槽通讯错误，药品发放失败");
                                    log4net.log.Info("药品发放失败：" + medid[i]);
                                    goto End1;
                                }
                                else
                                {
                                    provideSucQty[i]++;
                                    //provideSuc[i] = true;
                                    //qty_last[i]--;
                                    log4net.log.Info("药品发放成功：" + medid[i]);
                                }
                            }
                            else
                            {
                                this.InsertWarningRecord("药槽二维码找到，药品未核对成功");
                                log4net.log.Info("未找到匹配药品：" + medid[i]);
                                goto End1;
                            }
                        }
                    }
                }
                //X轴回零
                XMoveZero();
                //下一个
                i++;
            }
            success = true;
        End1:
            log4net.log.Info("处方发药结束: " + success.ToString());
            //发药结束，Y到第0层

            XMoveZero();
            return success;
        }
        public bool DeliveryMedicine()
        {
            //控制皮带转动3秒
            conveyorbelt();
            //打开取药门
            OpenDoor(true);

            return true;
        }
        public bool ReserveMedicine()
        {
            log4net.log.Info("药品未全部发放成功，回收");
            StartReserve();
            printtext = printtext + "发药品失败！";
            //X轴回零
            XMoveZero();
            return true;
        }
        //public bool CheckIfGetMedicine()
        //{
        //    //等待取药
        //    DateTime dt1 = DateTime.Now;
        //    bool removed = false;
        //    m_webcamera.nFailedLoop = 0;
        //    while (DateTime.Now.Subtract(dt1).TotalMilliseconds < 100000)
        //    {
        //        Thread.Sleep(1000);
        //        if (false == m_webcamera.CheckIfhaveBox())
        //        {
        //            removed = true;
        //            break;
        //        }

        //    }
        //    //关闭药门
        //    OpenDoor(false);
        //    if (removed == false)
        //    {
        //        log4net.log.Info("取药超时回收");
        //        printtext += "取药品失败，有药品未取，请联系医护人员。";
        //        //取药超时回收
        //        StartReserve();
        //        //记录异常
        //        //m_sql.InsertAbnormalRecord(currentPrescription, "取药超时", "未完成");
        //    }
        //    else
        //    {
        //        //补充打印内容
        //        printtext += "实际出药数量与处方预期出药数量\n不符，请联系医院药房\n如您所取药品与目录不符或有疑问，\n请暂勿使用";
        //    }
        //    return removed;
        //}
        public void KillRecordApp()
        {
            try
            {
                Process[] ps = Process.GetProcessesByName("RecordConsoleApp");
                if (ps.Length > 0)
                {
                    foreach (var p in ps)
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception)
            {
                log4net.log.Error("关闭录像附属程序失败");
            }
        }
        public void CheckRecordApp()
        {
            Process[] ps;
            System.Diagnostics.ProcessStartInfo info;
            string path = System.Environment.CurrentDirectory + @"\RecordProcess\RecordConsoleApp.exe";
            while (true)
            {
                ps = Process.GetProcessesByName("RecordConsoleApp");
                if (ps.Length > 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    info = new System.Diagnostics.ProcessStartInfo(path);
                    info.WorkingDirectory = Path.GetDirectoryName(path);
                    System.Diagnostics.Process.Start(info);
                    Thread.Sleep(1000);
                }

            }
        }
        public bool PrintPaper()
        {
            printtext += "\n\n" + ".";
            m_print.StartPrint(printtext, "txt");
            //m_rgPrint.RG_PrintText(printtext);

            bProviding = false;
            return true;
        }
        private void OpenDoor(Boolean bOpen)
        {
            //byte[] rcv;
            send[15] = m_ser2.set_bit(send[15], 4, bOpen);
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
        }
        private void YMoveLevel(int level, bool bDouble)
        {
            byte[] rcv;
            int timeout = 30000;
            int plus = nfactY[level];
            if (bDouble) plus = nFinishDoubleY;
            send[7] = Convert.ToByte(plus / 256);
            send[8] = Convert.ToByte(plus % 256);
            send[9] = 0x01;
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out rcv);
            if (rcv[0] != 0x26 || rcv[1] != 0x26)
            {
                log4net.log.Info("药架主板通讯故障：写入层数失败！");
            }
            //等待Y轴运动结束
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < timeout)
            {
                Thread.Sleep(1000);
                m_ser2.SendCommandWithReceive(send, out rcv);
                if (rcv[0] == 0x26 && rcv[1] == 0x26)
                {
                    if (Math.Abs(plus - (rcv[20] * 256 + rcv[21])) <= 10)
                    {
                        send[9] = 0x00;
                        break;
                    }
                }
            }
            if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= timeout)
            {
                log4net.log.Info("药架主板通讯故障：写入层数超时！");
            }
        }
        private void YMoveFail()
        {
            byte[] rcv;
            int timeout = 30000;
            int plus = nFailedY;
            send[7] = Convert.ToByte(plus / 256);
            send[8] = Convert.ToByte(plus % 256);
            send[9] = 0x01;
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out rcv);
            if (rcv[0] != 0x26 || rcv[1] != 0x26)
            {
                log4net.log.Info("药架主板通讯故障：写入层数失败！");
            }
            //等待Y轴运动结束
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < timeout)
            {
                Thread.Sleep(1000);
                m_ser2.SendCommandWithReceive(send, out rcv);
                if (rcv[0] == 0x26 && rcv[1] == 0x26)
                {
                    if (Math.Abs(plus - (rcv[20] * 256 + rcv[21])) <= 10)
                    {
                        send[9] = 0x00;
                        break;
                    }
                }
            }
            if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= timeout)
            {
                log4net.log.Info("药架主板通讯故障：写入层数超时！");
            }
        }
        private void XMoveZero()
        {
            byte[] rcv;

            send[12] = 0x00;
            send[14] = 0x01;
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
            log4net.log.Info("发送X轴回0指令：" + ToHexStrFromByte(send));
            //判断是否回零
            int xtimeout = 30000;
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < xtimeout)
            {
                m_ser2.SendCommandWithReceive(send, out rcv);
                if (rcv[0] == 0x26 && rcv[1] == 0x26)
                {
                    if (0 == rcv[22] * 256 + rcv[23])
                    {
                        send[14] = 0x00;
                        break;
                    }
                }
            }
        }
        private void YMoveZero()
        {
            byte[] tempSend = send;
            byte[] rcv;
            tempSend[13] = 0x00;
            tempSend[7] = 0x00;
            tempSend[8] = 0x00;
            tempSend[9] = 0x01;
            tempSend[20] = m_ser2.crc_xor(tempSend, 20);
            m_ser2.SendCommandWithReceive(tempSend, out _);
            log4net.log.Info("发送Y轴回0指令：" + ToHexStrFromByte(tempSend));
            //判断Y是否回零
            int ytimeout = 30000;
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < ytimeout)
            {
                m_ser2.SendCommandWithReceive(tempSend, out rcv);
                if (rcv[0] == 0x26 && rcv[1] == 0x26)
                {
                    if (Math.Abs(rcv[20] * 256 + rcv[21]) <= 10)
                    {
                        send[13] = 0x00;
                        break;
                    }
                }
            }
            log4net.log.Info("Y轴回0成功");
        }
        public string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }
        private bool StartProvideMedc(int qty, string sBox, string sMedc, string prescripition, string medcName)
        {
            bool ret = false;
            byte[] rcv;
            if (false == m_ser3.sp1.IsOpen)
            {
                //MessageBox.Show("药槽控制端口打开失败，发药失败");
                log4net.log.Info("药槽控制端口打开失败，发药失败");
            }
            else
            {
                //发送发药指令
                byte[] sendMedc ={0x26,0x26,0x50,0x11,0x00,0x0A,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,
                                       0x00,0x11, 0xcc,0x0D};
                if (qty > 0)
                {
                    //解析药盒id，
                    for (int j = 0; j < 4; j++)
                    {
                        sendMedc[6 + j] = Convert.ToByte(sBox[j]);
                    }
                    //药品id
                    for (int j = 0; j < 4; j++)
                    {
                        sendMedc[10 + j] = Convert.ToByte(sMedc[j]);
                    }
                    //是否发药
                    sendMedc[14] = 0x01;
                    //发药数量
                    sendMedc[15] = Convert.ToByte(qty);
                    //发送指令
                    sendMedc[16] = m_ser3.crc_xor(sendMedc, 16);
                    m_ser3.SendCommandWithReceive(sendMedc, out _);


                    //等待发药完成
                    int timeout = 10000;
                    sendMedc[14] = 0x00;
                    sendMedc[16] = m_ser3.crc_xor(sendMedc, 16);

                    DateTime startTime = DateTime.Now;
                    while (DateTime.Now.Subtract(startTime).TotalMilliseconds < timeout)
                    {
                        Thread.Sleep(1000);
                        log4net.log.Info("发送发药指令：" + ToHexStrFromByte(sendMedc));
                        m_ser3.SendCommandWithReceive(sendMedc, out rcv);
                        if (rcv[0] == 0x26 && rcv[1] == 0x26)
                        {
                            if (1 == m_ser3.get_bit(rcv[14], 2))
                            {
                                //m_sql.UpdateMedicineNumber(sBox, sMedc, qty_last - qty);
                                //m_sql.InsertGetMedicinesRecord(prescripition, medcName, qty, "完成");
                                ret = true;
                                break;
                            }
                        }
                    }

                    if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= timeout)
                    {
                        //MessageBox.Show("发药超时！");
                        log4net.log.Info("发药超时！");
                    }
                }
            }
            return ret;
        }
        private bool StartReserve()
        {
            byte[] rcv;
            send[15] = m_ser2.set_bit(send[15], 6, true);
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
            Thread.Sleep(3000);
            send[15] = m_ser2.set_bit(send[15], 6, false);
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
            return true;
        }
        private bool conveyorbelt()
        {
            byte[] rcv;
            send[15] = m_ser2.set_bit(send[15], 1, true);
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
            Thread.Sleep(3000);
            send[15] = m_ser2.set_bit(send[15], 1, false);
            send[20] = m_ser2.crc_xor(send, 20);
            m_ser2.SendCommandWithReceive(send, out _);
            return true;
        }

    }
    public class StringUtility
    {
        public static string BreakLongString(string SubjectString, int lineLength)
        {
            StringBuilder sb = new StringBuilder(SubjectString);
            int offset = 0;
            ArrayList indexList = buildInsertIndexList(SubjectString, lineLength);
            for (int i = 0; i < indexList.Count; i++)
            {
                sb.Insert((int)indexList[i] + offset, '\n');
                offset++;
            }
            return sb.ToString();
        }

        public static bool IsChinese(char c)
        {
            return (int)c >= 0x4E00 && (int)c <= 0x9FA5;
        }

        private static ArrayList buildInsertIndexList(string str, int maxLen)
        {
            int nowLen = 0;
            ArrayList list = new ArrayList();
            for (int i = 1; i < str.Length; i++)
            {
                if (IsChinese(str[i]))
                {
                    nowLen += 2;
                }
                else
                {
                    nowLen++;
                }
                if (nowLen > maxLen)
                {
                    nowLen = 0;
                    list.Add(i);
                }
            }
            return list;
        }
    }

}
