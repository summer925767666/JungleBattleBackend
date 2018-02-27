using System;
using System.Linq;
using System.Text;
using Common;

namespace GameServer.Servers
{
    public class Message
    {
        /// 打包数据
        public static byte[] PackData(ActionCode actionCode,string data)
        {
            byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode);
            byte[] dataBytes=Encoding.UTF8.GetBytes(data);
            int dataAmount = requestCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes=BitConverter.GetBytes(dataAmount);

            return dataAmountBytes.Concat(requestCodeBytes).Concat(dataBytes).ToArray();
        }

        private byte[] data = new byte[1024];//存储数据的数组
        private int dataCount;//表示已经存储的数据个数

        /// 存储数据的数组
        public byte[] Data { get { return data; } }

        /// 存数数据时起始索引
        public int StartIndex { get { return dataCount; } }

        /// 数组剩余容量
        public int RemainSize { get { return data.Length - dataCount; } }

        /// 解析读取数据
        public void ReadMessage(int newDataAmount,Action<RequestCode,ActionCode,string> processDataCallback )
        {
            dataCount += newDataAmount;

            while (true)
            {
                if (dataCount <= 4) return;
                int msgCount = BitConverter.ToInt32(data, 0);
                if (dataCount < msgCount + 4) return;

                //解析数据
                RequestCode requestCode=(RequestCode)BitConverter.ToInt32(data,4);
                ActionCode actionCode=(ActionCode)BitConverter.ToInt32(data,8);
                string msg = Encoding.UTF8.GetString(data, 12, msgCount-8);

                processDataCallback(requestCode,actionCode,msg);

                //更新数据个数，更新数组
                dataCount -= msgCount + 4;
                Array.Copy(data, msgCount + 4, data, 0, dataCount);
            }
        }
    }
}
