// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [ClientAdditionalRouteParameter(typeof (Guid), "scopeIdentifier", 1, "The project GUID to scope the request")]
  [ClientAdditionalRouteParameter(typeof (string), "hubName", 2, "The name of the server hub. Common examples: \"build\", \"rm\", \"checks\"")]
  [TaskYieldOnException]
  public abstract class TaskHubApiController : TfsApiController
  {
    private const string c_layer = "TaskHubApiController";

    public override string TraceArea => "DistributedTask";

    public override string ActivityLogArea => "DistributedTask";

    protected TaskHub Hub { get; private set; }

    protected Guid ScopeIdentifier { get; private set; }

    public TaskHubApiController()
    {
    }

    internal TaskHubApiController(TaskHub hub) => this.Hub = hub;

    [ClientIgnore]
    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        if (controllerContext.RouteData != null && controllerContext.RouteData.Values != null)
        {
          vssRequestContext = controllerContext.Request.GetIVssRequestContext();
          TaskHub taskHub = (TaskHub) null;
          DistributedTaskHubService service = vssRequestContext.GetService<DistributedTaskHubService>();
          string name;
          if (TaskHubApiController.TryGetValue<string>(controllerContext.RouteData.Values, "hubName", out name))
          {
            bool includeDefault = vssRequestContext.IsFeatureEnabled("DistributedTask.IncludeDefaultTaskHub");
            taskHub = service.GetTaskHub(vssRequestContext, name, includeDefault);
          }
          if (taskHub == null)
          {
            vssRequestContext.TraceError(nameof (TaskHubApiController), "Hub '{0}' not found", (object) name);
            return new NotFoundResult(controllerContext.Request).ExecuteAsync(cancellationToken);
          }
          Guid result = Guid.Empty;
          string input;
          if (TaskHubApiController.TryGetValue<string>(controllerContext.RouteData.Values, "scopeIdentifier", out input))
          {
            if (!Guid.TryParse(input, out result))
            {
              vssRequestContext.TraceError(nameof (TaskHubApiController), "Scope identifier '{0}' is not a GUID", (object) input);
              return new BadRequestResult(controllerContext.Request).ExecuteAsync(cancellationToken);
            }
            if (vssRequestContext.GetService<IDataspaceService>().QueryDataspace(vssRequestContext, taskHub.DataspaceCategory, result, false) == null)
            {
              vssRequestContext.TraceError(nameof (TaskHubApiController), "Database not found for scope identifier '{0}'", (object) result);
              return new NotFoundResult(controllerContext.Request).ExecuteAsync(cancellationToken);
            }
          }
          this.Hub = taskHub;
          this.ScopeIdentifier = result;
        }
        return base.ExecuteAsync(controllerContext, cancellationToken);
      }
      catch (Exception ex)
      {
        if (vssRequestContext != null)
          vssRequestContext.TraceException(nameof (TaskHubApiController), ex);
        throw;
      }
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FileIdNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPathException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentSessionConflictException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentSessionExpiredException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationJobNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanSecurityException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentPoolNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskDefinitionExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskDefinitionHostContextMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TimelineNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TimelineRecordUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TimelineRecordNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanGroupNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidLicenseHubException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskOrchestrationPlanLogNotFoundException>(HttpStatusCode.NotFound);
    }

    protected bool OctetStreamRequested()
    {
      bool flag1 = false;
      double num1 = 0.0;
      bool flag2 = false;
      double num2 = 0.0;
      foreach (MediaTypeWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.Accept)
      {
        double? quality;
        if (qualityHeaderValue.MediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
        {
          flag1 = true;
          quality = qualityHeaderValue.Quality;
          if (quality.HasValue)
          {
            quality = qualityHeaderValue.Quality;
            if (quality.HasValue)
            {
              quality = qualityHeaderValue.Quality;
              num1 = quality.Value;
            }
          }
        }
        if (qualityHeaderValue.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
        {
          flag2 = true;
          quality = qualityHeaderValue.Quality;
          if (quality.HasValue)
          {
            quality = qualityHeaderValue.Quality;
            if (quality.HasValue)
            {
              quality = qualityHeaderValue.Quality;
              num2 = quality.Value;
            }
          }
        }
      }
      return flag2 & flag1 && num1 > num2 || !flag2 & flag1 || !flag2 && !flag1;
    }

    private static bool TryGetValue<T>(
      IDictionary<string, object> collection,
      string key,
      out T value)
    {
      object obj1;
      if (collection.TryGetValue(key, out obj1) && obj1 is T obj2)
      {
        value = obj2;
        return true;
      }
      value = default (T);
      return false;
    }
  }
}
