using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
//using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for QueryAccNameDTO
/// </summary>
public class QueryAccNameService : QueryAccName
{
    dbConfig dbConfig;
    saveLog saveLog;

    public QueryAccNameService()
    {
        this.dbConfig = new dbConfig();
        this.saveLog = new saveLog();
    }

    public String QueryAccName(String ATMID, String AccNum)
    {
        var response = "";
        // var crossCurrency = "";

        try
        {

            var frmAcct = "";
            var toAcct = "";

            var str = AccNum + "|" + AccNum;

            String[] strArr = str.Split('|');
            String[] vArr = new string[strArr.Length];

            for (int i = 0; i < vArr?.Length; i++)
            {
                vArr[i] = strArr[i];
            }

            frmAcct = vArr?[0];
            toAcct = vArr?[1];



            if (ATMID != "" || AccNum != "")
            {
                using (var call = dbConfig.GetConnection())
                {
                    call.OpenAsync();
                    using (var cmd = new MySqlCommand("proc_account_name", call))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_from_acct", frmAcct.Trim()?.ToString());
                        cmd.Parameters.AddWithValue("_to_acct", toAcct.Trim()?.ToString());

                        using (var reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                string ccy = reader?.GetString(4)?.ToString();
                                response = reader?.GetString(2)?.ToString();
                                switch (ccy)
                                {
                                    // case allowed cross currency
                                    case "LAK-TO-LAK (allow)":
                                    case "THB-TO-LAK (allow)":
                                    case "USD-TO-LAK (allow)":
                                    case "CNY-TO-LAK (allow)":
                                    case "VND-TO-LAK (allow)":
                                    case "THB-TO-THB (allow)":
                                    case "USD-TO-USD (allow)":
                                    case "VND-TO-VND (allow)":
                                    case "CNY-TO-CNY (allow)":
                                        return response;
                                    default:
                                        return "54";

                                }
                            }
                        }
                    }

                }

                var dt = DateTime.Now.ToString("yyyyMMdd");
                var parameters = new MySqlParameter[]{

                    new MySqlParameter("ATMID",ATMID),
                    new MySqlParameter("frmAcct",frmAcct?.ToString()),
                    new MySqlParameter("toAcct",toAcct?.ToString()),
                    new MySqlParameter("AcctName",response?.ToString()),
                    new MySqlParameter("responseCode","00"?.ToString()),
                    new MySqlParameter("responseDate",dt?.ToString()),

                };

                saveLog.InsertData("createSaveAcctNameLog", parameters);

                System.Diagnostics.Debug.WriteLine(response);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ATMID & AccNum is null...with some request: " + AccNum);

                return "54";
            }



        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            return ex.SqlState;
        }
        catch (NullReferenceException e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return "54";
        }
        return response;

    }
}
