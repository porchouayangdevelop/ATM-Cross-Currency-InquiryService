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
  private static readonly string logPathG = @"C:\Logs\GeneralTransaction";

  public enum LogType
  {
    Transfer,
    Deposit,
    General
  }


  public static void InitializedLogging(LogType logType) {
    string logPath = GetLogPath(logType);
    InitialLogging(logPath,logType.ToString());
  }

  public static void LogMessage(LogType logType, string level, string message, Exception ex = null)
  {
    string logPath = GetLogPath(logType);
    LogMessageInternal(logPath, logType.ToString(), level, message, ex);
  }

  // Overloaded method for backward compatibility with your existing DepositInQuiryService
  public static void LogMessage(string level, string message, Exception ex = null)
  {
    LogMessage(LogType.General, level, message, ex);
  }

  // Helper method to get log path based on type
  private static string GetLogPath(LogType logType)
  {
    switch (logType)
    {
      case LogType.Transfer:
        return logPathT;
      case LogType.Deposit:
        return logPathD;
      case LogType.General:
        return logPathG;
      default:
        return logPathG;
    }
  }

  private static void InitialLogging(string logPath, string logTypeName)
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
      EventLog.WriteEntry($"{logTypeName}Transaction", $"Failed to initialize file logging: {ex.Message}", EventLogEntryType.Warning);
    }
  }

  private static void LogMessageInternal(string logPath, string logTypeName, string level, string message, Exception ex = null)
  {
    try
    {
      // Ensure log directory exists
      if (!Directory.Exists(logPath))
      {
        Directory.CreateDirectory(logPath);
      }

      string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
      string logEntry = $"[{timestamp}] [{level}] {message}";

      if (ex != null)
      {
        logEntry += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
      }

      // Log to file
      string logFileName = Path.Combine(logPath, $"{logTypeName}Transaction_{DateTime.Now:yyyyMMdd}.log");
      File.AppendAllText(logFileName, logEntry + Environment.NewLine);

      // Also log to trace (visible in DebugView)
      Trace.WriteLine(logEntry);

      // Log to Event Viewer for critical errors
      if (level == "ERROR" || level == "CRITICAL")
      {
        EventLog.WriteEntry($"{logTypeName}Transaction", logEntry, EventLogEntryType.Error);
      }
    }
    catch (Exception logEx)
    {
      // Last resort - write to Event Log
      EventLog.WriteEntry($"{logTypeName}Transaction", $"Logging failed: {logEx.Message}", EventLogEntryType.Error);
    }
  }

  public static void LogInfo(LogType logType, string message)
  {
    LogMessage(logType, "INFO", message);
  }

  public static void LogWarning(LogType logType, string message)
  {
    LogMessage(logType, "WARN", message);
  }

  public static void LogError(LogType logType, string message, Exception ex = null)
  {
    LogMessage(logType, "ERROR", message, ex);
  }

  public static void LogDebug(LogType logType, string message)
  {
    LogMessage(logType, "DEBUG", message);
  }

  public static void LogCritical(LogType logType, string message, Exception ex = null)
  {
    LogMessage(logType, "CRITICAL", message, ex);
  }
}