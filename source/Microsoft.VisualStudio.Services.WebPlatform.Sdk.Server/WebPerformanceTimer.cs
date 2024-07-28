// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebPerformanceTimer
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public static class WebPerformanceTimer
  {
    public static PerformanceTimer StartMeasure(
      HtmlHelper htmlHelper,
      string groupName,
      string entryContext = null)
    {
      return WebPerformanceTimer.StartMeasure(htmlHelper.ViewContext.RequestContext, groupName, entryContext);
    }

    public static PerformanceTimer StartMeasure(
      HttpContext httpContext,
      string groupName,
      string entryContext = null)
    {
      return WebPerformanceTimer.StartMeasure(httpContext == null ? (RequestContext) null : httpContext.Request.RequestContext, groupName, entryContext);
    }

    public static PerformanceTimer StartMeasure(
      RequestContext requestContext,
      string groupName,
      string entryContext = null)
    {
      return PerformanceTimer.StartMeasure(requestContext == null ? (IVssRequestContext) null : requestContext.TfsRequestContext(), groupName, entryContext);
    }
  }
}
