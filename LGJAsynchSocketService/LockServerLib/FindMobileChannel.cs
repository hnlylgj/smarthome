using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
     public class FindMobileChannel
    {
        private string _MobileID;
        public FindMobileChannel(string MobileID)
        {
            this._MobileID = MobileID;

        }

        public bool BindedMobileChannel(LoginUser AnyLoginUser)
        {
            if (AnyLoginUser.MobileID == this._MobileID && AnyLoginUser.LockID == "*********")
            {
                return true; 
            }
            else
            {
                return false; 
            }

           
        }


        public bool SelectMobileChannel(LoginUser AnyLoginUser)
        {
            if (AnyLoginUser.MobileID == this._MobileID)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
