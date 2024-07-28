// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemStateCacheService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class WorkItemStateCacheService : VssVersionedCacheService<WorkItemStateCachedData>
  {
    protected override WorkItemStateCachedData InitializeCache(IVssRequestContext requestContext) => new WorkItemStateCachedData((VssCacheBase) this);

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (Guid key in ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap.Keys)
        service.RegisterNotification(systemRequestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (Guid key in ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap.Keys)
        service.UnregisterNotification(systemRequestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
      base.ServiceEnd(systemRequestContext);
    }

    public void RemoveWorkItemStates(IVssRequestContext requestContext, Guid projectId)
    {
      this.TraceCacheReset(requestContext, string.Format("RemoveWorkItemStates: projectId:{0}", (object) projectId));
      this.Invalidate<bool>(requestContext, (Func<WorkItemStateCachedData, bool>) (cache => cache.Remove(projectId)));
    }

    public IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> GetWorkItemStatesMap(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>> funcGetStatesForProject)
    {
      IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> stateGroupToStateDefinitionMap;
      return this.TryGetWorkItemStatesFromCache(requestContext, projectId, out stateGroupToStateDefinitionMap) ? stateGroupToStateDefinitionMap : this.Synchronize<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>(requestContext, funcGetStatesForProject, (Action<WorkItemStateCachedData, IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) ((cache, stateMap) => cache.Add(projectId, stateMap)));
    }

    private void OnNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      string empty = string.Empty;
      if (!ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap.TryGetValue(eventClass, out empty))
        empty = eventClass.ToString();
      this.TraceCacheReset(requestContext, string.Format("OnNotification: eventClass:{0}, eventClassName: {1}, eventData: {2}", (object) eventClass, (object) empty, (object) eventData));
      this.Reset(requestContext);
    }

    private void TraceCacheReset(
      IVssRequestContext requestContext,
      string format,
      params string[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, 15114019, TraceLevel.Info, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, format, (object[]) args);
    }

    private bool TryGetWorkItemStatesFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      out IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> stateGroupToStateDefinitionMap)
    {
      stateGroupToStateDefinitionMap = (IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>) null;
      IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> retMap = (IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>) null;
      int num = this.TryRead(requestContext, (Func<WorkItemStateCachedData, bool>) (cacheData => cacheData.TryGetValue(projectId, out retMap))) ? 1 : 0;
      stateGroupToStateDefinitionMap = retMap;
      return num != 0;
    }
  }
}
