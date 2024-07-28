// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationSqlNotificationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationSqlNotificationService), typeof (VirtualSqlNotificationService))]
  public interface ITeamFoundationSqlNotificationService : IVssFrameworkService
  {
    Guid Author { get; }

    bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor);

    bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor);

    bool RegisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool filterByAuthor);

    long SendNotification(IVssRequestContext requestContext, Guid eventClass, string eventData);

    long SendNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      string eventData);

    void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications);

    void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications);

    void UnregisterNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      SqlNotificationCallback callback,
      bool waitForInFlightNotifications);
  }
}
