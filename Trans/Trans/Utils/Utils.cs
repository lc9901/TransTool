using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Trans.Utils
{
    class Utils
    {
        public static string GetMd5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] data2 = md5.ComputeHash(data);

            return GetByteToString(data2);
        }

        private static string GetByteToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var item in data)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
