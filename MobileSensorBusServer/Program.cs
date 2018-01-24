using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using AsynchSocketServiceUILib;

namespace MobileSensorBusServer
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
            MobileAppServiceUIForm MyMobileAppServiceUIForm = new MobileAppServiceUIForm();
            //MyMobileAppServiceUIForm.SetTCPListerPort(8920);
            MyMobileAppServiceUIForm.SetMainFormIcon(MobileAppTypeID.Windows);
            Application.Run(MyMobileAppServiceUIForm);
        }
    }
}
