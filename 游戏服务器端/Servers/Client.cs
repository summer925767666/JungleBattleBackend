using System;
using System.Net.Sockets;
using Common;
using GameServer.DAO;
using GameServer.Model;
using GameServer.Tool;
using MySql.Data.MySqlClient;

namespace GameServer.Servers
{
    //管理和客户端的连接、通信
    class Client
    {
        private Message msg = new Message(); //Message实例，用于消息处理
        private Socket clientSocket; //和客户端通信的socket
        private Server server; //服务器实例
        private MySqlConnection mySqlConnection; //和数据库的连接

        #region 用户user数据

        private User user;
        private Result result;


        // 设置账户数据
        public void SetUserData(User user, Result result)
        {
            this.user = user;
            this.result = result;
        }

        // 获取用户数据
        public void GetUserData(out User user, out Result result)
        {
            user = this.user;
            result = this.result;
        }

        //获取用户ID
        public int GetUserId()
        {
            return user.Id;
        }

        //更新战绩
        public void UpdateResult(bool isVictory)
        {
            result.TotalCount++;
            if (isVictory)
            {
                result.WinCount++;
            }
            ResultDAO.UpdateResult(mySqlConnection, ref result);
        }

        #endregion

        #region 房间room数据
        //todo，感觉房间数据放在client不合理
        private Room room;
        public Room Room
        {
            get { return room; }
            set { room = value; }
        }

        //判断是否是房主
        public bool IsOwner()
        {
            return room.IsOwner(this);
        }

        #endregion

        //战斗数据
        public int Hp { get; set; }

        // 和数据库的连接
        public MySqlConnection MySqlConnection
        {
            get { return mySqlConnection; }
        }

        // 构造函数
        // clientSocket：和客户端通信的socket
        // server：服务器实例
        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mySqlConnection = ConnectHelper.Connect();
        }

        // 开始接收消息
        public void StartReceiveSync()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }

        // 接收消息的异步回调
        private void ReceiveCallBack(IAsyncResult ar)
        {
            //客户端非正常关闭，会导致EndReceive()方法异常
            //使用try catch语句，避免客户端非正常关闭导致的服务器异常
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0) //客户端正常关闭时，count==0，主动执行Close方法
                {
                    Close();
                }
                msg.ReadMessage(count, OnProcessMessage); //解析数据，解析出一条数据，调用一次OnProcessMessage回调
                StartReceiveSync();
            }
            catch (Exception e)
            {
                Debug.Log("异步接收客户端消息异常" + e);
//                Close();
            }
        }

        //解析数据的回调
        //调用server，处理解析出来的数据
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            server.HandleRequest(this, requestCode, actionCode, data);
        }

        // 向客户端发送Response
        public void SendResponse(ActionCode actionCode, string data)
        {
            if (clientSocket == null || clientSocket.Connected == false) return;

            byte[] responseData = Message.PackData(actionCode, data);
            clientSocket.Send(responseData);
        }

        // 关闭client
        private void Close()
        {
            server.RemoveClient(this);//Server中移除对自身的管理
            ConnectHelper.Close(mySqlConnection);//关闭和数据库的连接

            if (clientSocket != null)//关闭和客户端通信的socket
            {
                clientSocket.Close();
                Debug.Log("关闭客户端连接");
            }

            //如果在房间中，对房间进行处理
            //如果是房主，关闭所在的房间
            //如果不是房主，从房间中把自身移除
            //todo,最好在server中统一处理客户端关闭的其他逻辑
            if (room == null) return;

            if (IsOwner())
            {
                room.Close();
            }
            else
            {
                room.RemoveClient(this);
            }
        }
    }
}