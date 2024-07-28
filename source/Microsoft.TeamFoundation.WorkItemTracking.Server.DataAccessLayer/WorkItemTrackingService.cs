// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTrackingService : IVssFrameworkService
  {
    private static readonly Guid s_queryItemNotificationId = new Guid("8E616C67-502E-4BB5-8037-B6D49CAA6E73");
    private ConcurrentDictionary<int, object> m_unsupportedCultures = new ConcurrentDictionary<int, object>();

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.QueryItemSecurity = systemRequestContext.GetService<LocalSecurityService>().GetSecurityNamespace(systemRequestContext, QueryItemSecurityConstants.NamespaceGuid);
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "WorkItem", WorkItemTrackingService.s_queryItemNotificationId, new SqlNotificationCallback(this.OnQueryItemsChanged), true);
    }

    internal void OnQueryItemsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.QueryItemSecurity.OnDataChanged(requestContext);
    }

    internal void OnQueryItemsDeleted(
      IVssRequestContext requestContext,
      IEnumerable<ServerQueryItem> deletedQueryItems)
    {
      this.QueryItemSecurity.RemoveAccessControlLists(requestContext, deletedQueryItems.Select<ServerQueryItem, string>((Func<ServerQueryItem, string>) (qi => qi.SecurityToken)), true);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.QueryItemSecurity == null)
        return;
      this.QueryItemSecurity = (LocalSecurityNamespace) null;
    }

    internal bool IsCultureSupportedByDatabase(int lcid) => !this.m_unsupportedCultures.ContainsKey(lcid);

    internal void MarkCultureUnsupportedByDatabase(int lcid) => this.m_unsupportedCultures.TryAdd(lcid, (object) null);

    internal LocalSecurityNamespace QueryItemSecurity { get; private set; }
  }
}
