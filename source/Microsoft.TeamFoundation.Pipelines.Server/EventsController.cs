// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.EventsController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "events")]
  public class EventsController : TfsApiController
  {
    [HttpPost]
    [ClientIgnore]
    public bool PostEvent(string provider)
    {
      IEventsService service = this.TfsRequestContext.GetService<IEventsService>();
      string str = this.Request.ReadBodyAsString();
      IDictionary<string, string> simpleHeaders = this.Request.GetSimpleHeaders();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string provider1 = provider;
      string jsonPayload = str;
      IDictionary<string, string> headers = simpleHeaders;
      if (service.PostEvent(tfsRequestContext, provider1, jsonPayload, headers))
        this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.ForceHostLastUserAccessUpdate] = (object) true;
      return true;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PipelineEventException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IdentityNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PipelineStatusException>(HttpStatusCode.BadRequest);
    }
  }
}
