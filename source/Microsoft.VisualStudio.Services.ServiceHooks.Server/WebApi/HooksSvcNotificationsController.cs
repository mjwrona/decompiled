// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcNotificationsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "Notifications")]
  public class HooksSvcNotificationsController : ServiceHooksSvcControllerBase
  {
    private const int c_defaultMaxResults = 100;

    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> GetNotifications(
      Guid subscriptionId,
      int? maxResults = 100,
      NotificationStatus? status = null,
      NotificationResult? result = null)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) this.TfsRequestContext.GetService<ServiceHooksService>().GetNotifications(this.TfsRequestContext, subscriptionId, status, result, maxResults);
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification GetNotification(
      Guid subscriptionId,
      int notificationId)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      return this.TfsRequestContext.GetService<ServiceHooksService>().GetNotification(this.TfsRequestContext, subscriptionId, notificationId);
    }
  }
}
