// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConnectionsController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "connections")]
  public class ConnectionsController : TfsApiController
  {
    [HttpPost]
    public PipelineConnection CreateProjectConnection(
      [FromUri] string project,
      [FromBody] CreatePipelineConnectionInputs createConnectionInputs)
    {
      ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(createConnectionInputs, nameof (createConnectionInputs));
      IConnectionsService service = this.TfsRequestContext.GetService<IConnectionsService>();
      TeamProject teamProject = new TeamProject();
      teamProject.Id = Guid.Empty;
      teamProject.Name = string.Empty;
      TeamProject project1 = teamProject;
      Guid result;
      if (Guid.TryParse(project, out result))
        project1.Id = result;
      else
        project1.Name = project;
      ProjectInfo infoFromTeamProject = ConnectionsService.GetProjectInfoFromTeamProject(this.TfsRequestContext, project1);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ProjectInfo projectInfo = infoFromTeamProject;
      CreatePipelineConnectionInputs createConnectionInputs1 = createConnectionInputs;
      return service.CreateConnection(tfsRequestContext, projectInfo, createConnectionInputs1);
    }

    [Obsolete("This method is no longer supported. Call CreateProjectConnection instead.")]
    [HttpPost]
    public Operation CreateConnection(
      [FromBody] CreatePipelineConnectionInputs createConnectionInputs)
    {
      PipelineConnection projectConnection = this.CreateProjectConnection(createConnectionInputs?.Project?.Id.ToString() ?? createConnectionInputs?.Project?.Name, createConnectionInputs);
      Operation connection = new Operation();
      connection.Status = OperationStatus.Succeeded;
      connection.Url = projectConnection.RedirectUrl;
      return connection;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PipelineConnectionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidOperationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTokenException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SignatureValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<OperationNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
