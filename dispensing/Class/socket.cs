using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dispensing
{
    public class socket
    {
        public delegate void SocketEventHandler(string msg);
        public event SocketEventHandler MsgReceived;
        Socket socketSend;
        string strIp;
        int port;
        public socket(string _strIp, int _port)
        {
            strIp = _strIp;
            port = _port;
            Thread th = new Thread(Connect);
            th.Start();
        }
        void Connect()
        {
            try
            {
                socketSend = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress iP = IPAddress.Parse(strIp);
                IPEndPoint endPoint = new IPEndPoint(iP, port);
                //客户端连接服务器端
                socketSend.Connect(endPoint); //服务器端是绑定,
                Thread th = new Thread(Recieve);
                th.IsBackground = true;
                th.Start(socketSend);
                log4net.log.Info("主机端连接成功");
            }
            catch (Exception e)
            {
                //log4net.log.Error("TCP异常断线，尝试重连" + e);
                Thread.Sleep(10);
                Thread th = new Thread(Connect);
                th.Start();
            }

        }
        void Recieve(object o)
        {
            Socket socketSendC = o as Socket;
            byte[] buffer = new byte[1024 * 1024 * 2];
            string str;
            int r;
            while (true)
            {
                try
                {
                    r = socketSendC.Receive(buffer);

                    if (r == 0)
                    {
                        break;
                    }
                    str = Encoding.UTF8.GetString(buffer, 0, r);//将字节转化为字符串
                    MsgReceived(str);
                }

                catch (Exception e)
                {
                    log4net.log.Error("Socket接收数据异常：" + e);
                    socketSendC.Close();
                    socketSendC.Dispose();
                    socketSend.Close();
                    socketSend.Dispose();
                    Thread th = new Thread(Connect);
                    th.Start();
                    break;
                }
            }
        }
        public void Send(string msg)
        {
            try
            {
                string str = msg.Trim();//移除前面和后面的空白字符
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                socketSend.Send(buffer);
            }
            catch (Exception)
            {

            }

        }
        public void CloseCon()
        {
            try
            {
                socketSend.Close();
                socketSend.Dispose();
            }
            catch (Exception ex)
            {
                log4net.log.Error(ex);
            }

        }
    }
}
