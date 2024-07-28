// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubOidcTokenController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "oidctoken")]
  public sealed class TaskHubOidcTokenController : TaskHubApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (TaskHubOidcToken), null, null)]
    public HttpResponseMessage CreateOidcToken(
      Guid planId,
      Guid jobId,
      [FromBody] Dictionary<string, string> claims,
      [ClientQueryParameter] Guid? serviceConnectionId = null)
    {
      string overwriteAudience = claims != null ? claims.GetValueOrDefault<string, string>("aud", (string) null) : (string) null;
      string str = serviceConnectionId.HasValue ? this.TfsRequestContext.RunSynchronously<string>((Func<Task<string>>) (() => this.Hub.GenerateOidcTokenAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, jobId, serviceConnectionId.Value, overwriteAudience))) : this.TfsRequestContext.RunSynchronously<string>((Func<Task<string>>) (() => this.Hub.GenerateOidcTokenAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, jobId, overwriteAudience)));
      if (string.IsNullOrEmpty(str))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      return this.Request.CreateResponse<TaskHubOidcToken>(HttpStatusCode.OK, new TaskHubOidcToken()
      {
        OidcToken = str
      });
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<EndpointNotFoundException>(HttpStatusCode.BadRequest);
    }
  }
}
