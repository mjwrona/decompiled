// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpointsProjectApiController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  public abstract class ServiceEndpointsProjectApiController : TfsProjectApiController
  {
    public override string TraceArea => "ServiceEndpoints";

    public override string ActivityLogArea => "ServiceEndpoints";

    protected static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("X-MS-ContinuationToken", tokenValue);
    }

    protected T GetService<T>() where T : class, IVssFrameworkService => this.TfsRequestContext.GetService<T>();

    [NonAction]
    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      object obj;
      if (controllerContext != null && controllerContext.RouteData != null && controllerContext.RouteData.Values != null && controllerContext.RouteData.Values.TryGetValue(TaskApiRouteParameters.ScopeIdentifier, out obj))
      {
        Guid result;
        if (!Guid.TryParse(obj.ToString(), out result))
          return this.BadRequest("ServiceEndpointResources.InvalidScopeId(mScopeId)").ExecuteAsync(cancellationToken);
        this.ProjectInfo = new ProjectInfo() { Id = result };
      }
      return base.ExecuteAsync(controllerContext, cancellationToken);
    }
  }
}
