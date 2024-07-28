// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.SetActivityLogAnonymousIdentifierAttribute
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class SetActivityLogAnonymousIdentifierAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      base.OnActionExecuting(actionContext);
      Guid guid;
      if (!HttpRequestMessageExtensions.TryGetHeaderGuid((HttpHeaders) actionContext.Request.Headers, "X-TFS-Session", out guid))
        return;
      IHttpController controller = actionContext.ControllerContext.Controller;
      IVssRequestContext vssRequestContext = typeof (TfsApiController).IsAssignableFrom(controller.GetType()) ? ((TfsApiController) controller).TfsRequestContext : throw new InvalidOperationException("SetActivityLogAnonymousIdentifierAttribute only applies to TfsApiController, but it was applied to a " + controller.GetType().Name);
      if (vssRequestContext == null)
        throw new NullReferenceException("tfsApiController.TfsRequestContext was null");
      if (vssRequestContext.Items == null)
        throw new NullReferenceException("IVssRequestContext.Items was null");
      if (vssRequestContext.Items.ContainsKey(RequestContextItemsKeys.AnonymousIdentifier))
        return;
      vssRequestContext.Items[RequestContextItemsKeys.AnonymousIdentifier] = (object) guid;
    }
  }
}
