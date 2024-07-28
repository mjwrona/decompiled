// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaLogger
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaLogger
  {
    private static bool _logToFile;
    private static TraceSource _traceSource = new TraceSource("DTA", SourceLevels.All);
    private static string _processName;
    private static int _processId;
    private const string TraceSourceName = "DTA";
    [ThreadStatic]
    private static string _sessionId = "default";

    static DtaLogger() => DtaLogger.InitializeLogger();

    public DtaLogger(TestExecutionRequestContext requestContext, string area, string layer)
    {
      this.Area = area;
      this.Layer = layer;
      this.TestExecutionRequestContext = requestContext;
    }

    public DtaLogger(TestExecutionRequestContext requestContext)
      : this(requestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.JobsLayer)
    {
    }

    public string Area { get; private set; }

    public string Layer { get; private set; }

    public TestExecutionRequestContext TestExecutionRequestContext { get; private set; }

    public string LogSession { get; }

    public void Error(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Error, format, args);

    public void Warning(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Warning, format, args);

    public void Info(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Info, format, args);

    public void Verbose(int tracePoint, string format, params object[] args) => this.Log(tracePoint, TraceLevel.Verbose, format, args);

    public static void SetSessionId(string sessionId)
    {
      if (string.IsNullOrWhiteSpace(sessionId))
        return;
      DtaLogger._sessionId = sessionId;
    }

    private void Log(int tracePoint, TraceLevel level, string format, params object[] args)
    {
      try
      {
        string str1 = format;
        if (args != null && args.Length != 0)
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
        string message1 = string.Format("{0}:{1}, {2}", (object) this.TestExecutionRequestContext.RequestContext.ServiceHost.InstanceId, (object) DtaLogger._sessionId, (object) str1);
        this.TestExecutionRequestContext.RequestContext.Trace(tracePoint, level, this.Area, this.Layer, message1);
        if (!DtaLogger._logToFile)
          return;
        string str2 = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture);
        string message2 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", (object) tracePoint, (object) DtaLogger._processId, (object) Environment.CurrentManagedThreadId, (object) str2, (object) DtaLogger._processName, (object) message1);
        switch (level)
        {
          case TraceLevel.Error:
            DtaLogger._traceSource.TraceEvent(TraceEventType.Error, 0, message2);
            break;
          case TraceLevel.Warning:
            DtaLogger._traceSource.TraceEvent(TraceEventType.Warning, 0, message2);
            break;
          case TraceLevel.Info:
            DtaLogger._traceSource.TraceEvent(TraceEventType.Information, 0, message2);
            break;
          case TraceLevel.Verbose:
            DtaLogger._traceSource.TraceEvent(TraceEventType.Verbose, 0, message2);
            break;
        }
      }
      catch (Exception ex)
      {
        this.TestExecutionRequestContext.RequestContext.Trace(tracePoint, TraceLevel.Error, this.Area, this.Layer, string.Format("Failed to log {0} : {1}", (object) format, (object) ex));
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
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TestExecutionServiceFileLogger", target)))
        {
          string fileName = Path.Combine(Environment.ExpandEnvironmentVariables(Path.GetTempPath()), DtaLogger.GetLogFileName());
          DtaLogger._traceSource.Listeners.Add((TraceListener) new TextWriterTraceListener(fileName));
          Process currentProcess = Process.GetCurrentProcess();
          DtaLogger._processName = Environment.MachineName + "/" + currentProcess.ProcessName;
          DtaLogger._processId = currentProcess.Id;
          DtaLogger._logToFile = true;
          Trace.AutoFlush = true;
          break;
        }
      }
    }

    private static string GetLogFileName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestExecutionService {0:dd} {0:MMM} {0:yyyy} ({0:HH} {0:mm} {0:ss} {0:fff}).log", (object) DateTime.UtcNow);
  }
}
