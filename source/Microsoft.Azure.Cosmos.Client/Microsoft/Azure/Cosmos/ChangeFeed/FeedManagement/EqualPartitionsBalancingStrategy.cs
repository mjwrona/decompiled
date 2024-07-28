// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.EqualPartitionsBalancingStrategy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class EqualPartitionsBalancingStrategy : LoadBalancingStrategy
  {
    internal static int DefaultMinLeaseCount;
    internal static int DefaultMaxLeaseCount;
    private readonly string hostName;
    private readonly int minPartitionCount;
    private readonly int maxPartitionCount;
    private readonly TimeSpan leaseExpirationInterval;

    public EqualPartitionsBalancingStrategy(
      string hostName,
      int minPartitionCount,
      int maxPartitionCount,
      TimeSpan leaseExpirationInterval)
    {
      this.hostName = hostName != null ? hostName : throw new ArgumentNullException(nameof (hostName));
      this.minPartitionCount = minPartitionCount;
      this.maxPartitionCount = maxPartitionCount;
      this.leaseExpirationInterval = leaseExpirationInterval;
    }

    public override IEnumerable<DocumentServiceLease> SelectLeasesToTake(
      IEnumerable<DocumentServiceLease> allLeases)
    {
      Dictionary<string, int> workerToPartitionCount = new Dictionary<string, int>();
      List<DocumentServiceLease> documentServiceLeaseList = new List<DocumentServiceLease>();
      Dictionary<string, DocumentServiceLease> allPartitions = new Dictionary<string, DocumentServiceLease>();
      this.CategorizeLeases(allLeases, allPartitions, documentServiceLeaseList, workerToPartitionCount);
      int count1 = allPartitions.Count;
      int count2 = workerToPartitionCount.Count;
      if (count1 <= 0)
        return Enumerable.Empty<DocumentServiceLease>();
      int targetPartitionCount = this.CalculateTargetPartitionCount(count1, count2);
      int num1 = workerToPartitionCount[this.hostName];
      int num2 = targetPartitionCount - num1;
      DefaultTrace.TraceInformation("Host '{0}' {1} partitions, {2} hosts, {3} available leases, target = {4}, min = {5}, max = {6}, mine = {7}, will try to take {8} lease(s) for myself'.", (object) this.hostName, (object) count1, (object) count2, (object) documentServiceLeaseList.Count, (object) targetPartitionCount, (object) this.minPartitionCount, (object) this.maxPartitionCount, (object) num1, (object) Math.Max(num2, 0));
      if (num2 <= 0)
        return Enumerable.Empty<DocumentServiceLease>();
      if (documentServiceLeaseList.Count > 0)
        return documentServiceLeaseList.Take<DocumentServiceLease>(num2);
      DocumentServiceLease leaseToSteal = EqualPartitionsBalancingStrategy.GetLeaseToSteal(workerToPartitionCount, targetPartitionCount, num2, allPartitions);
      if (leaseToSteal == null)
        return Enumerable.Empty<DocumentServiceLease>();
      return (IEnumerable<DocumentServiceLease>) new DocumentServiceLease[1]
      {
        leaseToSteal
      };
    }

    private static DocumentServiceLease GetLeaseToSteal(
      Dictionary<string, int> workerToPartitionCount,
      int target,
      int partitionsNeededForMe,
      Dictionary<string, DocumentServiceLease> allPartitions)
    {
      KeyValuePair<string, int> workerToStealFrom = EqualPartitionsBalancingStrategy.FindWorkerWithMostPartitions(workerToPartitionCount);
      return workerToStealFrom.Value > target - (partitionsNeededForMe > 1 ? 1 : 0) ? allPartitions.Values.First<DocumentServiceLease>((Func<DocumentServiceLease, bool>) (partition => string.Equals(partition.Owner, workerToStealFrom.Key, StringComparison.OrdinalIgnoreCase))) : (DocumentServiceLease) null;
    }

    private static KeyValuePair<string, int> FindWorkerWithMostPartitions(
      Dictionary<string, int> workerToPartitionCount)
    {
      KeyValuePair<string, int> withMostPartitions = new KeyValuePair<string, int>();
      foreach (KeyValuePair<string, int> keyValuePair in workerToPartitionCount)
      {
        if (withMostPartitions.Value <= keyValuePair.Value)
          withMostPartitions = keyValuePair;
      }
      return withMostPartitions;
    }

    private int CalculateTargetPartitionCount(int partitionCount, int workerCount)
    {
      int targetPartitionCount = 1;
      if (partitionCount > workerCount)
        targetPartitionCount = (int) Math.Ceiling((double) partitionCount / (double) workerCount);
      if (this.maxPartitionCount > 0 && targetPartitionCount > this.maxPartitionCount)
        targetPartitionCount = this.maxPartitionCount;
      if (this.minPartitionCount > 0 && targetPartitionCount < this.minPartitionCount)
        targetPartitionCount = this.minPartitionCount;
      return targetPartitionCount;
    }

    private void CategorizeLeases(
      IEnumerable<DocumentServiceLease> allLeases,
      Dictionary<string, DocumentServiceLease> allPartitions,
      List<DocumentServiceLease> expiredLeases,
      Dictionary<string, int> workerToPartitionCount)
    {
      foreach (DocumentServiceLease allLease in allLeases)
      {
        allPartitions.Add(allLease.CurrentLeaseToken, allLease);
        if (string.IsNullOrWhiteSpace(allLease.Owner) || this.IsExpired(allLease))
        {
          DefaultTrace.TraceVerbose("Found unused or expired lease: {0}", (object) allLease);
          expiredLeases.Add(allLease);
        }
        else
        {
          string owner = allLease.Owner;
          int num;
          if (workerToPartitionCount.TryGetValue(owner, out num))
            workerToPartitionCount[owner] = num + 1;
          else
            workerToPartitionCount.Add(owner, 1);
        }
      }
      if (workerToPartitionCount.ContainsKey(this.hostName))
        return;
      workerToPartitionCount.Add(this.hostName, 0);
    }

    private bool IsExpired(DocumentServiceLease lease) => lease.Timestamp.ToUniversalTime() + this.leaseExpirationInterval < DateTime.UtcNow;
  }
}
