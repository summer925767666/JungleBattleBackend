using System;
using GameServer.Servers;

namespace 游戏服务器端
{
    class Program
    {
        static void Main()
        {
            Server server = new Server("127.0.0.1", 6688);
            server.Start();

            Console.ReadKey();
        }
    }
}
