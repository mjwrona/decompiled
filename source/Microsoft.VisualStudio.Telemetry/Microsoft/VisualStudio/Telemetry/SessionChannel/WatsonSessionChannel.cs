// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.WatsonSessionChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class WatsonSessionChannel : ISessionChannel
  {
    public const string MaxWatsonReportsReached = "VS.Fault.MaximumWatsonReportsReached";

    private TelemetrySession TelemetrySession { get; }

    internal static Random Random { get; } = new Random();

    internal static int NumberOfWatsonReportsThisSession { get; set; }

    internal static DateTime DateTimeOfLastWatsonReport { get; set; }

    public int FaultEventWatsonSamplePercent { get; }

    public int FaultEventMaximumWatsonReportsPerSession { get; }

    public int FaultEventMinimumSecondsBetweenWatsonReports { get; }

    public WatsonSessionChannel(
      TelemetrySession telemetrySession,
      int faultEventWatsonSamplePercent,
      int faultEventMaximumWatsonReportsPerSession,
      int faultEventMinimumSecondsBetweenWatsonReports)
    {
      this.TelemetrySession = telemetrySession;
      this.FaultEventWatsonSamplePercent = faultEventWatsonSamplePercent;
      this.FaultEventMaximumWatsonReportsPerSession = faultEventMaximumWatsonReportsPerSession;
      this.FaultEventMinimumSecondsBetweenWatsonReports = faultEventMinimumSecondsBetweenWatsonReports;
      WatsonSessionChannel.DateTimeOfLastWatsonReport = DateTime.MinValue;
    }

    public string ChannelId => "WatsonChannel";

    public bool IsStarted => true;

    public ChannelProperties Properties { get; set; } = ChannelProperties.Default;

    public string TransportUsed => this.ChannelId;

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      if (!this.TelemetrySession.IsOptedIn)
        return;
      if (!(telemetryEvent is FaultEvent faultEvent))
        throw new InvalidOperationException("WatsonSession channel must have FaultEvent posted");
      int watsonSamplePercent = this.FaultEventWatsonSamplePercent;
      faultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonSamplePercentDefault", (object) this.FaultEventWatsonSamplePercent);
      int? nullable = FaultEvent.WatsonSamplePercent;
      if (nullable.HasValue)
      {
        nullable = FaultEvent.WatsonSamplePercent;
        watsonSamplePercent = nullable.Value;
        TelemetryPropertyBags.PrefixedNotConcurrent<object> reservedProperties = faultEvent.ReservedProperties;
        nullable = FaultEvent.WatsonSamplePercent;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (ValueType) nullable.Value;
        reservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonSamplePercentOverride", (object) local);
      }
      bool? includedInWatsonSample = faultEvent.IsIncludedInWatsonSample;
      if (!includedInWatsonSample.HasValue)
      {
        faultEvent.UserOptInToWatson = FaultEvent.FaultEventWatsonOptIn.Unspecified;
        faultEvent.IsIncludedInWatsonSample = watsonSamplePercent <= 0 ? new bool?(false) : new bool?(WatsonSessionChannel.Random.Next(100) < watsonSamplePercent);
      }
      else
      {
        includedInWatsonSample = faultEvent.IsIncludedInWatsonSample;
        bool flag = true;
        faultEvent.UserOptInToWatson = !(includedInWatsonSample.GetValueOrDefault() == flag & includedInWatsonSample.HasValue) ? FaultEvent.FaultEventWatsonOptIn.PropertyOptOut : FaultEvent.FaultEventWatsonOptIn.PropertyOptIn;
        faultEvent.Properties["VS.Fault.WatsonOptIn"] = (object) faultEvent.UserOptInToWatson.ToString();
      }
      WatsonReport watsonReport = new WatsonReport(faultEvent, this.TelemetrySession);
      int reportsPerSession = this.FaultEventMaximumWatsonReportsPerSession;
      faultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.MaxReportsPerSessionDefault", (object) this.FaultEventMaximumWatsonReportsPerSession);
      nullable = FaultEvent.MaximumWatsonReportsPerSession;
      if (nullable.HasValue)
      {
        nullable = FaultEvent.MaximumWatsonReportsPerSession;
        reportsPerSession = nullable.Value;
        faultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.MaxReportsPerSessionOverride", (object) this.FaultEventMaximumWatsonReportsPerSession);
      }
      if (watsonSamplePercent == 0 && reportsPerSession == 0)
        faultEvent.IsIncludedInWatsonSample = new bool?(false);
      int betweenWatsonReports = this.FaultEventMinimumSecondsBetweenWatsonReports;
      faultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.MinSecondsBetweenReportsDefault", (object) this.FaultEventMinimumSecondsBetweenWatsonReports);
      nullable = FaultEvent.MinimumSecondsBetweenWatsonReports;
      if (nullable.HasValue)
      {
        nullable = FaultEvent.MinimumSecondsBetweenWatsonReports;
        betweenWatsonReports = nullable.Value;
        faultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.MinSecondsBetweenReportsOverride", (object) FaultEvent.MinimumSecondsBetweenWatsonReports);
      }
      int maxReportsPerSession = reportsPerSession;
      int minSecondsBetweenReports = betweenWatsonReports;
      watsonReport.PostWatsonReport(maxReportsPerSession, minSecondsBetweenReports);
    }

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      throw new InvalidOperationException("WatsonSession channel doesn't take args on posted");
    }

    public void Start(string sessionId)
    {
    }
  }
}
