// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.BaseTeamFoundationWorkItemTrackingService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class BaseTeamFoundationWorkItemTrackingService : IVssFrameworkService
  {
    private List<BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription> m_subscriptions;
    private SqlNotificationCallback m_onSqlNotification;

    protected virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual void ServiceEnd(IVssRequestContext systemRequestContext) => this.TryUnregisterSqlNotifications(systemRequestContext);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.ServiceStart(systemRequestContext);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.ServiceEnd(systemRequestContext);

    protected virtual void RegisterSqlNotifications(
      IVssRequestContext requestContext,
      params BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription[] subscribers)
    {
      if (subscribers == null || subscribers.Length == 0)
        return;
      TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
      if (service != null)
      {
        if (this.m_onSqlNotification == null)
          this.m_onSqlNotification = new SqlNotificationCallback(this.OnSqlNotification);
        foreach (BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription subscriber in subscribers)
          service.RegisterNotification(requestContext, subscriber.DatabaseCategory, subscriber.EventClass, subscriber.Callback ?? this.m_onSqlNotification, subscriber.FilterByAuthor);
      }
      if (this.m_subscriptions == null)
        this.m_subscriptions = new List<BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription>();
      this.m_subscriptions.AddRange((IEnumerable<BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription>) subscribers);
    }

    private void TryUnregisterSqlNotifications(IVssRequestContext requestContext)
    {
      if (this.m_subscriptions != null && this.m_subscriptions.Count > 0)
      {
        TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
        foreach (BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription subscription in this.m_subscriptions)
          service.UnregisterNotification(requestContext, subscription.DatabaseCategory, subscription.EventClass, subscription.Callback ?? this.m_onSqlNotification, false);
      }
      this.m_onSqlNotification = (SqlNotificationCallback) null;
    }

    protected virtual void OnSqlNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
    }

    internal virtual T ExecuteSql<T, TComponent>(
      IVssRequestContext requestContext,
      Func<TComponent, T> func)
      where TComponent : WorkItemTrackingResourceComponent, new()
    {
      TComponent component = requestContext.CreateComponent<TComponent>();
      try
      {
        return func(component);
      }
      finally
      {
        if ((object) component != null)
          ((IDisposable) component).Dispose();
      }
    }

    internal virtual void ExecuteSql<TComponent>(
      IVssRequestContext requestContext,
      Action<TComponent> func)
      where TComponent : WorkItemTrackingResourceComponent, new()
    {
      TComponent component = requestContext.CreateComponent<TComponent>();
      try
      {
        func(component);
      }
      finally
      {
        if ((object) component != null)
          ((IDisposable) component).Dispose();
      }
    }

    protected struct SqlNotificationSubscription
    {
      public Guid EventClass { get; set; }

      public string DatabaseCategory { get; set; }

      public bool FilterByAuthor { get; set; }

      public SqlNotificationCallback Callback { get; set; }

      public static BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription Default(
        Guid eventClass)
      {
        return new BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription()
        {
          EventClass = eventClass,
          FilterByAuthor = true,
          DatabaseCategory = "WorkItem"
        };
      }
    }
  }
}
