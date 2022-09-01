using dispensing.CustomCommand;
using dispensing.Expends;
using dispensing.ViewModel;
using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dispensing.CustomControls
{
    /// <summary>
    /// TakeMedicinePage.xaml 的交互逻辑
    /// </summary>
    public partial class TakeMedicinePage : UserControl, INotifyPropertyChanged
    {
        public static TakeMedicineInfoModel model;
        DispensingDialog loadingDialog;
        DispensingDialog waitingDialog;
        DispensingDialog messageDialog;
        bool isNewMsgIn = false;

        public delegate void ManualSendCodeEventHandler(string code);
        public event ManualSendCodeEventHandler SendCode;
        /// <summary>
        /// 确定命令
        /// </summary>
        public ICommand SureCommand { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string MedicineSerial { get; set; }
        public string Test { get; set; }
        /// <summary>
        /// 媒体视频路径
        /// </summary>
        public string MediaUri { get; set; } = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\video_tutorail.mp4");
        TestProcedure testProcedure = new TestProcedure();
        Thread delayThread;
        public TakeMedicinePage()
        {
            InitializeComponent();
            testProcedure.medicPage = this;
            testProcedure.LoadEvent();
            testProcedure.InitAll();
            Loaded += TakeMedicinePage_Loaded;
            testProcedure.TaskGetted += CodeIn;
            testProcedure.ShowMsg += ShowMessage;
            testProcedure.StartLoding += StartLoading;
            testProcedure.StopLoading += StopLoading;
            testProcedure.StartWaiting += StartWaiting;
            testProcedure.StopWaiting += StopWaiting;
            testProcedure.HideTakeMedicDetail += HideMedicDetail;
            testProcedure.ShowWaitMessage += ShowWaitMessage;
            testProcedure.CloseMsg += CloseMessage;

            SureCommand = new SampleCommand(o => true, o =>
            {
                //this.ShowMessageAsync(new DispensingDialog(null, "正在提取药品...\n若药品较多，可能等待1-2分钟\n请您耐心等候如有多个处方\n请在出药完成后，再扫码下个处方", null, null, "wait"));
                //this.ShowMessageAsync(new DispensingDialog("确定", "出药完成\n请取走药品和凭条，请您核对药品\n种类和数量，如有出入和任何疑问\n请联系护士站人工帮助", null, null, "lament"));
                //CloseMessage();
                //ShowMessage("1234");
                //return;
                if (MedicineSerial == "")
                {
                    return;
                }
                SendCode(MedicineSerial);
                // 确定命令内容
                // MedicineSerial
                //MessageBox.Show("dddddd");
                //this.ShowMessageAsync(new DispensingDialog(null, "正在提取药品...\n若药品较多，可能等待\n1-2分钟，请您耐心等候\n如有多个处方，请在出药\n完成后，再扫码下个处方", null, null, "wait"));
                //TakeMedicineInfoPage.Visibility = Visibility.Visible;

            });
        }
        public void StartLoading(string msg)
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                loadingDialog = new DispensingDialog(null, msg, icon: "boll")
                {
                    MaskOpacity = 0.7,
                    MaskBrush = new SolidColorBrush(Colors.Gray),
                    DisplayBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"))
                };
                await this.ShowMessageAsync(loadingDialog);//显示
            }));

        }
        public void StopLoading()
        {
            if (loadingDialog != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    loadingDialog.CloseAsync();
                }));
            }
        }
        public void CloseMessage()
        {
            if (messageDialog != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    messageDialog.CloseAsync();
                }));
            }
        }
        public void ShowMessage(string msg)
        {
            if (delayThread != null)
            {
                delayThread.Abort();
            }
            this.Dispatcher.Invoke(new Action(async () =>
            {
                messageDialog = new DispensingDialog("确定", msg, null, null, "lament");
                await this.ShowMessageAsync(messageDialog);
            }));
            delayThread = new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    Thread.Sleep(1000);
                    i++;
                    if (i * 1000 >= testProcedure.nConfirmTimeOut)
                    {
                        CloseMessage();
                        break;
                    }
                }
            });
            delayThread.IsBackground = true;
            delayThread.Start();
        }
        public void ShowWaitMessage(string msg)
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                await this.ShowMessageAsync(new ToastDialog(false, msg));
            }));
        }
        public void StartWaiting(string msg)
        {

            this.Dispatcher.Invoke(new Action(async () =>
            {
                waitingDialog = new DispensingDialog(null, msg, null, null, "wait");
                await this.ShowMessageAsync(waitingDialog);//显示
            }));
        }
        public void StopWaiting()
        {
            if (waitingDialog != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    waitingDialog.CloseAsync();
                }));
            }
        }

        public void CodeIn(TaskList taskList)
        {
            //log4net.log.Info("事件激活");
            TakeMedicineInfoModel Model = new TakeMedicineInfoModel()
            {
                Address = DosingAdministration.hospitalName,
                Age = taskList.patAge,
                TakeDate = taskList.taskTime,
                MedicineDate = taskList.createTime,
                Name = taskList.patName,
                Serial = taskList.code,
                Sex = taskList.patGender,
                MedicineInfos = new ObservableCollection<MedicineInfo>()
            };
            foreach (var item in taskList.drugTaskDetailList)
            {
                Model.MedicineInfos.Add(new MedicineInfo()
                {
                    Count = item.quantity,
                    ImageUri = new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"),
                    Name = item.name
                });
            }
            //log4net.log.Info("传入model");
            this.Dispatcher.Invoke(new Action(() =>
            {
                TakeMedicineInfoPage.DataContext = Model;
                TakeMedicineInfoPage.Visibility = Visibility.Visible;
            }));
            //log4net.log.Info("显示药品详情");

            //SureCommand = new SampleCommand(o => true, o =>
            //{
            //    TakeMedicineInfoPage.Visibility = Visibility.Visible;
            //    //await this.ShowMessageAsync(new DispensingDialog("ok", "测试", "取消"));
            //});
        }
        public void HideMedicDetail()
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                if (TakeMedicineInfoPage.Visibility == Visibility.Visible)
                {
                    TakeMedicineInfoPage.Visibility = Visibility.Collapsed; ;
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TakeMedicinePage_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;


            //初始化播放器
            PlayElement.SourceProvider.CreatePlayer(vlcLibDirectory, options);
            PlayElement.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
            PlayElement.SourceProvider.MediaPlayer.Play(new Uri(MediaUri));


        }
        DirectoryInfo vlcLibDirectory = new DirectoryInfo(@"C:\Program Files\VideoLAN\VLC");
        string[] options = new string[]
        {
                //添加日志
                "--file-logging", "-vvv", "--logfile=Logs.log"
            // VLC options can be given here. Please refer to the VLC command line documentation.
        };
        private async void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            var task = new Task(() =>
            {
                PlayElement.Dispose();
            });
            task.Start();
            await task;

            PlayElement.SourceProvider.CreatePlayer(vlcLibDirectory, options);
            PlayElement.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
            PlayElement.SourceProvider.MediaPlayer.Play(new Uri(MediaUri));

        }



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //testProcedure.printtext = "     自助药房用药指导单\n姓名：王大新\n处方号：O220616000697\n处方时间：2022 - 06 - 16\n取药时间：2022 - 07 - 12\n   ----------------------------------------------------\n药品名 数量(盒)\n----------------------------------------------------\n小儿柴桂退热颗粒[基]     1\n用法：4，，每次：50.00000盒\n----------------------------------------------------\ntest";
        //    testProcedure.printtext = "打印测试";
        //    testProcedure.PrintPaper();
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    testProcedure.printtext = "     自助药房用药指导单\n姓名：王大新 处方号：O220616000697\n处方时间：2022 - 06 - 16 取药时间：2022 - 07 - 12\n   ----------------------------------------------------\n药品名 数量(盒)\n----------------------------------------------------\n小儿柴桂退热颗粒[基]     1\n用法：4，，每次：50.00000盒\n----------------------------------------------------\ntest";
        //    testProcedure.PrintPaper();
        //}


        //private void media_Loaded(object sender, RoutedEventArgs e)
        //{
        //    (sender as MediaElement).Play();
        //}
        //private void media_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    // MediaElement需要先停止播放才能再开始播放，
        //    // 否则会停在最后一帧不动
        //    (sender as MediaElement).Stop();
        //    (sender as MediaElement).Play();
        //}

    }
}
