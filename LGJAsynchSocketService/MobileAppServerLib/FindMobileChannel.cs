using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.MobileAppServerLib
{
     public class FindMobileChannel
    {
         private string _LockID;
         private string _MobileID;
         private int FlagID;
       public FindMobileChannel(string LockID)
      {
            this.FlagID = 0;
            this._LockID=LockID;

      }
       public FindMobileChannel(string LockMobileID, int InFlagID)
       {
           FlagID = InFlagID;
           if (FlagID >0)
           {
               this._MobileID = LockMobileID;//1
           }
           else
           {
               this._LockID = LockMobileID;//0
           }


       }
       public bool BindedMobileChannel(LoginUser AnyLoginUser)
       {

           if (FlagID >0)
           {
               return AnyLoginUser.MobileID == this._MobileID;
             
           }
           else
           {
               return AnyLoginUser.LockID == this._LockID;
               
           }
          

       }
     
   }
}
