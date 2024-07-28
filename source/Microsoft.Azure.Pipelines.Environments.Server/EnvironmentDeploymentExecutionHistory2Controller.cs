// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.EnvironmentDeploymentExecutionHistory2Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "environmentdeploymentrecords")]
  public sealed class EnvironmentDeploymentExecutionHistory2Controller : 
    EnvironmentsProjectApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<EnvironmentDeploymentExecutionRecord>), null, null)]
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
        EnvironmentsProjectApiController.SetContinuationToken(response, executionRecords.ContinuationToken);
      return response;
    }
  }
}
