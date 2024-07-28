// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WatsonSessionChannelBuilder
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class WatsonSessionChannelBuilder
  {
    private readonly int faultEventSamplePercent;
    private readonly int faultEventMaximumWatsonReportsPerSession;
    private readonly int faultEventMinimumSecondsBetweenWatsonReports;
    private readonly ChannelProperties properties;

    public WatsonSessionChannelBuilder(
      int faultEventSamplePercent,
      int faultEventMaximumWatsonReportsPerSession,
      int faultEventMinimumSecondsBetweenWatsonReports,
      ChannelProperties properties)
    {
      this.faultEventSamplePercent = faultEventSamplePercent;
      this.faultEventMaximumWatsonReportsPerSession = faultEventMaximumWatsonReportsPerSession;
      this.faultEventMinimumSecondsBetweenWatsonReports = faultEventMinimumSecondsBetweenWatsonReports;
      this.properties = properties;
    }

    public WatsonSessionChannel WatsonSessionChannel { get; private set; }

    public void Build(TelemetrySession hostSession)
    {
      hostSession.RequiresArgumentNotNull<TelemetrySession>(nameof (hostSession));
      this.WatsonSessionChannel = new WatsonSessionChannel(hostSession, this.faultEventSamplePercent, this.faultEventMaximumWatsonReportsPerSession, this.faultEventMinimumSecondsBetweenWatsonReports)
      {
        Properties = this.properties
      };
    }
  }
}
