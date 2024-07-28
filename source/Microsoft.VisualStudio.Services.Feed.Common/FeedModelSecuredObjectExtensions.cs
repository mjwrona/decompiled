// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedModelSecuredObjectExtensions
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedModelSecuredObjectExtensions
  {
    public static Microsoft.VisualStudio.Services.Feed.WebApi.Feed SetSecuredObject(this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (feed == null)
        return (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      feed = feed.SetSecuredObject<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(securedObjectReadOnly);
      feed.Permissions = feed.Permissions.SetSecuredObject<FeedPermission>((ISecuredObject) feed);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      ReferenceLinks links = feed.Links;
      ReferenceLinks securedObject = links != null ? links.ToSecuredObject((ISecuredObject) feed) : (ReferenceLinks) null;
      feed1.Links = securedObject;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed;
      FeedView view = feed.View;
      FeedView feedView = view != null ? FeedModelSecuredObjectExtensions.SetSecuredObject(view, feed) : (FeedView) null;
      feed2.View = feedView;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed3 = feed;
      ProjectReference project = feed.Project;
      ProjectReference projectReference = (object) project != null ? project.SetSecuredObject<ProjectReference>((ISecuredObject) feed) : (ProjectReference) null;
      feed3.Project = projectReference;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed4 = feed;
      IList<UpstreamSource> upstreamSources = feed.UpstreamSources;
      List<UpstreamSource> list = upstreamSources != null ? upstreamSources.SetSecuredObject((ISecuredObject) feed).ToList<UpstreamSource>() : (List<UpstreamSource>) null;
      feed4.UpstreamSources = (IList<UpstreamSource>) list;
      return feed;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> SetSecuredObject(
      this IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds)
    {
      return feeds == null ? (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) null : feeds.Select<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feed => feed.SetSecuredObject()));
    }

    public static ReferenceLinks ToSecuredObject(this ReferenceLinks links, ISecuredObject feed)
    {
      if (links == null)
        return (ReferenceLinks) null;
      ReferenceLinks target = new ReferenceLinks();
      links.CopyTo(target, feed);
      return target;
    }

    public static IEnumerable<FeedView> SetSecuredObject(
      this IEnumerable<FeedView> views,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (views == null)
        return (IEnumerable<FeedView>) null;
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return views.Select<FeedView, FeedView>((Func<FeedView, FeedView>) (view => view.SetSecuredObject(securedObject)));
    }

    public static FeedView SetSecuredObject(this FeedView view, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (view == null)
        return (FeedView) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return view.SetSecuredObject(securedObjectReadOnly);
    }

    public static Package SetSecuredObject(this Package package, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (package == null)
        return (Package) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return package.SetSecuredObject(securedObjectReadOnly);
    }

    public static IEnumerable<Package> SetSecuredObject(
      this IEnumerable<Package> packages,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (packages == null)
        return packages;
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return packages.Select<Package, Package>((Func<Package, Package>) (package => package.SetSecuredObject(securedObject)));
    }

    public static Package SetSecuredObject(this Package package, ISecuredObject securedObject)
    {
      if (package == null)
        return (Package) null;
      package.SetSecuredObject<FeedSecuredObject>(securedObject);
      Package package1 = package;
      ReferenceLinks links = package.Links;
      ReferenceLinks securedObject1 = links != null ? links.ToSecuredObject((ISecuredObject) package) : (ReferenceLinks) null;
      package1.Links = securedObject1;
      Package package2 = package;
      IEnumerable<MinimalPackageVersion> versions = package.Versions;
      IEnumerable<MinimalPackageVersion> minimalPackageVersions = versions != null ? versions.SetSecuredObject((ISecuredObject) package) : (IEnumerable<MinimalPackageVersion>) null;
      package2.Versions = minimalPackageVersions;
      return package;
    }

    public static PackageVersion SetSecuredObject(this PackageVersion version, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (version == null)
        return (PackageVersion) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return FeedModelSecuredObjectExtensions.SetSecuredObject(version, securedObjectReadOnly);
    }

    public static IEnumerable<PackageVersion> SetSecuredObject(
      this IEnumerable<PackageVersion> versions,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return versions == null ? (IEnumerable<PackageVersion>) null : versions.Select<PackageVersion, PackageVersion>((Func<PackageVersion, PackageVersion>) (version => FeedModelSecuredObjectExtensions.SetSecuredObject(version, feed)));
    }

    public static PackageVersionProvenance SetSecuredObject(
      this PackageVersionProvenance provenance,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (provenance == null)
        return (PackageVersionProvenance) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      provenance.SetSecuredObject<FeedSecuredObject>(securedObjectReadOnly);
      provenance.Provenance.SetSecuredObject<FeedSecuredObject>(securedObjectReadOnly);
      return provenance;
    }

    public static IEnumerable<PackageMetrics> SetSecuredObject(
      this IEnumerable<PackageMetrics> packageMetrics,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (packageMetrics == null)
        return (IEnumerable<PackageMetrics>) null;
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return packageMetrics.Select<PackageMetrics, PackageMetrics>((Func<PackageMetrics, PackageMetrics>) (x => x.SetSecuredObject<PackageMetrics>(securedObject)));
    }

    public static IEnumerable<PackageVersionMetrics> SetSecuredObject(
      this IEnumerable<PackageVersionMetrics> packageVersionMetrics,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (packageVersionMetrics == null)
        return (IEnumerable<PackageVersionMetrics>) null;
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return packageVersionMetrics.Select<PackageVersionMetrics, PackageVersionMetrics>((Func<PackageVersionMetrics, PackageVersionMetrics>) (x => x.SetSecuredObject<PackageVersionMetrics>(securedObject)));
    }

    public static PackageDetails SetSecuredObject(this PackageDetails packageDetails, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (packageDetails == null)
        return (PackageDetails) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      packageDetails.SetSecuredObject<FeedSecuredObject>(securedObjectReadOnly);
      packageDetails.Package.SetSecuredObject(securedObjectReadOnly);
      FeedModelSecuredObjectExtensions.SetSecuredObject(packageDetails.PackageVersion, securedObjectReadOnly);
      return packageDetails;
    }

    public static T SetSecuredObject<T>(this T self, ISecuredObject other) where T : FeedSecuredObject
    {
      if ((object) self == null)
        return default (T);
      self.NamespaceId = other.NamespaceId;
      self.requiredPermissions = other.RequiredPermissions;
      self.token = other.GetToken();
      return self;
    }

    public static FeedRetentionPolicy SetSecuredObject(this FeedRetentionPolicy policy, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (policy == null)
        return (FeedRetentionPolicy) null;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly((FeedCore) feed);
      return policy.SetSecuredObject<FeedRetentionPolicy>(securedObjectReadOnly);
    }

    public static IEnumerable<T> SetSecuredObject<T>(this IEnumerable<T> self, ISecuredObject other) where T : FeedSecuredObject => self == null ? (IEnumerable<T>) null : self.Select<T, T>((Func<T, T>) (x => x.SetSecuredObject<T>(other)));

    public static IEnumerable<PackageListDataItem> SetSecuredObject(
      this IEnumerable<PackageListDataItem> self,
      ISecuredObject other)
    {
      return self == null ? (IEnumerable<PackageListDataItem>) null : self.Select<PackageListDataItem, PackageListDataItem>((Func<PackageListDataItem, PackageListDataItem>) (x => x.SetSecuredObject(other)));
    }

    public static PackageListDataItem SetSecuredObject(
      this PackageListDataItem self,
      ISecuredObject other)
    {
      if (self == null)
        return (PackageListDataItem) null;
      self.SetSecuredObject<FeedSecuredObject>(other);
      self.Package.SetSecuredObject<MinimalPackage>(other);
      self.LatestVersion.SetSecuredObject<SkeletalPackageVersion>(other);
      return self;
    }

    private static FeedView SetSecuredObject(this FeedView view, ISecuredObject securedObject)
    {
      if (view == null)
        return (FeedView) null;
      view.SetSecuredObject<FeedSecuredObject>(securedObject);
      if (view.Links != null)
        view.Links = view.Links.ToSecuredObject(securedObject);
      return view;
    }

    public static IEnumerable<UpstreamSource> SetSecuredObject(
      this IEnumerable<UpstreamSource> upstreamSources,
      ISecuredObject securedObject)
    {
      if (upstreamSources == null)
        return (IEnumerable<UpstreamSource>) null;
      upstreamSources.ForEach<UpstreamSource>((Action<UpstreamSource>) (u =>
      {
        u.SetSecuredObject<UpstreamSource>(securedObject);
        IEnumerable<UpstreamStatusDetail> statusDetails = u.StatusDetails;
        if (statusDetails == null)
          return;
        statusDetails.ForEach<UpstreamStatusDetail>((Action<UpstreamStatusDetail>) (usd => usd.SetSecuredObject<UpstreamStatusDetail>(securedObject)));
      }));
      return upstreamSources;
    }

    public static PackageVersion SetSecuredObject(
      this PackageVersion version,
      ISecuredObject securedObject)
    {
      if (version == null)
        return (PackageVersion) null;
      FeedModelSecuredObjectExtensions.SetSecuredObject(version, securedObject);
      version.ProtocolMetadata = version.ProtocolMetadata.SetSecuredObject<ProtocolMetadata>(securedObject);
      version.Dependencies = version.Dependencies.SetSecuredObject<PackageDependency>(securedObject);
      version.OtherVersions = version.OtherVersions.SetSecuredObject(securedObject);
      version.Files = version.Files.SetSecuredObject(securedObject);
      PackageVersion packageVersion = version;
      ReferenceLinks links = version.Links;
      ReferenceLinks securedObject1 = links != null ? links.ToSecuredObject(securedObject) : (ReferenceLinks) null;
      packageVersion.Links = securedObject1;
      version.SourceChain = version.SourceChain.SetSecuredObject(securedObject);
      return version;
    }

    private static IEnumerable<PackageFile> SetSecuredObject(
      this IEnumerable<PackageFile> files,
      ISecuredObject securedObject)
    {
      return files == null ? (IEnumerable<PackageFile>) null : files.Select<PackageFile, PackageFile>((Func<PackageFile, PackageFile>) (file => file.SetSecuredObject(securedObject)));
    }

    private static PackageFile SetSecuredObject(this PackageFile file, ISecuredObject securedObject)
    {
      if (file == null)
        return (PackageFile) null;
      file.SetSecuredObject<FeedSecuredObject>(securedObject);
      PackageFile packageFile = file;
      ProtocolMetadata protocolMetadata1 = file.ProtocolMetadata;
      ProtocolMetadata protocolMetadata2 = protocolMetadata1 != null ? protocolMetadata1.SetSecuredObject<ProtocolMetadata>(securedObject) : (ProtocolMetadata) null;
      packageFile.ProtocolMetadata = protocolMetadata2;
      return file;
    }

    private static MinimalPackageVersion SetSecuredObject(
      this MinimalPackageVersion version,
      ISecuredObject securedObject)
    {
      if (version == null)
        return (MinimalPackageVersion) null;
      MinimalPackageVersion self = version;
      if (self != null)
        self.SetSecuredObject<FeedSecuredObject>(securedObject);
      MinimalPackageVersion minimalPackageVersion = version;
      IEnumerable<FeedView> feedViews;
      if (version == null)
      {
        feedViews = (IEnumerable<FeedView>) null;
      }
      else
      {
        IEnumerable<FeedView> views = version.Views;
        feedViews = views != null ? views.SetSecuredObject<FeedView>(securedObject) : (IEnumerable<FeedView>) null;
      }
      minimalPackageVersion.Views = feedViews;
      return version;
    }

    private static IEnumerable<MinimalPackageVersion> SetSecuredObject(
      this IEnumerable<MinimalPackageVersion> versions,
      ISecuredObject securedObject)
    {
      return versions == null ? (IEnumerable<MinimalPackageVersion>) null : versions.Select<MinimalPackageVersion, MinimalPackageVersion>((Func<MinimalPackageVersion, MinimalPackageVersion>) (version => version.SetSecuredObject(securedObject)));
    }
  }
}
