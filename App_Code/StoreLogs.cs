using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StoreLogs
/// </summary>
public class StoreLogs
{

    private static readonly string logPathT = @"C:\Logs\TransferTransaction";
    private static readonly string logPathD = @"C:\Logs\DepositTransaction";
    private static readonly TraceSource traceSourceT = new TraceSource("DepositTransaction");
    private static readonly TraceSource traceSourceD = new TraceSource("TransferTransaction");

    private static void InitialLogging(string t)
    {

        try
        {
            if (t.Equals("T"))
            {
                if (!Directory.Exists(logPathT))
                {
                    Directory.CreateDirectory(logPathT);

                }
            }

            else if (t.Equals("D"))
            {
                if (!Directory.Exists(logPathT))
                {
                    Directory.CreateDirectory(logPathT);


                }

            }
        }
        catch (Exception ex)
        {

            EventLog.WriteEntry("DepositTransaction", $"Failed to initialize file logging: {ex.Message}", EventLogEntryType.Warning);

        }
    }

    private static void LogMessage(string t, string level, string message, Exception ex = null)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";

            if (ex != null)
            {
                logEntry += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            }


            if (t.Equals("T"))
            {

                // Log to file
                string logFileName = Path.Combine(logPathT, $"TransferTransaction_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(logFileName, logEntry + Environment.NewLine);

                // Also log to trace (visible in DebugView)
                Trace.WriteLine(logEntry);

                // Log to Event Viewer for critical errors
                if (level == "ERROR" || level == "CRITICAL")
                {
                    EventLog.WriteEntry("TransferTransaction", logEntry, EventLogEntryType.Error);
                }
            }
            else if (t.Equals("D"))
            {
                // Log to file
                string logFileName = Path.Combine(logPathD, $"DepositTransaction_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(logFileName, logEntry + Environment.NewLine);

                // Also log to trace (visible in DebugView)
                Trace.WriteLine(logEntry);

                // Log to Event Viewer for critical errors
                if (level == "ERROR" || level == "CRITICAL")
                {
                    EventLog.WriteEntry("DepositTransaction", logEntry, EventLogEntryType.Error);
                }
            }
           

           
        }
        catch (Exception logEx)
        {
            // Last resort - write to Event Log
            EventLog.WriteEntry("DepositTransaction", $"Logging failed: {logEx.Message}", EventLogEntryType.Error);
        }
    }
}