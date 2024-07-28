// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.WindowsCoreEventSource
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Diagnostics.Tracing;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  [EventSource(Name = "Microsoft-ApplicationInsights-Core")]
  internal sealed class WindowsCoreEventSource : EventSource, ICoreEventSource
  {
    private readonly ApplicationNameProvider nameProvider = new ApplicationNameProvider();
    public static readonly WindowsCoreEventSource Log = new WindowsCoreEventSource();

    [Event(10, Keywords = (EventKeywords) 4, Message = "[msg=Log verbose];[msg={0}]", Level = EventLevel.Verbose)]
    public void LogVerbose(string msg, string appDomainName = "Incorrect") => this.WriteEvent(10, msg ?? string.Empty, this.nameProvider.Name);

    [Event(20, Keywords = (EventKeywords) 3, Message = "Diagnostics event throttling has been started for the event {0}", Level = EventLevel.Informational)]
    public void DiagnosticsEventThrottlingHasBeenStartedForTheEvent(
      int eventId,
      string appDomainName = "Incorrect")
    {
      this.WriteEvent(20, (object) eventId, (object) this.nameProvider.Name);
    }

    [Event(30, Keywords = (EventKeywords) 3, Message = "Diagnostics event throttling has been reset for the event {0}, event was fired {1} times during last interval", Level = EventLevel.Informational)]
    public void DiagnosticsEventThrottlingHasBeenResetForTheEvent(
      int eventId,
      int executionCount,
      string appDomainName = "Incorrect")
    {
      this.WriteEvent(30, (object) eventId, (object) executionCount, (object) this.nameProvider.Name);
    }

    [Event(40, Keywords = (EventKeywords) 2, Message = "Scheduler timer dispose failure: {0}", Level = EventLevel.Warning)]
    public void DiagnoisticsEventThrottlingSchedulerDisposeTimerFailure(
      string exception,
      string appDomainName = "Incorrect")
    {
      this.WriteEvent(40, exception ?? string.Empty, this.nameProvider.Name);
    }

    [Event(50, Keywords = (EventKeywords) 2, Message = "A scheduler timer was created for the interval: {0}", Level = EventLevel.Verbose)]
    public void DiagnoisticsEventThrottlingSchedulerTimerWasCreated(
      int intervalInMilliseconds,
      string appDomainName = "Incorrect")
    {
      this.WriteEvent(50, (object) intervalInMilliseconds, (object) this.nameProvider.Name);
    }

    [Event(60, Keywords = (EventKeywords) 2, Message = "A scheduler timer was removed", Level = EventLevel.Verbose)]
    public void DiagnoisticsEventThrottlingSchedulerTimerWasRemoved(string appDomainName = "Incorrect") => this.WriteEvent(60, this.nameProvider.Name);

    [Event(70, Message = "No Telemetry Configuration provided. Using the default TelemetryConfiguration.Active.", Level = EventLevel.Warning)]
    public void TelemetryClientConstructorWithNoTelemetryConfiguration(string appDomainName = "Incorrect") => this.WriteEvent(70, this.nameProvider.Name);

    [Event(71, Message = "Value for property '{0}' of {1} was not found. Populating it by default.", Level = EventLevel.Verbose)]
    public void PopulateRequiredStringWithValue(
      string parameterName,
      string telemetryType,
      string appDomainName = "Incorrect")
    {
      this.WriteEvent(71, parameterName ?? string.Empty, telemetryType ?? string.Empty, this.nameProvider.Name);
    }

    [Event(72, Message = "Invalid duration for Request Telemetry. Setting it to '00:00:00'.", Level = EventLevel.Warning)]
    public void RequestTelemetryIncorrectDuration(string appDomainName = "Incorrect") => this.WriteEvent(72, this.nameProvider.Name);

    [Event(80, Message = "Telemetry tracking was disabled. Message is dropped.", Level = EventLevel.Verbose)]
    public void TrackingWasDisabled(string appDomainName = "Incorrect") => this.WriteEvent(80, this.nameProvider.Name);

    [Event(81, Message = "Telemetry tracking was enabled. Messages are being logged.", Level = EventLevel.Verbose)]
    public void TrackingWasEnabled(string appDomainName = "Incorrect") => this.WriteEvent(81, this.nameProvider.Name);

    [Event(90, Keywords = (EventKeywords) 8, Message = "[msg=Log Error];[msg={0}]", Level = EventLevel.Error)]
    public void LogError(string msg, string appDomainName = "Incorrect") => this.WriteEvent(90, msg ?? string.Empty, this.nameProvider.Name);

    [Event(91, Keywords = (EventKeywords) 9, Message = "BuildInfo.config file has incorrect xml structure. Context component version will not be populated. Exception: {0}.", Level = EventLevel.Error)]
    public void BuildInfoConfigBrokenXmlError(string msg, string appDomainName = "Incorrect") => this.WriteEvent(91, msg ?? string.Empty, this.nameProvider.Name);

    public sealed class Keywords
    {
      public const EventKeywords UserActionable = (EventKeywords) 1;
      public const EventKeywords Diagnostics = (EventKeywords) 2;
      public const EventKeywords VerboseFailure = (EventKeywords) 4;
      public const EventKeywords ErrorFailure = (EventKeywords) 8;
    }
  }
}
