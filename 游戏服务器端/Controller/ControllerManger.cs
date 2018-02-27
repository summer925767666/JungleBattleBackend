using System.Collections.Generic;
using System.Reflection;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class ControllerManger
    {
        //所有Controller的字典
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;

        //构造方法
        public ControllerManger(Server server)
        {
            this.server = server;
            InitController();
        }

        //初始化所有的Controller
        private void InitController()
        {
            controllerDict.Add(RequestCode.None, new DefaultController());
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());
        }

        /// 处理客户端接收到的消息
        public void HandleRequest(Client client, RequestCode requestCode, ActionCode actionCode, string data)
        {
            //1、取得指定的Controller
            BaseController baseController;
            bool isGet = controllerDict.TryGetValue(requestCode, out baseController);

            if (isGet == false)
            {
                Debug.Log(string.Format("无法得到【{0}】所对应的Controller，无法处理请求", requestCode));
                return;
            }

            //2、利用反射，执行指定名称的方法
            string methodName = actionCode.ToString();
            MethodInfo methodInfo = baseController.GetType().GetMethod(methodName);
            if (methodInfo == null)
            {
                Debug.Log(string.Format("【警告】在Controller【{0}】没有对应的处理方法【{1}】", baseController.GetType(), methodName));
                return;
            }

            object[] parameters = {server, client, data}; //构造方法参数
            object result = methodInfo.Invoke(baseController, parameters); //执行方法

            if (string.IsNullOrEmpty(result.ToString()))
            {
                return;
            }

            //3、通过server向客户端返回Response
            server.SendResponse(client, actionCode, result.ToString());
        }
    }
}