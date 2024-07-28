// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IServiceHostInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IServiceHostInternal
  {
    void CheckShutdown();

    int DatabaseId { get; }

    string DataDirectory { get; }

    int StorageAccountId { get; }

    string ServiceLevel { get; }

    LockManager LockManager { get; }

    TeamFoundationMetabase Metabase { get; }

    IDisposableReadOnlyList<ITeamFoundationRequestFilter> RequestFilters { get; set; }

    long TotalExecutionElapsedThreshold { get; }

    DateTime StartTime { get; }

    IVssFrameworkServiceProvider ServiceProvider { get; }

    bool IsVirtualServiceHost { get; }

    void Initialize(DateTime startTime, TeamFoundationServiceHostStatus initialStatus);

    void ApplicationEndRequest(IVssRequestContext requestContext);

    bool FlushNotificationQueue(IVssRequestContext context);

    bool FlushNotificationQueue(
      IVssRequestContext context,
      Guid processId,
      TimeSpan flushTimeout,
      bool ignoreDefaultMaxFlushTimeout = false);

    IVssRequestContext[] GetActiveRequests();

    void RecycleServices(IVssRequestContext requestContext);

    void SetRegistered();

    bool TryGetRequest(long requestId, out IVssRequestContext activeRequest);

    event EventHandler<HostPropertiesChangedEventArgs> PropertiesChanged;

    ServiceHostSubStatus SubStatus { get; }
  }
}
