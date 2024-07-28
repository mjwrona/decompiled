// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TraceDetailsFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class TraceDetailsFilterAttribute : TraceFilterAttribute
  {
    private FilterAttributeHelper filterHelper;
    private const string ResponseTraceFormatString = "Responding to request with header = '{0}', content = '{1}'";
    private const string RequestTraceFormatString = "Incoming request with header = '{0}', content = '{1}'";

    public TraceDetailsFilterAttribute(int tracePointEnter, int tracePointLeave)
      : this(tracePointEnter, tracePointLeave, new FilterAttributeHelper())
    {
    }

    internal TraceDetailsFilterAttribute(
      int tracePointEnter,
      int tracePointLeave,
      FilterAttributeHelper filterHelper)
      : base(tracePointEnter, tracePointLeave)
    {
      this.filterHelper = filterHelper;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      CommerceControllerBase contextController = this.filterHelper.GetActionContextController(actionContext);
      if (contextController == null)
        return;
      try
      {
        contextController.TfsRequestContext.TraceEnter(this.TracePointEnter, contextController.Area, contextController.Layer, this.filterHelper.GetActionName(actionContext));
        if (!contextController.TfsRequestContext.IsTracing(this.TracePointEnter, TraceLevel.Info, contextController.Area, contextController.Layer))
          return;
        string requestContent = this.filterHelper.GetRequestContent(actionContext);
        string headersAsString = this.filterHelper.GetHeadersAsString(this.filterHelper.GetRequestHeaders(actionContext));
        contextController.TfsRequestContext.Trace(this.TracePointEnter, TraceLevel.Info, contextController.Area, contextController.Layer, "Incoming request with header = '{0}', content = '{1}'", (object) headersAsString, (object) requestContent);
      }
      catch (Exception ex)
      {
        contextController.TfsRequestContext.TraceException(this.TracePointEnter, contextController.Area, contextController.Layer, ex);
      }
    }

    public override void OnActionExecuted(HttpActionExecutedContext executedContext)
    {
      CommerceControllerBase contextController = this.filterHelper.GetActionContextController(executedContext);
      if (contextController == null)
        return;
      try
      {
        contextController.TfsRequestContext.TraceLeave(this.TracePointLeave, contextController.Area, contextController.Layer, this.filterHelper.GetActionName(executedContext));
        string responseContent = this.filterHelper.GetResponseContent(executedContext);
        string headersAsString = this.filterHelper.GetHeadersAsString(this.filterHelper.GetResponseHeaders(executedContext));
        contextController.TfsRequestContext.Trace(this.TracePointLeave, TraceLevel.Info, contextController.Area, contextController.Layer, "Responding to request with header = '{0}', content = '{1}'", (object) headersAsString, (object) responseContent);
      }
      catch (Exception ex)
      {
        contextController.TfsRequestContext.TraceException(this.TracePointLeave, contextController.Area, contextController.Layer, ex);
      }
    }
  }
}
