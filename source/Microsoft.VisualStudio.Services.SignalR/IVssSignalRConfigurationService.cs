// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.IVssSignalRConfigurationService
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Messaging;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.SignalR
{
  [DefaultServiceImplementation(typeof (VssSignalRConfigurationService))]
  public interface IVssSignalRConfigurationService : IVssFrameworkService
  {
    int GetConnectionCleanupTimeoutForMonitoring(IVssRequestContext requestContext);

    int GetConnectionTimeout(IVssRequestContext requestContext);

    int GetDisconnectTimeout(IVssRequestContext requestContext);

    int GetTransportConnectTimeout(IVssRequestContext requestContext);

    int GetLongPollDelay(IVssRequestContext requestContext);

    int GetGroupCleanupTimeoutForMonitoring(IVssRequestContext requestContext);

    int GetMaxScaleoutMappingsPerStream(IVssRequestContext requestContext);

    int GetMessageBufferSize(IVssRequestContext requestContext);

    int GetHeartbeatIntervalForMonitoringKeepAlive(IVssRequestContext requestContext);

    VssMessageBusConfiguration GetMessageBusConfiguration(IVssRequestContext requestContext);

    TraceListener GetTraceListener(IVssRequestContext requestContext);

    SourceLevels GetTraceSourceLevels(IVssRequestContext requestContext);

    void RegisterTraceSettingsChangedNotification(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, SourceLevels> settingsChangedCallback);

    void SetMessageBusConfiguration(
      IVssRequestContext requestContext,
      VssMessageBusConfiguration configuration);

    void UnregisterTraceSettingsChangedNotification(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, SourceLevels> settingsChangedCallback);
  }
}
