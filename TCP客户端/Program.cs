﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace TCP客户端
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("192.168.1.78");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 88);

            clientSocket.Connect(ipEndPoint);

            //接收服务器的一条消息
            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);//Receive方法阻塞
            string msgReceive = Encoding.UTF8.GetString(dataBuffer, 0, count);
            Console.WriteLine("客户端接收到了服务器的一条消息："+msgReceive);

            #region 发送输入
            //            while (true)
            //            {
            //                //向服务器发送一条消息
            //                string msg = Console.ReadLine();
            //
            //                if (msg=="close")
            //                {
            //                    clientSocket.Close();
            //                    return;
            //                }
            //
            //                byte[] data = Encoding.UTF8.GetBytes(msg);
            //                clientSocket.Send(data);//Send方法不阻塞
            //            }
            #endregion

            #region 粘包测试
            for (int i = 0; i < 100; i++)
            {
                clientSocket.Send(Message.GetBytes(i.ToString()));
            }
            #endregion

            #region 分包测试
            //            const string s = "罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉" +
            //                             "斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类" +
            //                             "科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你" +
            //                             "老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来" +
            //                             "考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家" +
            //                             "格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各" +
            //                             "类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老" +
            //                             "公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试" +
            //                             "大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯" +
            //                             "家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿" +
            //                             "萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱" +
            //                             "斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款" +
            //                             "力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感" +
            //                             "就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开" +
            //                             "家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各" +
            //                             "类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老" +
            //                             "公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试" +
            //                             "大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家" +
            //                             "的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德" +
            //                             "离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击" +
            //                             "各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公" +
            //                             "卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲" +
            //                             "罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感" +
            //                             "就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家" +
            //                             "格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科" +
            //                             "技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱" +
            //                             "斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲罚款力度你老公卡纳莱斯打击各类科技阿萨德离开家格拉斯家的灵感就来考试大纲";
            //
            //            clientSocket.Send(Encoding.UTF8.GetBytes(s));
            #endregion

            Console.ReadKey();
        }
    }
}
