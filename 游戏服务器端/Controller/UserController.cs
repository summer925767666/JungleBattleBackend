using Common;
using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;

namespace GameServer.Controller
{
    class UserController : BaseController
    {
        public UserController()
        {
            _RequestCode = RequestCode.User;
        }

        // 登录请求的处理
        //todo，未判断是否已经登录，不能避免重复登录问题
        public string Login(Server server, Client client, string data)
        {
            string[] strs = data.Split(',');//利用逗号分割用户名和密码
            User user = UserDAO.VerifyUser(client.MySqlConnection, strs[0], strs[1]);//通过DAO判断用户名密码是否正确
            if (user==null)
            {
                return ((int) RetuenCode.Fail).ToString();//返回登录失败
            }

            //取得战绩
            Result res = ResultDAO.GetResultByUserid(client.MySqlConnection, user.Id);
            int totalCount = res.TotalCount;
            int winCount = res.WinCount;

            //把账户和战绩信息保存到Client实例中
            client.SetUserData(user,res);

            //返回账户、战绩数据
            return string.Format("{0},{1},{2},{3}", (int)RetuenCode.Sucess, user.Name, totalCount, winCount);
        }

        /// 注册请求的处理
        public string Register(Server server, Client client, string data)
        {
            //1、通过DAO判断用户名是否存在
            string[] strs = data.Split(',');
            bool isExit = UserDAO.GetUserByName(client.MySqlConnection, strs[0]);

            if (isExit)
            {
                return ((int)RetuenCode.Fail).ToString();
            }

            //2、添加到数据库
            UserDAO.AddUser(client.MySqlConnection, strs[0], strs[1]);
            return ((int) RetuenCode.Sucess).ToString();
        }
    }
}