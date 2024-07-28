// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AdminLog
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Admin
{
  public static class AdminLog
  {
    private static string s_currentLogPath;
    private static string s_logFolder;
    private static object s_locker = new object();
    private static FileLogWriter s_logWriter;
    private static ILogWriter s_aditionalLogWriter;
    private const string s_fileNameTemplate = "{0}_{1:MM}{1:dd}_{1:HH}{1:mm}{1:ss}.log";
    private static LogType s_currentLogType;
    private static LogArea s_currentLogArea;
    private static LogActivity s_currentLogActivity;

    static AdminLog()
    {
      lock (AdminLog.s_locker)
      {
        if (Process.GetCurrentProcess().ProcessName.Equals("LightRail", StringComparison.OrdinalIgnoreCase))
        {
          AdminLog.s_logFolder = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "LightRail", "Logs");
        }
        else
        {
          AdminLog.s_logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Azure DevOps\\Server Configuration\\Logs");
          if (!Directory.Exists(AdminLog.s_logFolder))
            Directory.CreateDirectory(AdminLog.s_logFolder);
        }
        AdminLog.Initialize();
      }
    }

    private static void Initialize()
    {
      lock (AdminLog.s_locker)
      {
        AdminLog.s_currentLogType = LogType.Configuration;
        AdminLog.s_currentLogArea = LogArea.Unknown;
        AdminLog.s_currentLogActivity = LogActivity.Configuration;
        AdminLog.s_aditionalLogWriter = (ILogWriter) null;
      }
    }

    internal static string LogFolder => AdminLog.s_logFolder;

    internal static void StartActivity(LogType logType, LogActivity logActivity, LogArea logArea) => AdminLog.StartActivity(logType, logActivity, logArea, (ILogWriter) null);

    internal static void StartActivity(
      LogType logType,
      LogActivity logActivity,
      LogArea logArea,
      ILogWriter aditionalLogWriter)
    {
      lock (AdminLog.s_locker)
      {
        AdminLog.s_currentLogType = logType;
        AdminLog.s_currentLogActivity = logActivity;
        AdminLog.s_currentLogArea = logArea;
        AdminLog.s_aditionalLogWriter = aditionalLogWriter;
        string logBaseName = AdminLog.GetLogBaseName(logType, logActivity, logArea);
        AdminLog.StartActivity(logType, logActivity, logArea, logBaseName, DateTime.UtcNow);
      }
    }

    internal static void StartActivity(
      LogType logType,
      LogActivity logActivity,
      LogArea logArea,
      string baseName,
      DateTime dateTime)
    {
      lock (AdminLog.s_locker)
      {
        AdminLog.SetLogPath(AdminLog.BuildLogPath(baseName, dateTime));
        foreach (string message in AdminLog.BuildLogHeading(logType, logActivity, logArea))
          AdminLog.Info(message);
      }
    }

    public static void EndActivity() => AdminLog.Initialize();

    private static string GetLogBaseName(LogType logType, LogActivity logActivity, LogArea logArea) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}", (object) AdminLogTokens.GetTypeToken(logType), (object) AdminLogTokens.GetActivityToken(logActivity), (object) AdminLogTokens.GetAreaToken(logArea));

    private static string BuildLogPath(string baseName, DateTime timestamp)
    {
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:MM}{1:dd}_{1:HH}{1:mm}{1:ss}.log", (object) "{0}", (object) timestamp);
      return Path.Combine(AdminLog.s_logFolder, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) baseName));
    }

    internal static void Heading(string message)
    {
      AdminLog.Info(string.Empty);
      AdminLog.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
      AdminLog.Info(message);
      AdminLog.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
    }

    internal static void Heading(string message, params object[] args) => AdminLog.Heading(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    internal static void Heading2(string message)
    {
      AdminLog.Info(string.Empty);
      AdminLog.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "+-+-+-+-+-| {0} |+-+-+-+-+-", (object) message));
    }

    internal static void Heading2(string message, params object[] args) => AdminLog.Heading2(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    internal static void Info(string message)
    {
      AdminTrace.Info(message);
      AdminLog.WriteLine(LogEntryType.Info, message);
    }

    internal static void Info(string message, params object[] args) => AdminLog.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    internal static void Warning(string message)
    {
      AdminTrace.Warning(message);
      AdminLog.WriteLine(LogEntryType.Warning, message);
    }

    internal static void Warning(string message, params object[] args) => AdminLog.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    internal static void Warning(Exception e)
    {
      if (e == null)
        return;
      AdminLog.Warning(TeamFoundationExceptionFormatter.FormatException(e, false));
    }

    internal static void Error(string message)
    {
      AdminTrace.Error(message);
      AdminLog.WriteLine(LogEntryType.Error, message);
    }

    internal static void Error(string message, params object[] args) => AdminLog.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    internal static void Error(Exception e)
    {
      if (e == null)
        return;
      AdminLog.Error(TeamFoundationExceptionFormatter.FormatException(e, false));
    }

    internal static void UnexpectedCondition(string message) => AdminLog.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UNEXPECTED CONDITION: {0}", (object) message));

    internal static void UnexpectedCondition(string message, params object[] args) => AdminLog.UnexpectedCondition(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public static string GetCurrentLogPath()
    {
      lock (AdminLog.s_locker)
      {
        if (AdminLog.s_currentLogPath == null)
          AdminLog.SetUnknownLogPath();
        return AdminLog.s_currentLogPath;
      }
    }

    public static string GetCollectionLogPath(string baseName, DateTime timestamp)
    {
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:MM}{1:dd}_{1:HH}{1:mm}{1:ss}.log", (object) "{0}", (object) timestamp);
      string collectionLogPath = Path.Combine(AdminLog.s_logFolder, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) baseName));
      AdminTrace.Verbose("CollectionLogPath: {0}", (object) collectionLogPath);
      return collectionLogPath;
    }

    public static string ResetCollectionLogPath(string baseName, DateTime timestamp)
    {
      lock (AdminLog.s_locker)
      {
        AdminLog.SetLogPath(AdminLog.GetCollectionLogPath(baseName, timestamp));
        AdminTrace.Info("Using TeamProjectCollectionPath: {0}", (object) AdminLog.s_currentLogPath);
        return AdminLog.s_currentLogPath;
      }
    }

    private static void SetLogPath(string logPath)
    {
      AdminLog.s_currentLogPath = logPath;
      using (AdminLog.s_logWriter)
        ;
      AdminLog.s_logWriter = new FileLogWriter(AdminLog.s_currentLogPath);
    }

    private static void SetUnknownLogPath()
    {
      AdminTrace.Info("null log path");
      if (AdminLog.s_currentLogPath != null)
        return;
      AdminLog.SetLogPath(AdminLog.BuildLogPath(AdminLog.GetLogBaseName(AdminLog.s_currentLogType, AdminLog.s_currentLogActivity, AdminLog.s_currentLogArea), DateTime.UtcNow));
    }

    private static void WriteLine(LogEntryType entryType, string message)
    {
      try
      {
        lock (AdminLog.s_locker)
        {
          if (AdminLog.s_logWriter == null)
          {
            AdminTrace.Info("s_logWriter is unexpectedly null - allocating a new one");
            AdminLog.SetUnknownLogPath();
          }
          string text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}{3}", (object) entryType.ToString(), (object) DateTime.UtcNow, (object) message, (object) Environment.NewLine);
          AdminLog.s_logWriter.Write(text);
          if (AdminLog.s_aditionalLogWriter == null)
            return;
          AdminLog.s_aditionalLogWriter.Write(text);
        }
      }
      catch (Exception ex)
      {
        AdminTrace.Error(ex);
      }
    }

    private static string[] BuildLogHeading(
      LogType logType,
      LogActivity logActivity,
      LogArea logArea)
    {
      List<string> stringList = new List<string>()
      {
        "====================================================================",
        "Azure DevOps Server Administration Log"
      };
      try
      {
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version  : {0}", (object) TeamFoundationProductVersion.ProductVersion));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DateTime : {0}", (object) DateTime.Now));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Type     : {0}", (object) logType.ToString()));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Activity : {0}", (object) AdminLogTokens.GetActivityDisplayName(logActivity)));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Area     : {0}", (object) AdminLogTokens.GetAreaDisplayName(logArea)));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User     : {0}\\{1}", (object) Environment.UserDomainName, (object) Environment.UserName));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Machine  : {0}", (object) Environment.MachineName));
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "System   : {0} ({1})", (object) Environment.OSVersion.ToString(), (object) Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")));
      }
      catch (Exception ex)
      {
        stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error retrieving environment information: ", (object) TeamFoundationExceptionFormatter.FormatException(ex, false)));
      }
      stringList.Add("====================================================================");
      return stringList.ToArray();
    }
  }
}
