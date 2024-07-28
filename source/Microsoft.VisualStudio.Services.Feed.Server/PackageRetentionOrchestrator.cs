// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRetentionOrchestrator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Retention;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRetentionOrchestrator
  {
    private readonly IFeedService feedService;
    private readonly IPackageRetentionPolicyStore policyStore;
    private readonly IPackageRetentionVersionSource retentionSource;
    private readonly IPackagingProtocolService protocolService;
    private readonly IRetentionOperationsLimitCalculator operationsLimitCalculator;

    public PackageRetentionOrchestrator(
      IFeedService feedService,
      IPackageRetentionPolicyStore policyStore,
      IPackageRetentionVersionSource retentionSource,
      IPackagingProtocolService protocolService,
      IRetentionOperationsLimitCalculator operationsLimitCalculator)
    {
      this.feedService = feedService;
      this.policyStore = policyStore;
      this.retentionSource = retentionSource;
      this.protocolService = protocolService;
      this.operationsLimitCalculator = operationsLimitCalculator;
      this.ProcessingResults = (IDictionary<Guid, FeedProtocolDeletionResult>) new Dictionary<Guid, FeedProtocolDeletionResult>();
    }

    public IDictionary<Guid, FeedProtocolDeletionResult> ProcessingResults { get; }

    public bool DeletedAtLeastOnePackageVersion { get; private set; }

    public bool? ProcessingSucceeded { get; private set; }

    public async Task DeletePackagesEligibleForDeletionAsync(IVssRequestContext requestContext)
    {
      int maxOperationsPerFeedProtocol = this.operationsLimitCalculator.GetFeedProtocolOperationsLimit(requestContext);
      await Task.WhenAll(this.GetFeedsWithRetentionPolicy(requestContext).Select<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Task>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Task>) (feed =>
      {
        FeedRetentionPolicy retentionPolicy = this.GetRetentionPolicy(requestContext, feed);
        if (retentionPolicy == null)
          return Task.CompletedTask;
        IEnumerable<ProtocolPackageVersionIdentity> versionsToDelete = RetentionTrimmedPackageVersionHardcodeRewriter.RewriteKnownTrimmedVersions(requestContext, this.retentionSource.GetPackagesEligibleForDeletion(requestContext, feed, retentionPolicy));
        return Task.WhenAll(this.DeletePackageVersions(requestContext, feed, versionsToDelete, maxOperationsPerFeedProtocol));
      })));
      if (this.ProcessingSucceeded.HasValue)
        return;
      this.ProcessingSucceeded = new bool?(true);
    }

    private FeedRetentionPolicy GetRetentionPolicy(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedRetentionPolicy policy = this.policyStore.GetPolicy(requestContext, feed);
      if (policy != null && policy.CountLimit.HasValue)
      {
        this.GetProcessingResults(feed).PolicyCountLimit = policy.CountLimit.Value;
        this.GetProcessingResults(feed).PolicyDaysToKeepRecentlyDownloaded = policy.DaysToKeepRecentlyDownloadedPackages.GetValueOrDefault();
      }
      return policy;
    }

    private IList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsWithRetentionPolicy(
      IVssRequestContext requestContext)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds = this.feedService.GetFeeds(requestContext, (ProjectReference) null);
      ISet<Guid> retentionFeedIds = this.policyStore.GetFeeds(requestContext);
      Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, bool> predicate = (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, bool>) (x => retentionFeedIds.Contains(x.Id));
      return (IList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) feeds.Where<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(predicate).ToList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
    }

    private IEnumerable<Task> DeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<ProtocolPackageVersionIdentity> versionsToDelete,
      int maxOperationsPerFeedProtocol)
    {
      this.DeletedAtLeastOnePackageVersion = false;
      foreach (IGrouping<string, ProtocolPackageVersionIdentity> source in versionsToDelete.GroupBy<ProtocolPackageVersionIdentity, string>((Func<ProtocolPackageVersionIdentity, string>) (x => x.Protocol), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        string key = source.Key;
        int eligibleForDeletionCount = source.Count<ProtocolPackageVersionIdentity>();
        this.GetProcessingResults(feed).SetProtocolEligibleForDeletionCount(key, eligibleForDeletionCount);
        if (eligibleForDeletionCount > maxOperationsPerFeedProtocol)
          requestContext.TraceAlways(10019126, TraceLevel.Warning, "Feed", "PackageRetention", string.Format("Max limit of package retention operations reached. Only the limit of {0} ", (object) maxOperationsPerFeedProtocol) + string.Format("operations will be processed. Feed: {0}, protocol: {1}, eligible versions for deletion: {2}.", (object) feed.Id, (object) key, (object) eligibleForDeletionCount));
        yield return this.DeletePackageVersions(requestContext, feed, key, (IEnumerable<PackageVersionIdentity>) source.Take<ProtocolPackageVersionIdentity>(maxOperationsPerFeedProtocol));
      }
    }

    private async Task DeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocol,
      IEnumerable<PackageVersionIdentity> packageVersions)
    {
      try
      {
        await this.protocolService.GetPackagingOperations(protocol).DeletePackageVersions(requestContext, feed, packageVersions);
        int deletionCount = packageVersions.Count<PackageVersionIdentity>();
        this.GetProcessingResults(feed).SetProtocolCount(protocol, deletionCount);
        if (deletionCount > 0)
          this.DeletedAtLeastOnePackageVersion = true;
        FeedCiPublisher.PublishDeletePackageFromRetentionEvent(requestContext, feed, protocol, packageVersions);
      }
      catch (FeedIdNotFoundException ex)
      {
      }
      catch (ProjectDoesNotExistException ex)
      {
        requestContext.Trace(10019089, TraceLevel.Verbose, "Feed", "PackageRetention", "Ignoring project does not exist");
      }
      catch (Exception ex) when (ex.GetType().Name == "UpstreamProjectDoesNotExistException")
      {
        requestContext.Trace(10019089, TraceLevel.Verbose, "Feed", "PackageRetention", "Ignoring upstream project does not exist");
      }
      catch (NotSupportedException ex)
      {
        requestContext.Trace(10019090, TraceLevel.Verbose, "Feed", "PackageRetention", "Ignoring missing packaging operation for protocol '" + protocol + "'");
      }
      catch (TypeLoadException ex)
      {
        requestContext.TraceException(10019090, "Feed", "PackageRetention", (Exception) ex);
        this.ProcessingSucceeded = new bool?(false);
      }
      catch (FeedNeedsPermissionsException ex)
      {
        requestContext.TraceException(10019089, "Feed", "PackageRetention", (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019089, "Feed", "PackageRetention", ex);
        this.ProcessingSucceeded = new bool?(false);
      }
    }

    private FeedProtocolDeletionResult GetProcessingResults(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedProtocolDeletionResult processingResults;
      if (!this.ProcessingResults.TryGetValue(feed.Id, out processingResults))
      {
        processingResults = new FeedProtocolDeletionResult(feed.Id);
        this.ProcessingResults[feed.Id] = processingResults;
      }
      return processingResults;
    }
  }
}
