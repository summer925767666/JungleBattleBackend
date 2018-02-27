using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Common;
using GameServer.Controller;

namespace GameServer.Servers
{
    // 服务器类,管理所有的Client，所有的controller
    class Server
    {
        private IPEndPoint ipEndPoint; //ip和端口
        private Socket serverSocket; //服务器socket，管理和客户端的连接等

        private List<Client> clientList = new List<Client>(); //client类实例的数组
        private List<Room> roomList = new List<Room>();//所有的房间列表
        private ControllerManger controllerManger;//所有controller的管理类

        // 构造函数
        public Server(string ip, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);//设置ip和端口号
            controllerManger = new ControllerManger(this);
        }

        /// 开启服务器，创建服务器连接socket
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建socket
            serverSocket.Bind(ipEndPoint); //绑定ip和端口
            serverSocket.Listen(0); //开始监听，参数表示最大连接队列数，0表示不限制
            Debug.Log("启动服务器成功！");
            serverSocket.BeginAccept(AcceptCallBack, null); //异步连接
        }

        // 异步连接回调
        private void AcceptCallBack(IAsyncResult ar)
        {
            Debug.Log("有客户端连接");
            Socket clientSocket = serverSocket.EndAccept(ar);//创建客户端连接socket
            Client client = new Client(clientSocket, this);//创建客户端类，用于和客户端的通信
            client.StartReceiveSync();//开始接收消息
            lock (clientList)
            {
                clientList.Add(client);//添加到List，被server管理
            }
            serverSocket.BeginAccept(AcceptCallBack, null); //异步连接
        }

        // 移除Client
        public void RemoveClient(Client client)
        {
            lock (clientList)
            {
                if (clientList.Contains(client))
                {
                    clientList.Remove(client);
                }
            }
        }

        // 根据RequestCode，分派给不同的Handler处理请求
        public void HandleRequest(Client client, RequestCode requestCode, ActionCode actionCode, string data)
        {
            controllerManger.HandleRequest(client, requestCode, actionCode, data);
        }

        // 给客户端发送Response
        public void SendResponse(Client client, ActionCode actionCode, string data)
        {
            client.SendResponse(actionCode, data);
        }

        // 创建房间
        public void CraetRoom(Client client)
        {
            Room room = new Room(this);
            room.AddClient(client);

            lock (roomList)
            {
                roomList.Add(room);
            }
            
        }

        // 移除房间
        public void RemoveRoom(Room room)
        {
            if (room == null) return;
            if (roomList.Count <= 0) return;
            
            lock (roomList)
            {
                roomList.Remove(room);
            }
        }

        // 取得RoomList
        public List<Room> GetRoomList()
        {
            return roomList;
        }

        //根据id取得房间
        public Room GetRoomById(int id)
        {
            lock (roomList)
            {
                return roomList.FirstOrDefault(room => room.GetId() == id);
            }
        }
    }
}
