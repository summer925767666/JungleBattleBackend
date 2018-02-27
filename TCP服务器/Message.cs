using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP服务器
{
    internal class Message
    {
        private  byte[] _data=new byte[1024];//存储数据的数组
        private  int _dataCount = 0;//表示已经存储的数据个数

        public byte[] Data { get { return _data; } }
        public int StartIndex { get { return _dataCount; } }
        public int RemainSize { get { return _data.Length - _dataCount; } }

        /// <summary>
        /// 增加数据个数
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            _dataCount += count;
        }

        /// <summary>
        /// 解析读取数据
        /// </summary>
        public void Read()
        {
            while (true)
            {
                if (_dataCount <= 4) return;
                int msgCount = BitConverter.ToInt32(_data, 0);
                if (_dataCount < msgCount + 4) return;

                //解析数据
                string msg = Encoding.UTF8.GetString(_data, 4, msgCount);
                Console.WriteLine("解析出来一条数据：" + msg);

                //更新数据个数，更新数组
                _dataCount -= msgCount + 4;
                Array.Copy(_data, msgCount + 4, _data, 0, _dataCount);
            }
        }
    }
}
