using System;
using GameServer.Servers;
using MySql.Data.MySqlClient;

namespace GameServer.Tool
{
    class ConnectHelper
    {
        private const string ConnStr = "database=game001;datasource=127.0.0.1;port=3306;user=root;pwd=root";

        /// <summary>
        /// 创建和数据库的连接
        /// </summary>
        /// <returns>连接对象</returns>
        public static MySqlConnection Connect()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(ConnStr);
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                Debug.Log("连接数据库异常："+e);
                return null;
            }
        }

        /// <summary>
        /// 关闭和数据库的连接
        /// </summary>
        /// <param name="connection"></param>
        public static void Close( MySqlConnection  connection)
        {
            if (connection==null)
            {
                Debug.Log("连接为空，无法关闭");
            }
            else
            {
                connection.Close();
                Debug.Log("关闭和数据库连接");
            }
        }
    }
}
