// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.ICoreEventSource
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  internal interface ICoreEventSource
  {
    void LogVerbose(string message, string appDomainName = "Incorrect");

    void BuildInfoConfigBrokenXmlError(string msg, string appDomainName = "Incorrect");

    void PopulateRequiredStringWithValue(
      string parameterName,
      string telemetryType,
      string appDomainName = "Incorrect");

    void RequestTelemetryIncorrectDuration(string appDomainName = "Incorrect");

    void LogError(string msg, string appDomainName = "Incorrect");

    void DiagnosticsEventThrottlingHasBeenStartedForTheEvent(int eventId, string appDomainName = "Incorrect");

    void DiagnosticsEventThrottlingHasBeenResetForTheEvent(
      int eventId,
      int executionCount,
      string appDomainName = "Incorrect");

    void DiagnoisticsEventThrottlingSchedulerTimerWasCreated(
      int intervalInMilliseconds,
      string appDomainName = "Incorrect");

    void DiagnoisticsEventThrottlingSchedulerTimerWasRemoved(string appDomainName = "Incorrect");

    void DiagnoisticsEventThrottlingSchedulerDisposeTimerFailure(
      string exception,
      string appDomainName = "Incorrect");

    void TrackingWasDisabled(string appDomainName = "Incorrect");

    void TrackingWasEnabled(string appDomainName = "Incorrect");

    void TelemetryClientConstructorWithNoTelemetryConfiguration(string appDomainName = "Incorrect");
  }
}
