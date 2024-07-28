// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesArtifactsController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "artifacts")]
  public class PipelinesArtifactsController : PipelinesProjectApiController
  {
    [HttpGet]
    public Microsoft.Azure.Pipelines.WebApi.Artifact GetArtifact(
      int pipelineId,
      int runId,
      [ClientQueryParameter] string artifactName,
      [FromUri(Name = "$expand")] GetArtifactExpandOptions expandOptions = GetArtifactExpandOptions.None)
    {
      IReadOnlyList<Microsoft.Azure.Pipelines.Server.ObjectModel.Artifact> artifacts = this.TfsRequestContext.GetService<IExternalRunsService>().GetArtifacts(this.TfsRequestContext, this.ProjectId, pipelineId, runId, artifactName);
      if (!string.IsNullOrEmpty(artifactName) && artifacts.Count == 0)
        throw new ArtifactNotFoundException(WebServerResources.ArtifactNotFound((object) artifactName)).Expected("Pipelines");
      return this.GetClientConverter<ArtifactConverter>().ToWebApiArtifact(this.TfsRequestContext, this.ProjectId, artifacts.FirstOrDefault<Microsoft.Azure.Pipelines.Server.ObjectModel.Artifact>(), pipelineId, runId, expandOptions.HasFlag((System.Enum) GetArtifactExpandOptions.SignedContent), (Action<IVssRequestContext, int>) ((context, id) => { }));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RunNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ArtifactNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
