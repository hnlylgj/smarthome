using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
    public class RandomRegisterCode
    {
        //数字+字母
        private const string CHAR = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string ErrorStr;

        /// <summary>
        /// 随机排序
        /// </summary>
        /// <param name="charList"></param>
        /// <returns></returns>
        private List<string> SortByRandom(List<string> charList)
        {
            Random rand = new Random();
            for (int i = 0; i < charList.Count; i++)
            {
                int index = rand.Next(0, charList.Count);
                string temp = charList[i];
                charList[i] = charList[index];
                charList[index] = temp;
            }

            return charList;
        }

        private void ShowError(string strError)
        {
            //MessageBox.Show(strError, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorStr = strError;
        }
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="len"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<string> GetRandString(int len, int count)
        {
            double max_value = Math.Pow(36, len);
            if (max_value > long.MaxValue)
            {
                ShowError(string.Format("Math.Pow(36, {0}) 超出 long最大值！", len));
                return null;
            }

            long all_count = (long)max_value;
            long stepLong = all_count / count;
            if (stepLong > int.MaxValue)
            {
                ShowError(string.Format("stepLong ({0}) 超出 int最大值！", stepLong));
                return null;
            }
            int step = (int)stepLong;
            if (step < 3)
            {
                ShowError("step 不能小于 3!");
                return null;
            }
            long begin = 0;
            List<string> list = new List<string>();
            Random rand = new Random();
            while (true)
            {
                long value = rand.Next(1, step) + begin;
                begin += step;
                list.Add(GetChart(len, value));
                if (list.Count == count)
                {
                    break;
                }
            }

            list = SortByRandom(list);

            return list;
        }
        /// <summary>
        /// 将数字转化成字符串
        /// </summary>
        /// <param name="len"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetChart(int len, long value)
        {
            StringBuilder str = new StringBuilder();
            while (true)
            {
                str.Append(CHAR[(int)(value % 36)]);
                value = value / 36;
                if (str.Length == len)
                {
                    break;
                }
            }

            return str.ToString();
        }

        public string CreateRegsisterCode()
        {
           //6位的随机字符串数
          string RandString= GetRandString(6, 2)[0];
          return RandString;
        }
        public string CreateRegsisterCode2()
        {
            //12位的随机字符串数
            string RandString = GetRandString(12, 2)[0];
            return RandString;
        }
        public string CreateRegsisterCode3()
        {
            //15位的随机字符串数
            string RandString = GetRandString(15, 2)[0];
            return RandString;
        }
        public string CreateRegsisterCode4()
        {
            //18位的随机字符串数
            string RandString = GetRandString(18, 2)[0];
            return RandString; 
        }
        public string CreateRegsisterNumberCode()
        {
            //6位的随机数字
            Random NewRandom = new Random(System.Environment.TickCount);
            int MyGetRandom = NewRandom.Next(100000, 999999);
            return MyGetRandom.ToString();
        }
       
       
    }
}
