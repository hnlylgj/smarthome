using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsynchSocketServiceUILib;

namespace SmartWareServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CloudLockServiceUIForm MyCloudLockServiceUIForm=new CloudLockServiceUIForm();
            
            Application.Run(MyCloudLockServiceUIForm);
        }
    }
}
