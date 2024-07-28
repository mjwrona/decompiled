// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.ExternalWebHookEventsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "webhooks")]
  [ClientIgnore]
  [TaskYieldOnException]
  public class ExternalWebHookEventsController : TfsApiController
  {
    private const int MaxPostRequestSize = 5242880;

    [ClientResponseType(typeof (void), null, null)]
    [HttpPost]
    public virtual HttpResponseMessage ReceiveExternalEvent(string webHookId)
    {
      string eventPayload = string.Empty;
      using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 5242881L, true))
        eventPayload = new StreamReader((Stream) restrictedStream).ReadToEnd();
      IWebHookService service = this.TfsRequestContext.GetService<IWebHookService>();
      WebHook webHook = (WebHook) null;
      Guid result;
      if (Guid.TryParse(webHookId, out result) && result != Guid.Empty)
        webHook = service.GetWebHook(this.TfsRequestContext, result, true);
      else if (!string.IsNullOrEmpty(webHookId))
        webHook = service.GetIncomingWebHook(this.TfsRequestContext, webHookId, true);
      if (webHook == null)
        throw new WebHookException(TaskResources.WebHookIdNotFound((object) webHookId));
      IArtifactType artifactType = this.TfsRequestContext.GetService<IArtifactService>().GetArtifactType(this.TfsRequestContext, webHook.ArtifactType);
      if (artifactType == null)
      {
        this.TfsRequestContext.TraceAlways(10016130, "WebHook", "Cannot fetch artifact type definition for artifact type {0}", (object) webHook.ArtifactType);
        throw new WebHookException(TaskResources.CannotFetchArtifactTypeDefinition((object) webHook.ArtifactType));
      }
      if (!WebHookHelper.ValidateWebHookPayload(this.TfsRequestContext, this.ActionContext.Request, eventPayload, webHook, artifactType))
        throw new WebHookException(TaskResources.WebHookPayloadHashDoesNotMatch((object) eventPayload));
      service.GetWebHookExtension(this.TfsRequestContext)?.QueueJobToTriggerPipeline(this.TfsRequestContext, webHook.WebHookId, eventPayload);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
