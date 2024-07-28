// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApiVersionResponseFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ApiVersionResponseFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if (actionExecutedContext == null || actionExecutedContext.Response == null || actionExecutedContext.ActionContext == null || actionExecutedContext.ActionContext.ControllerContext == null || actionExecutedContext.ActionContext.ControllerContext.RouteData == null)
        return;
      HttpResponseMessage response = actionExecutedContext.Response;
      if (response.Content == null || response.Content.Headers == null || response.Content.Headers.ContentType == null || response.Content.Headers.ContentType.Parameters == null)
        return;
      ApiResourceVersion versionFromRouteData = VersionedApiResourceConstraint.GetApiVersionFromRouteData(actionExecutedContext.ActionContext.ControllerContext.RouteData);
      if (versionFromRouteData == null)
        return;
      response.Content.Headers.ContentType.Parameters.AddApiResourceVersionValues(versionFromRouteData, false);
    }
  }
}
