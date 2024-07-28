// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskScopeApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Constants;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientAdditionalRouteParameter(typeof (Guid), "scopeIdentifier", 1, "The project GUID to scope the request")]
  public abstract class DistributedTaskScopeApiController : TfsApiController
  {
    private Guid mScopeId;

    public Guid ScopeIdentifier => this.mScopeId;

    [NonAction]
    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      object obj;
      return controllerContext != null && controllerContext.RouteData != null && controllerContext.RouteData.Values != null && controllerContext.RouteData.Values.TryGetValue(TaskApiRouteParameters.ScopeIdentifier, out obj) && !Guid.TryParse(obj.ToString(), out this.mScopeId) ? this.BadRequest(TaskResources.InvalidScopeId((object) this.mScopeId)).ExecuteAsync(cancellationToken) : base.ExecuteAsync(controllerContext, cancellationToken);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidAuthorizationDetailsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidEndpointResponseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointQueryFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DataNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidDataSourceBindingException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidServiceEndpointRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidAuthorizationDetailsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidEndpointResponseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidDatasourceException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointQueryFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointNotFoundException>(HttpStatusCode.NotFound);
    }

    public override string TraceArea => "DistributedTask";

    public override string ActivityLogArea => "DistributedTask";
  }
}
