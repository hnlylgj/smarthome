using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
     public class CreateMobileID
    {
         string RandomPreStr = "110";
         int nCount1 = 100000;
         int nCount2 = 1000000;
         
         public string GetMobileID()
         {
             Random MyRandom = new Random();
             //int MyNumber1 = MyRandom.Next(100000, 1000000);
             //int MyNumber2 = MyRandom.Next(100000, 1000000);
             int MyNumber1 = MyRandom.Next(nCount1, nCount2);
             int MyNumber2 = MyRandom.Next(nCount1, nCount2);

             double MyDNumber = Convert.ToDouble(MyNumber1.ToString() + MyNumber2.ToString());
             string Random15Str = RandomPreStr + MyDNumber.ToString();
             return Random15Str;
         }
         public string GetMobileIDCode6()
         {
             //6位的随机数字
             Random NewRandom = new Random(System.Environment.TickCount);
             int MyGetRandom = NewRandom.Next(100000, 999999);
             return MyGetRandom.ToString();
         }
         public string GetMobileIDNumberCode8()
         {
             //8位的随机数字
             Random NewRandom = new Random(System.Environment.TickCount);
             int MyGetRandom = NewRandom.Next(10000000, 99999999);
             return MyGetRandom.ToString();
         }
         public string GetMobileIDNumberCode15()
         {
             //15位的随机数字
             Random NewRandom = new Random(System.Environment.TickCount);
             int MyGetRandom1 = NewRandom.Next(10000000, 99999999);//8位
             int MyGetRandom2 = NewRandom.Next(100000, 999999);//6位
             int MyGetRandom3 = NewRandom.Next(1, 9);//1位
             return MyGetRandom3.ToString() + MyGetRandom1.ToString() + MyGetRandom2.ToString();

         }


            
    }
}
