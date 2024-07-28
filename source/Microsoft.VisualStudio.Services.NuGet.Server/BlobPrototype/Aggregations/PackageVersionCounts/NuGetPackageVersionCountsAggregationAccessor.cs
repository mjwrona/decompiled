// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.NuGetPackageVersionCountsAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class NuGetPackageVersionCountsAggregationAccessor : 
    INuGetPackageVersionCountsAggregationAccessor,
    INuGetPackageVersionCountsService,
    INuGetNamesService,
    IPackageNamesService<VssNuGetPackageName, VssNuGetPackageIdentity>,
    INuGetPackageVersionCountsDocumentBytesService,
    IAggregationAccessor<NuGetPackageVersionCountsAggregation>,
    IAggregationAccessor
  {
    private const string CacheRequestContextItemsKey = "NuGetPackageVersionCountsAggregationAccessor.Cache";
    private readonly IConverter<IFeedRequest, Guid> requestViewExtractor;
    private readonly IVersionListsFileProvider versionListsFileProvider;
    private readonly ITracerService tracer;
    private readonly IRetryCountProvider retryCountProvider;
    private readonly IFeatureFlagService featureFlagService;
    private readonly ICache<string, object> requestContextItems;
    private int cacheHits;
    private int cacheMisses;
    private int applyCalls;
    private readonly IVersionCountsFromFileProvider versionCountsFromFileProvider;

    public NuGetPackageVersionCountsAggregationAccessor(
      IAggregation aggregation,
      IConverter<IFeedRequest, Guid> requestViewExtractor,
      IVersionListsFileProvider versionListsFileProvider,
      ITracerService tracer,
      IRetryCountProvider retryCountProvider,
      IFeatureFlagService featureFlagService,
      ICache<string, object> requestContextItems,
      IVersionCountsFromFileProvider countsFromFileProvider)
    {
      this.Aggregation = aggregation;
      this.versionListsFileProvider = versionListsFileProvider;
      this.tracer = tracer;
      this.retryCountProvider = retryCountProvider;
      this.featureFlagService = featureFlagService;
      this.requestContextItems = requestContextItems;
      this.requestViewExtractor = requestViewExtractor;
      this.versionCountsFromFileProvider = countsFromFileProvider;
    }

    public async Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> GetPackageNamesAsync(
      IFeedRequest feedRequest)
    {
      IMutableVersionListsFile versionListsFile1;
      (await this.versionListsFileProvider.GetVersionListDocument(feedRequest)).Deconstruct<IMutableVersionListsFile>(out versionListsFile1, out string _);
      IMutableVersionListsFile versionListsFile2 = versionListsFile1;
      Guid viewId = this.requestViewExtractor.Convert(feedRequest);
      return (IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>) (!(viewId == Guid.Empty) ? versionListsFile2.Packages.Select<ILazyVersionListsPackage, IVersionListsPackage>((Func<ILazyVersionListsPackage, IVersionListsPackage>) (lazyPackage => lazyPackage.Get())).Where<IVersionListsPackage>((Func<IVersionListsPackage, bool>) (package => package.Versions.Any<IVersionListsPackageVersion>((Func<IVersionListsPackageVersion, bool>) (versionEntry => versionEntry.ViewIds.Contains<Guid>(viewId))))).Cast<IPackageNameEntry<VssNuGetPackageName>>() : (IEnumerable<IPackageNameEntry<VssNuGetPackageName>>) versionListsFile2.Packages).ToImmutableList<IPackageNameEntry<VssNuGetPackageName>>();
    }

    public async Task ApplyCommitsAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<PackageCommit<VssNuGetPackageIdentity>> packageCommits)
    {
      NuGetPackageVersionCountsAggregationAccessor sendInTheThisObject = this;
      ++sendInTheThisObject.applyCalls;
      using (ITracerBlock trace = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (ApplyCommitsAsync)))
      {
        if (!packageCommits.Any<PackageCommit<VssNuGetPackageIdentity>>())
          return;
        bool cacheIsEnabled = sendInTheThisObject.featureFlagService.IsEnabled("NuGet.CacheVersionListFileBetweenCommits");
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
          EtagValue<IMutableVersionListsFile> versionListsFile;
          if (cacheIsEnabled && sendInTheThisObject.TryGetVersionListDocumentCache(feedRequest.Feed.Id, out versionListsFile))
          {
            ++sendInTheThisObject.cacheHits;
            trace.TraceInfo("Got version lists doc from cache. ETag: " + (versionListsFile.Etag ?? "(null)"));
          }
          else
          {
            ++sendInTheThisObject.cacheMisses;
            versionListsFile = await sendInTheThisObject.versionListsFileProvider.GetVersionListDocument(feedRequest);
            trace.TraceInfo("Got version lists doc from provider. ETag: " + (versionListsFile.Etag ?? "(null)"));
          }
          sendInTheThisObject.ApplyChangesToVersionListsFile(packageCommits, versionListsFile.Value);
          string etag;
          if (versionListsFile.Value.NeedsSave)
          {
            etag = await sendInTheThisObject.versionListsFileProvider.PutVersionListDocument(feedRequest, (ILazyVersionListsFile) versionListsFile.Value, versionListsFile.Etag);
            trace.TraceInfo("PUT version list doc. New ETag: " + (etag ?? "(null)"));
            if (etag != null)
              success = true;
          }
          else
          {
            etag = versionListsFile.Etag;
            success = true;
            trace.TraceInfo("Doc unchanged, not PUTting. ETag still " + (etag ?? "(null)"));
          }
          if (success)
          {
            sendInTheThisObject.PutVersionListDocumentCache(feedRequest.Feed.Id, new EtagValue<IMutableVersionListsFile>(versionListsFile.Value, etag));
            trace.TraceInfo("Updating cache with ETag " + (etag ?? "(null)"));
          }
          else
          {
            trace.TraceInfo("PUT failed, clearing cache.");
            sendInTheThisObject.ClearVersionListDocumentCache();
          }
          maxDelay += (int) ((double) maxDelay * incrementRatio);
          versionListsFile = new EtagValue<IMutableVersionListsFile>();
        }
        TimeSpan elapsed = stopwatch.Elapsed;
        if (iterations > 10 || elapsed >= TimeSpan.FromSeconds(8.0))
          trace.TraceInfoAlways(string.Format("{0} {1} tries to save changes to {2} from {3} commits, total delay {4} ms, wall-clock time {5} ms", success ? (object) "Took" : (object) "Failed after", (object) iterations, (object) nameof (NuGetPackageVersionCountsAggregationAccessor), (object) packageCommits.Count, (object) totalDelayMs, (object) elapsed.TotalMilliseconds));
        trace.TraceInfo(string.Format("Stats for this {0} object: {1} apply calls, {2} cache hits, {3} cache misses", (object) nameof (NuGetPackageVersionCountsAggregationAccessor), (object) sendInTheThisObject.applyCalls, (object) sendInTheThisObject.cacheHits, (object) sendInTheThisObject.cacheMisses));
        if (!success)
          throw new TargetModifiedAfterReadException("Ran out of retries while updating document for NuGetPackageVersionCountsAggregationAccessor.");
        stopwatch = (Stopwatch) null;
        r = (Random) null;
      }
    }

    private void ApplyChangesToVersionListsFile(
      IReadOnlyList<PackageCommit<VssNuGetPackageIdentity>> packageCommits,
      IMutableVersionListsFile versionListsFile)
    {
      using (this.tracer.Enter((object) this, nameof (ApplyChangesToVersionListsFile)))
      {
        foreach (PackageCommit<VssNuGetPackageIdentity> packageCommit in (IEnumerable<PackageCommit<VssNuGetPackageIdentity>>) packageCommits)
        {
          switch (packageCommit.Commit.CommitOperationData)
          {
            case IAddOperationData addOperationData:
              if (addOperationData.PackageStorageId.IsLocal)
              {
                versionListsFile.AddPackageVersionToFeed(packageCommit.PackageRequest.PackageId, packageCommit.Commit.CreatedDate);
                continue;
              }
              continue;
            case IViewOperationData viewOperationData:
              if (viewOperationData.MetadataSuboperation != MetadataSuboperation.Add)
                throw new InvalidOperationException(string.Format("Unknown view operation {0}", (object) viewOperationData));
              versionListsFile.AddPackageVersionToView(packageCommit.PackageRequest.PackageId, viewOperationData.ViewId, packageCommit.Commit.CreatedDate);
              continue;
            case IDelistOperationData _:
              versionListsFile.SetPackageVersionListedState(packageCommit.PackageRequest.PackageId, false, packageCommit.Commit.CreatedDate);
              continue;
            case IRelistOperationData _:
              versionListsFile.SetPackageVersionListedState(packageCommit.PackageRequest.PackageId, true, packageCommit.Commit.CreatedDate);
              continue;
            case IDeleteOperationData _:
              versionListsFile.SetPackageVersionDeletedState(packageCommit.PackageRequest.PackageId, true, packageCommit.Commit.CreatedDate);
              continue;
            case IRestoreToFeedOperationData _:
              versionListsFile.SetPackageVersionDeletedState(packageCommit.PackageRequest.PackageId, false, packageCommit.Commit.CreatedDate);
              continue;
            case IPermanentDeleteOperationData _:
              versionListsFile.PermanentlyDeletePackageVersionFromFeed(packageCommit.PackageRequest.PackageId, packageCommit.Commit.CreatedDate);
              continue;
            case IUpdateUpstreamMetadataVersionOperationData _:
            case AddProblemPackageOperationData _:
              continue;
            default:
              throw new InvalidOperationException("Unknown operation type " + packageCommit.Commit.CommitOperationData.GetType().FullName);
          }
        }
      }
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

    private void ClearVersionListDocumentCache() => this.requestContextItems.Invalidate("NuGetPackageVersionCountsAggregationAccessor.Cache");

    private void PutVersionListDocumentCache(
      Guid feedId,
      EtagValue<IMutableVersionListsFile> docAndETag)
    {
      this.requestContextItems.Set("NuGetPackageVersionCountsAggregationAccessor.Cache", (object) new NuGetPackageVersionCountsAggregationAccessor.CacheEntry(feedId, docAndETag));
    }

    private bool TryGetVersionListDocumentCache(
      Guid feedId,
      out EtagValue<IMutableVersionListsFile> doc)
    {
      doc = new EtagValue<IMutableVersionListsFile>();
      object val;
      if (this.requestContextItems.TryGet("NuGetPackageVersionCountsAggregationAccessor.Cache", out val) && val is NuGetPackageVersionCountsAggregationAccessor.CacheEntry cacheEntry && cacheEntry.FeedId == feedId)
      {
        doc = cacheEntry.DocAndETag;
        return true;
      }
      this.ClearVersionListDocumentCache();
      return false;
    }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<PackageCommit<VssNuGetPackageIdentity>> list = NuGetPackageVersionCountsAggregationAccessor.FilterToPackageCommits<VssNuGetPackageIdentity>(feedRequest, NuGetPackageVersionCountsAggregationAccessor.FlattenCommits((IEnumerable<ICommitLogEntry>) commitLogEntries)).ToList<PackageCommit<VssNuGetPackageIdentity>>();
      await this.ApplyCommitsAsync(feedRequest, (IReadOnlyList<PackageCommit<VssNuGetPackageIdentity>>) list);
    }

    public async Task<GetVersionCountsResult> GetVersionCounts(
      IFeedRequest feedRequest,
      NuGetSearchCategoryToggles query)
    {
      Guid viewId = this.requestViewExtractor.Convert(feedRequest);
      IMutableVersionListsFile versionListsFile;
      (await this.versionListsFileProvider.GetVersionListDocument(feedRequest)).Deconstruct<IMutableVersionListsFile>(out versionListsFile, out string _);
      IMutableVersionListsFile metrics = versionListsFile;
      return new GetVersionCountsResult(this.versionCountsFromFileProvider.GetVersionCountsFromVersionListFile((ILazyVersionListsFile) metrics, query, viewId, true), (IVersionCountsImplementationMetrics) metrics);
    }

    public async Task<byte[]> GetVersionListDocumentBytes(IFeedRequest feedRequest)
    {
      byte[] listDocumentBytes;
      (await this.versionListsFileProvider.GetVersionListDocumentBytes(feedRequest)).Deconstruct<byte[]>(out listDocumentBytes, out string _);
      return listDocumentBytes;
    }

    private class CacheEntry
    {
      public Guid FeedId { get; }

      public EtagValue<IMutableVersionListsFile> DocAndETag { get; }

      public CacheEntry(Guid feedId, EtagValue<IMutableVersionListsFile> docAndETag)
      {
        this.FeedId = feedId;
        this.DocAndETag = docAndETag;
      }
    }
  }
}
