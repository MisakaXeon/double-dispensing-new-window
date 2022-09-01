using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

namespace RecordConsoleApp
{
    internal class Program
    {
        static string recordPath;
        static bool isRecording = false;
        static void Main(string[] args)
        {
            DirectoryInfo path = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string strPath = path.Parent.FullName;
            recordPath = strPath + @"\Record\";
            Console.Title = "RecordConsoleApp";
            //OpenCamera("");
            //Instantiate our server channel.
            IpcChannel channel = new IpcChannel("RecordChannal");
            //Register the server channel.
            ChannelServices.RegisterChannel(channel, false);
            //Register this service type.
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MyProcessSendObject), "RecordObj", WellKnownObjectMode.SingleCall);
            WriteLine("初始化IPC信道通讯", 0);
            MyProcessSendObject.StartRecord += OpenCamera;
            MyProcessSendObject.StopRecord += CloseCamera;
            WriteLine("开始接收控制指令", 0);
            //Console.ReadLine();
            //CloseCamera();
            Thread runTh = new Thread(RunThread);
            runTh.Start();
            Thread checkTh = new Thread(CheckMainProcess);
            checkTh.Start();

        }

        private static VideoCaptureDevice Camera = null;
        //用来把每一帧图像编码到视频文件
        private static VideoFileWriter VideoOutPut = new VideoFileWriter();
        public static void OpenCamera(string fileName)
        {
            try
            {
                if (isRecording)
                {
                    CloseCamera();
                }
                isRecording = true;
                WriteLine("接收到开始录像指令：" + fileName, 0);
                //获取摄像头列表
                var devs = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                //实例化设备控制类(我选了第1个)
                Camera = new VideoCaptureDevice(devs[0].MonikerString);
                //配置录像参数(宽,高,帧率,比特率等参数)VideoCapabilities这个属性会返回摄像头支持哪些配置,从这里面选一个赋值接即可,我选了第1个
                Camera.VideoResolution = Camera.VideoCapabilities[0];
                //设置回调,aforge会不断从这个回调推出图像数据
                Camera.NewFrame += Camera_NewFrame;
                //打开摄像头
                Camera.Start();
                //打开录像文件(如果没有则创建,如果有也会清空),这里还有关于
                VideoOutPut.Open(recordPath + @"\" + fileName, Camera.VideoResolution.FrameSize.Width, Camera.VideoResolution.FrameSize.Height, Camera.VideoResolution.AverageFrameRate, VideoCodec.MPEG4, Camera.VideoResolution.BitCount);
            }
            catch (Exception ex)
            {
                WriteLine(fileName + "录像异常：" + ex.Message, 1);
                CloseCamera();
            }

        }
        //图像缓存
        private static Bitmap bmp = new Bitmap(1, 1);

        //摄像头输出回调
        private static void Camera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //写到文件
            VideoOutPut.WriteVideoFrame(eventArgs.Frame);
            lock (bmp)
            {
                //释放上一个缓存
                bmp.Dispose();
                //保存一份缓存
                bmp = eventArgs.Frame.Clone() as Bitmap;
            }
        }
        private static void CloseCamera()
        {
            try
            {
                isRecording = false;
                WriteLine("接收到停止录像指令", 0);
                //停摄像头
                Camera.Stop();
                //关闭录像文件,如果忘了不关闭,将会得到一个损坏的文件,无法播放
                VideoOutPut.Close();
            }
            catch (Exception)
            {

            }
        }
        public static string GetTimeString()
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
        }
        public static void WriteLine(string msg, int msgType)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(GetTimeString());
            switch (msgType)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            Console.WriteLine(msg);
        }
        public static void RunThread()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
        public static void CheckMainProcess()
        {
            while (true)
            {
                Process[] ps = Process.GetProcessesByName("dispensing");
                if (ps.Length > 0)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    CloseCamera();
                    WriteLine("主进程不存在 程序将在3秒后关闭", 0);
                    Thread.Sleep(3000);
                    System.Environment.Exit(0);
                }
            }
        }
    }
}

