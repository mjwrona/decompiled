// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotificationRegistration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct SqlNotificationRegistration : INotificationRegistration
  {
    private readonly string m_databaseCategory;
    private readonly Guid m_eventClass;
    private readonly SqlNotificationHandler m_handler;
    private readonly SqlNotificationCallback m_callback;
    private readonly bool m_waitForInFlightNotifications;
    private readonly bool m_startAtParentContext;
    private bool m_registered;

    private SqlNotificationRegistration(
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler = null,
      SqlNotificationCallback callback = null,
      bool waitForInFlightNotifications = false,
      bool startAtParentContext = false)
    {
      this.m_databaseCategory = databaseCategory;
      this.m_eventClass = eventClass;
      this.m_handler = handler;
      this.m_callback = callback;
      this.m_waitForInFlightNotifications = waitForInFlightNotifications;
      this.m_startAtParentContext = startAtParentContext;
      this.m_registered = true;
      this.Validate();
    }

    private void Validate()
    {
      if (this.m_callback == null == (this.m_handler == null))
        throw new InvalidOperationException("Invalid registration setup: Must pass exactly one SqlNotificationCallback or SqlNotificationHandler.");
    }

    public void Unregister(IVssRequestContext context) => this.Unregister(context, false);

    public void Unregister(IVssRequestContext requestContext, bool forceUnregistration)
    {
      if (!this.m_registered || !forceUnregistration && requestContext.ServiceHost.DeploymentServiceHost.Status == TeamFoundationServiceHostStatus.Stopping)
        return;
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      if (this.m_handler != null)
        service.UnregisterNotification(requestContext, this.m_databaseCategory, this.m_eventClass, this.m_handler, this.m_waitForInFlightNotifications);
      else
        service.UnregisterNotification(requestContext, this.m_databaseCategory, this.m_eventClass, this.m_callback, this.m_waitForInFlightNotifications);
      this.m_registered = false;
    }

    internal static SqlNotificationRegistration BuildRegistration(
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications)
    {
      return new SqlNotificationRegistration(databaseCategory, eventClass, handler, waitForInFlightNotifications: waitForInFlightNotifications);
    }

    internal static SqlNotificationRegistration BuildRegistration(
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool waitForInFlightNotifications)
    {
      return new SqlNotificationRegistration(databaseCategory, eventClass, callback: callback, waitForInFlightNotifications: waitForInFlightNotifications);
    }
  }
}
