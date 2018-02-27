using System;
using GameServer.Model;
using GameServer.Servers;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    public static class UserDAO
    {
        // 检测指定的用户是否存在
        public static User VerifyUser(MySqlConnection conn, string name, string pwd)
        {
            MySqlDataReader reader = null;
            try
            {
                //sql命令
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username and password = @password ",conn);
                //通过【赋值参数】的方法，避免sql注入的问题
                cmd.Parameters.AddWithValue("username", name);
                cmd.Parameters.AddWithValue("password", pwd);
                reader = cmd.ExecuteReader();//执行sql语句

                if (!reader.Read()) return null;

                int id = reader.GetInt32("id");
                return new User(id, name, pwd);
            }
            catch (Exception e)
            {
                Debug.Log("在VerifyUser过程中出现异常：" + e);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        
        // 判断指定用户名的用户是否存在
        public static bool GetUserByName(MySqlConnection conn, string name)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username",conn);
                cmd.Parameters.AddWithValue("username", name);
                reader = cmd.ExecuteReader();

                return reader.Read();
            }
            catch (Exception e)
            {
                Debug.Log("在GetUserByName过程中出现异常：" + e);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        // 添加用户
        public static void AddUser(MySqlConnection conn, string name, string pwd)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into user set username = @username , password = @password ", conn);
                cmd.Parameters.AddWithValue("username", name);
                cmd.Parameters.AddWithValue("password", pwd);
                cmd.ExecuteNonQuery();//返回值为受影响的数据个数，可以作为添加成功与否的标志
            }
            catch (Exception e)
            {
                Debug.Log("在AddUser过程中出现异常：" + e);
            }
        }

        //移除用户
        public static void RemoveUser()
        {

        }

        //修改用户密码
        public static void UpdateUser()
        {

        }
    }
}
