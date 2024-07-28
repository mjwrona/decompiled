// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Web;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  internal static class SmartRouterExtensions
  {
    public static bool IsStaticContent(this IVssWebRequestContext requestContext)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal))
        return false;
      HttpContextBase httpContext = requestContextInternal.HttpContext;
      return httpContext != null && httpContext.Items.Contains((object) HttpContextConstants.IsStaticContentRequest);
    }

    public static bool IsRequestArrRouted(this IVssWebRequestContext requestContext)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal))
        return false;
      HttpContextBase httpContext = requestContextInternal.HttpContext;
      return httpContext != null && httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted);
    }

    public static bool IsRequestArrForwardedFor(this IVssWebRequestContext requestContext) => requestContext is IWebRequestContextInternal requestContextInternal && requestContextInternal.HttpContext.Request.Headers["X-Arr-Forwarded-For"] != null;

    public static bool IsReverseProxyTarget(
      this IVssWebRequestContext requestContext,
      out string? forwardedFor)
    {
      forwardedFor = (string) null;
      if (requestContext is IWebRequestContextInternal requestContextInternal)
      {
        if (requestContext.IsSmartRouterForwardingIpValidationEnabled())
        {
          forwardedFor = requestContextInternal.HttpContext.Request.Headers["X-SmartRouter-Forwarded-For"];
          return forwardedFor != null;
        }
        if (bool.TrueString.Equals(requestContextInternal.HttpContext.Request.Headers["X-VSS-RequestSmartRouted"]))
          return true;
      }
      return false;
    }

    public static bool IsDebugEnvironment(this IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsDevFabricDeployment && Debugger.IsAttached;
  }
}
