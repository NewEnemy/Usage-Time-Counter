using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeCounter
{
    class ActiveWindow
    {
        
        [DllImport("user32.dll", CharSet = CharSet.Auto,ExactSpelling =true)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto,SetLastError =true)]
        public static extern int GetWindowThreadProcessId(IntPtr Handle, out int ProcessId);
        
        public string GetActive()
        {
            IntPtr Window_handle = GetForegroundWindow();
            int ProcessId = 0;
            GetWindowThreadProcessId(Window_handle, out ProcessId);
           
            


            return Process.GetProcessById(ProcessId).ProcessName;
        }
    }

}
