// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSqlNotificationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationSqlNotificationService : 
    ITeamFoundationSqlNotificationService,
    IVssFrameworkService
  {
    internal NotificationProcessor m_notificationProcessor;
    private const string c_area = "SqlNotification";
    private const string c_layer = "TeamFoundationSqlNotificationService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        systemRequestContext.ServiceHost.CheckDisposedOrDisposing();
        this.m_notificationProcessor = new NotificationProcessor(systemRequestContext);
      }
      else
        this.m_notificationProcessor = systemRequestContext.ServiceHost.DeploymentServiceHost.ServiceHostInternal().ServiceProvider.GetService<TeamFoundationSqlNotificationService>(systemRequestContext).m_notificationProcessor;
      this.m_notificationProcessor.RegisterNotification(systemRequestContext, systemRequestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler("Default", Guid.Empty, SqlNotificationEventClasses.FlushNotificationQueueRequest, new SqlNotificationHandler(this.FlushNotificationQueueRequestCallback)));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_notificationProcessor == null)
        return;
      this.m_notificationProcessor.UnregisterNotification(systemRequestContext, systemRequestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler("Default", Guid.Empty, SqlNotificationEventClasses.FlushNotificationQueueRequest, new SqlNotificationHandler(this.FlushNotificationQueueRequestCallback)), false);
      this.m_notificationProcessor.Unload(systemRequestContext);
      this.m_notificationProcessor = (NotificationProcessor) null;
    }

    private void FlushNotificationQueueRequestCallback(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      requestContext.TraceEnter(946948215, "SqlNotification", nameof (TeamFoundationSqlNotificationService), nameof (FlushNotificationQueueRequestCallback));
      try
      {
        TeamFoundationHostManagementService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
        FlushNotificationQueueRequest notificationQueueRequest = args.Deserialize<FlushNotificationQueueRequest>();
        Guid processId = service.ProcessId;
        requestContext.Trace(1052182650, TraceLevel.Info, "SqlNotification", nameof (TeamFoundationSqlNotificationService), "Processing FlushNotificationQueue request {0}, process {1}, current process {2}", (object) notificationQueueRequest.RequestId, (object) notificationQueueRequest.ProcessId, (object) processId);
        if (!(notificationQueueRequest.ProcessId == processId))
          return;
        this.SendNotification(requestContext, notificationQueueRequest.RequestId, "FlushNotificationQueueResponse");
      }
      finally
      {
        requestContext.TraceLeave(559374786, "SqlNotification", nameof (TeamFoundationSqlNotificationService), nameof (FlushNotificationQueueRequestCallback));
      }
    }

    public Guid Author => this.m_notificationProcessor.Author;

    public bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor)
    {
      return this.RegisterNotification(requestContext, databaseCategory, Guid.Empty, eventClass, handler, filterByAuthor);
    }

    public virtual bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor)
    {
      return this.m_notificationProcessor.RegisterNotification(requestContext, requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler(databaseCategory, dataspaceIdentifier, eventClass, handler, filterByAuthor));
    }

    public virtual bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool filterByAuthor)
    {
      return this.m_notificationProcessor.RegisterNotification(requestContext, requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlCallback(databaseCategory, Guid.Empty, eventClass, callback, filterByAuthor));
    }

    public virtual long SendNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      return this.m_notificationProcessor.SendNotification(requestContext, eventClass, eventData, requestContext.ServiceHost.InstanceId);
    }

    public virtual long SendNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      string eventData)
    {
      return this.m_notificationProcessor.SendNotification(requestContext, databaseCategory, eventClass, eventData, requestContext.ServiceHost.InstanceId);
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications)
    {
      this.UnregisterNotification(requestContext, databaseCategory, Guid.Empty, eventClass, handler, waitForInFlightNotifications);
    }

    public virtual void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications)
    {
      this.m_notificationProcessor.UnregisterNotification(requestContext, requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlHandler(databaseCategory, dataspaceIdentifier, eventClass, handler), waitForInFlightNotifications);
    }

    public virtual void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool waitForInFlightNotifications)
    {
      this.m_notificationProcessor.UnregisterNotification(requestContext, requestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlCallback(databaseCategory, Guid.Empty, eventClass, callback), waitForInFlightNotifications);
    }

    internal void SetNotificationHangLimit(TimeSpan? hangLimit) => this.m_notificationProcessor.SetNotificationHangLimit(hangLimit);

    internal string LastEventDetails() => this.m_notificationProcessor.LastEventDetails();
  }
}
