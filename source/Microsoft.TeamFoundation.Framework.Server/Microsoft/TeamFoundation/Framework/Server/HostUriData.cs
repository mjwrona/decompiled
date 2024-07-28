// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostUriData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostUriData : IHostUriData, IInternalHostUriData
  {
    private string m_domainPrefix;
    private string m_pathSuffix;
    private string m_accessMappingMoniker;
    private bool m_virtualPathInAccessPoint;

    internal HostUriData(
      string domainPrefix,
      string pathSuffix,
      string accessMappingMoniker,
      bool virtualPathInAccessPoint = false)
    {
      this.m_domainPrefix = domainPrefix;
      this.m_pathSuffix = VirtualPathUtility.AppendTrailingSlash(pathSuffix) ?? "/";
      this.m_accessMappingMoniker = accessMappingMoniker;
      this.m_virtualPathInAccessPoint = virtualPathInAccessPoint;
    }

    public string RelativeVirtualPath => this.m_pathSuffix;

    internal string AccessMappingMoniker => this.m_accessMappingMoniker;

    internal bool VirtualPathInAccessPoint => this.m_virtualPathInAccessPoint;

    Uri IHostUriData.BuildUri(Uri baseUri) => this.BuildUri(baseUri, true);

    public Uri BuildUri(Uri baseUri, bool includeVirtualPath = true)
    {
      UriBuilder uriBuilder = new UriBuilder(baseUri);
      if (!string.IsNullOrEmpty(this.m_domainPrefix))
        uriBuilder.Host = this.m_domainPrefix + "." + uriBuilder.Host;
      if (includeVirtualPath && this.m_pathSuffix != "/")
        uriBuilder.Path = PathUtility.Combine(uriBuilder.Path, this.m_pathSuffix);
      return uriBuilder.Uri;
    }

    Uri IHostUriData.BuildUri(
      IVssRequestContext requestContext,
      Guid serviceIdentifier,
      bool throwOnMissingService)
    {
      return this.BuildUri(requestContext, serviceIdentifier, throwOnMissingService, true);
    }

    public Uri BuildUri(
      IVssRequestContext requestContext,
      Guid serviceIdentifier = default (Guid),
      bool throwOnMissingService = true,
      bool includeVirtualPath = true)
    {
      Uri serviceBaseUri = LocationServiceHelper.GetServiceBaseUri(requestContext, this.m_accessMappingMoniker, serviceIdentifier, throwOnMissingService);
      return !(serviceBaseUri != (Uri) null) ? (Uri) null : this.BuildUri(serviceBaseUri, includeVirtualPath);
    }

    public AccessMapping BuildAccessMapping(
      IVssRequestContext requestContext,
      string moniker,
      string displayName,
      bool throwOnMissingService = true)
    {
      IVssRequestContext requestContext1 = requestContext;
      bool flag = throwOnMissingService;
      bool pathInAccessPoint = this.m_virtualPathInAccessPoint;
      Guid serviceIdentifier = new Guid();
      int num1 = flag ? 1 : 0;
      int num2 = pathInAccessPoint ? 1 : 0;
      Uri uri = this.BuildUri(requestContext1, serviceIdentifier, num1 != 0, num2 != 0);
      if (uri == (Uri) null)
        return (AccessMapping) null;
      AccessMapping accessMapping1 = new AccessMapping();
      accessMapping1.Moniker = moniker;
      accessMapping1.DisplayName = displayName;
      accessMapping1.AccessPoint = uri.AbsoluteUri;
      AccessMapping accessMapping2 = accessMapping1;
      string str;
      if (this.m_virtualPathInAccessPoint)
        str = string.Empty;
      else
        str = this.RelativeVirtualPath.Trim('/');
      accessMapping2.VirtualDirectory = str;
      return accessMapping1;
    }
  }
}
