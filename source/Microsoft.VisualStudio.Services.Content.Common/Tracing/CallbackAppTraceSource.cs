// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.CallbackAppTraceSource
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public class CallbackAppTraceSource : IAppTraceSource
  {
    private readonly Action<string, SourceLevels> traceMessageSeverityCallback;
    private readonly SourceLevels leastSevereLevelToTrace;
    private readonly bool includeSeverityLevelInMessage;
    private static Dictionary<TraceEventType, SourceLevels> toLevel = new Dictionary<TraceEventType, SourceLevels>()
    {
      {
        TraceEventType.Critical,
        SourceLevels.Critical
      },
      {
        TraceEventType.Error,
        SourceLevels.Error
      },
      {
        TraceEventType.Information,
        SourceLevels.Information
      },
      {
        TraceEventType.Resume,
        SourceLevels.ActivityTracing
      },
      {
        TraceEventType.Start,
        SourceLevels.ActivityTracing
      },
      {
        TraceEventType.Stop,
        SourceLevels.ActivityTracing
      },
      {
        TraceEventType.Suspend,
        SourceLevels.ActivityTracing
      },
      {
        TraceEventType.Transfer,
        SourceLevels.ActivityTracing
      },
      {
        TraceEventType.Verbose,
        SourceLevels.Verbose
      },
      {
        TraceEventType.Warning,
        SourceLevels.Warning
      }
    };

    public CallbackAppTraceSource(
      Action<string> traceMessageCallback,
      SourceLevels leastSevereLevelToTrace,
      bool includeSeverityLevel = true)
    {
      ArgumentUtility.CheckForNull<Action<string>>(traceMessageCallback, nameof (traceMessageCallback));
      this.traceMessageSeverityCallback = (Action<string, SourceLevels>) ((message, severity) => traceMessageCallback(message));
      this.leastSevereLevelToTrace = leastSevereLevelToTrace;
      this.includeSeverityLevelInMessage = includeSeverityLevel;
    }

    public CallbackAppTraceSource(
      Action<string, SourceLevels> traceMessageSeverityCallback,
      SourceLevels leastSevereLevelToTrace)
    {
      ArgumentUtility.CheckForNull<Action<string, SourceLevels>>(traceMessageSeverityCallback, nameof (traceMessageSeverityCallback));
      this.traceMessageSeverityCallback = traceMessageSeverityCallback;
      this.leastSevereLevelToTrace = leastSevereLevelToTrace;
      this.includeSeverityLevelInMessage = false;
    }

    private void TraceInternal(SourceLevels severity, string message)
    {
      if (severity == SourceLevels.All)
        throw new ArgumentException(SafeStringFormat.FormatSafe("Message's severity must not be the behavior value SourceLevels.All"));
      if (severity == SourceLevels.Off)
        throw new ArgumentException(SafeStringFormat.FormatSafe("Message's severity must not be the behavior value SourceLevels.Off"));
      if (!(this.leastSevereLevelToTrace == SourceLevels.All | severity <= this.leastSevereLevelToTrace))
        return;
      if (this.includeSeverityLevelInMessage)
        message = SafeStringFormat.FormatSafe(severity.ToString() + ", " + message);
      this.traceMessageSeverityCallback(message, severity);
    }

    private void TraceInternal(
      SourceLevels severity,
      int? id,
      Exception ex,
      string format,
      params object[] args)
    {
      StringBuilder message = new StringBuilder();
      if (id.HasValue)
      {
        message.Append((object) id);
        if (!string.IsNullOrEmpty(format) || ex != null)
          message.Append(", ");
      }
      if (!string.IsNullOrEmpty(format))
      {
        message.AppendFormatSafe(format, args);
        if (ex != null)
          message.Append(" ");
      }
      if (ex != null)
        message.Append(ex.ToString());
      this.TraceInternal(severity, message.ToString());
    }

    private void TraceEventInternal(
      TraceEventType type,
      int? id,
      Exception ex,
      string format,
      params object[] args)
    {
      StringBuilder message = new StringBuilder();
      SourceLevels severity = CallbackAppTraceSource.toLevel[type];
      if (id.HasValue)
      {
        message.Append((object) id);
        if (severity == SourceLevels.ActivityTracing || !string.IsNullOrEmpty(format) || ex != null)
          message.Append(", ");
      }
      if (severity == SourceLevels.ActivityTracing)
      {
        message.Append(type.ToString());
        if (!string.IsNullOrEmpty(format) || ex != null)
          message.Append(", ");
      }
      if (!string.IsNullOrEmpty(format))
      {
        message.AppendFormatSafe(format, args);
        if (ex != null)
          message.Append(" ");
      }
      if (ex != null)
        message.Append(ex.ToString());
      this.TraceInternal(severity, message.ToString());
    }

    public TraceListenerCollection Listeners => (TraceListenerCollection) null;

    public bool HasError => false;

    public SourceLevels SwitchLevel => this.leastSevereLevelToTrace;

    public void AddConsoleTraceListener()
    {
    }

    public void AddFileTraceListener(string fullFileName)
    {
    }

    public void Critical(string format, params object[] args) => this.TraceInternal(SourceLevels.Critical, new int?(), (Exception) null, format, args);

    public void Critical(int id, string format, params object[] args) => this.TraceInternal(SourceLevels.Critical, new int?(id), (Exception) null, format, args);

    public void Critical(Exception ex) => this.TraceInternal(SourceLevels.Critical, new int?(), ex, (string) null);

    public void Critical(int id, Exception ex) => this.TraceInternal(SourceLevels.Critical, new int?(id), ex, (string) null);

    public void Critical(Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Critical, new int?(), ex, format, args);

    public void Critical(int id, Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Critical, new int?(id), ex, format, args);

    public void Error(string format, params object[] args) => this.TraceInternal(SourceLevels.Error, new int?(), (Exception) null, format, args);

    public void Error(int id, string format, params object[] args) => this.TraceInternal(SourceLevels.Error, new int?(id), (Exception) null, format, args);

    public void Error(Exception ex) => this.TraceInternal(SourceLevels.Error, new int?(), ex, (string) null);

    public void Error(int id, Exception ex) => this.TraceInternal(SourceLevels.Error, new int?(id), ex, (string) null);

    public void Error(Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Error, new int?(), ex, format, args);

    public void Error(int id, Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Error, new int?(id), ex, format, args);

    public void Info(string format, params object[] args) => this.TraceInternal(SourceLevels.Information, new int?(), (Exception) null, format, args);

    public void Info(int id, string format, params object[] args) => this.TraceInternal(SourceLevels.Information, new int?(id), (Exception) null, format, args);

    public void TraceEvent(TraceEventType eventType, int id) => this.TraceEventInternal(eventType, new int?(id), (Exception) null, (string) null);

    public void TraceEvent(TraceEventType eventType, int id, string message) => this.TraceEventInternal(eventType, new int?(id), (Exception) null, message);

    public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args) => this.TraceEventInternal(eventType, new int?(id), (Exception) null, format, args);

    public void Verbose(string format, params object[] args) => this.TraceInternal(SourceLevels.Verbose, new int?(), (Exception) null, format, args);

    public void Verbose(int id, string format, params object[] args) => this.TraceInternal(SourceLevels.Verbose, new int?(id), (Exception) null, format, args);

    public void Verbose(Exception ex) => this.TraceInternal(SourceLevels.Verbose, new int?(), ex, (string) null);

    public void Verbose(int id, Exception ex) => this.TraceInternal(SourceLevels.Verbose, new int?(id), ex, (string) null);

    public void Verbose(Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Verbose, new int?(), ex, format, args);

    public void Verbose(int id, Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Verbose, new int?(id), ex, format, args);

    public void Warn(string format, params object[] args) => this.TraceInternal(SourceLevels.Warning, new int?(), (Exception) null, format, args);

    public void Warn(int id, string format, params object[] args) => this.TraceInternal(SourceLevels.Warning, new int?(id), (Exception) null, format, args);

    public void Warn(Exception ex) => this.TraceInternal(SourceLevels.Warning, new int?(), ex, (string) null);

    public void Warn(int id, Exception ex) => this.TraceInternal(SourceLevels.Warning, new int?(id), ex, (string) null);

    public void Warn(Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Warning, new int?(), ex, format, args);

    public void Warn(int id, Exception ex, string format, params object[] args) => this.TraceInternal(SourceLevels.Warning, new int?(id), ex, format, args);

    public void ResetErrorDetection()
    {
    }
  }
}
