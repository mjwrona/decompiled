// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.IAppInsightsClientWrapper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal interface IAppInsightsClientWrapper : IDisposable, IDisposeAndTransmit
  {
    string InstrumentationKey { get; }

    void Initialize(string sessionId, string userId);

    void TrackEvent(EventTelemetry ev);

    bool TryGetTransport(out string transportUsed);
  }
}
