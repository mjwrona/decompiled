// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMLogger
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TCMLogger
  {
    private static bool _logToFile;
    private static TraceSource _traceSource = new TraceSource("TCM", SourceLevels.All);
    private static string _processName;
    private static int _processId;
    private const string TraceSourceName = "TCM";
    [ThreadStatic]
    private static string _sessionId = "default";

    static TCMLogger() => TCMLogger.InitializeLogger();

    public TCMLogger(IVssRequestContext requestContext, string area, string layer)
    {
      this.Area = area;
      this.Layer = layer;
      this.RequestContext = requestContext;
    }

    public TCMLogger(IVssRequestContext requestContext)
      : this(requestContext, "TestManagement", "TestManagementJob")
    {
    }

    public string Area { get; private set; }

    public string Layer { get; private set; }

    public IVssRequestContext RequestContext { get; private set; }

    public string LogSession { get; }

    public void Error(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Error, format, args);

    public void Warning(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Warning, format, args);

    public void Info(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Info, format, args);

    public void Verbose(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Verbose, format, args);

    public static void SetSessionId(string sessionId)
    {
      if (string.IsNullOrWhiteSpace(sessionId))
        return;
      TCMLogger._sessionId = sessionId;
    }

    private void Log(int tracePoint, TraceLevel level, string format, params object[] args)
    {
      try
      {
        string str1 = format;
        if (args != null && args.Length != 0)
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
        string message1 = string.Format("{0}:{1}, {2}", (object) this.RequestContext.ServiceHost.InstanceId, (object) TCMLogger._sessionId, (object) str1);
        this.RequestContext.Trace(tracePoint, level, this.Area, this.Layer, message1);
        if (!TCMLogger._logToFile)
          return;
        string str2 = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture);
        string message2 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", (object) tracePoint, (object) TCMLogger._processId, (object) Environment.CurrentManagedThreadId, (object) str2, (object) TCMLogger._processName, (object) message1);
        switch (level)
        {
          case TraceLevel.Error:
            TCMLogger._traceSource.TraceEvent(TraceEventType.Error, 0, message2);
            break;
          case TraceLevel.Warning:
            TCMLogger._traceSource.TraceEvent(TraceEventType.Warning, 0, message2);
            break;
          case TraceLevel.Info:
            TCMLogger._traceSource.TraceEvent(TraceEventType.Information, 0, message2);
            break;
          case TraceLevel.Verbose:
            TCMLogger._traceSource.TraceEvent(TraceEventType.Verbose, 0, message2);
            break;
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.Trace(tracePoint, TraceLevel.Error, this.Area, this.Layer, string.Format("Failed to log {0} : {1}", (object) format, (object) ex));
      }
    }

    private static void InitializeLogger()
    {
      EnvironmentVariableTarget[] environmentVariableTargetArray = new EnvironmentVariableTarget[3]
      {
        EnvironmentVariableTarget.Machine,
        EnvironmentVariableTarget.Process,
        EnvironmentVariableTarget.User
      };
      foreach (EnvironmentVariableTarget target in environmentVariableTargetArray)
      {
        string str = (string) null;
        try
        {
          str = Environment.GetEnvironmentVariable("TCMFileLogger", target);
        }
        catch (SecurityException ex)
        {
        }
        if (!string.IsNullOrWhiteSpace(str))
        {
          string fileName = Path.Combine(Environment.ExpandEnvironmentVariables(Path.GetTempPath()), TCMLogger.GetLogFileName());
          TCMLogger._traceSource.Listeners.Add((TraceListener) new TextWriterTraceListener(fileName));
          Process currentProcess = Process.GetCurrentProcess();
          TCMLogger._processName = Environment.MachineName + "/" + currentProcess.ProcessName;
          TCMLogger._processId = currentProcess.Id;
          TCMLogger._logToFile = true;
          Trace.AutoFlush = true;
          break;
        }
      }
    }

    private static string GetLogFileName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TCM {0:dd} {0:MMM} {0:yyyy} ({0:HH} {0:mm} {0:ss} {0:fff}).log", (object) DateTime.UtcNow);
  }
}
