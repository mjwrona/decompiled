// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientIgnore]
  [TaskYieldOnException]
  public abstract class TaskHubCompatApiController : TfsApiController
  {
    private const string c_layer = "TaskHubCompatApiController";

    public override string TraceArea => "DistributedTask";

    protected TaskHub Hub { get; private set; }

    public TaskHubCompatApiController()
    {
    }

    internal TaskHubCompatApiController(TaskHub hub) => this.Hub = hub;

    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        vssRequestContext = controllerContext.Request.GetIVssRequestContext();
        TaskHub defaultTaskHub = vssRequestContext.GetService<DistributedTaskHubService>().GetDefaultTaskHub(vssRequestContext);
        if (defaultTaskHub == null)
        {
          vssRequestContext.TraceError(nameof (TaskHubCompatApiController), "Default task hub not found");
          return new NotFoundResult(controllerContext.Request).ExecuteAsync(cancellationToken);
        }
        this.Hub = defaultTaskHub;
        return base.ExecuteAsync(controllerContext, cancellationToken);
      }
      catch (Exception ex)
      {
        if (vssRequestContext != null)
          vssRequestContext.TraceException(nameof (TaskHubCompatApiController), ex);
        throw;
      }
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FileIdNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPathException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentSessionConflictException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentSessionExpiredException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationJobNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanSecurityException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentPoolNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<TaskDefinitionExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskDefinitionHostContextMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TimelineRecordUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanGroupNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidLicenseHubException>(HttpStatusCode.NotFound);
    }
  }
}
