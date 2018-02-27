using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP客户端
{
    public class Message
    {
        public static byte[] GetBytes(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int count = dataBytes.Length;
            byte[] countBytes = BitConverter.GetBytes(count);
            byte[] newDataBytes = countBytes.Concat(dataBytes).ToArray();
            return newDataBytes;
        }
    }
}
