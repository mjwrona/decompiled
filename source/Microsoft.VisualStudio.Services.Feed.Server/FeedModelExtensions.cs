// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedModelExtensions
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public static class FeedModelExtensions
  {
    public static async Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> IncludeUrlsAsync(
      this IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> bareFeeds,
      IVssRequestContext requestContext)
    {
      List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds = new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
      foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed bareFeed in bareFeeds)
      {
        List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feedList = feeds;
        IVssRequestContext requestContext1 = requestContext;
        feedList.Add(await bareFeed.IncludeUrlsAsync(requestContext1));
        feedList = (List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) null;
      }
      List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feedList1 = feeds;
      feeds = (List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) null;
      return feedList1;
    }

    public static async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> IncludeUrlsAsync(
      this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, "WebApi.Feed");
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", feedUrl);
      referenceLinks.AddLink("packages", FeedModelExtensions.GetPackageUrl(requestContext, feed));
      if (feed.View == null)
        referenceLinks.AddLink("permissions", FeedModelExtensions.GetFeedPermissionsUrl(requestContext, feed));
      feed.Url = feedUrl;
      feed.Links = referenceLinks;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      feed1.UpstreamSources = await feed.UpstreamSources.ResolveUpstreamSourceUrlsAsync(requestContext);
      feed1 = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      return feed;
    }

    public static void RemoveDeletedUpstreamSources(this IList<UpstreamSource> upstreamSources)
    {
      if (upstreamSources == null)
        return;
      while (upstreamSources.Count > 0 && upstreamSources[upstreamSources.Count - 1].DeletedDate.HasValue)
        upstreamSources.RemoveAt(upstreamSources.Count - 1);
    }

    public static async Task<IList<UpstreamSource>> ResolveUpstreamSourceUrlsAsync(
      this IList<UpstreamSource> upstreamSources,
      IVssRequestContext requestContext)
    {
      if (upstreamSources != null)
      {
        for (int i = 0; i < upstreamSources.Count; ++i)
        {
          IList<UpstreamSource> upstreamSourceList = upstreamSources;
          int index = i;
          upstreamSourceList[index] = await upstreamSources[i].ResolveInternalUrlsAsync(requestContext);
          upstreamSourceList = (IList<UpstreamSource>) null;
        }
      }
      return upstreamSources;
    }

    public static async Task<UpstreamSource> ResolveInternalUrlsAsync(
      this UpstreamSource source,
      IVssRequestContext requestContext)
    {
      if (source.UpstreamSourceType != UpstreamSourceType.Internal || !source.InternalUpstreamFeedId.HasValue || !source.InternalUpstreamViewId.HasValue)
      {
        source.DisplayLocation = source.Location;
        return source;
      }
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed resolvedFeed = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      string resolvedCollectionName = (string) null;
      string resolvedProjectName = (string) null;
      try
      {
        Guid? nullable = source.InternalUpstreamCollectionId;
        Guid collectionId = nullable ?? requestContext.ServiceHost.InstanceId;
        if (collectionId == requestContext.ServiceHost.InstanceId)
        {
          resolvedCollectionName = requestContext.ServiceHost.CollectionServiceHost.Name;
          nullable = source.InternalUpstreamProjectId;
          if (nullable.HasValue)
          {
            IProjectService service = requestContext.GetService<IProjectService>();
            IVssRequestContext requestContext1 = requestContext;
            nullable = source.InternalUpstreamProjectId;
            Guid projectId = nullable.Value;
            resolvedProjectName = service.GetProjectName(requestContext1, projectId);
          }
          if (requestContext.IsFeatureEnabled("Packaging.Feed.ResolveUpstreamFeedLocation"))
          {
            IFeedService service = requestContext.GetService<IFeedService>();
            IVssRequestContext requestContext2 = requestContext;
            nullable = source.InternalUpstreamFeedId;
            string str1 = nullable.ToString();
            nullable = source.InternalUpstreamViewId;
            string str2 = nullable.ToString();
            string feedId = str1 + "@" + str2;
            ProjectReference projectReference = source.InternalUpstreamProjectId.ToProjectReference();
            resolvedFeed = service.GetFeed(requestContext2, feedId, projectReference);
          }
        }
        else
        {
          string collectionName;
          if (FeedModelExtensions.TryGetExternalCollectionName(requestContext, collectionId, out collectionName))
            resolvedCollectionName = collectionName;
          nullable = source.InternalUpstreamProjectId;
          if (nullable.HasValue)
          {
            IVssRequestContext requestContext3 = requestContext;
            nullable = source.InternalUpstreamCollectionId;
            Guid collectionId1 = nullable.Value;
            nullable = source.InternalUpstreamProjectId;
            Guid projectId = nullable.Value;
            resolvedProjectName = await FeedModelExtensions.GetExternalProjectName(requestContext3, collectionId1, projectId);
          }
          if (requestContext.IsFeatureEnabled("Packaging.Feed.ResolveUpstreamFeedLocation"))
          {
            IVssRequestContext requestContext4 = requestContext;
            Guid hostId = collectionId;
            Guid? upstreamProjectId = source.InternalUpstreamProjectId;
            nullable = source.InternalUpstreamFeedId;
            string feedId = nullable.ToString();
            nullable = source.InternalUpstreamViewId;
            string viewId = nullable.ToString();
            resolvedFeed = await UpstreamSourceHelper.GetExternalCollectionFeedAsync(requestContext4, hostId, upstreamProjectId, feedId, viewId);
          }
        }
        collectionId = new Guid();
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
      finally
      {
        if (resolvedCollectionName == null || resolvedFeed == null || source.InternalUpstreamProjectId.HasValue && resolvedProjectName == null)
        {
          if (source.InternalUpstreamProjectId.HasValue)
            requestContext.Trace(10019102, TraceLevel.Warning, "Feed", "Model", "Failed to resolve " + source.Location + ". Resolved collection: " + (resolvedCollectionName ?? "[failed]") + ". Resolved project: " + (resolvedProjectName ?? "[failed]") + ". Resolved feed: " + (resolvedFeed?.FullyQualifiedName ?? "[failed]"));
          else
            requestContext.Trace(10019102, TraceLevel.Warning, "Feed", "Model", "Failed to resolve " + source.Location + ". Resolved collection: " + (resolvedCollectionName ?? "[failed]") + ". Resolved feed: " + (resolvedFeed?.FullyQualifiedName ?? "[failed]"));
        }
        source.Location = UpstreamSourceValidator.Create(requestContext, source).GetNormalizedLocation();
        if (resolvedCollectionName != null && resolvedFeed != null)
          source.DisplayLocation = UpstreamSourceHelper.CreateInternalUpstreamLocator(resolvedCollectionName, resolvedProjectName, resolvedFeed.Name, resolvedFeed.ViewName);
        requestContext.Trace(10019102, TraceLevel.Info, "Feed", "Model", "Resolved as Location " + source.Location + ", DisplayLocation " + source.DisplayLocation);
      }
      return source;
    }

    private static bool TryGetExternalCollectionName(
      IVssRequestContext requestContext,
      Guid collectionId,
      out string collectionName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      INameResolutionService service = vssRequestContext.GetService<INameResolutionService>();
      collectionName = string.Empty;
      try
      {
        NameResolutionEntry primaryEntryForValue = service.GetPrimaryEntryForValue(vssRequestContext, collectionId);
        if (primaryEntryForValue != null)
        {
          if (primaryEntryForValue.IsEnabled)
          {
            if (!(primaryEntryForValue.Namespace == "Collection"))
            {
              if (!(primaryEntryForValue.Namespace == "GlobalCollection"))
                goto label_7;
            }
            collectionName = primaryEntryForValue.Name;
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
label_7:
      return false;
    }

    private static async Task<string> GetExternalProjectName(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid projectId)
    {
      try
      {
        string projectNameAsync = await UpstreamSourceHelper.GetExternalProjectNameAsync(requestContext, collectionId, projectId);
        if (!string.IsNullOrEmpty(projectNameAsync))
          return projectNameAsync;
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
      return (string) null;
    }

    public static FeedView IncludeUrls(
      this FeedView feedView,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      ArgumentUtility.CheckForNull<FeedView>(feedView, "FeedView");
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      string feedViewUrl = FeedModelExtensions.GetFeedViewUrl(requestContext, feed, feedView.Id.ToString());
      string packageUrl = FeedModelExtensions.GetPackageUrl(requestContext, feed);
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", feedViewUrl);
      referenceLinks.AddLink(nameof (feed), feedUrl);
      referenceLinks.AddLink("packages", packageUrl);
      feedView.Url = feedViewUrl;
      feedView.Links = referenceLinks;
      return feedView;
    }

    public static Package IncludeUrls(
      this Package package,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      feed.Id.ToString();
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      string packageUrl = FeedModelExtensions.GetPackageUrl(requestContext, feed, package.Id.ToString());
      string packageVersionUrl = FeedModelExtensions.GetPackageVersionUrl(requestContext, feed, package.Id.ToString());
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", packageUrl);
      referenceLinks.AddLink(nameof (feed), feedUrl);
      referenceLinks.AddLink("versions", packageVersionUrl);
      package.Links = referenceLinks;
      package.Url = packageUrl;
      return package;
    }

    public static Package IncludeRecycleBinUrls(
      this Package package,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      feed.Id.ToString();
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      string recycleBinPackageUrl = FeedModelExtensions.GetRecycleBinPackageUrl(requestContext, feed, package.Id.ToString());
      string packageVersionUrl = FeedModelExtensions.GetRecycleBinPackageVersionUrl(requestContext, feed, package.Id.ToString());
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", recycleBinPackageUrl);
      referenceLinks.AddLink(nameof (feed), feedUrl);
      referenceLinks.AddLink("versions", packageVersionUrl);
      package.Links = referenceLinks;
      package.Url = recycleBinPackageUrl;
      return package;
    }

    public static PackageVersion IncludeUrls(
      this PackageVersion packageVersion,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId)
    {
      ArgumentUtility.CheckForNull<PackageVersion>(packageVersion, "PackgeVersionMetadata");
      feed.Id.ToString();
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      string packageUrl = FeedModelExtensions.GetPackageUrl(requestContext, feed, packageId);
      string packageVersionUrl = FeedModelExtensions.GetPackageVersionUrl(requestContext, feed, packageId, packageVersion.Id.ToString());
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", packageVersionUrl);
      referenceLinks.AddLink(nameof (feed), feedUrl);
      referenceLinks.AddLink("package", packageUrl);
      packageVersion.Links = referenceLinks;
      packageVersion.Url = packageVersionUrl;
      return packageVersion;
    }

    public static RecycleBinPackageVersion IncludeUrls(
      this RecycleBinPackageVersion packageVersion,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId)
    {
      ArgumentUtility.CheckForNull<RecycleBinPackageVersion>(packageVersion, "PackgeVersionMetadata");
      feed.Id.ToString();
      string feedUrl = FeedModelExtensions.GetFeedUrl(requestContext, feed);
      string recycleBinPackageUrl = FeedModelExtensions.GetRecycleBinPackageUrl(requestContext, feed, packageId);
      string packageVersionUrl = FeedModelExtensions.GetRecycleBinPackageVersionUrl(requestContext, feed, packageId, packageVersion.Id.ToString());
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", packageVersionUrl);
      referenceLinks.AddLink(nameof (feed), feedUrl);
      referenceLinks.AddLink("package", recycleBinPackageUrl);
      packageVersion.Links = referenceLinks;
      packageVersion.Url = packageVersionUrl;
      return packageVersion;
    }

    public static async Task<FeedChange> IncludeUrlsAsync(
      this FeedChange feedChange,
      IVssRequestContext requestContext)
    {
      feedChange.Feed.Url = FeedModelExtensions.GetFeedUrl(requestContext, feedChange.Feed);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedChange.Feed;
      feed.UpstreamSources = await feedChange.Feed.UpstreamSources.ResolveUpstreamSourceUrlsAsync(requestContext);
      feed = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      return feedChange;
    }

    public static async Task<FeedChangesResponse> ToResponseAsync(
      this IEnumerable<FeedChange> feedChangeCollection,
      IVssRequestContext requestContext,
      int batchSize)
    {
      if (feedChangeCollection.IsNullOrEmpty<FeedChange>())
        return new FeedChangesResponse()
        {
          Count = 0,
          FeedChanges = (IEnumerable<FeedChange>) new List<FeedChange>(),
          Links = (ReferenceLinks) null
        };
      List<FeedChange> feedChangeList = new List<FeedChange>();
      long maxToken = 0;
      foreach (FeedChange change in feedChangeCollection)
      {
        FeedChange feedChange = await change.IncludeUrlsAsync(requestContext);
        maxToken = change.FeedContinuationToken > maxToken ? change.FeedContinuationToken : maxToken;
        feedChangeList.Add(feedChange);
      }
      string nextFeedChangesUrl = FeedModelExtensions.GetNextFeedChangesUrl(requestContext, maxToken, batchSize);
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("next", nextFeedChangesUrl);
      return new FeedChangesResponse()
      {
        Count = feedChangeList.Count,
        FeedChanges = (IEnumerable<FeedChange>) feedChangeList,
        NextFeedContinuationToken = maxToken,
        Links = referenceLinks
      };
    }

    public static PackageChangesResponse ToResponse(
      this IEnumerable<PackageChange> packageChangeCollection,
      IVssRequestContext requestContext,
      FeedCore feed,
      int batchSize)
    {
      if (packageChangeCollection.IsNullOrEmpty<PackageChange>())
        return new PackageChangesResponse()
        {
          Count = 0,
          PackageChanges = (IEnumerable<PackageChange>) new List<PackageChange>(),
          Links = (ReferenceLinks) null
        };
      PackageChange[] array = packageChangeCollection.ToArray<PackageChange>();
      long token = ((IEnumerable<PackageChange>) array).Aggregate<PackageChange, long>(0L, (Func<long, PackageChange, long>) ((current, packageChange) => packageChange.PackageVersionChange.ContinuationToken <= current ? current : packageChange.PackageVersionChange.ContinuationToken));
      string packageChangesUrl = FeedModelExtensions.GetNextPackageChangesUrl(requestContext, feed, token, batchSize);
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("next", packageChangesUrl);
      return new PackageChangesResponse()
      {
        Count = ((IEnumerable<PackageChange>) array).Count<PackageChange>(),
        PackageChanges = (IEnumerable<PackageChange>) array,
        NextPackageContinuationToken = token,
        Links = referenceLinks
      };
    }

    public static Microsoft.VisualStudio.Services.Feed.WebApi.Feed Clone(this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = new Microsoft.VisualStudio.Services.Feed.WebApi.Feed();
      feed1.Id = feed.Id;
      feed1.Name = feed.Name;
      feed1.UpstreamEnabled = feed.UpstreamEnabled;
      feed1.AllowUpstreamNameConflict = feed.AllowUpstreamNameConflict;
      feed1.View = feed.View;
      feed1.Capabilities = feed.Capabilities;
      feed1.UpstreamSources = feed.UpstreamSources;
      feed1.BadgesEnabled = feed.BadgesEnabled;
      feed1.DefaultViewId = feed.DefaultViewId;
      feed1.DeletedDate = feed.DeletedDate;
      feed1.Description = feed.Description;
      feed1.HideDeletedPackageVersions = feed.HideDeletedPackageVersions;
      feed1.Links = feed.Links;
      feed1.Permissions = feed.Permissions;
      feed1.UpstreamEnabledChangedDate = feed.UpstreamEnabledChangedDate;
      feed1.Url = feed.Url;
      feed1.Project = feed.Project;
      return feed1;
    }

    public static Microsoft.VisualStudio.Services.Feed.WebApi.Feed ThrowIfReadOnly(
      this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      bool canBypassUnderMaintenance)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed.Clone();
      if (canBypassUnderMaintenance)
        feed1.Capabilities &= ~FeedCapabilities.UnderMaintenance;
      if (feed1.IsReadOnly)
        throw FeedIsReadOnlyException.Create(feed);
      return feed;
    }

    public static Microsoft.VisualStudio.Services.Feed.WebApi.Feed ThrowIfViewNotation(
      this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return feed.View == null ? feed : throw new NotSupportedException(Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Error_CannotUseViewNotation());
    }

    private static string GetFeedUrl(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.FeedLocationId, projectId, (object) new
      {
        feedId = feed.Id.ToString()
      }).AbsoluteUri;
    }

    private static string GetFeedViewUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string feedViewId)
    {
      if (requestContext == null)
        return (string) null;
      ILocationService service = requestContext.GetService<ILocationService>();
      ProjectReference project = feed.Project;
      Guid guid = (object) project != null ? project.Id : Guid.Empty;
      IVssRequestContext requestContext1 = requestContext;
      Guid feedViewLocationId = FeedWebApiConstants.FeedViewLocationId;
      Guid projectId = guid;
      var routeValues = new
      {
        feedId = feed.Id,
        viewId = feedViewId
      };
      Guid serviceOwner = new Guid();
      return service.GetResourceUri(requestContext1, "Packaging", feedViewLocationId, projectId, (object) routeValues, serviceOwner).AbsoluteUri;
    }

    private static string GetPackageUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId = null)
    {
      ILocationService service = requestContext != null ? requestContext.GetService<ILocationService>() : (ILocationService) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return service == null ? (string) null : service.GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.PackageLocationId, projectId, (object) new
      {
        feedId = feed.FullyQualifiedId,
        packageId = packageId
      }).AbsoluteUri;
    }

    private static string GetPackageVersionUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId = null)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.PackageVersionLocationId, projectId, (object) new
      {
        feedId = feed.FullyQualifiedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }).AbsoluteUri;
    }

    private static string GetRecycleBinPackageVersionUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId = null)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.PackageVersionRecycleBinLocationId, projectId, (object) new
      {
        feedId = feed.Id.ToString(),
        packageId = packageId,
        packageVersionId = packageVersionId
      }).AbsoluteUri;
    }

    private static string GetRecycleBinPackageUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.PackageRecycleBinLocationId, projectId, (object) new
      {
        feedId = feed.Id.ToString(),
        packageId = packageId
      }).AbsoluteUri;
    }

    private static string GetFeedPermissionsUrl(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.FeedPermissionsLocationId, projectId, (object) new
      {
        feedId = feed.Id.ToString()
      }).AbsoluteUri;
    }

    private static string GetNextPackageChangesUrl(
      IVssRequestContext requestContext,
      FeedCore feed,
      long token,
      int batchSize)
    {
      if (requestContext == null)
        return (string) null;
      ProjectReference project = feed.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.PackageChangeLocationId, projectId, (object) new
      {
        feedId = feed.Id.ToString()
      }).AbsoluteUri + "?continuationtoken=" + token.ToString() + "&batchSize=" + batchSize.ToString();
    }

    private static string GetNextFeedChangesUrl(
      IVssRequestContext requestContext,
      long token,
      int batchSize)
    {
      if (requestContext == null)
        return (string) null;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Packaging", FeedWebApiConstants.FeedChangeLocationId, (object) null).AbsoluteUri + "?continuationtoken=" + token.ToString() + "&batchSize=" + batchSize.ToString();
    }
  }
}
