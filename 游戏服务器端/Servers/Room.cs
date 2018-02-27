using System.Collections.Generic;
using System.Threading;
using Common;

namespace GameServer.Servers
{
    enum RoomState
    {
        WaitJoin,
        WaitBattle,
        Battle,
        End,
    }

    class Room
    {
        private const int MaxHp = 100;

        private Server server;
        private List<Client> clientList = new List<Client>(); //房间内的所有Client
        private RoomState state = RoomState.WaitJoin; //房间状态

        private Client owner
        {
            get { return clientList.Count > 0 ? clientList[0] : null; }
        }

        //构造方法
        public Room(Server server)
        {
            this.server = server;
        }

        //判断是否处于待加入状态
        public bool IsWaitJoin()
        {
            return state == RoomState.WaitJoin;
        }

        //添加客户端到房间
        public void AddClient(Client client)
        {
            client.Hp = MaxHp;
            clientList.Add(client);
            client.Room = this;

            if (clientList.Count >= 2)
            {
                state = RoomState.WaitBattle;
            }
        }

        //从房间中移除客户端
        public void RemoveClient(Client client)
        {
            if (client == null) return;
            if (clientList.Count <= 0) return;
            clientList.Remove(client);
            client.Room = null;

            state = clientList.Count >= 2 ? RoomState.WaitBattle : RoomState.WaitJoin;
        }

        //取得房间房主
        public Client GetOwner()
        {
            return owner;
        }

        //判断客户端是不是房主
        public bool IsOwner(Client client)
        {
            return owner == client;
        }

        //取得房主Id
        public int GetId()
        {
            return clientList.Count > 0 ? owner.GetUserId() : 0;
        }

        //关闭房间
        public void Close()
        {
            clientList.ForEach(c => c.Room = null);
            server.RemoveRoom(this);
        }

        //广播消息
        public void BroadcastMsg(ActionCode actionCode, string data)
        {
            clientList.ForEach(c => server.SendResponse(c, actionCode, data));
        }

        //广播消息
        public void BroadcastMsg(Client exceptClient, ActionCode actionCode, string data)
        {
            clientList.ForEach(c =>
            {
                if (c == exceptClient) return;
                server.SendResponse(c, actionCode, data);
            });
        }

        //开始计时
        //todo,逻辑处理应该放在controller中，room只提供方法，不处理逻辑
        public void StartTimer()
        {
            new Thread(RunTimer).Start();
        }

        private void RunTimer()
        {
            Thread.Sleep(500);
            for (int i = 3; i > 0; i--)
            {
                BroadcastMsg(ActionCode.ShowTimer, i.ToString());
                Thread.Sleep(1000);
            }
            BroadcastMsg(ActionCode.StartPlay, "StartPlay");
        }

        //伤害计算
        //todo,逻辑处理应该放在controller中，room只提供方法，不处理逻辑
        public void TakeDamage(Client source, int damage)
        {
            //伤害计算，以及判断游戏是否结束
            bool isOver = false;

            clientList.ForEach(c =>
            {
                if (c == source) return;

                c.Hp -= damage;
                isOver = c.Hp <= 0;
            });

            if (isOver == false) return;

            //结束游戏，群发游戏结束的消息
            clientList.ForEach(c =>
            {
                //向客户端发送战斗结果
                c.SendResponse(ActionCode.GameOver,
                    c == source ? ((int) RetuenCode.Sucess).ToString() : ((int) RetuenCode.Fail).ToString());
                //更新服务器的战绩
                c.UpdateResult(c==source);
            });

            //关闭房间
            Close();
        }
    }
}