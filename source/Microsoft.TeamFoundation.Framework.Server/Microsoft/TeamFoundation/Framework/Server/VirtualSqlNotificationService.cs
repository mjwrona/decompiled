// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualSqlNotificationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VirtualSqlNotificationService : TeamFoundationSqlNotificationService
  {
    public override long SendNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      return this.m_notificationProcessor.SendNotification(requestContext.To(TeamFoundationHostType.Deployment), eventClass, eventData, requestContext.ServiceHost.InstanceId);
    }

    public override long SendNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      string eventData)
    {
      return this.m_notificationProcessor.SendNotification(requestContext.To(TeamFoundationHostType.Deployment), databaseCategory, eventClass, eventData, requestContext.ServiceHost.InstanceId);
    }

    public override bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor)
    {
      return this.m_notificationProcessor.RegisterNotification(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler(databaseCategory, dataspaceIdentifier, eventClass, handler, filterByAuthor));
    }

    public override bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool filterByAuthor)
    {
      return this.m_notificationProcessor.RegisterNotification(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlCallback(databaseCategory, Guid.Empty, eventClass, callback, filterByAuthor));
    }

    public override void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications)
    {
      this.m_notificationProcessor.UnregisterNotification(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler(databaseCategory, dataspaceIdentifier, eventClass, handler), waitForInFlightNotifications);
    }

    public override void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool waitForInFlightNotifications)
    {
      this.m_notificationProcessor.UnregisterNotification(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlCallback(databaseCategory, Guid.Empty, eventClass, callback), waitForInFlightNotifications);
    }
  }
}
