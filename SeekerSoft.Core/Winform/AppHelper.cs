using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SeekerSoft.Core.Winform
{
    public class AppHelper
    {
        /// <summary>
        /// 检查当前进程是否已存在
        /// </summary>
        /// <returns></returns>
        public static bool CurrentExists()
        {
            return (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1);
        }

        /// <summary>
        /// 保证只有一个应用程序在运行
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public static bool CheckApplicationSingle(int millisecondsTimeout = 2000)
        {
            if (CurrentExists())
            {
                if (millisecondsTimeout > 0)
                {
                    Thread.Sleep(millisecondsTimeout);// 延迟三秒后再试
                    if (!CurrentExists())
                    {
                        return true;
                    }
                }
                MessageBox.Show("程序正在运行中.\r\nThe application is running.");
                Application.Exit();
                return false;
            }
            return true;
        }
    }
}
