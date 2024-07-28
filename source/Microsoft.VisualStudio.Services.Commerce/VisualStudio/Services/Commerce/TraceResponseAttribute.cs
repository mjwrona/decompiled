// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TraceResponseAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class TraceResponseAttribute : ActionFilterAttribute
  {
    private const string TraceFormatString = "Responding to request with header = '{0}', content = '{1}'";
    private FilterAttributeHelper filterHelper;

    private int TracePoint { get; set; }

    public TraceResponseAttribute(int tracePoint)
      : this(tracePoint, new FilterAttributeHelper())
    {
      this.TracePoint = tracePoint;
    }

    internal TraceResponseAttribute(int tracePoint, FilterAttributeHelper filterHelper) => this.filterHelper = filterHelper;

    public override void OnActionExecuted(HttpActionExecutedContext executedContext)
    {
      CommerceControllerBase contextController = this.filterHelper.GetActionContextController(executedContext);
      if (contextController == null)
        return;
      try
      {
        string responseContent = this.filterHelper.GetResponseContent(executedContext);
        string headersAsString = this.filterHelper.GetHeadersAsString(this.filterHelper.GetResponseHeaders(executedContext));
        contextController.TfsRequestContext.Trace(this.TracePoint, TraceLevel.Info, contextController.Area, contextController.Layer, "Responding to request with header = '{0}', content = '{1}'", (object) headersAsString, (object) responseContent);
      }
      catch (Exception ex)
      {
        contextController.TfsRequestContext.TraceException(this.TracePoint, contextController.Area, contextController.Layer, ex);
      }
    }
  }
}
