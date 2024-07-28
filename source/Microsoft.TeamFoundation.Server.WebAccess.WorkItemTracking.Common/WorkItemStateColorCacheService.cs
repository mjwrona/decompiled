// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemStateColorCacheService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class WorkItemStateColorCacheService : 
    VssVersionedCacheService<WorkItemStateColorCachedData>
  {
    protected override WorkItemStateColorCachedData InitializeCache(
      IVssRequestContext requestContext)
    {
      return new WorkItemStateColorCachedData((VssCacheBase) this);
    }

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

    public void RemoveProjectStateColors(IVssRequestContext requestContext, Guid projectId)
    {
      this.TraceCacheReset(requestContext, string.Format("RemoveProjectStateColors: projectId:{0}", (object) projectId));
      this.Invalidate<bool>(requestContext, (Func<WorkItemStateColorCachedData, bool>) (cache => cache.Remove(projectId)));
    }

    public bool TryGetWorkItemStateColorMapFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      out IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> stateColorMap)
    {
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      stateColorMap = (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) null;
      if (!requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
        return false;
      IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> cachedMap = (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) null;
      if (this.TryRead(requestContext, (Func<WorkItemStateColorCachedData, bool>) (cacheData => cacheData.TryGetValue(projectId, out cachedMap))))
        stateColorMap = this.GetSecureStateColorMap(cachedMap, processReadSecuredObject);
      return stateColorMap != null;
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColorMap(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>> funcGetWorkItemTypeStateColorMap)
    {
      IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> stateColorMap1;
      if (this.TryGetWorkItemStateColorMapFromCache(requestContext, projectId, out stateColorMap1))
        return stateColorMap1;
      IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> stateColorMap2 = this.Synchronize<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>(requestContext, funcGetWorkItemTypeStateColorMap, (Action<WorkItemStateColorCachedData, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) ((cache, stateMap) => cache.Add(projectId, stateMap)));
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      return !requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject) ? (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) null : this.GetSecureStateColorMap(stateColorMap2, processReadSecuredObject);
    }

    private IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetSecureStateColorMap(
      IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> stateColorMap,
      ProcessReadSecuredObject processReadSecuredObject)
    {
      IDictionary<string, IReadOnlyCollection<WorkItemStateColor>> secureStateColorMap = (IDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) new Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>();
      foreach (string key in stateColorMap.Keys)
      {
        IReadOnlyCollection<WorkItemStateColor> stateColor = stateColorMap[key];
        WorkItemStateColor[] workItemStateColorArray = new WorkItemStateColor[stateColor.Count];
        int index = 0;
        foreach (WorkItemStateColor workItemStateColor1 in (IEnumerable<WorkItemStateColor>) stateColor)
        {
          WorkItemStateColor workItemStateColor2 = new WorkItemStateColor();
          workItemStateColor2.SetSecuredObjectProperties(processReadSecuredObject);
          workItemStateColor2.Category = workItemStateColor1.Category;
          workItemStateColor2.Color = workItemStateColor1.Color;
          workItemStateColor2.Name = workItemStateColor1.Name;
          workItemStateColorArray[index] = workItemStateColor2;
          ++index;
        }
        secureStateColorMap[key] = (IReadOnlyCollection<WorkItemStateColor>) workItemStateColorArray;
      }
      return (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) secureStateColorMap;
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
      VssRequestContextExtensions.Trace(requestContext, 15114018, TraceLevel.Info, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, format, (object[]) args);
    }
  }
}
