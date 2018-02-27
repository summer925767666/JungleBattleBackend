using System.Text;
using Common;
using GameServer.Model;
using GameServer.Servers;

namespace GameServer.Controller
{
    class RoomController : BaseController
    {
        //处理创建房间请求
        public string CreatRoom(Server server, Client client, string data)
        {
            server.CraetRoom(client);
            return ((int) RetuenCode.Sucess).ToString();
        }

        //处理房间列表请求
        public string ListRoom(Server server, Client client, string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Room room in server.GetRoomList())
            {
                if (!room.IsWaitJoin()) continue;

                User user;
                Result result;
                room.GetOwner().GetUserData(out user, out result);

                sb.Append(user.Id + "," + user.Name + "," + result.TotalCount + "," + result.WinCount + "|");
            }

            if (sb.Length == 0)
            {
                sb.Append("0");
            }
            else
            {
                sb.Remove(sb.Length - 1, 1); //移除最后一个“|”
            }

            return sb.ToString();
        }

        //处理加入房间请求
        public string JoinRoom(Server server, Client client, string data)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                return ((int) RetuenCode.NotFound).ToString();//notfound表示房间为找到
            }

            if (room.IsWaitJoin() == false)
            {
                return ((int)RetuenCode.Fail).ToString();//fail表示房间满员
            }

            Client owner = room.GetOwner();
            User u1;
            Result re1;
            owner.GetUserData(out u1, out re1);

            User u2;
            Result re2;
            client.GetUserData(out u2,out re2);

            //向房主发送加入请求
            server.SendResponse(owner, ActionCode.UpdateRoom, string.Format("{0},{1},{2},{3}", (int)RetuenCode.Join, u2.Name, re2.TotalCount, re2.WinCount));

            //向自身客户端发送加入成功消息
            room.AddClient(client);
            return string.Format("{0},{1},{2},{3}", (int) RetuenCode.Sucess, u1.Name, re1.TotalCount, re1.WinCount);
        }

        //处理退出房间请求
        public string QuitRoom(Server server, Client client, string data)
        {
            Room room = client.Room;

            //判断是否是房主
            if (client.IsOwner())
            {
                //向其他客户端广播退出成功消息
                room.BroadcastMsg(client,ActionCode.QuitRoom, ((int)RetuenCode.Sucess).ToString());
                //关闭Room
                room.Close();
            }
            else
            {
                //房间删除对当前客户端的管理
                room.RemoveClient(client);
                //广播消息，通知房间内其他玩家有客户端退出
                room.BroadcastMsg(client, ActionCode.UpdateRoom,((int)RetuenCode.Quit).ToString());
            }

            //当前客户端返回退出成功
            return ((int)RetuenCode.Sucess).ToString();
            
        }
    }
}