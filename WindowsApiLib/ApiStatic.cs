using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApiLib.API
{
    public static class ApiStatic
    {
        // size of a device name string
        public const int CCHDEVICENAME = 32;

        public const int WM_COPYDATA = 0x004A;

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
