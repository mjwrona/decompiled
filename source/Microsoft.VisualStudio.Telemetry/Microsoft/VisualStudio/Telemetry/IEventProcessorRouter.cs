// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.IEventProcessorRouter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface IEventProcessorRouter : IDisposable
  {
    void Reset();

    bool TryGetRouteArgument(
      string channelId,
      out IEnumerable<ITelemetryManifestRouteArgs> routeArguments);

    bool TryAddRouteArgument(string channelId, ITelemetryManifestRouteArgs routeArgument);

    void DisableChannel(string channelId);

    bool IsChannelDisabled(string channelId);

    void AddChannel(ISessionChannel channel);

    void RouteEvent(TelemetryEvent telemetryEvent, string sessionId, bool isDropped);

    Task DisposeAndTransmitAsync(CancellationToken token);

    void UpdateDefaultChannel(bool useCollector);
  }
}
