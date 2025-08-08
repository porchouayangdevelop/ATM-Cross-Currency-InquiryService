using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for dbConfig
/// </summary>
public class dbConfig
{
    private MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Core"].ConnectionString);
    //private static String conStr = "server=10.151.145.165;database=core001;uid=core;password=Apb@123456";


    public MySqlConnection GetConnection()
    {
        return con;
    }
}