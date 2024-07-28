// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageUpstreamBehaviorService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  internal class PackageUpstreamBehaviorService : 
    IPackageUpstreamBehaviorService,
    IVssFrameworkService
  {
    private ConcurrentDictionary<IProtocol, IUpstreamBehaviorProvider> BehaviorProviders = new ConcurrentDictionary<IProtocol, IUpstreamBehaviorProvider>();

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public UpstreamingBehavior GetBehavior(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<IPackageName>(packageName, nameof (packageName));
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      UpstreamingBehavior specifiedBehavior1 = this.GetUserSpecifiedBehavior(requestContext, feed, packageName);
      if ((object) specifiedBehavior1 != null)
        return specifiedBehavior1;
      UpstreamingBehavior specifiedBehavior2 = this.GetProtocolSpecifiedBehavior(requestContext, feed, packageName);
      return (object) specifiedBehavior2 != null ? specifiedBehavior2 : UpstreamingBehavior.Auto;
    }

    public void SetBehavior(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      UpstreamingBehavior behavior)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<IPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForNull<UpstreamingBehavior>(behavior, nameof (behavior));
      FeedSecurityHelper.CheckAdministerFeedPermissions(requestContext, feed);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string path = this.RootPath(feed, packageName.Protocol) + "/" + this.RegistrySafePathName(packageName);
      if (behavior.VersionsFromExternalUpstreams == UpstreamVersionVisibility.AllowExternalVersions)
        service.SetValue<bool>(requestContext, path, true);
      else
        service.SetValue(requestContext, path, (object) null);
    }

    private UpstreamingBehavior GetUserSpecifiedBehavior(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = this.RootPath(feed, packageName.Protocol);
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = (RegistryQuery) (str + "/**");
      return !service.ReadEntries(requestContext1, query).ContainsPath(this.RegistrySafePathName(packageName)) ? (UpstreamingBehavior) null : UpstreamingBehavior.AllowExternalVersions;
    }

    private UpstreamingBehavior GetProtocolSpecifiedBehavior(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName)
    {
      return this.BehaviorProviders.GetOrAdd<IVssRequestContext>(packageName.Protocol, (Func<IProtocol, IVssRequestContext, IUpstreamBehaviorProvider>) ((p, rc) => rc.To(TeamFoundationHostType.Deployment).GetService<IVssExtensionManagementService>().GetExtension<IUpstreamBehaviorProvider>(rc, strategy: p.CorrectlyCasedName)), requestContext)?.GetBehavior(requestContext, feed, packageName);
    }

    private string RootPath(FeedCore feed, IProtocol protocol) => string.Format("/Service/Packaging/Feed/{0}/AllowExternalUpstreamVersions/{1}", (object) feed.Id, (object) protocol.CorrectlyCasedName);

    private string RegistrySafePathName(IPackageName packageName) => packageName.NormalizedName.Replace(':', '/');
  }
}
