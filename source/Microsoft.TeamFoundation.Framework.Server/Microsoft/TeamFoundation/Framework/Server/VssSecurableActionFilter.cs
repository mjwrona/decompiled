// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssSecurableActionFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssSecurableActionFilter : ActionFilterAttribute
  {
    private static readonly Type s_objectContentType = typeof (ObjectContent);

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      base.OnActionExecuted(actionExecutedContext);
      if (actionExecutedContext.Response == null || actionExecutedContext.Response.Content == null || actionExecutedContext.Response.Content is ObjectContent || actionExecutedContext.Response.Content is IVssServerHttpContent || !(actionExecutedContext.ActionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (tfsRequestContext.RootContext.Items.TryGetValue<TrackedSecurityCollection>(RequestContextItemsKeys.SecurityTracking, out TrackedSecurityCollection _))
      {
        HttpContextFactory.Current.Items[(object) HttpContextConstants.SecurityTrackingException] = (object) true;
        string message = string.Format("Returned an unsafe HttpContent type from public API controller {0}. Type: {1}", (object) controller.ControllerContext.ControllerDescriptor.ControllerName, (object) actionExecutedContext.Response.Content.GetType());
        tfsRequestContext.Trace(707183817, TraceLevel.Error, nameof (VssSecurableActionFilter), "AnonymousAccessKalypsoAlert", message);
        throw new InvalidOperationException(message);
      }
    }
  }
}
