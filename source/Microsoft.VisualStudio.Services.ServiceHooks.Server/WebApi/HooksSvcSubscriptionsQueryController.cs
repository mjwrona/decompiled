// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcSubscriptionsQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "SubscriptionsQuery")]
  public class HooksSvcSubscriptionsQueryController : ServiceHooksSvcControllerBase
  {
    [HttpPost]
    public SubscriptionsQuery QuerySubscriptions(
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      ServiceHooksService service = this.TfsRequestContext.GetService<ServiceHooksService>();
      query.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) service.QuerySubscriptions(this.TfsRequestContext, query, unmaskConfidentialInputs).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      query.Results.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return query;
    }
  }
}
