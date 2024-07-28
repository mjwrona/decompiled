// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.EnvironmentsProjectApiController
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  public abstract class EnvironmentsProjectApiController : TfsProjectApiController
  {
    public override string TraceArea => "Environments";

    public override string ActivityLogArea => "Environments";

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
      if (controllerContext != null && controllerContext.RouteData != null && controllerContext.RouteData.Values != null && controllerContext.RouteData.Values.TryGetValue(EnvironmentApiRouteParameters.ScopeIdentifier, out obj))
      {
        Guid result;
        if (!Guid.TryParse(obj.ToString(), out result))
          return this.BadRequest("EnvironmentsResources.InvalidScopeId(mScopeId)").ExecuteAsync(cancellationToken);
        this.ProjectInfo = new ProjectInfo() { Id = result };
      }
      return base.ExecuteAsync(controllerContext, cancellationToken);
    }
  }
}
