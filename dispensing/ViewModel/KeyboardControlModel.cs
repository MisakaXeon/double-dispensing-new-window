using dispensing.CustomCommand;
using dispensing.Enums;
using dispensing.Expends;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WindowsApiLib.API;

namespace dispensing.ViewModel
{
    public class KeyboardControlModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 普通按键命令
        /// </summary>
        public ICommand KeyCommand { get; set; }
        /// <summary>
        /// 切换语言命令
        /// </summary>
        public ICommand SwitchCommand { get; set; }
        /// <summary>
        /// 隐藏命令
        /// </summary>
        public ICommand HiddenCommand { get; set; }
        /// <summary>
        /// 当前的输入类型
        /// </summary>
        public SwitchInputType InputType { get; set; }

        public KeyboardControlModel()
        {
            KeyCommand = new SampleCommand(o => true, o => {
                try
                {
                    string keyStr = o as string;
                    //退格
                    if (keyStr.Equals("BackSpace"))
                    {
                        WindowsAPI.KeyDown(EnumKeyboardKey.Back);
                        Thread.Sleep(20);
                        WindowsAPI.KeyUp(EnumKeyboardKey.Back);
                        return;
                    }
                    //回车
                    else if (keyStr.Equals("Enter"))
                    {
                        WindowsAPI.KeyDown(EnumKeyboardKey.Space);
                        Thread.Sleep(20);
                        WindowsAPI.KeyUp(EnumKeyboardKey.Space);
                        return;
                    }
                    try
                    {
                        Convert.ToInt32(keyStr);
                        keyStr = "D" + keyStr;
                    }
                    catch (Exception){}
                    EnumKeyboardKey key = keyStr.Converter<EnumKeyboardKey>(false);
                    WindowsAPI.KeyDown(key);
                    Thread.Sleep(20);
                    WindowsAPI.KeyUp(key);
                }
                catch (Exception){}
            });

            SwitchCommand = new SampleCommand(o => true, o => {
                if (InputType == SwitchInputType.English)
                {
                    SetWindowInpuType(SwitchInputType.Chinese);
                }
                else
                {
                    SetWindowInpuType(SwitchInputType.English);
                }
            });
            ///隐藏
            HiddenCommand = new SampleCommand(o => true, o => { 
                FrameworkElement control = o as FrameworkElement;
                control.Visibility = Visibility.Collapsed;
            });

            InputType = GetCultureType();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 设置指定窗口为 简体中文 字符集的 输入法
        /// </summary>
        /// <param name="handle"></param>
        public void SetWindowInpuType(SwitchInputType type)
        {
            if (this.InputType == type) return;
            InputType = type;
            //Win32API.PostMessage(MainWindowHandle, (uint)MsgType.WM_INPUTLANGCHANGEREQUEST, 0, (uint)(type == SwitchInputType.Chinese ? 0x08040804 : 0x04090409)); //英文  0x04090409 // 中文  0x08040804

            if (type == SwitchInputType.Chinese)
            {
                //WindowsAPI.SetChineseImm(MainWindowHandle);
                SwitchToLanguageMode(new List<string> { "zh-CN" });
            }
            else if(type == SwitchInputType.English)
            {
                //WindowsAPI.SetEnglishImm(MainWindowHandle);
                SwitchToLanguageMode(new List<string> { "en-US" , "en-GB" });
            }
        }


        /// <summary>
        /// 获取当前输入法
        /// </summary>
        /// <returns></returns>
        private SwitchInputType GetCultureType()
        {
            var currentInputLanguage = InputLanguage.CurrentInputLanguage;
            var cultureInfo = currentInputLanguage.Culture;
            //同 cultureInfo.IetfLanguageTag;
            if (cultureInfo.Name.Equals("zh-CN"))
            {
                return SwitchInputType.Chinese;
            }
            else if (cultureInfo.Name.Equals("en-US") || cultureInfo.Name.Equals("en-GB"))
            {
                return SwitchInputType.English;
            }
            return SwitchInputType.English;
        }

        /// <summary>
        /// 切换输入法
        /// </summary>
        /// <param name="cultureType">语言项，如zh-CN，en-US</param>
        private void SwitchToLanguageMode(List<string> cultureTypes)
        {
            var installedInputLanguages = InputLanguage.InstalledInputLanguages;

            foreach (var cultureType in cultureTypes)
            {
                if (installedInputLanguages.Cast<InputLanguage>().Any(i => i.Culture.Name == cultureType))
                {
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(System.Globalization.CultureInfo.GetCultureInfo(cultureType));
                    break;
                }
            }
        }
    }
}
