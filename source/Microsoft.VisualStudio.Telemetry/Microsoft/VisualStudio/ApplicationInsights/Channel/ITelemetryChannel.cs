// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.ITelemetryChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  public interface ITelemetryChannel : IDisposable
  {
    bool DeveloperMode { get; set; }

    string EndpointAddress { get; set; }

    void Send(ITelemetry item);

    void Flush();

    Task FlushAndTransmitAsync(CancellationToken token);
  }
}
