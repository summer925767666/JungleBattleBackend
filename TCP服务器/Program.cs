using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP服务器
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        //同步接收消息
        private static void StartServerSync()
        {
            //创建server Socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //本机IP：192.168.1.78 127.0.0.1
            //IpAdress xxx.xxx.xxx.xxx  IpEndPoint xxx.xxx.xxx.xxx:port
            IPAddress ipAddress = IPAddress.Parse("192.168.1.78");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 88);
            serverSocket.Bind(ipEndPoint);//绑定ip和端口号
            serverSocket.Listen(0);//开始监听端口号

            //接收一个客户端连接
            Socket clientSocket = serverSocket.Accept();

            //向客户端发送一条消息
            string msg = "Hello client! 你好";
            byte[] data = Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);

            //接收客户端的一条消息
            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);
            string msgReceive = Encoding.UTF8.GetString(dataBuffer, 0, count);
            Console.WriteLine("接收到了客户端的一条信息：" + msgReceive);

            clientSocket.Close();
            serverSocket.Close();
            
        }

        //异步接收客户端连接
        private static void StartServerAsync()
        {
            //创建server Socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("192.168.1.78");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 88);
            serverSocket.Bind(ipEndPoint);//绑定ip和端口号
            serverSocket.Listen(0);//开始监听端口号

            //异步接收客户端连接
            serverSocket.BeginAccept(AcceptCallBack,serverSocket);
        }

        private static readonly Message Msg = new Message();
        //连接客户端异步回调
        private static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);

            //向客户端发送一条消息
            const string msgSend = "Hello client! 你好";
            byte[] dataSend = Encoding.UTF8.GetBytes(msgSend);
            clientSocket.Send(dataSend);

            //异步接收客户端的消息
            clientSocket.BeginReceive(Msg.Data, Msg.StartIndex, Msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);

            //处理下一个客户端的连接
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        //接收消息异步回调
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = ar.AsyncState as Socket;

            //客户端非正常关闭，会导致EndReceive()方法异常
            //使用try catch语句，避免客户端非正常关闭导致的服务器异常
            try
            {
                int count = clientSocket.EndReceive(ar);

                if (count==0)//客户端正常关闭时，会收到0字节的消息
                {
                    clientSocket.Close();
                    return;
                }
                Msg.AddCount(count);

//                string msgReceive = Encoding.UTF8.GetString(DataBuffer, 0, count);
//                Console.WriteLine("接收到了客户端的一条信息：" + msgReceive);
                Msg.Read();

                clientSocket.BeginReceive(Msg.Data, Msg.StartIndex, Msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine("异常："+ e);
                if (clientSocket != null) clientSocket.Close();
            }
        }
    }
}
