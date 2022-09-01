using dispensing.CustomCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using dispensing.Expends;

namespace dispensing.CustomControls
{
    /// <summary>
    /// IndexContentPage.xaml 的交互逻辑
    /// </summary>
    public partial class IndexContentPage : UserControl
    {
        public IndexContentPage()
        {
            InitializeComponent();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void OpenDosingAdministration(object sender, MouseButtonEventArgs e)
        {
            var dialog = new VerifyDialog()
            {
                MaskOpacity = 1,
                MaskBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
            };

            dialog.VertifyCommand = new SampleCommand(o => true, vertifyContent =>
            {
                MessageBox.Show(vertifyContent.ToString());
                // 验证逻辑

                //验证成功显示这个
                DosingAdministrationControl.Visibility = Visibility.Visible;
                dialog.CloseAsync();
            });

            this.ShowMessageAsync(dialog, true);

            
        }
        /// <summary>
        /// 回退命令
        /// </summary>
        public void BackCommandEvent()
        {
            DosingAdministrationControl.Visibility = Visibility.Collapsed;
        }

        private void OpenSetting(object sender, MouseButtonEventArgs e) 
        {
            var dialog = new VerifyDialog()
            {
                MaskOpacity = 1,
                MaskBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
            };

            dialog.VertifyCommand = new SampleCommand(o => true, vertifyContent =>
            {
                MessageBox.Show(vertifyContent.ToString());
                // 验证逻辑

                dialog.CloseAsync();
            });

            this.ShowMessageAsync(dialog);
        }
    }
}
