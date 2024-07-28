// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebPlatformHelpers
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public static class WebPlatformHelpers
  {
    private const string c_TeamFoundationRequestContext = "IVssRequestContext";

    public static IVssRequestContext TfsRequestContext(this ViewContext viewContext, bool required = true) => viewContext.HttpContext.TfsRequestContext(required);

    public static IVssRequestContext TfsRequestContext(
      this HttpContextBase httpContext,
      bool required = false)
    {
      IVssRequestContext vssRequestContext = httpContext.Items[(object) "IVssRequestContext"] as IVssRequestContext;
      return !required || vssRequestContext != null ? vssRequestContext : throw new InvalidOperationException("RequestContext not found");
    }

    public static IVssRequestContext TfsRequestContext(
      this RequestContext requestContext,
      bool required = false)
    {
      return requestContext.HttpContext.TfsRequestContext(required);
    }

    public static string TrimVirtualPath(string virtualPath)
    {
      if (string.IsNullOrEmpty(virtualPath))
        return virtualPath;
      if (virtualPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
        virtualPath = virtualPath.Substring(2);
      return virtualPath.Trim('/');
    }

    public static string TrimmedVirtualDirectory(this TeamProjectCollectionProperties tpcProps) => WebPlatformHelpers.TrimVirtualPath(VirtualPathUtility.ToAppRelative(tpcProps.VirtualDirectory));

    public static string TrimmedVirtualDirectory(this IVssRequestContext tfsRequestContext) => WebPlatformHelpers.TrimVirtualPath(VirtualPathUtility.ToAppRelative(tfsRequestContext.VirtualPath()));

    public static TeamFoundationHostType IntendedHostType(this IVssRequestContext requestContext) => requestContext.IntendedHostType(TeamFoundationHostType.Unknown);

    public static TeamFoundationHostType IntendedHostType(
      this IVssRequestContext requestContext,
      TeamFoundationHostType hostType)
    {
      TeamFoundationHostType foundationHostType = hostType;
      if (hostType == TeamFoundationHostType.Unknown)
        foundationHostType = requestContext.ServiceHost.HostType;
      if ((foundationHostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        if (requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>().IsHosted)
          foundationHostType = TeamFoundationHostType.Deployment;
        else
          foundationHostType &= ~TeamFoundationHostType.Deployment;
      }
      return foundationHostType;
    }

    public static bool IntendedHostTypeIs(
      this IVssRequestContext requestContext,
      TeamFoundationHostType hostType)
    {
      return (requestContext.IntendedHostType() & hostType) == hostType;
    }
  }
}
