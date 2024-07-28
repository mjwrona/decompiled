// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.BasePipelineRunsController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  public class BasePipelineRunsController : PipelinesProjectApiController
  {
    protected const int MaxItems = 10000;

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public Microsoft.Azure.Pipelines.WebApi.Run GetRun(int pipelineId, int runId)
    {
      ArgumentUtility.CheckForNonPositiveInt(pipelineId, nameof (pipelineId));
      ArgumentUtility.CheckForNonPositiveInt(runId, nameof (runId));
      IExternalRunsService service = this.TfsRequestContext.GetService<IExternalRunsService>();
      RunConverter clientConverter = this.GetClientConverter<RunConverter>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      int pipelineId1 = pipelineId;
      int runId1 = runId;
      return clientConverter.ToWebApiRun(this.TfsRequestContext, service.GetRun(tfsRequestContext, projectId, pipelineId1, runId1) ?? throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines"));
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public List<Microsoft.Azure.Pipelines.WebApi.Run> ListRuns(int pipelineId)
    {
      IExternalRunsService service = this.TfsRequestContext.GetService<IExternalRunsService>();
      RunConverter converter = this.GetClientConverter<RunConverter>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      int pipelineId1 = pipelineId;
      return service.GetRuns(tfsRequestContext, projectId, pipelineId1, 10000).Select<Microsoft.Azure.Pipelines.Server.ObjectModel.Run, Microsoft.Azure.Pipelines.WebApi.Run>((Func<Microsoft.Azure.Pipelines.Server.ObjectModel.Run, Microsoft.Azure.Pipelines.WebApi.Run>) (r => converter.ToWebApiRun(this.TfsRequestContext, r))).ToList<Microsoft.Azure.Pipelines.WebApi.Run>();
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RunNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PipelineNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
