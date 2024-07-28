// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Logging.EventSourceLogEngine
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Diagnostics.Tracing;
using System.Globalization;

namespace Microsoft.Cloud.Metrics.Client.Logging
{
  [EventSource(Name = "Microsoft-MDMetricsClient", Guid = "{FEB9BEAF-6D93-442E-BB78-7F581B618201}")]
  public sealed class EventSourceLogEngine : EventSource, ILogEngine, IDisposable
  {
    private static readonly Lazy<EventSourceLogEngine> Instance = new Lazy<EventSourceLogEngine>((Func<EventSourceLogEngine>) (() => new EventSourceLogEngine()));

    private EventSourceLogEngine()
    {
    }

    public static EventSourceLogEngine Logger => EventSourceLogEngine.Instance.Value;

    public void Log(
      LoggerLevel level,
      object logId,
      string tag,
      string format,
      params object[] objectParams)
    {
      if (!this.IsLogged(level, logId, tag))
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Level=[{0}] LogId=[{1}] Tag=[{2}] {3}", (object) level, logId, (object) tag, (object) format), objectParams);
      switch (level)
      {
        case LoggerLevel.Error:
          this.EventSourceLogError(message);
          break;
        case LoggerLevel.Warning:
          this.EventSourceLogWarning(message);
          break;
        case LoggerLevel.CustomerFacingInfo:
          this.EventSourceLogInfo(message);
          break;
        case LoggerLevel.Info:
          this.EventSourceLogInfo(message);
          break;
        case LoggerLevel.Debug:
          this.EventSourceLogDebug(message);
          break;
      }
    }

    [Event(1, Level = EventLevel.Verbose)]
    public void EventSourceLogDebug(string message) => this.WriteEvent(1, message);

    [Event(2, Level = EventLevel.Informational)]
    public void EventSourceLogInfo(string message) => this.WriteEvent(2, message);

    [Event(3, Level = EventLevel.Warning)]
    public void EventSourceLogWarning(string message) => this.WriteEvent(3, message);

    [Event(4, Level = EventLevel.Error)]
    public void EventSourceLogError(string message) => this.WriteEvent(4, message);

    public bool IsLogged(LoggerLevel level, object logId, string tag) => this.IsEnabled(this.GetEtwLevelFromLogLevel(level), EventKeywords.None);

    private EventLevel GetEtwLevelFromLogLevel(LoggerLevel level)
    {
      switch (level)
      {
        case LoggerLevel.Error:
          return EventLevel.Error;
        case LoggerLevel.Warning:
          return EventLevel.Warning;
        case LoggerLevel.Info:
          return EventLevel.Informational;
        case LoggerLevel.Debug:
          return EventLevel.Verbose;
        default:
          return EventLevel.Informational;
      }
    }
  }
}
