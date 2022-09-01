using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace dispensing
{
    class Http
    {
        object lockObj = new object();
        private string serverUrl;
        IniFile m_ini = new IniFile("");
        //public string equipNo;
        //OutMedicList oml;
        ////bool isAddFinish = true;
        //public static DrugInTaskList ditl;
        //public static DrugInTaskDetail[] ditd;
        //public static InventoryDrugList[] idl;
        //public static StorageDetail[] sd;
        //public static StorageList sl;
        //public Form1 form1;
        sqlite m_sql = new sqlite();
        public Http()
        {
            serverUrl = m_ini.IniReadStringValue("Config", "ServerURL");
            //equipNo = m_ini.IniReadStringValue("Config", "equipNo");
            //m_sql.Init();
        }
        public string HttpGet(string Url, string contentType)
        {
            lock (lockObj)
            {
                try
                {
                    string retString = string.Empty;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                    request.Method = "GET";
                    request.ContentType = contentType;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(myResponseStream);
                    retString = streamReader.ReadToEnd();
                    streamReader.Close();
                    myResponseStream.Close();
                    return retString;
                }
                catch (Exception ex)
                {
                    log4net.log.Error(ex);
                    return "";
                }
            }
        }


        public string HttpPost(string Url, string postDataStr, string contentType, out bool isOK)
        {
            string retString = string.Empty;
            lock (lockObj)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                    request.Method = "POST";
                    request.ContentType = contentType;
                    request.Timeout = 5000;//设置超时时间
                    request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                    Stream requestStream = request.GetRequestStream();
                    StreamWriter streamWriter = new StreamWriter(requestStream);
                    streamWriter.Write(postDataStr);
                    streamWriter.Close();

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    retString = streamReader.ReadToEnd();
                    streamReader.Close();
                    responseStream.Close();

                    isOK = true;
                }
                catch (Exception ex)
                {
                    log4net.log.Error(ex);
                    if (ex.GetType() == typeof(WebException))//捕获400错误
                    {
                        var response = ((WebException)ex).Response;
                        Stream responseStream = response.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream);
                        retString = streamReader.ReadToEnd();
                        streamReader.Close();
                        responseStream.Close();
                    }
                    else
                    {
                        retString = "";
                    }
                    isOK = false;
                }

                return retString;
            }
        }
        public bool GetPreScription(string prescriptionNo, out SinglePresc singlePresc)
        {
            singlePresc = null;
            try
            {
                TaskList taskList = new TaskList();
                string ret = HttpGet(serverUrl + "/api/fatm/getSinglePresc?prescriptionNo=" + prescriptionNo, "application/json");
                log4net.log.Info("服务器处方查询结果：\n" + ret);
                if (!ret.Contains("prescriptionNo"))
                {
                    log4net.log.Error("SinglePresc服务器返回内容错误");
                    return false;
                }
                singlePresc = new SinglePresc();
                singlePresc = JsonConvert.DeserializeObject<SinglePresc>(ret);//反序列化json
                return true;
            }
            catch (Exception e)
            {
                log4net.log.Error("SinglePresc获取失败：" + e);
                return false;
            }
        }
    }
}
