using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    internal abstract class BaseController
    {
        private RequestCode requestCode = RequestCode.None;
        // ReSharper disable once InconsistentNaming
        protected RequestCode _RequestCode
        {
            set { requestCode = value; }
        }

        public virtual string DefaultHandle(string data,Client client,Server server)
        {
            return null;
        }
    }
}
