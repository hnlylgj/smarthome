using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService
{
    public class FindGarbageChannel
    {
        private int _TimeSpanValue;//分钟计算


        public FindGarbageChannel(int InTimeSpan)
        {
            this._TimeSpanValue = InTimeSpan;

        }

        public bool SelectGarbageChannel(LoginUser AnyLoginUser)
        {
            TimeSpan MyTimeSpan;
            MyTimeSpan = DateTime.Now.Subtract(AnyLoginUser.KeepTime);
            return MyTimeSpan.Minutes > this._TimeSpanValue;
        
            /*if (MyTimeSpan.Minutes > this._TimeSpanValue)
            {
                return true;
            }
            */

        }

      

    }
}
