// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcEventsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "Events")]
  public class HooksSvcEventsController : ServiceHooksSvcControllerBase
  {
    [ClientResponseType(typeof (void), null, null)]
    [HttpPost]
    public HttpResponseMessage CreateEvents(PublishEventsRequestData eventsRequestData)
    {
      this.CheckPermission(this.TfsRequestContext, 8);
      this.TfsRequestContext.GetService<ServiceHooksService>().PublishEvents(this.TfsRequestContext, eventsRequestData.Events);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
