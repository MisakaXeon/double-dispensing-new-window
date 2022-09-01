using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;
using MvCamCtrl.NET;
using HalconDotNet;
using System.IO;

namespace dispensing
{
    class u3Camera
    {
        //HTuple hv_WindowHandle = null;
        MyCamera device = new MyCamera();
        MyCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        static string strShmPath = System.Environment.CurrentDirectory + "\\shm\\";
        static int NUMSHM = 4;
        HTuple[] hv_ModelID = new HTuple[NUMSHM];
        HObject[] ho_ModelContours = new HObject[NUMSHM];
        public int nQRPosition = 200;
        public int nModelPosition = 270;
        public string strQrcode = "Y001";
        public int bSaveLast = 1;
        public bool bFindQr = false;
        public bool bSaveImage = false;
        public string camID;
        object saveImageLockObj = new object();


        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
        public int MvsEmulateCamera(out int camIndex)
        {
            int nRet = MyCamera.MV_OK;
            camIndex = -1;
            // ch:枚举设备 | en:Enum device

            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }

            //Console.WriteLine("Enum device count : " + Convert.ToString(stDevList.nDeviceNum));

            if (0 == stDevList.nDeviceNum)
            {
                log4net.log.Info("未找到任何MVS相机");
                return 1;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < stDevList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chSerialNumber.Contains(camID))
                    {
                        camIndex = i;
                    }
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        log4net.log.Info("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        log4net.log.Info("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        log4net.log.Info("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        log4net.log.Info("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (stDevList.nDeviceNum != 0)
            {
                //cbDeviceList.SelectedIndex = 0;
                //bEmulated = true;
            }
            return nRet;
        }
        public int MvsOpenCamera(int index)
        {
            int nRet = MyCamera.MV_OK;
            MyCamera.MV_CC_DEVICE_INFO stDevInfo;
            Int32 nDevIndex = index;
            stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[nDevIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));

            // ch:创建设备 | en:Create device
            nRet = device.MV_CC_CreateDevice_NET(ref stDevInfo);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Create device failed:{0:x8}", nRet);
                return nRet;
            }

            // ch:打开设备 | en:Open device
            nRet = device.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Open device failed:{0:x8}", nRet);
                return nRet;
            }

            // ch:开始采集 | en:Start Grabbing
            nRet = device.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                return nRet;
            }

            return nRet;
        }

        public int MvsCloseCamera()
        {
            int nRet = 0;
            // ch:关闭设备 | en:Close device
            nRet = device.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Close device failed:{0:x8}", nRet);
                return nRet;
            }

            // ch:销毁设备 | en:Destroy device
            nRet = device.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Destroy device failed:{0:x8}", nRet);
            }
            return nRet;
        }
        public bool ReConnect()
        {
            MyCamera.MV_FRAME_OUT stFrameOut = new MyCamera.MV_FRAME_OUT();
            int nRet = device.MV_CC_GetImageBuffer_NET(ref stFrameOut, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                if (0 == MvsEmulateCamera(out int camIndex))
                {
                    if (camIndex == -1)
                    {
                        log4net.log.Error("未找到匹配的相机，重连失败");
                        return false;
                    }
                    log4net.log.Info("相机重连成功");
                    MvsOpenCamera(camIndex);
                    return true;
                }
                else
                {
                    log4net.log.Error("没有连接任何相机，重连失败");
                    return false;
                }
            }
            return true;
        }
        public bool MvsGrabOneImage()
        {
            int nRet = MyCamera.MV_OK;
            MyCamera.MV_FRAME_OUT stFrameOut = new MyCamera.MV_FRAME_OUT();

            IntPtr pImageBuf = IntPtr.Zero;
            int nImageBufSize = 0;

            HObject Hobj = new HObject();
            IntPtr pTemp = IntPtr.Zero;

            //while (m_bGrabbing)
            {

                try
                {
                    nRet = device.MV_CC_GetImageBuffer_NET(ref stFrameOut, 1000);
                    if (MyCamera.MV_OK == nRet)
                    {
                        if (stFrameOut.stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                        {
                            pTemp = stFrameOut.pBufAddr;
                        }
                        else
                        {
                            if (IntPtr.Zero == pImageBuf || nImageBufSize < (stFrameOut.stFrameInfo.nWidth * stFrameOut.stFrameInfo.nHeight))
                            {
                                if (pImageBuf != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(pImageBuf);
                                    pImageBuf = IntPtr.Zero;
                                }

                                pImageBuf = Marshal.AllocHGlobal((int)stFrameOut.stFrameInfo.nWidth * stFrameOut.stFrameInfo.nHeight);
                                if (IntPtr.Zero == pImageBuf)
                                {
                                    return false;
                                }
                                nImageBufSize = stFrameOut.stFrameInfo.nWidth * stFrameOut.stFrameInfo.nHeight;
                            }

                            MyCamera.MV_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                            stPixelConvertParam.pSrcData = stFrameOut.pBufAddr;//源数据
                            stPixelConvertParam.nWidth = stFrameOut.stFrameInfo.nWidth;//图像宽度
                            stPixelConvertParam.nHeight = stFrameOut.stFrameInfo.nHeight;//图像高度
                            stPixelConvertParam.enSrcPixelType = stFrameOut.stFrameInfo.enPixelType;//源数据的格式
                            stPixelConvertParam.nSrcDataLen = stFrameOut.stFrameInfo.nFrameLen;

                            stPixelConvertParam.nDstBufferSize = (uint)nImageBufSize;
                            stPixelConvertParam.pDstBuffer = pImageBuf;//转换后的数据
                            stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                            nRet = device.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
                            if (MyCamera.MV_OK != nRet)
                            {
                                return false;
                            }
                            pTemp = pImageBuf;
                        }
                        try
                        {
                            HOperatorSet.GenImage1Extern(out Hobj, "byte", stFrameOut.stFrameInfo.nWidth, stFrameOut.stFrameInfo.nHeight, pTemp, IntPtr.Zero);
                            log4net.log.Info("采集到一张图片：" + DateTime.Now.ToString());
                            //AppendLinkList(Hobj);
                            device.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                            return ProcessImageWorkThread(Hobj);
                        }
                        catch (System.Exception ex)
                        {
                            log4net.log.Info(ex.ToString());
                        }

                        device.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                    }
                    else
                    {
                        log4net.log.Info(string.Format("mvs usb3.0相机采图失败:{0:x8}", nRet));

                    }
                }
                catch (Exception ex)
                {
                    log4net.log.Info("MV_CC_GetImageBuffer_NET" + ex.ToString());
                }


                if (pImageBuf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pImageBuf);
                    pImageBuf = IntPtr.Zero;
                }
            }
            return false;
        }
        public void WriteImageThreadFunc(object obj)
        {
            lock (saveImageLockObj)
            {
                try
                {
                    object tempObj = obj;
                    HObject ho_Image;
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    ho_Image.Dispose();
                    HOperatorSet.CopyImage((HObject)tempObj, out ho_Image);
                    string sTemp;
                    sTemp = string.Format("{0}\\images\\{1}.bmp", System.Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
                    HOperatorSet.WriteImage(ho_Image, "bmp", 0, sTemp);
                    ho_Image.Dispose();
                    ClearMemory();
                }
                catch (Exception ex)
                {
                    //Console.Write(ex.ToString());
                    log4net.log.Info(ex.ToString());
                    //throw;
                }
            }
        }
        public bool ReadAllShapeModel(string szProductName)
        {
            try
            {
                for (int i = 0; i < NUMSHM; i++)
                {
                    HOperatorSet.ReadShapeModel(strShmPath + szProductName + "_" + (i + 1).ToString() + ".jpg.shm", out hv_ModelID[i]);
                    HOperatorSet.GenEmptyObj(out ho_ModelContours[i]);
                    ho_ModelContours[i].Dispose();
                    HOperatorSet.GetShapeModelContours(out ho_ModelContours[i], hv_ModelID[i], 1);
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                log4net.log.Info(ex.ToString());
                return false;
                //throw;
            }
        }
        public void ClearAllShapeModel()
        {
            try
            {
                for (int i = 0; i < NUMSHM; i++)
                {
                    if (hv_ModelID[i] != null)
                    {
                        HOperatorSet.ClearShapeModel(hv_ModelID[i]);
                        hv_ModelID[i] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.log.Info(ex.ToString());
                return;
                //throw;
            }

        }
        public bool ProcessImageWorkThread(object obj)
        {
            bool bRet = false;
            // Local control variables 
            HObject ho_Image, ho_ContoursAffineTrans = null;
            HObject ho_Rectangle = null, ho_ImageReduced = null, ho_SymbolXLDs = null;
            HObject ho_ModelRectangle = null, ho_ModelReduced = null;
            HTuple hv_t1 = null, hv_Row = null, hv_Column = null, hv_Angle = null;
            HTuple hv_Score = null, hv_Length = null, hv_HomMat2D = new HTuple();
            HTuple hv_t2 = null, hv_Time = null;
            HTuple hv_ResultHandles = null, hv_Scale = null;
            HTuple hv_DecodedDataStrings = null;
            HTuple hv_DataCodeHandle = null;
            HTuple hv_Area, hv_Row1, hv_Column1, hv_PointOrder, hv_num;
            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ModelRectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelReduced);
            try
            {
                //lock (processobj)
                {
                    
                    ho_Image.Dispose();
                    ho_ContoursAffineTrans.Dispose();
                    ho_SymbolXLDs.Dispose();
                    ho_Rectangle.Dispose();
                    ho_ModelRectangle.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_ModelReduced.Dispose();
                    //获取图片内容
                    HOperatorSet.CopyImage((HObject)obj, out ho_Image);
                    //HOperatorSet.DispObj(ho_Image, hv_WindowHandle);

                    //开始计时
                    bFindQr = false;
                    HOperatorSet.CountSeconds(out hv_t1);

                    //读二维码
                    HOperatorSet.GenRectangle1(out ho_Rectangle, nQRPosition, 10, nQRPosition + 180, 1270);
                    HOperatorSet.GenRectangle1(out ho_ModelRectangle, nModelPosition, 0, nModelPosition + 800, 1440);
                    //HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, 1080, 1440);
                    //HOperatorSet.SetColor(hv_WindowHandle, "blue");
                    //HOperatorSet.SetDraw(hv_WindowHandle, "margin");
                    //HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandle);
                    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
                    HOperatorSet.ReduceDomain(ho_Image, ho_ModelRectangle, out ho_ModelReduced);
                    HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out hv_DataCodeHandle);
                    //HOperatorSet.FindDataCode2d(ho_ImageReduced, out ho_SymbolXLDs, hv_DataCodeHandle, "stop_after_result_num",
                    //    7, out hv_ResultHandles, out hv_DecodedDataStrings);
                    //设置参数
                    //HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "default_parameters", "standard_recognition");
                    HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "default_parameters", "enhanced_recognition");
                    HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "timeout", 100);
                    HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "polarity", "dark_on_light");
                    //找码
                    HOperatorSet.FindDataCode2d(ho_ImageReduced, out ho_SymbolXLDs, hv_DataCodeHandle, "stop_after_result_num",
                        3, out hv_ResultHandles, out hv_DecodedDataStrings);

                    //清除模板
                    HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
                    HOperatorSet.CountSeconds(out hv_t2);
                    hv_Time = hv_t2 - hv_t1;
                    //HOperatorSet.DispText(hv_WindowHandle, hv_Time, "window", 30, 10, "black", new HTuple(), new HTuple());
                    if (hv_DecodedDataStrings.Length >= 1)
                    {
                        HOperatorSet.AreaCenterXld(ho_SymbolXLDs, out hv_Area, out hv_Row1, out hv_Column1, out hv_PointOrder);
                        HOperatorSet.CountObj(ho_SymbolXLDs, out hv_num);
                        //...
                        log4net.log.Info("读到二维码：" + hv_DecodedDataStrings.Length.ToString() + "个");
                        //HOperatorSet.DispObj(ho_SymbolXLDs, hv_WindowHandle);
                        //HOperatorSet.DispText(hv_WindowHandle, hv_DecodedDataStrings, "window", 100, 12, "black", new HTuple(), new HTuple());
                        for (int i = 0; i < hv_DecodedDataStrings.Length; i++)
                        {
                            //HOperatorSet.DispText(hv_WindowHandle, hv_DecodedDataStrings[i], "image", hv_Row1[i], hv_Column1[i], "black", new HTuple(), new HTuple());
                            log4net.log.Info(string.Format("二维码内容：{0}，坐标：{1},{2}", (string)hv_DecodedDataStrings.SArr[i],
                                hv_Row1[i].D, hv_Column1[i].D));
                            if (true == ((string)hv_DecodedDataStrings.SArr[i]).Contains(this.strQrcode))
                            {
                                if (bSaveImage)
                                {
                                    Thread th = new Thread(() => WriteImageThreadFunc(ho_Image));
                                    th.Start();
                                }
                                bFindQr = true;
                            }
                        }
                    }
                    else
                    {
                        //HOperatorSet.DispText(hv_WindowHandle, "未找到二维码", "window", 100, 12, "black", new HTuple(), new HTuple());
                        ClearMemory();
                        log4net.log.Info("未找到二维码");
                        return false;
                    }

                    //找模板
                    if (true == bFindQr)
                    {
                        log4net.log.Info("开始匹配模板");
                        bool bFindShm = false;
                        for (int i = 0; i < NUMSHM; i++)
                        {
                            //HOperatorSet.FindScaledShapeModel(ho_ModelReduced, hv_ModelID[i], (new HTuple(-90)).TupleRad() , (new HTuple(270)).TupleRad(), 0.7, 1.1, 0.3, 1, 0.5, "interpolation", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                            //HOperatorSet.FindShapeModel(ho_ModelReduced, hv_ModelID[i], -0.39, 0.79, 0.3, 1, 0.5, "least_squares", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                            HOperatorSet.FindShapeModel(ho_ModelReduced, hv_ModelID[i], (new HTuple(-90)).TupleRad(), (new HTuple(270)).TupleRad(), 0.3, 1, 0.5, "least_squares", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                            HOperatorSet.TupleLength(hv_Row, out hv_Length);
                            if (hv_Length.D > 0)
                            {
                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_Row, hv_Column, hv_Angle, out hv_HomMat2D);
                                ho_ContoursAffineTrans.Dispose();
                                HOperatorSet.AffineTransContourXld(ho_ModelContours[i], out ho_ContoursAffineTrans, hv_HomMat2D);
                                //HOperatorSet.DispObj(ho_ContoursAffineTrans, hv_WindowHandle);
                                bFindShm = true;
                                //HOperatorSet.DispText(hv_WindowHandle, "Find at shm number" + (i + 1).ToString(), "window", 10, 10, "black", new HTuple(), new HTuple());
                                log4net.log.Info("在第" + i.ToString() + "个模板匹配到药品");
                                break;
                            }
                        }

                        HOperatorSet.CountSeconds(out hv_t2);
                        hv_Time = hv_t2 - hv_t1;
                        //HOperatorSet.DispText(hv_WindowHandle, hv_Time, "window", 30, 10, "black", new HTuple(), new HTuple());

                        if (bFindShm == true)
                        {
                            log4net.log.Info("找到匹配药品");
                            //保存最后一张照片
                            //if (1 == bSaveLast)
                            //{
                            //    Thread th = new Thread(() => WriteImageThreadFunc(ho_Image));
                            //    th.Start();
                            //    //string sTemp;
                            //    //sTemp = string.Format("{0}\\images\\{1}.jpg", System.Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
                            //    //HOperatorSet.WriteImage(ho_Image, "jpg", 0, sTemp);
                            //}
                            //dump window
                            //string sTemp = string.Format("{0}\\images\\dump_{1}.jpg", System.Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
                            //HOperatorSet.DumpWindow(hv_WindowHandle, "jpeg", sTemp);
                            //...
                            bRet = true;
                        }
                        else
                        {
                            log4net.log.Info("药品跟二维码不匹配");
                            //HOperatorSet.DispText(hv_WindowHandle, "药品跟二维码不匹配", "window", 10, 10, "black", new HTuple(), new HTuple());
                        }
                    }
                    
                    //log4net.log.Info("结束图像算法");
                }
            }
            catch (Exception ex)
            {
                log4net.log.Info(ex.ToString());
                //Console.WriteLine(ex.ToString());
                //throw;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ContoursAffineTrans.Dispose();
                ho_SymbolXLDs.Dispose();
                ho_Rectangle.Dispose();
                ho_ModelRectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_ModelReduced.Dispose();
            }
            return bRet;
        }
        public bool FindMedicineProcedure()
        {
            return MvsGrabOneImage();
        }
    }

    class webCamera
    {
        private const int WM_USER = 0x400;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CAP_START = WM_USER;
        private const int WM_CAP_STOP = WM_CAP_START + 68;
        private const int WM_CAP_DRIVER_CONNECT = WM_CAP_START + 10;
        private const int WM_CAP_DRIVER_DISCONNECT = WM_CAP_START + 11;
        private const int WM_CAP_SAVEDIB = WM_CAP_START + 25;
        private const int WM_CAP_GRAB_FRAME = WM_CAP_START + 60;
        private const int WM_CAP_SEQUENCE = WM_CAP_START + 62;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEA = WM_CAP_START + 20;
        private const int WM_CAP_SEQUENCE_NOFILE = WM_CAP_START + 63;
        private const int WM_CAP_SET_OVERLAY = WM_CAP_START + 51;
        private const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;
        private const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = WM_CAP_START + 6;
        private const int WM_CAP_SET_CALLBACK_ERROR = WM_CAP_START + 2;
        private const int WM_CAP_SET_CALLBACK_STATUSA = WM_CAP_START + 3;
        private const int WM_CAP_SET_CALLBACK_FRAME = WM_CAP_START + 5;
        private const int WM_CAP_SET_SCALE = WM_CAP_START + 53;
        private const int WM_CAP_SET_PREVIEWRATE = WM_CAP_START + 52;
        private IntPtr hWndC;
        private bool bStat = false;
        private IntPtr mControlPtr;
        private int mWidth;
        private int mHeight;
        private int mLeft;
        private int mTop;
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <param name="handle">控件的句柄</param>
        /// <param name="left">开始显示的左边距</param>
        /// <param name="top">开始显示的上边距</param>
        /// <param name="width">要显示的宽度</param>
        /// <param name="height">要显示的长度</param>
        public void Init(IntPtr handle, int left, int top, int width, int height)
        {
            mControlPtr = handle;
            mWidth = width;
            mHeight = height;
            mLeft = left;
            mTop = top;
        }
        [DllImport("avicap32.dll")]
        private static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);
        [DllImport("avicap32.dll")]
        private static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);
        [DllImport("User32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, long lParam);
        /// <summary>
        /// 开始显示图像
        /// </summary>
        public void Start()
        {
            if (bStat)
                return;
            bStat = true;
            byte[] lpszName = new byte[100];
            hWndC = capCreateCaptureWindowA(lpszName, WS_CHILD | WS_VISIBLE, mLeft, mTop, mWidth, mHeight, mControlPtr, 0);
            if (hWndC.ToInt32() != 0)
            {
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_VIDEOSTREAM, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_ERROR, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_STATUSA, 0, 0);
                SendMessage(hWndC, WM_CAP_DRIVER_CONNECT, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_SCALE, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEWRATE, 66, 0);
                SendMessage(hWndC, WM_CAP_SET_OVERLAY, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEW, 1, 0);
            }
            return;
        }
        /// <summary>
        /// 停止显示
        /// </summary>
        public void Stop()
        {
            SendMessage(hWndC, WM_CAP_DRIVER_DISCONNECT, 0, 0);
            bStat = false;
        }
        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="path">要保存bmp文件的路径</param>
        public void GrabImage(string path)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
            SendMessage(hWndC, WM_CAP_SAVEDIB, 0, hBmp.ToInt64());
        }
        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="path">要保存avi文件的路径</param>
        public void Kinescope(string path)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
            SendMessage(hWndC, WM_CAP_FILE_SET_CAPTURE_FILEA, 0, hBmp.ToInt64());
            SendMessage(hWndC, WM_CAP_SEQUENCE, 0, 0);
        }
        /// <summary>
        /// 停止录像
        /// </summary>
        public void StopKinescope()
        {
            SendMessage(hWndC, WM_CAP_STOP, 0, 0);
        }
    }
}
