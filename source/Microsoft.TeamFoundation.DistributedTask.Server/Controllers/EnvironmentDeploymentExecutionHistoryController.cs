// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.EnvironmentDeploymentExecutionHistoryController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "environmentdeploymentRecords")]
  public sealed class EnvironmentDeploymentExecutionHistoryController : 
    DistributedTaskProjectApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<EnvironmentDeploymentExecutionRecord>), null, null)]
    public HttpResponseMessage GetEnvironmentDeploymentExecutionRecords(
      int environmentId,
      [ClientQueryParameter] string continuationToken = "",
      [ClientQueryParameter] int top = 25)
    {
      if (top <= 0)
        top = 25;
      top = Math.Min(top, 500);
      IPagedList<EnvironmentDeploymentExecutionRecord> executionRecords = this.TfsRequestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().GetEnvironmentDeploymentExecutionRecords(this.TfsRequestContext, environmentId, this.ProjectId, continuationToken, top);
      HttpResponseMessage response = this.Request.CreateResponse<IPagedList<EnvironmentDeploymentExecutionRecord>>(HttpStatusCode.OK, executionRecords);
      if (!string.IsNullOrWhiteSpace(executionRecords.ContinuationToken))
        DistributedTaskProjectApiController.SetContinuationToken(response, executionRecords.ContinuationToken);
      return response;
    }
  }
}
