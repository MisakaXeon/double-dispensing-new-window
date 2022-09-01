using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace WindowsApiLib.API
{
    public class WindowsAPI
    {
        public delegate bool EnumWindowsProc(IntPtr pHandle, int p_Param);
        public delegate bool CallBack(IntPtr pwnd, int lParam);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct STRINGBUFFER
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string szText;
        }

        private struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }
        public const uint KEYEVENTF_KEYUP = 2u;
        public const uint KEYEVENTF_KEYDOWN = 0u;
        private const int STRINGBUFFER_SizeConst = 512;
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
         IntPtr hwnd,               // Window to copy,Handle to the window that will be copied. 
         IntPtr hdcBlt,             // HDC to print into,Handle to the device context. 
         UInt32 nFlags              // Optional flags,Specifies the drawing options. It can be one of the following values. 
         );
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern bool BitBlt(
             IntPtr hdcDest,     //目标设备的句柄
             int XDest,          //目标对象的左上角X坐标
             int YDest,          //目标对象的左上角的Y坐标
             int Width,          //目标对象的宽度
             int Height,         //目标对象的高度
             IntPtr hdcScr,      //源设备的句柄
             int XScr,           //源设备的左上角X坐标
             int YScr,           //源设备的左上角Y坐标
             Int32 drRop         //光栅的操作值
            );
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobjBm);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        public static Bitmap GetDesktop()
        {
            IntPtr dC = WindowsAPI.GetDC(WindowsAPI.GetDesktopWindow());
            IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
            int systemMetrics = WindowsAPI.GetSystemMetrics(0);
            int systemMetrics2 = WindowsAPI.GetSystemMetrics(1);
            IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, systemMetrics, systemMetrics2);
            Bitmap result;
            if (intPtr2 != IntPtr.Zero)
            {
                IntPtr hgdiobjBm = WindowsAPI.SelectObject(intPtr, intPtr2);
                WindowsAPI.BitBlt(intPtr, 0, 0, systemMetrics, systemMetrics2, dC, 0, 0, 13369376);
                WindowsAPI.SelectObject(intPtr, hgdiobjBm);
                WindowsAPI.DeleteDC(intPtr);
                WindowsAPI.ReleaseDC(WindowsAPI.GetDesktopWindow(), dC);
                Bitmap bitmap = Image.FromHbitmap(intPtr2);
                WindowsAPI.DeleteObject(intPtr2);
                GC.Collect();
                result = bitmap;
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 截取桌面指定区域的矩形
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Bitmap GetDesktop(System.Drawing.Rectangle rectangle)
        {
            IntPtr dC = WindowsAPI.GetDC(WindowsAPI.GetDesktopWindow());
            IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
            IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, rectangle.Width, rectangle.Height);
            Bitmap result;
            if (intPtr2 != IntPtr.Zero)
            {
                IntPtr hgdiobjBm = WindowsAPI.SelectObject(intPtr, intPtr2);
                WindowsAPI.BitBlt(intPtr, 0, 0, rectangle.Width, rectangle.Height, dC, rectangle.X, rectangle.Y, 13369376);
                //WindowsAPI.SelectObject(intPtr, hgdiobjBm);
                WindowsAPI.DeleteDC(intPtr);
                WindowsAPI.ReleaseDC(WindowsAPI.GetDesktopWindow(), dC);
                Bitmap bitmap = Image.FromHbitmap(intPtr2);
                DeleteObject(intPtr2);
                result = bitmap;
                bitmap = null;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static Bitmap GetInterPtrMap(IntPtr dC, System.Drawing.Rectangle rectangle)
        {
            //IntPtr dC = WindowsAPI.GetDC(WindowsAPI.GetDesktopWindow());
            IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
            IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, rectangle.Width, rectangle.Height);
            Bitmap result;
            if (intPtr2 != IntPtr.Zero)
            {
                IntPtr hgdiobjBm = WindowsAPI.SelectObject(intPtr, intPtr2);
                WindowsAPI.BitBlt(intPtr, 0, 0, rectangle.Width, rectangle.Height, dC, rectangle.X, rectangle.Y, 13369376);
                //WindowsAPI.SelectObject(intPtr, hgdiobjBm);
                WindowsAPI.DeleteDC(intPtr);
                //WindowsAPI.ReleaseDC(WindowsAPI.GetDesktopWindow(), dC);
                Bitmap bitmap = Image.FromHbitmap(intPtr2);
                DeleteObject(intPtr2);
                result = bitmap;
                bitmap = null;
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 设置目标窗体大小，位置
        /// </summary>
        /// <param name="hWnd">目标句柄</param>
        /// <param name="x">目标窗体新位置X轴坐标</param>
        /// <param name="y">目标窗体新位置Y轴坐标</param>
        /// <param name="nWidth">目标窗体新宽度</param>
        /// <param name="nHeight">目标窗体新高度</param>
        /// <param name="BRePaint">是否刷新窗体</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        ///// <summary>
        ///// 截取桌面指定区域的bitmap
        ///// </summary>
        ///// <param name="rectangle"></param>
        ///// <returns></returns>
        //public static Bitmap GetRectBitmap(Rectangle rectangle)
        //{

        //}
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        [DllImport("user32.dll")]
        public static extern void keybd_event(EnumKeyboardKey bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern byte MapVirtualKey(EnumKeyboardKey wCode, int wMap);
        public static void KeyDown(EnumKeyboardKey key)
        {
            WindowsAPI.keybd_event(key, WindowsAPI.MapVirtualKey(key, 0), 0u, 0u);
        }
        public static void KeyUp(EnumKeyboardKey key)
        {
            WindowsAPI.keybd_event(key, WindowsAPI.MapVirtualKey(key, 0), 2u, 0u);
        }
        public static void KeyPress(EnumKeyboardKey key)
        {
            WindowsAPI.keybd_event(key, WindowsAPI.MapVirtualKey(key, 0), 0u, 0u);
            Thread.Sleep(0);
            WindowsAPI.keybd_event(key, WindowsAPI.MapVirtualKey(key, 0), 2u, 0u);
        }
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            int hWnd,　　　// handle to destination window 
            int Msg,　　　 // message 
            int wParam,　// first message parameter 
            int lParam // second message parameter 
        );
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        [DllImport("user32")]
        public static extern void mouse_event(EnumVirtualDeviceActionType dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, StringBuilder lpszClass, StringBuilder lpszWindow);
        [DllImport("user32.dll")]
        public static extern int EnumWindows(WindowsAPI.EnumWindowsProc ewp, int lParam);
        /// <summary>
        /// 查找窗口句柄根据窗口Text
        /// </summary>
        /// <param name="windowText"></param>
        /// <returns></returns>
        public static IntPtr FindWindowHandleByName(string windowText)
        {
            IntPtr handle = IntPtr.Zero;
            ///筛选模拟器
            EnumWindows((ptr, para) =>
            {
                string name = "";
                WindowsAPI.GetWindowText(ptr, out name);
                if (!name.Equals(windowText))
                {
                    return true;
                }
                else
                {
                    handle = ptr;
                    return false;
                }
            }, 0);
            return handle;
        }
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, WindowsAPI.CallBack lpfn, int lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, out WindowsAPI.STRINGBUFFER text, int nMaxCount);
        public static int GetWindowText(IntPtr hWnd, out string text)
        {
            WindowsAPI.STRINGBUFFER sTRINGBUFFER = default(WindowsAPI.STRINGBUFFER);
            int windowText = WindowsAPI.GetWindowText(hWnd, out sTRINGBUFFER, 512);
            text = sTRINGBUFFER.szText;
            return windowText;
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, out WindowsAPI.STRINGBUFFER lpString, int nMaxCount);
        public static int GetClassName(IntPtr hWnd, out string lpString)
        {
            WindowsAPI.STRINGBUFFER sTRINGBUFFER = default(WindowsAPI.STRINGBUFFER);
            int className = WindowsAPI.GetClassName(hWnd, out sTRINGBUFFER, 512);
            lpString = sTRINGBUFFER.szText;
            return className;
        }
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(
        IntPtr hWnd, // 窗口句柄
        out RECT lpRect // 客户区坐标
        );
        public static bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect)
        {
            RECT rect = default(RECT);
            WindowsAPI.GetWindowRect(hWnd, out rect);
            lpRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Down - rect.Top);
            return true;
        }
        public static bool GetClientRect(IntPtr hWnd, out Rectangle lpRect)
        {
            RECT rect = default(RECT);
            WindowsAPI.GetClientRect(hWnd, out rect);
            lpRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Down - rect.Top);
            return true;
        }
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern int GetWindow(int hwnd, int wCmd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        //切换窗体显示
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        //public static int GetCursorPos(ref Point lpPoint)
        //{
        //    POINT pOINT = new POINT() { x = 0, y = 0 };
        //    int cursorPos = WindowsAPI.GetCursorPos(ref pOINT);
        //    lpPoint.X = (int)pOINT.x;
        //    lpPoint.Y = (int)pOINT.y;
        //    return cursorPos;
        //}
        [DllImport("user32.dll")]
        public static extern bool SetCurorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point Point);
        [DllImport("user32.dll")]
        private static extern IntPtr GetCursor();
        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out WindowsAPI.CURSORINFO pci);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();


        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        //使用此功能，通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // 取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);
        //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。 
                                         int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
                                         byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
                                         byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。 
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 

        //获取按键的状态
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetAsyncKeyState(int vKey);

        /// <summary>
        /// 设置大写锁定
        /// </summary>
        public static void SetCapital(bool isUpper)
        {
            bool bOn = (GetKeyState((int)EnumKeyboardKey.Capital) & 1) == 1;
            if(bOn != isUpper)
            {
                WindowsAPI.KeyDown(EnumKeyboardKey.Capital);
                Thread.Sleep(20);
                WindowsAPI.KeyUp(EnumKeyboardKey.Capital);
            }
        }

        //用户获取当前输入法句柄
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        //用于获取当前输入法的状态
        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC,
            ref int conversion, ref int sentence);

        //用于设置输入法的状态
        [DllImport("imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int conversion, int sentence);
        /// <summary>
        /// 设置中文输入法
        /// </summary>
        public static bool SetChineseImm(IntPtr handle)
        {
            IntPtr prt = ImmGetContext(handle);
            int curIMode = 0;
            int curISentence = 0;
            ImmGetConversionStatus(prt, ref curIMode, ref curISentence);
            if ((curIMode != 1025 || curISentence != 0))
            {
                //如果是中文则切换成中文输入
                int iMode = 1025;
                int iSentence = 0;
                if (!ImmSetConversionStatus(prt, iMode, iSentence))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 设置英文输入法
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool SetEnglishImm(IntPtr handle)
        {
            IntPtr prt = ImmGetContext(handle);
            int curIMode = 0;
            int curISentence = 0;
            ImmGetConversionStatus(prt, ref curIMode, ref curISentence);
            if ((curIMode != 0 || curISentence != 0))
            {

                //是英文则切换成英文输入
                int iMode = 0;
                int iSentence = 0;
                if (!ImmSetConversionStatus(prt, iMode, iSentence))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public enum EnumKeyboardKey : byte
    {
        Back = 8,
        Tab,
        Clear = 12,
        Return,
        ShiftLeft = 160,
        ControlLeft = 162,
        ShiftRight = 161,
        ControlRight = 163,
        AltLeft,
        AltRight,
        Menu = 18,
        Pause,
        Capital,
        Escape = 27,
        Space = 32,
        Prior,
        Next,
        End,
        Home,
        Left,
        Up,
        Right,
        Down,
        Select,
        Print,
        Execute,
        Snapshot,
        Insert,
        Delete,
        Help,
        D0,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        A = 65,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LWindows,
        RWindows,
        Apps,
        NumPad0 = 96,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        Multiply,
        Add,
        Separator,
        Subtract,
        Decimal,
        Divide,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        NumLock = 144,
        Scroll
    }
    public enum EnumVirtualDeviceActionType
    {
        Delay = 33024,
        KeyDown = 33280,
        KeyUp = 33536,
        KeyPress = 30467,
        MoveTo = 32769,
        Move = 1,
        Scroll = 2048,
        LeftDown = 2,
        LeftUp = 4,
        LeftClick = 6,
        RightDown = 8,
        RightUp = 16,
        RightClick = 24,
        MiddleDown = 32,
        MiddleUp = 64,
        MiddleClick = 96
    }
}
