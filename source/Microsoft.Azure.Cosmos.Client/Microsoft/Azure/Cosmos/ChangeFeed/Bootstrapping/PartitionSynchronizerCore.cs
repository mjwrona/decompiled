// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping.PartitionSynchronizerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping
{
  internal sealed class PartitionSynchronizerCore : PartitionSynchronizer
  {
    internal static int DefaultDegreeOfParallelism = 25;
    private readonly ContainerInternal container;
    private readonly DocumentServiceLeaseContainer leaseContainer;
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly int degreeOfParallelism;
    private readonly PartitionKeyRangeCache partitionKeyRangeCache;
    private readonly string containerRid;

    public PartitionSynchronizerCore(
      ContainerInternal container,
      DocumentServiceLeaseContainer leaseContainer,
      DocumentServiceLeaseManager leaseManager,
      int degreeOfParallelism,
      PartitionKeyRangeCache partitionKeyRangeCache,
      string containerRid)
    {
      this.container = container;
      this.leaseContainer = leaseContainer;
      this.leaseManager = leaseManager;
      this.degreeOfParallelism = degreeOfParallelism;
      this.partitionKeyRangeCache = partitionKeyRangeCache;
      this.containerRid = containerRid;
    }

    public override async Task CreateMissingLeasesAsync()
    {
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.partitionKeyRangeCache.TryGetOverlappingRangesAsync(this.containerRid, FeedRangeEpk.FullRange.Range, (ITrace) NoOpTrace.Singleton, false);
      DefaultTrace.TraceInformation("Source collection: '{0}', {1} partition(s)", (object) this.container.LinkUri, (object) overlappingRangesAsync.Count);
      await this.CreateLeasesAsync(overlappingRangesAsync).ConfigureAwait(false);
    }

    public override async Task<(IEnumerable<DocumentServiceLease>, bool)> HandlePartitionGoneAsync(
      DocumentServiceLease lease)
    {
      string leaseToken = lease != null ? lease.CurrentLeaseToken : throw new ArgumentNullException(nameof (lease));
      string lastContinuationToken = lease.ContinuationToken;
      DefaultTrace.TraceInformation("Lease {0} is gone due to split or merge", (object) leaseToken);
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.partitionKeyRangeCache.TryGetOverlappingRangesAsync(this.containerRid, ((FeedRangeEpk) lease.FeedRange).Range, (ITrace) NoOpTrace.Singleton, true);
      if (overlappingRangesAsync.Count == 0)
      {
        DefaultTrace.TraceError("Lease {0} is gone but we failed to find at least one child range", (object) leaseToken);
        throw new InvalidOperationException();
      }
      (IEnumerable<DocumentServiceLease>, bool) valueTuple1;
      if (lease is DocumentServiceLeaseCoreEpk feedRangeBasedLease)
        valueTuple1 = await this.HandlePartitionGoneAsync(leaseToken, lastContinuationToken, feedRangeBasedLease, overlappingRangesAsync);
      else
        valueTuple1 = await this.HandlePartitionGoneAsync(leaseToken, lastContinuationToken, (DocumentServiceLeaseCore) lease, overlappingRangesAsync);
      (IEnumerable<DocumentServiceLease>, bool) valueTuple2 = valueTuple1;
      leaseToken = (string) null;
      lastContinuationToken = (string) null;
      return valueTuple2;
    }

    private async Task<(IEnumerable<DocumentServiceLease>, bool)> HandlePartitionGoneAsync(
      string leaseToken,
      string lastContinuationToken,
      DocumentServiceLeaseCore partitionBasedLease,
      IReadOnlyList<PartitionKeyRange> overlappingRanges)
    {
      ConcurrentQueue<DocumentServiceLease> newLeases = new ConcurrentQueue<DocumentServiceLease>();
      if (overlappingRanges.Count > 1)
      {
        await overlappingRanges.ForEachAsync<PartitionKeyRange>((Func<PartitionKeyRange, Task>) (addedRange =>
        {
          DocumentServiceLease leaseIfNotExistAsync = await this.leaseManager.CreateLeaseIfNotExistAsync(addedRange, lastContinuationToken);
          if (leaseIfNotExistAsync == null)
            return;
          newLeases.Enqueue(leaseIfNotExistAsync);
        }), this.degreeOfParallelism).ConfigureAwait(false);
        DefaultTrace.TraceInformation("Lease {0} split into {1}", (object) leaseToken, (object) string.Join(", ", newLeases.Select<DocumentServiceLease, string>((Func<DocumentServiceLease, string>) (l => l.CurrentLeaseToken))));
      }
      else
      {
        PartitionKeyRange overlappingRange = overlappingRanges[0];
        DefaultTrace.TraceInformation("Lease {0} merged into {1}", (object) leaseToken, (object) overlappingRange.Id);
        DocumentServiceLease leaseIfNotExistAsync = await this.leaseManager.CreateLeaseIfNotExistAsync((FeedRangeEpk) partitionBasedLease.FeedRange, lastContinuationToken);
        if (leaseIfNotExistAsync != null)
          newLeases.Enqueue(leaseIfNotExistAsync);
      }
      return ((IEnumerable<DocumentServiceLease>) newLeases, true);
    }

    private async Task<(IEnumerable<DocumentServiceLease>, bool)> HandlePartitionGoneAsync(
      string leaseToken,
      string lastContinuationToken,
      DocumentServiceLeaseCoreEpk feedRangeBasedLease,
      IReadOnlyList<PartitionKeyRange> overlappingRanges)
    {
      List<DocumentServiceLease> newLeases = new List<DocumentServiceLease>();
      if (overlappingRanges.Count > 1)
      {
        FeedRangeEpk feedRange = (FeedRangeEpk) feedRangeBasedLease.FeedRange;
        string min = feedRange.Range.Min;
        string max = feedRange.Range.Max;
        for (int i = 0; i < overlappingRanges.Count - 1; ++i)
        {
          Range<string> partitionRange = overlappingRanges[i].ToRange();
          DocumentServiceLease leaseIfNotExistAsync = await this.leaseManager.CreateLeaseIfNotExistAsync(new FeedRangeEpk(new Range<string>(min, partitionRange.Max, true, false)), lastContinuationToken);
          if (leaseIfNotExistAsync != null)
            newLeases.Add(leaseIfNotExistAsync);
          min = partitionRange.Max;
          partitionRange = (Range<string>) null;
        }
        DocumentServiceLease leaseIfNotExistAsync1 = await this.leaseManager.CreateLeaseIfNotExistAsync(new FeedRangeEpk(new Range<string>(min, max, true, false)), lastContinuationToken);
        if (leaseIfNotExistAsync1 != null)
          newLeases.Add(leaseIfNotExistAsync1);
        DefaultTrace.TraceInformation("Lease {0} split into {1}", (object) leaseToken, (object) string.Join(", ", newLeases.Select<DocumentServiceLease, string>((Func<DocumentServiceLease, string>) (l => l.CurrentLeaseToken))));
        return ((IEnumerable<DocumentServiceLease>) newLeases, true);
      }
      newLeases.Add((DocumentServiceLease) feedRangeBasedLease);
      DefaultTrace.TraceInformation("Lease {0} redirected to {1}", (object) leaseToken, (object) overlappingRanges[0].Id);
      return ((IEnumerable<DocumentServiceLease>) newLeases, false);
    }

    private async Task CreateLeasesAsync(
      IReadOnlyList<PartitionKeyRange> partitionKeyRanges)
    {
      PartitionSynchronizerCore synchronizerCore = this;
      IReadOnlyList<DocumentServiceLease> source1 = await synchronizerCore.leaseContainer.GetAllLeasesAsync().ConfigureAwait(false);
      IReadOnlyList<PartitionKeyRange> source2 = partitionKeyRanges;
      if (source1.Count > 0)
      {
        List<string> list = source1.Where<DocumentServiceLease>((Func<DocumentServiceLease, bool>) (lease => lease is DocumentServiceLeaseCore)).Select<DocumentServiceLease, string>((Func<DocumentServiceLease, string>) (lease => lease.CurrentLeaseToken)).ToList<string>();
        List<PartitionKeyRange> partitionKeyRangeList = new List<PartitionKeyRange>();
        foreach (PartitionKeyRange partitionKeyRange in (IEnumerable<PartitionKeyRange>) partitionKeyRanges)
        {
          if (!list.Contains(partitionKeyRange.Id))
          {
            Range<string> partitionRange = partitionKeyRange.ToRange();
            if (!source1.Where<DocumentServiceLease>((Func<DocumentServiceLease, bool>) (lease =>
            {
              if (!(lease is DocumentServiceLeaseCoreEpk) || !(lease.FeedRange is FeedRangeEpk feedRange2))
                return false;
              return partitionRange.Min == feedRange2.Range.Min || partitionRange.Max == feedRange2.Range.Max;
            })).Any<DocumentServiceLease>())
              partitionKeyRangeList.Add(partitionKeyRange);
          }
        }
        source2 = (IReadOnlyList<PartitionKeyRange>) partitionKeyRangeList;
      }
      // ISSUE: reference to a compiler-generated method
      await source2.ForEachAsync<PartitionKeyRange>(new Func<PartitionKeyRange, Task>(synchronizerCore.\u003CCreateLeasesAsync\u003Eb__12_0), synchronizerCore.degreeOfParallelism).ConfigureAwait(false);
    }
  }
}
