// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildStagesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "stages", ResourceVersion = 1)]
  [ClientGroupByResource("stages")]
  [CheckWellFormedProject(Required = true)]
  public class BuildStagesController : BuildApiController
  {
    [HttpPatch]
    public async Task UpdateStage(
      [FromBody] UpdateStageParameters updateParameters,
      [FromUri] int buildId,
      [FromUri] string stageRefName)
    {
      BuildStagesController stagesController = this;
      if (updateParameters == null)
        throw new ArgumentNullException(nameof (updateParameters));
      if (updateParameters.State == StageUpdateType.Retry)
      {
        await stagesController.BuildService.RetryStageAsync(stagesController.TfsRequestContext, stagesController.ProjectId, buildId, stageRefName, updateParameters.ForceRetryAllJobs, updateParameters.RetryDependencies.GetValueOrDefault(true));
      }
      else
      {
        if (updateParameters.State != StageUpdateType.Cancel)
          throw new InvalidTimelineRecordStateChange(BuildServerResources.InvalidTimelineRecordStateChange((object) updateParameters.State));
        stagesController.BuildService.CancelStage(stagesController.TfsRequestContext, stagesController.ProjectId, buildId, stageRefName);
      }
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidPipelineOperationException>(HttpStatusCode.Conflict);
    }
  }
}
