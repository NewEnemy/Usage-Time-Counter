using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCounter
{
    class Helpers
    {
        public  string UnixToTimeFormat(long sec)
        {
            int h = (int)sec / 3600;
            int m = (int)sec%3600 /60;
            int s = (int)sec%60;

            return string.Format("{0}:{1}:{2}", appendZero( h), appendZero(m), appendZero(s));
        }
        private string appendZero(int number)
        {
            if(number < 10)
            {
                return string.Format("0{0}", number);
            }

            
               return number.ToString();
            
        }
    }
}
