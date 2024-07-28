// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.ITeamFoundationSqlNotificationServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal static class ITeamFoundationSqlNotificationServiceExtensions
  {
    public static IDisposable RegisterLocalNotification(
      this ITeamFoundationSqlNotificationService service,
      IVssRequestContext requestContext,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor)
    {
      Dataspace dataspace = requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, "Orchestration", Guid.Empty, false);
      string str = dataspace != null ? dataspace.DataspaceCategory : "Default";
      service.RegisterNotification(requestContext, str, eventClass, handler, filterByAuthor);
      ITeamFoundationSqlNotificationServiceExtensions.LocalSqlNotificationRegistration notificationRegistration = new ITeamFoundationSqlNotificationServiceExtensions.LocalSqlNotificationRegistration(requestContext, service, str, eventClass, handler);
      requestContext.AddDisposableResource((IDisposable) notificationRegistration);
      return (IDisposable) notificationRegistration;
    }

    private sealed class LocalSqlNotificationRegistration : IDisposable
    {
      private bool m_disposed;
      private readonly Guid m_eventClass;
      private readonly SqlNotificationHandler m_handler;
      private readonly IVssRequestContext m_requestContext;
      private readonly ITeamFoundationSqlNotificationService m_sqlNotification;
      private readonly string m_sqlNotificationDataspaceCategory;

      public LocalSqlNotificationRegistration(
        IVssRequestContext requestContext,
        ITeamFoundationSqlNotificationService sqlNotification,
        string sqlNotificationDataspaceCategory,
        Guid eventClass,
        SqlNotificationHandler handler)
      {
        this.m_handler = handler;
        this.m_eventClass = eventClass;
        this.m_requestContext = requestContext;
        this.m_sqlNotification = sqlNotification;
        this.m_sqlNotificationDataspaceCategory = sqlNotificationDataspaceCategory;
      }

      public void Dispose()
      {
        if (this.m_disposed)
          return;
        try
        {
          this.m_sqlNotification.UnregisterNotification(this.m_requestContext, this.m_sqlNotificationDataspaceCategory, this.m_eventClass, this.m_handler, false);
          this.m_disposed = true;
        }
        catch (Exception ex)
        {
        }
      }
    }
  }
}
