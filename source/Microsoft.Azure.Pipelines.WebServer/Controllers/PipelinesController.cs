// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.Azure.Pipelines.WebServer.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "pipelines", ResourceVersion = 1)]
  public class PipelinesController : PipelinesProjectApiController
  {
    protected const int DefaultTop = 10000;
    protected const int MaxTop = 10000;

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public Microsoft.Azure.Pipelines.WebApi.Pipeline GetPipeline(
      int pipelineId,
      int? pipelineVersion = null)
    {
      ArgumentUtility.CheckForNonPositiveInt(pipelineId, nameof (pipelineId));
      return this.GetClientConverter<PipelineConverter>().ToWebApiPipeline(this.TfsRequestContext, this.TfsRequestContext.GetService<IExternalPipelinesService>().GetPipeline(this.TfsRequestContext, this.ProjectId, pipelineId, pipelineVersion) ?? throw new PipelineNotFoundException(WebServerResources.PipelineNotFound((object) pipelineId)).Expected("Pipelines"), true);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientResponseType(typeof (List<Microsoft.Azure.Pipelines.WebApi.Pipeline>), null, null)]
    public HttpResponseMessage ListPipelines(
      [FromUri(Name = "$orderBy"), ClientParameterType(typeof (string), false)] PipelineOrderByExpression orderBy = null,
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = null)
    {
      ArgumentUtility.CheckForNonnegativeInt(top, "$top");
      if (top == 0)
        return this.Request.CreateResponse<IEnumerable<Microsoft.Azure.Pipelines.WebApi.Pipeline>>(Enumerable.Empty<Microsoft.Azure.Pipelines.WebApi.Pipeline>());
      top = Math.Min(top, 10000);
      orderBy = orderBy ?? PipelineOrderByExpression.Default;
      QueryPipelinesParameters queryParameters = orderBy.GenerateQueryParameters(continuationToken);
      IReadOnlyList<Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline> pipelines = this.TfsRequestContext.GetService<IExternalPipelinesService>().GetPipelines(this.TfsRequestContext, this.ProjectId, queryParameters, orderBy, top + 1);
      PipelineConverter converter = this.GetClientConverter<PipelineConverter>();
      HttpResponseMessage response = this.Request.CreateResponse<List<Microsoft.Azure.Pipelines.WebApi.Pipeline>>(pipelines.Take<Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline>(top).Select<Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline, Microsoft.Azure.Pipelines.WebApi.Pipeline>((Func<Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline, Microsoft.Azure.Pipelines.WebApi.Pipeline>) (p => converter.ToWebApiPipeline(this.TfsRequestContext, p, false))).ToList<Microsoft.Azure.Pipelines.WebApi.Pipeline>());
      if (pipelines.Count > top)
        response.Headers.Add("x-ms-continuationtoken", orderBy.GenerateContinuationToken(pipelines[top]));
      return response;
    }

    [HttpPost]
    public Microsoft.Azure.Pipelines.WebApi.Pipeline CreatePipeline(
      Microsoft.Azure.Pipelines.WebApi.CreatePipelineParameters inputParameters)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.WebApi.CreatePipelineParameters>(inputParameters, nameof (inputParameters), "pipelines");
      Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineParameters pipelineParameters = inputParameters.ToCreatePipelineParameters();
      if (string.IsNullOrWhiteSpace(pipelineParameters.Name))
        pipelineParameters.Name = pipelineParameters.Configuration.GeneratePipelineName();
      return this.GetClientConverter<PipelineConverter>().ToWebApiPipeline(this.TfsRequestContext, this.TfsRequestContext.GetService<IPipelinesService>().CreatePipeline(this.TfsRequestContext, this.ProjectId, pipelineParameters) ?? throw new UnsupportedConfigurationTypeException(WebServerResources.CreatePipeline_UnsupportedConfigurationType((object) inputParameters.Configuration.Type)), true);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<InvalidRepositoryInformationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotImplementedException>(HttpStatusCode.NotImplemented);
      exceptionMap.AddStatusCode<PipelineExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<PipelineNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UnsupportedConfigurationTypeException>(HttpStatusCode.BadRequest);
      base.InitializeExceptionMap(exceptionMap);
    }
  }
}
