using dispensing.ViewModel;
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

namespace dispensing.CustomControls
{
    /// <summary>
    /// DosingAdministration.xaml 的交互逻辑
    /// </summary>
    public partial class DosingAdministration : UserControl
    {
        public static string doctorName = "李慧于";
        public static string hospitalName = "深圳大学总医院";
        IniFile m_ini = new IniFile("");
        public static socket m_socket;
        public static DosingAdministration dosingAdministration;
        /// <summary>
        /// 数据模型
        /// </summary>
        public DosingAdministrationModel Model { get; set; } = new DosingAdministrationModel();


        public DosingAdministration()
        {
            dosingAdministration = this;
            InitializeComponent();
            this.Loaded += DosingAdministration_Loaded;
            m_socket = new socket(m_ini.IniReadStringValue("Config", "SocketIP"), m_ini.IniReadIntValue("Config", "SocketPort"));
        }

        private void DosingAdministration_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model;
        }
    }
}
