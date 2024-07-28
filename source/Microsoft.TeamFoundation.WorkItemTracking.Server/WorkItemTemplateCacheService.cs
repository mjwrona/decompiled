// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplateCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTemplateCacheService : 
    VssMemoryCacheService<Tuple<Guid, Guid>, IEnumerable<WorkItemTemplateDescriptor>>
  {
    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      this.RegisterNotifications(systemRequestContext);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.UnregisterNotifications(systemRequestContext);
      base.ServiceEnd(systemRequestContext);
    }

    internal bool TryGetCacheKey(
      IVssRequestContext requestContext,
      string eventData,
      out Tuple<Guid, Guid> key)
    {
      key = (Tuple<Guid, Guid>) null;
      string[] strArray = eventData.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
      int result1;
      Guid result2;
      if (strArray.Length != 2 || !int.TryParse(strArray[0], out result1) || !Guid.TryParse(strArray[1], out result2))
        return false;
      Guid dataspaceIdentifier = requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, result1).DataspaceIdentifier;
      key = Tuple.Create<Guid, Guid>(dataspaceIdentifier, result2);
      return true;
    }

    private void RegisterNotifications(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(requestContext, "WorkItem", DBNotificationIds.WorkItemTemplatesModified, new SqlNotificationCallback(this.OnNotification), false);
      service.RegisterNotification(requestContext, "WorkItem", DBNotificationIds.WorkItemTemplatesModifiedByWorkItemTypeRename, new SqlNotificationCallback(this.OnNotification), false);
    }

    private void UnregisterNotifications(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "WorkItem", DBNotificationIds.WorkItemTemplatesModified, new SqlNotificationCallback(this.OnNotification), false);
      service.UnregisterNotification(requestContext, "WorkItem", DBNotificationIds.WorkItemTemplatesModifiedByWorkItemTypeRename, new SqlNotificationCallback(this.OnNotification), false);
    }

    private void OnNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (eventClass == DBNotificationIds.WorkItemTemplatesModified)
      {
        Tuple<Guid, Guid> key;
        if (this.TryGetCacheKey(requestContext, eventData, out key))
        {
          this.TraceCacheRemove(requestContext, string.Format("OnNotification: eventClass: {0}, eventData: {1}, key: {2}", (object) eventClass, (object) eventData, (object) key));
          this.Remove(requestContext, key);
        }
        else
          requestContext.Trace(15116011, TraceLevel.Error, "Services", "WorkItemService", string.Format("OnNotification: Could not parse cache key eventClass: {0}, eventData: {1}", (object) eventClass, (object) eventData));
      }
      else
      {
        if (!(eventClass == DBNotificationIds.WorkItemTemplatesModifiedByWorkItemTypeRename))
          return;
        this.MemoryCache.Clear();
      }
    }

    private void TraceCacheRemove(
      IVssRequestContext requestContext,
      string format,
      params string[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, 15116010, TraceLevel.Info, "Services", "WorkItemService", format, (object[]) args);
    }
  }
}
