// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsUrlHelper
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsUrlHelper : UrlHelper
  {
    public AnalyticsUrlHelper(HttpRequestMessage request)
      : base(request)
    {
    }

    public override string Route(string routeName, IDictionary<string, object> routeValues)
    {
      IVssRequestContext ivssRequestContext = this.Request.GetIVssRequestContext();
      string virtualPath = base.Route(routeName, routeValues);
      IVssServiceHost serviceHost = ivssRequestContext.ServiceHost;
      IHttpRoute route = this.Request.GetConfiguration().Routes[routeName];
      if (!serviceHost.Is(TeamFoundationHostType.ProjectCollection))
        return virtualPath;
      string applicationPath = HostingEnvironment.IsHosted ? HostingEnvironment.ApplicationVirtualPath : "/";
      string str = VirtualPathUtility.ToAppRelative(virtualPath, applicationPath).TrimStart('~', '/');
      return new HttpVirtualPathData(route, ivssRequestContext.VirtualPath() + str).VirtualPath;
    }

    public override string Link(string routeName, IDictionary<string, object> routeValues)
    {
      string relativeUri = this.Route(routeName, routeValues);
      if (!string.IsNullOrEmpty(relativeUri))
        relativeUri = !this.Request.GetIVssRequestContext().GetService<IVssRegistryService>().GetValue<bool>(this.Request.GetIVssRequestContext(), (RegistryQuery) "/Service/Analytics/Settings/OData/UseLocationServiceForNextLink", false) ? new Uri(this.Request.RequestUri, relativeUri).AbsoluteUri : new Uri(this.Request.GetIVssRequestContext().GetService<ILocationService>().GetResourceUri(this.Request.GetIVssRequestContext(), "Analytics", ResourceIds.AnalyticsResourceId, (object) routeValues), relativeUri).AbsoluteUri;
      return relativeUri;
    }
  }
}
