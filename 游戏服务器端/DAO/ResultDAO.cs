using System;
using GameServer.Model;
using GameServer.Servers;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    public static class ResultDAO
    {
        //根据用户ID取得战绩信息
        public static Result GetResultByUserid(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                //sql命令
                MySqlCommand cmd = new MySqlCommand("select * from result where userid = @userid", conn);
                //通过【赋值参数】的方法，避免sql注入的问题
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader(); //执行sql语句

                if (!reader.Read()) return new Result(-1, userId, 0, 0);

                int id = reader.GetInt32("id");
                int totalCount = reader.GetInt32("totalcount");
                int winCount = reader.GetInt32("wincount");

                return new Result(id, userId, totalCount, winCount);
            }
            catch (Exception e)
            {
                Debug.Log("在GetResultByUserid过程中出现异常：" + e);
                return new Result(-1, userId, 0, 0);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        //更新战绩
        public static void UpdateResult(MySqlConnection conn, ref Result result)
        {
            try
            {
                //sql命令，不存在就插入一条数据;存在就更新数据
                MySqlCommand cmd = result.Id < 0? 
                    new MySqlCommand("insert into result set userid = @userid , totalcount = @totalcount , wincount = @wincount",conn): 
                    new MySqlCommand("update result set wincount = @wincount , totalcount = @totalcount where userid = @userid", conn);

                //参数赋值
                cmd.Parameters.AddWithValue("totalcount", result.TotalCount);
                cmd.Parameters.AddWithValue("wincount", result.WinCount);
                cmd.Parameters.AddWithValue("userid", result.UserId);

                cmd.ExecuteNonQuery(); //执行sql语句

                if (result.Id >= 0) return;

                Result r = GetResultByUserid(conn, result.UserId);
                result.Id = r.Id;
            }
            catch (Exception e)
            {
                Debug.Log("在UpdateResult过程中出现异常：" + e);
            }
        }
    }
}