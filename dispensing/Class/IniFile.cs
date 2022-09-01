using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
//因为我们需要调用API函数，所以必须创建System.Runtime.InteropServices 命名空间以提供可用于访问 .NET 中的 COM 对象和本机 API 的类的集合。



namespace dispensing
{

    public class IniFile
    {

        public string path;    //INI文件名
        string sConfig = System.Environment.CurrentDirectory + @"\config.ini";
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        //类的构造函数，传递INI文件名
        public IniFile(string INIPath)
        {
            path = sConfig;
        }

        #region 写INI文件
        //写INI文件
        public void IniWriteValueString(string Section, string Key, string Value)
        {

            WritePrivateProfileString(Section, Key, Value, this.path);

        }

        public void IniWriteValueDouble(string Section, String Key, long dValue)
        {
            string sTemp = dValue.ToString("f2");
            WritePrivateProfileString(Section, Key, sTemp, this.path);
        }

        public void IniWriteValueint(string Section, String Key, int nValue)
        {
            string sTemp = nValue.ToString();
            WritePrivateProfileString(Section, Key, sTemp, this.path);
        }
        #endregion 写INI文件

        #region 读INI文件
        public string IniReadStringValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            StringBuilder strTemp = new StringBuilder("-1");
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            //当temp为空时，temp=strTemp （-1）表示无值
            if (temp.Length == 0)
            {
                temp = strTemp;
            }
            return temp.ToString();
        }

        public int IniReadIntValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int nValue = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            if (nValue > 0)
            {
                string s = temp.ToString();
                nValue = Convert.ToInt32(s);
            }
            return nValue;
        }
        #endregion 读INI文件

        #region 读取ini文件主键
        public static void ReadSections(string ReadFile_ini, string str, ref List<string> list_checkItems)
        {

        }

        public static void GetStringsFromBuffer(Byte[] Buffer, int bufLen, ref List<string> list_checkItems)
        {
            list_checkItems.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        list_checkItems.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        #endregion

    }
}