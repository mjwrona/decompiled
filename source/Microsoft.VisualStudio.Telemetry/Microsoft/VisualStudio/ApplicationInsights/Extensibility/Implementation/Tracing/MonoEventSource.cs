// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.MonoEventSource
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  internal sealed class MonoEventSource : ICoreEventSource
  {
    public void BuildInfoConfigBrokenXmlError(string msg, string appDomainName = "Incorrect")
    {
    }

    public void DiagnoisticsEventThrottlingSchedulerDisposeTimerFailure(
      string exception,
      string appDomainName = "Incorrect")
    {
    }

    public void DiagnoisticsEventThrottlingSchedulerTimerWasCreated(
      int intervalInMilliseconds,
      string appDomainName = "Incorrect")
    {
    }

    public void DiagnoisticsEventThrottlingSchedulerTimerWasRemoved(string appDomainName = "Incorrect")
    {
    }

    public void DiagnosticsEventThrottlingHasBeenResetForTheEvent(
      int eventId,
      int executionCount,
      string appDomainName = "Incorrect")
    {
    }

    public void DiagnosticsEventThrottlingHasBeenStartedForTheEvent(
      int eventId,
      string appDomainName = "Incorrect")
    {
    }

    public void LogError(string msg, string appDomainName = "Incorrect")
    {
    }

    public void LogVerbose(string message, string appDomainName = "Incorrect")
    {
    }

    public void PopulateRequiredStringWithValue(
      string parameterName,
      string telemetryType,
      string appDomainName = "Incorrect")
    {
    }

    public void RequestTelemetryIncorrectDuration(string appDomainName = "Incorrect")
    {
    }

    public void TelemetryClientConstructorWithNoTelemetryConfiguration(string appDomainName = "Incorrect")
    {
    }

    public void TrackingWasDisabled(string appDomainName = "Incorrect")
    {
    }

    public void TrackingWasEnabled(string appDomainName = "Incorrect")
    {
    }
  }
}
