// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeAggregationAccessor`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public abstract class VersionListWithSizeAggregationAccessor<TPackageIdentity> : 
    IAggregationAccessor,
    IVersionListWithSizeService
    where TPackageIdentity : 
    #nullable disable
    IPackageIdentity
  {
    private const string CacheRequestContextItemsKey = "VersionListWithSizeAggregationAccessor.Cache";
    private readonly IVersionListWithSizeFileProvider versionListWithSizeFileProvider;
    private readonly ITracerService tracer;
    private readonly IRetryCountProvider retryCountProvider;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IVssRequestContext requestContext;
    private readonly ICache<string, object> requestContextItems;
    private int cacheHits;
    private int cacheMisses;
    private int applyCalls;

    public VersionListWithSizeAggregationAccessor(
      IAggregation aggregation,
      IVersionListWithSizeFileProvider versionListWithSizeFileProvider,
      ITracerService tracer,
      IRetryCountProvider retryCountProvider,
      IFeatureFlagService featureFlagService,
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
    {
      this.Aggregation = aggregation;
      this.versionListWithSizeFileProvider = versionListWithSizeFileProvider;
      this.tracer = tracer;
      this.retryCountProvider = retryCountProvider;
      this.featureFlagService = featureFlagService;
      this.requestContext = requestContext;
      this.requestContextItems = requestContextItems;
      throw new NotSupportedException("This aggregation has multiple serious bugs (#1938395, #1938413) which must be fixed before it can be used");
    }

    public async Task ApplyCommitsAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits)
    {
      VersionListWithSizeAggregationAccessor<TPackageIdentity> sendInTheThisObject = this;
      ++sendInTheThisObject.applyCalls;
      using (ITracerBlock trace = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (ApplyCommitsAsync)))
      {
        if (!packageCommits.Any<PackageCommit<TPackageIdentity>>())
          return;
        bool cacheIsEnabled = sendInTheThisObject.featureFlagService.IsEnabled("Packaging.CacheVersionListWithSizeFileBetweenCommits");
        trace.TraceInfo("Cache is " + (cacheIsEnabled ? "enabled" : "disabled") + ".");
        int num = 100;
        int maxIterations = sendInTheThisObject.retryCountProvider.Get();
        double splitRatio = 1.0 / 3.0;
        double incrementRatio = 0.375;
        Stopwatch stopwatch = Stopwatch.StartNew();
        int totalDelayMs = 0;
        Random r = new Random();
        int maxDelay = num;
        int iterations = 0;
        bool success;
        for (success = false; iterations < maxIterations && !success; ++iterations)
        {
          trace.TraceInfo(string.Format("Iteration {0} / {1}", (object) iterations, (object) maxIterations));
          if (iterations != 0)
          {
            int millisecondsTimeout = r.Next((int) ((double) maxDelay * splitRatio), maxDelay);
            totalDelayMs += millisecondsTimeout;
            Thread.Sleep(millisecondsTimeout);
          }
          EtagValue<IMutableVersionListWithSizeFile> versionListWithSizeFile;
          if (cacheIsEnabled && sendInTheThisObject.TryGetVersionListWithSizeDocumentCache(feedRequest.Feed.Id, out versionListWithSizeFile))
          {
            ++sendInTheThisObject.cacheHits;
            trace.TraceInfo("Got version lists doc from cache. ETag: " + (versionListWithSizeFile.Etag ?? "(null)"));
          }
          else
          {
            ++sendInTheThisObject.cacheMisses;
            versionListWithSizeFile = await sendInTheThisObject.versionListWithSizeFileProvider.GetVersionListWithSizeDocument(feedRequest);
            trace.TraceInfo("Got version lists doc from provider. ETag: " + (versionListWithSizeFile.Etag ?? "(null)"));
          }
          sendInTheThisObject.ApplyChangesToVersionListWithSizeFile(packageCommits, versionListWithSizeFile.Value, feedRequest);
          string etag;
          if (versionListWithSizeFile.Value.NeedsSave)
          {
            etag = await sendInTheThisObject.versionListWithSizeFileProvider.PutVersionListWithSizeDocument(feedRequest, (ILazyVersionListWithSizeFile) versionListWithSizeFile.Value, versionListWithSizeFile.Etag);
            trace.TraceInfo("PUT version list doc. New ETag: " + (etag ?? "(null)"));
            if (etag != null)
              success = true;
          }
          else
          {
            etag = versionListWithSizeFile.Etag;
            success = true;
            trace.TraceInfo("Doc unchanged, not PUTting. ETag still " + (etag ?? "(null)"));
          }
          if (success)
          {
            sendInTheThisObject.PutVersionListWithSizeDocumentCache(feedRequest.Feed.Id, new EtagValue<IMutableVersionListWithSizeFile>(versionListWithSizeFile.Value, etag));
            trace.TraceInfo("Updating cache with ETag " + (etag ?? "(null)"));
          }
          else
          {
            trace.TraceInfo("PUT failed, clearing cache.");
            sendInTheThisObject.ClearVersionListWithSizeDocumentCache();
          }
          maxDelay += (int) ((double) maxDelay * incrementRatio);
          versionListWithSizeFile = new EtagValue<IMutableVersionListWithSizeFile>();
        }
        TimeSpan elapsed = stopwatch.Elapsed;
        if (iterations > 10 || elapsed >= TimeSpan.FromSeconds(8.0))
          trace.TraceInfoAlways(string.Format("{0} {1} tries to save changes to {2} from {3} commits, total delay {4} ms, wall-clock time {5} ms", success ? (object) "Took" : (object) "Failed after", (object) iterations, (object) nameof (VersionListWithSizeAggregationAccessor<TPackageIdentity>), (object) packageCommits.Count, (object) totalDelayMs, (object) elapsed.TotalMilliseconds));
        trace.TraceInfo(string.Format("Stats for this {0} object: {1} apply calls, {2} cache hits, {3} cache misses", (object) nameof (VersionListWithSizeAggregationAccessor<TPackageIdentity>), (object) sendInTheThisObject.applyCalls, (object) sendInTheThisObject.cacheHits, (object) sendInTheThisObject.cacheMisses));
        if (!success)
          throw new TargetModifiedAfterReadException("Ran out of retries while updating document for VersionListWithSizeAggregationAccessor.");
        stopwatch = (Stopwatch) null;
        r = (Random) null;
      }
    }

    private void ApplyChangesToVersionListWithSizeFile(
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IMutableVersionListWithSizeFile versionListWithSizeFile,
      IFeedRequest feedRequest)
    {
      using (this.tracer.Enter((object) this, nameof (ApplyChangesToVersionListWithSizeFile)))
      {
        foreach (PackageCommit<TPackageIdentity> packageCommit in (IEnumerable<PackageCommit<TPackageIdentity>>) packageCommits)
        {
          PackageCommit<TPackageIdentity> commit = packageCommit;
          switch (commit.Commit.CommitOperationData)
          {
            case IAddOperationData addOperationData:
              if (addOperationData.PackageStorageId.IsLocal)
              {
                IReadMetadataDocumentService metadataService = AsyncPump.Run<IReadMetadataDocumentService>((Func<Task<IReadMetadataDocumentService>>) (async () => await this.GetMetadataServiceAsync(feedRequest, (IPackageIdentity) commit.PackageRequest.PackageId)));
                IPackageNameRequest<IPackageName> packageNameRequest = feedRequest.WithPackageName<IPackageName>(commit.PackageRequest.PackageId.Name);
                MetadataDocument<IMetadataEntry> metadataDocument = AsyncPump.Run<MetadataDocument<IMetadataEntry>>((Func<Task<MetadataDocument<IMetadataEntry>>>) (async () => await metadataService.GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync((IPackageNameRequest) packageNameRequest)));
                if (metadataDocument != null)
                {
                  IMetadataEntry metadataEntry = metadataDocument.Entries.Where<IMetadataEntry>((Func<IMetadataEntry, bool>) (e => e.IsLocal)).Single<IMetadataEntry>((Func<IMetadataEntry, bool>) (e => PackageVersionComparer.NormalizedVersion.Equals(commit.PackageRequest.PackageId.Version, e.PackageIdentity.Version)));
                  versionListWithSizeFile.AddPackageVersionToFeed((IPackageIdentity) commit.PackageRequest.PackageId, commit.Commit.CreatedDate, metadataEntry.PackageFiles.ToList<IPackageFile>());
                  continue;
                }
                continue;
              }
              continue;
            case IViewOperationData _:
            case IDelistOperationData _:
            case IRelistOperationData _:
            case IUpdateUpstreamMetadataVersionOperationData _:
            case IDeprecateOperationData _:
            case AddProblemPackageOperationData _:
              continue;
            case IDeleteOperationData _:
              versionListWithSizeFile.SetPackageVersionDeletedState((IPackageIdentity) commit.PackageRequest.PackageId, true, commit.Commit.CreatedDate);
              continue;
            case IRestoreToFeedOperationData _:
              versionListWithSizeFile.SetPackageVersionDeletedState((IPackageIdentity) commit.PackageRequest.PackageId, false, commit.Commit.CreatedDate);
              continue;
            case IPermanentDeleteOperationData _:
              versionListWithSizeFile.PermanentlyDeletePackageVersionFromFeed((IPackageIdentity) commit.PackageRequest.PackageId, commit.Commit.CreatedDate);
              continue;
            default:
              throw new InvalidOperationException("Unknown operation type " + commit.Commit.CommitOperationData.GetType().FullName);
          }
        }
      }
    }

    private async Task<IReadMetadataDocumentService> GetMetadataServiceAsync(
      IFeedRequest feedRequest,
      IPackageIdentity packageIdentity)
    {
      return await ProtocolRegistrar.Instance.GetBootstrappers(packageIdentity.Name.Protocol).GetReadMetadataDocumentServiceFactoryBootstrapper(this.requestContext).Bootstrap().Get(feedRequest);
    }

    private static IEnumerable<ICommitLogEntry> FlattenCommits(IEnumerable<ICommitLogEntry> input)
    {
      foreach (ICommitLogEntry commitLogEntry in input)
      {
        if (commitLogEntry.CommitOperationData is IBatchCommitOperationData commitOperationData)
        {
          foreach (ICommitOperationData operation in commitOperationData.Operations)
            yield return AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation(commitLogEntry, operation);
        }
        else
          yield return commitLogEntry;
      }
    }

    private static IEnumerable<PackageCommit<TPackageId>> FilterToPackageCommits<TPackageId>(
      IFeedRequest feedRequest,
      IEnumerable<ICommitLogEntry> input)
      where TPackageId : IPackageIdentity
    {
      foreach (ICommitLogEntry commitLogEntry in input)
      {
        if (commitLogEntry.CommitOperationData is IPackageVersionOperationData commitOperationData)
          yield return new PackageCommit<TPackageId>(feedRequest.WithPackage<TPackageId>((TPackageId) commitOperationData.Identity), commitLogEntry);
      }
    }

    public IAggregation Aggregation { get; }

    private void ClearVersionListWithSizeDocumentCache() => this.requestContextItems.Invalidate("VersionListWithSizeAggregationAccessor.Cache");

    private void PutVersionListWithSizeDocumentCache(
      Guid feedId,
      EtagValue<IMutableVersionListWithSizeFile> docAndETag)
    {
      this.requestContextItems.Set("VersionListWithSizeAggregationAccessor.Cache", (object) new VersionListWithSizeAggregationAccessor<TPackageIdentity>.CacheEntry(feedId, docAndETag));
    }

    private bool TryGetVersionListWithSizeDocumentCache(
      Guid feedId,
      out EtagValue<IMutableVersionListWithSizeFile> doc)
    {
      doc = new EtagValue<IMutableVersionListWithSizeFile>();
      object val;
      if (this.requestContextItems.TryGet("VersionListWithSizeAggregationAccessor.Cache", out val) && val is VersionListWithSizeAggregationAccessor<TPackageIdentity>.CacheEntry cacheEntry && cacheEntry.FeedId == feedId)
      {
        doc = cacheEntry.DocAndETag;
        return true;
      }
      this.ClearVersionListWithSizeDocumentCache();
      return false;
    }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<PackageCommit<TPackageIdentity>> list = VersionListWithSizeAggregationAccessor<TPackageIdentity>.FilterToPackageCommits<TPackageIdentity>(feedRequest, VersionListWithSizeAggregationAccessor<TPackageIdentity>.FlattenCommits((IEnumerable<ICommitLogEntry>) commitLogEntries)).ToList<PackageCommit<TPackageIdentity>>();
      await this.ApplyCommitsAsync(feedRequest, (IReadOnlyList<PackageCommit<TPackageIdentity>>) list);
    }

    public async Task<IEnumerable<ILazyVersionListWithSizePackage>> GetGenericVersionListWithSizeDocument(
      IFeedRequest feedRequest)
    {
      return (await this.versionListWithSizeFileProvider.GetVersionListWithSizeDocument(feedRequest)).Value.Packages;
    }

    private class CacheEntry
    {
      public Guid FeedId { get; }

      public EtagValue<IMutableVersionListWithSizeFile> DocAndETag { get; }

      public CacheEntry(
        Guid feedId,
        EtagValue<IMutableVersionListWithSizeFile> docAndETag)
      {
        this.FeedId = feedId;
        this.DocAndETag = docAndETag;
      }
    }
  }
}
