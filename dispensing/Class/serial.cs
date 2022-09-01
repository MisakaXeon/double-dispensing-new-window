using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;


namespace dispensing
{
    class serial
    {

        public SerialPort sp1 = new SerialPort();
        public string strPortName = "COM3";
        public int iPortBaudRate = 115200;
        public int nWait = 200;
        private object sendobj = new object();
        public void OpenPort()
        {
            try
            {
                if (sp1.IsOpen) return;
                sp1.PortName = strPortName;
                sp1.BaudRate = iPortBaudRate;
                sp1.Parity = Parity.None;
                sp1.DataBits = 8;
                sp1.StopBits = StopBits.One;
                //sp1.ReadTimeout = 50;
                sp1.Open();
            }
            catch (Exception exc)
            {

                log4net.log.Info("控制卡端口初始化出错:" + exc.ToString());
            }
        }
        public void ClosePort()
        {
            if (sp1.IsOpen == true)
            {
                sp1.Close();
            }
        }


        public bool SendCommand(byte[] sendData)
        {
            byte[] receiveData;
            return SendCommandWithReceive(sendData, out receiveData);
        }
        public string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    returnStr += " ";
                }
            }
            return returnStr;
        }

        public bool SendCommandWithReceive(byte[] sendData, out byte[] receiveData)
        {
            receiveData = new byte[501];

            if (false == sp1.IsOpen)
                return false;
            try
            {
                lock (sendobj)
                {

                    try
                    {
                        byte[] tempArry;
                        int readLength;
                        int readlen;
                        //log4net.log.Info("开始发送串口数据：" + strPortName.ToString());
                        sp1.DiscardOutBuffer();
                        sp1.DiscardInBuffer();
                        //write
                        sp1.Write(sendData, 0, sendData.Length);  //01 06 A0 80 01 22 2B AB
                        //log4net.log.Info("send " + sendData.Length.ToString() + ": " + byteToHexStr(sendData));

                        int timeout = 1000;
                        DateTime dt1 = DateTime.Now;
                        while (DateTime.Now.Subtract(dt1).TotalMilliseconds < timeout)
                        {
                            if (sp1.BytesToRead < sendData.Length)
                            {
                                Thread.Sleep(10);
                                continue;
                            }
                            else
                            {
                                //read
                                readlen = sp1.BytesToRead;
                                //log4net.log.Info("start read, length: " + readlen.ToString());
                                tempArry = new byte[readlen];
                                readLength = sp1.Read(tempArry, 0, readlen);
                                Array.Copy(tempArry, 0, receiveData, 0, readLength);
                                //log4net.log.Info("read " + readLength.ToString() + ": " + byteToHexStr(tempArry));
                                //log4net.log.Info("完成读取串口数据：" + strPortName.ToString());
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log4net.log.Info("com port Write/Read: " + ex.ToString());
                        throw;
                    }

                    return false;

                }
            }
            catch (System.ArgumentNullException arg)
            {
                log4net.log.Info("串口错误:" + arg.Message + "  " + arg.Source);
            }

            return false;
        }

        public static byte[] ToModbus(byte[] byteData, int Length)
        {
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < Length; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;//异或多项式
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);//高位在后
            CRC[0] = (byte)(wCrc & 0x00FF);       //低位在前
            return CRC;
        }
        public bool updateboxinfo(string boxid, string medcid, int x, int y, int z, int qty, out string strState)
        {
            strState = "";
            byte[] sendQty ={0x26,0x26,0x50,0x12,0x00,0x09,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,
                                       0x99,0xcc,0x0D};
            //
            byte[] sendSize ={0x26,0x26,0x50,0x10,0x00,0x0E,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,0xAA,
                                       0x11,0x11,0x22,0x22,0x33,0x33,0xcc,0x0D};
            byte[] rcv;// = new byte[500];

            //解析药槽id
            if (boxid.Length < 4)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                sendSize[6 + i] = sendQty[6 + i] = Convert.ToByte(boxid[i]);

            }
            //药品id
            if (medcid.Length < 4)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                sendSize[10 + i] = sendQty[10 + i] = Convert.ToByte(medcid[i]);

            }
            //写入尺寸
            sendSize[14] = Convert.ToByte(Convert.ToInt16(x) / 256);
            sendSize[15] = Convert.ToByte(Convert.ToInt16(x) % 256);
            sendSize[16] = Convert.ToByte(Convert.ToInt16(y) / 256);
            sendSize[17] = Convert.ToByte(Convert.ToInt16(y) % 256);
            sendSize[18] = Convert.ToByte(Convert.ToInt16(z) / 256);
            sendSize[19] = Convert.ToByte(Convert.ToInt16(z) % 256);
            sendSize[20] = crc_xor(sendSize, 20);
            this.SendCommandWithReceive(sendSize, out rcv);
            if (rcv[0] != 0x26 || rcv[1] != 0x26)
            {
                strState = "药槽信息写入故障：写入药品尺寸失败！";
                Console.WriteLine("药槽信息写入故障：写入药品尺寸失败！");
                return false;
            }
            //写入数量
            Thread.Sleep(100);
            sendQty[14] = Convert.ToByte(qty);
            sendQty[15] = crc_xor(sendQty, 15);
            this.SendCommandWithReceive(sendQty, out rcv);
            if (rcv[0] != 0x26 || rcv[1] != 0x26)
            {
                strState = "药槽信息写入故障：写入药品数量失败！";
                Console.WriteLine("药槽信息写入故障：写入药品数量失败！");
                return false;
            }

            return true;
        }
        public byte crc_xor(byte[] byteData, int len)
        {
            byte ret = 0;
            try
            {
                for (int i = 1; i < len; i++)
                {
                    if (i == 1)
                        ret = Convert.ToByte(byteData[0] ^ byteData[1]);
                    else
                        ret ^= byteData[i];
                }
            }
            catch (Exception ex)
            {
                log4net.log.Info("crc_xor: " + ex.ToString());
                throw;
            }

            return ret;
        }
        public byte set_bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }
        int power(int a, int n)
        {
            int i = 0;
            int ret = 1;
            while (i < n)
            {
                ret *= a;
                i++;
            }
            return ret;
        }
        public int get_bit(byte input, int index) //index 1~8
        {
            int value = 0;

            if (index < 1 || index > 8)
            {
                return -1;
            }
            //要判断第几位为1,
            int n = index - 1;

            if ((input & (byte)power(2, n)) == power(2, n))
            {
                value = 1;
            }
            else
            {
                value = 0;
            }

            return value;
        }
    }
}
