// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Logging.Logger
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Logging
{
  public static class Logger
  {
    private static ILogEngine[] logEngines = new ILogEngine[2]
    {
      (ILogEngine) new ConsoleLogEngine(),
      (ILogEngine) EventSourceLogEngine.Logger
    };
    private static LoggerLevel maxLogLevel = LoggerLevel.Info;

    public static bool DisableLogging { get; set; }

    public static void SetLogEngine(params ILogEngine[] logEngines) => Logger.logEngines = logEngines != null ? logEngines : throw new ArgumentNullException(nameof (logEngines));

    public static void SetMaxLogLevel(LoggerLevel level) => Logger.maxLogLevel = level;

    internal static bool IsLogged(LoggerLevel level, object logId, string tag) => level <= Logger.maxLogLevel && ((IEnumerable<ILogEngine>) Logger.logEngines).Any<ILogEngine>((Func<ILogEngine, bool>) (e => e.IsLogged(level, logId, tag)));

    internal static LoggerLevel GetMaxLogLevel() => Logger.maxLogLevel;

    internal static object CreateCustomLogId(string logIdName) => !string.IsNullOrEmpty(logIdName) ? (object) logIdName : throw new ArgumentNullException(nameof (logIdName));

    internal static void Log(
      LoggerLevel level,
      object logId,
      string tag,
      string format,
      params object[] objectParams)
    {
      if (level > Logger.maxLogLevel || Logger.DisableLogging)
        return;
      foreach (ILogEngine logEngine in Logger.logEngines)
        logEngine.Log(level, logId, tag, format, objectParams);
    }
  }
}
