using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Ubiety.Dns.Core;

/// <summary>
/// Summary description for DepositTransaction
/// </summary>
public class DepositInQuiryService : IDeposit
{

    dbConfig dbConfig;
    saveLog saveLog;


    private static readonly string logPath = @"C:\Logs\DepositTransaction";
    private static readonly TraceSource traceSource = new TraceSource("DepositTransaction");

    public DepositInQuiryService()
    {
        //
        // TODO: Add constructor logic here
        //

        this.dbConfig = new dbConfig();
        this.saveLog = new saveLog();

        InitialLogging();
    }


    private void InitialLogging()
    {

        try
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);

            }

        }
        catch (Exception ex)
        {

            EventLog.WriteEntry("DepositTransaction", $"Failed to initialize file logging: {ex.Message}", EventLogEntryType.Warning);

        }
    }

    private void LogMessage(string level, string message, Exception ex = null)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";

            if (ex != null)
            {
                logEntry += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            }

            // Log to file
            string logFileName = Path.Combine(logPath, $"DepositTransaction_{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(logFileName, logEntry + Environment.NewLine);

            // Also log to trace (visible in DebugView)
            Trace.WriteLine(logEntry);

            // Log to Event Viewer for critical errors
            if (level == "ERROR" || level == "CRITICAL")
            {
                EventLog.WriteEntry("DepositTransaction", logEntry, EventLogEntryType.Error);
            }
        }
        catch (Exception logEx)
        {
            // Last resort - write to Event Log
            EventLog.WriteEntry("DepositTransaction", $"Logging failed: {logEx.Message}", EventLogEntryType.Error);
        }
    }


    public string ProcessDeposit(string Deposit_ccy, string AccNum)
    {
        LogMessage("INFO", $"ProcessDeposit called with Deposit_ccy: '{Deposit_ccy}', AccNum: '{AccNum}'");
        return ProcessTransaction(Deposit_ccy, AccNum);
    }

    public string ProcessTransaction(string Deposit_ccy, string AccNum)
    {
        var Result = string.Empty;

        string correlationId = Guid.NewGuid().ToString("N"); // Short correlation ID for tracking

        LogMessage("INFO", $"[{correlationId}] ProcessTransaction started - Deposit_ccy: '{Deposit_ccy}', AccNum: '{AccNum}'");

        try
        {

            if (string.IsNullOrEmpty(Deposit_ccy) || string.IsNullOrEmpty(AccNum))
            {
                //Invalid { Deposit_ccy}, { AccNum}
                LogMessage("WARN", $"[{correlationId}] Invalid parameters - Deposit_ccy: '{Deposit_ccy}', AccNum: '{AccNum}'");
                Result = "12";
                LogMessage("INFO", $"[{correlationId}] ProcessTransaction completed with Result: '{Result}' (Invalid parameters)");
                return Result;
            }

            string currencyCode = Deposit_ccy.Trim();
            LogMessage("INFO", $"[{correlationId}] Processing currency code: '{currencyCode}'");


            switch (Deposit_ccy.Trim().ToString())
            {
                //840,764,978,704,156,418
                case "840":
                case "764":
                case "978":
                case "704":
                case "156":
                case "418":

                    LogMessage("INFO", $"[{correlationId}] Currency code '{currencyCode}' is supported, connecting to database");

                    using (var callProcedure = dbConfig.GetConnection())
                    {
                        callProcedure.OpenAsync();

                        using (var cmd = new MySqlCommand("proc_deposit_cross_currency_inquiry", callProcedure))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_deposit_ccy", Deposit_ccy);
                            cmd.Parameters.AddWithValue("_to_acct", AccNum);

                            LogMessage("INFO", $"[{correlationId}] Executing stored procedure 'proc_deposit_cross_currency_inquiry'");


                            using (var reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {

                                    string targetAccount = reader.GetString("acct")?.ToString();
                                    var result = reader?.GetString("name")?.ToString();
                                    string ccy = reader?.GetString("ccy")?.ToString();
                                    string transactionType = reader?.GetString("transaction_type")?.ToString();
                                    string isAllowed = reader?.GetString("is_allowed")?.ToString();
                                    string from_ccy = reader?.GetString("from_ccy")?.ToString();
                                    string to_ccy = reader?.GetString("to_ccy")?.ToString();

                                    LogMessage("INFO", $"[{correlationId}] Database query results - ccy: '{ccy}', result: '{result}', transactionType: '{transactionType}', isAllowed : {isAllowed}");


                                    switch (transactionType)
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
                                            Result = result;
                                            LogMessage("INFO", $"[{correlationId}] Transaction allowed - ccy: '{ccy}', Result: '{Result}'");

                                            break;
                                        default:
                                            Result = "76";
                                            LogMessage("WARN", $"[{correlationId}] Transaction not allowed - ccy: '{ccy}', Result: '{Result}'");
                                            break;
                                    }
                                    LogMessage("DEBUG", $"[{correlationId}] Transaction details - from_ccy: '{from_ccy}', to_ccy: '{to_ccy}', targetAccount: '{targetAccount}'");

                                    var parameters = new MySqlParameter[]
                                    {
                                        new MySqlParameter("_from_currency",from_ccy),
                                        new MySqlParameter("_to_currency",to_ccy),
                                        new MySqlParameter("_target_account",targetAccount),
                                        new MySqlParameter("_target_account_name",Result),
                                        new MySqlParameter("_target_currency",to_ccy),
                                        new MySqlParameter("_result_ccy",ccy),
                                        new MySqlParameter("_transaction_type",transactionType),
                                        new MySqlParameter("_is_allowed",isAllowed),


                                    };
                                    saveLog.InsertData("proc_atm_deposit_cross_currency", parameters);

                                }
                            }
                        }
                    }
                    break;
                default:
                    LogMessage("WARN", $"[{correlationId}] Unsupported currency code: '{currencyCode}'");
                    Result = "99";
                    break;
            }
        }
        catch (MySqlException ex)
        {
            LogMessage("ERROR", $"[{correlationId}] MySQL Exception occurred - SqlState: {ex.SqlState}, ErrorCode: {ex.ErrorCode}, Message: {ex.Message}", ex);
            Result = ex.SqlState;
        }
        catch (NullReferenceException ex)
        {
            LogMessage("ERROR", $"[{correlationId}] Null Reference Exception occurred", ex);
            Result = "76";
        }
        catch (Exception ex)
        {
            LogMessage("CRITICAL", $"[{correlationId}] Unexpected exception occurred", ex);
            Result = "99";
        }
        LogMessage("INFO", $"[{correlationId}] ProcessTransaction completed with Result: '{Result}'");
        LogMessage("", "v");
        LogMessage("", "v");
        return Result;
    }
}