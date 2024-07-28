// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatformHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class PlatformHelpers
  {
    public static Microsoft.TeamFoundation.Server.WebAccess.WebContext WebContext(
      this ViewContext viewContext)
    {
      return viewContext.RequestContext.WebContext();
    }

    public static IVssRequestContext TfsRequestContext(
      this HttpContextBase httpContext,
      bool required = false)
    {
      return PlatformHelpers.Implementation.Instance.TfsRequestContext(httpContext, required);
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

    public static string TrimmedVirtualDirectory(this TeamProjectCollectionProperties tpcProps) => PlatformHelpers.TrimVirtualPath(VirtualPathUtility.ToAppRelative(tpcProps.VirtualDirectory));

    public static string TrimmedVirtualDirectory(this IVssRequestContext tfsRequestContext) => PlatformHelpers.TrimVirtualPath(VirtualPathUtility.ToAppRelative(tfsRequestContext.VirtualPath()));

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

    public class Implementation
    {
      private static PlatformHelpers.Implementation s_instance;

      protected Implementation()
      {
      }

      public static PlatformHelpers.Implementation Instance
      {
        get
        {
          if (PlatformHelpers.Implementation.s_instance == null)
            PlatformHelpers.Implementation.s_instance = new PlatformHelpers.Implementation();
          return PlatformHelpers.Implementation.s_instance;
        }
        internal set => PlatformHelpers.Implementation.s_instance = value;
      }

      public virtual IVssRequestContext TfsRequestContext(
        HttpContextBase httpContext,
        bool required)
      {
        return WebPlatformHelpers.TfsRequestContext(httpContext, required);
      }
    }
  }
}
