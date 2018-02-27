using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class GameController : BaseController
    {
        //开始游戏
        public string StartGame(Server server, Client client, string data)
        {
            if (client.IsOwner()) //判断是否是房主
            {
                Room room = client.Room; //todo，需要判断当前房间状态
                //群发消息，开始战斗
                room.BroadcastMsg(client, ActionCode.StartGame, ((int) RetuenCode.Sucess).ToString());
                room.StartTimer();

                return ((int) RetuenCode.Sucess).ToString();
            }
            else
            {
                return ((int) RetuenCode.Fail).ToString();
            }
        }

        //位置同步
        //todo,未同步设计动画
        public string Move(Server server, Client client, string data)
        {
            Room room = client.Room;
            if (room == null) return "";
            room.BroadcastMsg(client, ActionCode.Move, data);
            return "";
        }

        //射击同步
        public string Shoot(Server server, Client client, string data)
        {
            Room room = client.Room;
            if (room == null) return "";
            room.BroadcastMsg(client, ActionCode.Shoot, data);
            return "";
        }

        //伤害同步
        public string Attack(Server server, Client client, string data)
        {
            int damage = int.Parse(data);
            Room room = client.Room;
            if (room == null) return "";

            //房间实例进行伤害逻辑的计算
            room.TakeDamage(client, damage);

            return "";
        }        
        
        //退出战斗
        public string QuitBattle(Server server, Client client, string data)
        {
            Room room = client.Room;
            if (room == null) return "";

            room.BroadcastMsg(ActionCode.QuitBattle,"QuitBattle");
            room.Close();

            return "";
        }
    }
}