// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process.LegacyWorkItemTrackingProcessWorkDefinitionCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process
{
  internal class LegacyWorkItemTrackingProcessWorkDefinitionCache : 
    VssMemoryCacheService<Guid, ProcessWorkDefinition>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private const int c_maxCacheSize = 318;

    public LegacyWorkItemTrackingProcessWorkDefinitionCache()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, ProcessWorkDefinition>().WithMaxElements(318).WithCleanupInterval(LegacyWorkItemTrackingProcessWorkDefinitionCache.s_cacheCleanupInterval))
    {
      this.InactivityInterval.Value = LegacyWorkItemTrackingProcessWorkDefinitionCache.s_maxCacheInactivityAge;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.LegacyProcessFieldDefinitionChanged, new SqlNotificationCallback(this.ClearCache), true);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.LegacyProcessFieldDefinitionChanged, new SqlNotificationCallback(this.ClearCache), true);

    private void ClearCache(IVssRequestContext requestContext, Guid eventClass, string eventData) => this.Clear(requestContext);
  }
}
