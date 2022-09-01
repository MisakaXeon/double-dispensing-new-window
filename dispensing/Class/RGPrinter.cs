using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace dispensing
{
    class RGPrinter
    {

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_GetSDKVersion(byte[] ListFilePath);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_GetSupportPrinters(byte[] lpPrinters, Int32 len);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_ConnectDevices(string prtName, String port, int timeout);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_CloseDevices(int objCode);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_QueryPrinterFirmware(int objCode, byte[] szFirmware, int len);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_QueryStatus(int objCode);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_PageStart(int objCode, bool graphicMode, int width, int height);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_PageEnd(int objCode, int tm);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 ASCII_PrintText(int objCode, string szText);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_PageSend(int objCode);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 ASCII_CtrlCutPaper(int objCode, int cutWay, int postion);

        [DllImport("PrintSDK.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 CON_StartRecord(string path);

        private Int32 m_objID;
        public int RG_OpenPrinter()
        {
            Int32 iRet = 0;

            /*byte[] btListFilePath = new byte[1024];
            Int32 len = CON_GetSDKVersion(btListFilePath);
            if(len==0)
                MessageBox.Show("打印机dll读取失败！");
            string str = Encoding.Unicode.GetString(btListFilePath, 0, (len) * 2);

            len = CON_GetSupportPrinters(btListFilePath, 1024);
            str = Encoding.Unicode.GetString(btListFilePath, 0, (len) * 2);*/

            CON_StartRecord("Log.txt");

            for (int i = 0; i < 1; i++)
            {
                iRet = CON_ConnectDevices("RG-K532", "USB", 500);
                if (iRet!=0)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            
            if (iRet != 0)
            {
                m_objID = iRet;
                //byte[] btVersion = new byte[256];
                //len = CON_QueryPrinterFirmware(m_objID, btVersion, 256);
                //str = Encoding.Unicode.GetString(btVersion, 0, (len) * 2);
            }
            else
            {
                iRet = 0;
                //MessageBox.Show("打印机RG-K532打开失败！");
            }
            
            
            return iRet;
        }
        public void RG_ClosePrinter()
        {
            CON_CloseDevices(m_objID);
            m_objID = 0;
            return;
        }

        public int RG_QueryPrintStatusWithopen()
        {
            int status = 7;
            if (RG_OpenPrinter() == 0)
            {
                status = CON_QueryStatus(m_objID);
                RG_ClosePrinter();
            }
            return status;
        }
        public int RG_QueryPrintStatus()
        {
            
            int status = CON_QueryStatus(m_objID);
            
            /*switch (status)
            {
                case 0:
                    listBox1.Items.Add("Status OK");
                    break;
                case 1:
                    listBox1.Items.Add("out of paper");
                    break;
                case 2:
                    listBox1.Items.Add("Print paper will be done");
                    break;
                case 3:
                    listBox1.Items.Add("Printer not connected");
                    break;
                case 4:
                    listBox1.Items.Add("Printer is not verified");
                    break;
                case 5:
                    listBox1.Items.Add("Printer paper crest open");
                    break;
                case 6:
                    listBox1.Items.Add("Printer cutter error");
                    break;
            }*/
            return status;
        }

        public int RG_PrintText(string text)
        {
            
            int iRet = CON_PageStart(m_objID, false, 0, 0);
            ASCII_PrintText(m_objID, text);
            Thread.Sleep(200);
            ASCII_CtrlCutPaper(m_objID, 65, 0);
            CON_PageEnd(m_objID, 0);
            CON_PageSend(m_objID);

            return iRet;
        }
    }
}
