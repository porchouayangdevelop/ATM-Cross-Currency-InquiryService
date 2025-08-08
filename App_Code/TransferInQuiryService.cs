using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for TransferInquiry
/// </summary>
public class TransferInQuiryService : ITransfer
{

  dbConfig dbConfig;
  saveLog saveLog;

  public TransferInQuiryService()
  {
    //
    // TODO: Add constructor logic here
    //
    this.dbConfig = new dbConfig();
    this.saveLog = new saveLog();


    StoreLogs.InitializedLogging(StoreLogs.LogType.Transfer);

  }

  public string InQuiryTransfer(string ATMID, string AccNum)
  {
    var response = "";
    string correlationId = Guid.NewGuid().ToString("N");

    // Log the start of the method
    StoreLogs.LogInfo(StoreLogs.LogType.Transfer,
        $"[{correlationId}] InQuiryTransfer started - ATMID: '{ATMID}', AccNum: '{AccNum}'");


    try
    {

      var frmAcct = "";
      var toAcct = "";

      var str = AccNum + "|" + AccNum;

      StoreLogs.LogDebug(StoreLogs.LogType.Transfer,
                $"[{correlationId}] Processing account string: '{str}'");


      String[] strArr = str.Split('|');
      String[] vArr = new string[strArr.Length];

      for (int i = 0; i < vArr?.Length; i++)
      {
        vArr[i] = strArr[i];
      }

      frmAcct = vArr?[0];
      toAcct = vArr?[1];


      if (string.IsNullOrEmpty(ATMID) || string.IsNullOrEmpty(AccNum))
      {
        StoreLogs.LogWarning(StoreLogs.LogType.Transfer,
            $"[{correlationId}] Invalid parameters - ATMID: '{ATMID}', AccNum: '{AccNum}'");
        return "12";
      }

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
                  StoreLogs.LogInfo(StoreLogs.LogType.Transfer,$"[{correlationId}] InQuiryTransfer transaction type: '{ccy}'");
                  StoreLogs.LogInfo(StoreLogs.LogType.Transfer,$"[{correlationId}] InQuiryTransfer completed successfully with response: '{response}'");
                  return response;
                default:
                  StoreLogs.LogInfo(StoreLogs.LogType.Transfer, $"[{correlationId}] InQuiryTransfer TransactionType: '{ccy}' is Not allow");
                  StoreLogs.LogInfo(StoreLogs.LogType.Transfer, $"[{correlationId}] InQuiryTransfer Code : '76', Reason : {ccy} ,Message : Cross currecy not allowed");
                  return "76";

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
      StoreLogs.LogInfo(StoreLogs.LogType.Transfer,
             $"[{correlationId}] InQuiryTransfer completed successfully with response: '{response}'");

    }
    catch (NullReferenceException ex)
    {
      StoreLogs.LogError(StoreLogs.LogType.Transfer,
          $"[{correlationId}] Null Reference Exception in InQuiryTransfer", ex);
      return "12";
    }
    catch (Exception ex)
    {
      StoreLogs.LogCritical(StoreLogs.LogType.Transfer,
          $"[{correlationId}] Unexpected exception in InQuiryTransfer", ex);
      StoreLogs.LogCritical(StoreLogs.LogType.Transfer,
         $"v");
      StoreLogs.LogCritical(StoreLogs.LogType.Transfer,
         $"v");
      return "99";
    }
    return response;
  }
}