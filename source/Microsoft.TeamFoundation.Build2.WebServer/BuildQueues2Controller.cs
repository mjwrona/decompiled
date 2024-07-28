// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildQueues2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [Obsolete("Use the Distributed Task Queue API instead")]
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "queues", ResourceVersion = 2)]
  public class BuildQueues2Controller : BuildApiController
  {
    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue GetAgentPoolQueue(int controllerId)
    {
      Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue queue = this.TfsRequestContext.GetService<IBuildQueueService>().GetQueue(this.TfsRequestContext, controllerId);
      if (queue == null)
        throw new QueueNotFoundException(Resources.QueueNotFound((object) controllerId));
      return this.ProjectInfo == null ? queue.ToWebApiUnsecuredAgentPoolQueue(this.TfsRequestContext) : queue.ToWebApiAgentPoolQueue(this.TfsRequestContext, (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));
    }

    [HttpGet]
    public List<Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue> GetQueues(string name = "*")
    {
      IBuildQueueService service = this.TfsRequestContext.GetService<IBuildQueueService>();
      return this.ProjectInfo == null ? service.GetQueues(this.TfsRequestContext, name).Select<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue, Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>((Func<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue, Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>) (x => x.ToWebApiUnsecuredAgentPoolQueue(this.TfsRequestContext))).ToList<Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>() : service.GetQueues(this.TfsRequestContext, name).Select<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue, Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>((Func<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue, Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>) (x => x.ToWebApiAgentPoolQueue(this.TfsRequestContext, (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext)))).ToList<Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue>();
    }

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue), null, null)]
    public HttpResponseMessage CreateQueue(Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue queue) => this.GetErrorResponse();

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteQueue(int id) => this.GetErrorResponse();

    private HttpResponseMessage GetErrorResponse() => this.Request.CreateResponse<WrappedException>(HttpStatusCode.BadRequest, new WrappedException((Exception) new InvalidOperationException(Resources.UseDTQueueAPIs()), this.Request.ShouldIncludeErrorDetail(), this.Request.GetApiVersion()));
  }
}
