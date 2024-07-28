// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinePreviewController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebServer.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "preview", ResourceVersion = 1)]
  public class PipelinePreviewController : PipelinesProjectApiController
  {
    [HttpPost]
    public PreviewRun Preview(
      int pipelineId,
      [FromBody] Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters runParameters,
      [FromUri] int? pipelineVersion = null)
    {
      ArgumentUtility.CheckForNonPositiveInt(pipelineId, nameof (pipelineId));
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters>(runParameters, nameof (runParameters));
      runParameters.PreviewRun = true;
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters pipelineParameters = runParameters.ToRunPipelineParameters();
      pipelineParameters.OrchestrationIdentifier = new Guid?(this.TfsRequestContext.E2EId);
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run run = this.TfsRequestContext.GetService<IExternalRunsService>().RunPipeline(this.TfsRequestContext, this.ProjectId, pipelineId, pipelineVersion, pipelineParameters);
      return new PreviewRun(run.ToSecuredObject())
      {
        FinalYaml = run.FinalYaml
      };
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<PipelineValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RunOrchestrationExistsException>(HttpStatusCode.BadRequest);
    }
  }
}
