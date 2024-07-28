// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcNotificationsQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "NotificationsQuery")]
  public class HooksSvcNotificationsQueryController : ServiceHooksSvcControllerBase
  {
    [HttpPost]
    public NotificationsQuery QueryNotifications(NotificationsQuery query)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      this.TfsRequestContext.GetService<ServiceHooksService>().QueryNotifications(this.TfsRequestContext, query);
      if (query.AssociatedSubscriptions != null)
        query.AssociatedSubscriptions.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return query;
    }
  }
}
