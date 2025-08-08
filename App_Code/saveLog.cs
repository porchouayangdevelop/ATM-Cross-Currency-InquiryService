using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for saveLog
/// </summary>
public class saveLog
{
    private MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Log"].ConnectionString);
 

    private void initialDb()
    {
        //String conStr = "server=10.151.133.62;Port=4006;database=cardzoneLog;uid=kcb;password=Kcb@2023";
        //connection = new MySqlConnection(conStr);
    }




    public MySqlConnection GetConnection()
    {
        return connection;
    }


    public bool executeNonQueryInsert(String sql, MySqlCommand cmd)
    {

        using (var con = this.GetConnection())
        {
            con.Open();
            cmd = new MySqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            return true;
        }



    }


    public bool executeNonQueryUpdate(String sql, MySqlCommand cmd)
    {

        using (var con = this.GetConnection())
        {
            cmd = new MySqlCommand();
            cmd.CommandText = sql;
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
            return true;
        }

    }


    public bool InsertData(String procedure, MySqlParameter[] parameters)
    {

        using (var con = this.GetConnection())
        {
            con.OpenAsync();
            using (var cmd = new MySqlCommand(procedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
                return true;
            }
                //MySqlCommand cmd = new MySqlCommand(procedure, con);
           
        }


    }

    public bool updateData(String procedure, MySqlParameter[] parameters)
    {

        using (var con = this.GetConnection())
        {
            MySqlCommand cmd = new MySqlCommand(procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
            return true;
        }



    }

    public bool deleteData(String procedure, MySqlParameter[] parameters)
    {
        using (var con = this.GetConnection())
        {
            MySqlCommand cmd = new MySqlCommand(procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
            return true;
        }
    }



}