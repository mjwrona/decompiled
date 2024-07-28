// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TraceRequestAttribute
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
  public class TraceRequestAttribute : ActionFilterAttribute
  {
    private FilterAttributeHelper filterHelper;
    private const string RequestTraceFormatString = "Incoming request with header = '{0}', content = '{1}'";

    internal int TracePoint { get; private set; }

    public TraceRequestAttribute(int tracePoint)
      : this(tracePoint, new FilterAttributeHelper())
    {
    }

    internal TraceRequestAttribute(int tracePoint, FilterAttributeHelper filterHelper)
    {
      this.filterHelper = filterHelper;
      this.TracePoint = tracePoint;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      CommerceControllerBase contextController = this.filterHelper.GetActionContextController(actionContext);
      if (contextController == null)
        return;
      try
      {
        string requestContent = this.filterHelper.GetRequestContent(actionContext);
        string headersAsString = this.filterHelper.GetHeadersAsString(this.filterHelper.GetRequestHeaders(actionContext));
        contextController.TfsRequestContext.Trace(this.TracePoint, TraceLevel.Info, contextController.Area, contextController.Layer, "Incoming request with header = '{0}', content = '{1}'", (object) headersAsString, (object) requestContent);
      }
      catch (Exception ex)
      {
        contextController.TfsRequestContext.TraceException(this.TracePoint, contextController.Area, contextController.Layer, ex);
      }
    }
  }
}
