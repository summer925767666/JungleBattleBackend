using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySql数据库操作
{
    class Program
    {
//        private const 
        static void Main(string[] args)
        {
//            string ConnStr1 = "database=game001;datasource=127.0.0.1;port=3306;user=root;pwd=root";
//            MySqlConnection connection1 = new MySqlConnection(ConnStr1);
//            connection1.Open();
//
//            MySqlCommand cmd1 = new MySqlCommand("select * form user where username = @username and password = @password ");
//            cmd1.Parameters.AddWithValue("username", "111");
//            cmd1.Parameters.AddWithValue("password", "111");
//            MySqlDataReader reader1 = cmd1.ExecuteReader();
//
//            Console.WriteLine(reader1.Read() ? "测试成功，读取到数据" : "测试成功，未读取到数据");


            string connStr = "database=test007;datasource=127.0.0.1;port=3306;user=root;pwd=root";
            MySqlConnection connection = new MySqlConnection(connStr);

            connection.Open();

            #region 插入测试
//            string username = "zjd";
//            string password = "123";
//
//            //字符串拼接会导致sql注入问题
////            MySqlCommand cmd = new MySqlCommand("insert into user set username = "+username+", password = "+password, connection);
//            MySqlCommand cmd = new MySqlCommand("insert into user set username = @un, password = @pwd", connection);
//            cmd.Parameters.AddWithValue("un", username);
//            cmd.Parameters.AddWithValue("pwd", password);
//
//            cmd.ExecuteNonQuery();
            #endregion

            #region 删除测试
//            MySqlCommand cmd=new MySqlCommand("delete from user where id= @id",connection);
//            cmd.Parameters.AddWithValue("id",5);
//            cmd.ExecuteNonQuery();
            #endregion

            #region 修改测试
//            string password = "sikiedu.com";
//            MySqlCommand cmd = new MySqlCommand("update user set password = @pwd where id = 1", connection);
//            cmd.Parameters.AddWithValue("pwd", password);
//            cmd.ExecuteNonQuery();
            #endregion

            #region 查询测试
                        MySqlCommand cmd=new MySqlCommand("select * from user",connection);
                        MySqlDataReader reader= cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string username= reader.GetString("username");
                            string password = reader.GetString("password");
            
                            Console.WriteLine("用户名："+username+" 密码："+password);
                        }
                        reader.Close();
            #endregion

            connection.Close();

            Console.ReadKey();
        }
    }
}
