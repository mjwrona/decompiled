// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ValidateInputAttribute
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public abstract class ValidateInputAttribute : ActionFilterAttribute
  {
    public abstract bool ValidateAndSetParameters(HttpActionContext actionContext);

    public abstract bool IsAPIEnabled(IVssRequestContext requestContext);

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      base.OnActionExecuting(actionContext);
      IVssRequestContext property = (IVssRequestContext) actionContext.ControllerContext.Request.Properties[TfsApiPropertyKeys.TfsRequestContext];
      HttpResponseMessage response;
      if (property != null)
      {
        if (this.ValidateAndSetParameters(actionContext))
        {
          TeamFoundationExecutionEnvironment executionEnvironment = property.ExecutionEnvironment;
          if (!executionEnvironment.IsOnPremisesDeployment || this.IsAPIEnabled(property))
          {
            executionEnvironment = property.ExecutionEnvironment;
            response = !executionEnvironment.IsHostedDeployment || property.IsFeatureEnabled("CodeSense.Server.Jobs") ? (HttpResponseMessage) null : actionContext.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.Forbidden);
          }
          else
            response = actionContext.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.Forbidden);
        }
        else
          response = actionContext.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.BadRequest);
      }
      else
        response = actionContext.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.InternalServerError);
      HttpResponseMessage httpResponseMessage = response;
      if (httpResponseMessage == null)
        return;
      actionContext.Response = httpResponseMessage;
    }
  }
}
