// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.DefaultSizeBasedShardAllocator
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class DefaultSizeBasedShardAllocator
  {
    public readonly int MaxNumberOfShardsToUse;

    internal DefaultSizeBasedShardAllocator(int maxNumberOfShardsToUse) => this.MaxNumberOfShardsToUse = maxNumberOfShardsToUse > 0 ? maxNumberOfShardsToUse : throw new ArgumentException("Expected positive integer.", nameof (maxNumberOfShardsToUse));

    public virtual IList<ShardAssignmentDetails> ProvisionShards(
      IndexingExecutionContext indexingExecutionContext,
      IList<ShardAssignmentDetails> availableShards,
      IList<IndexingUnitWithSize> indexingUnitsWithSizes)
    {
      if (availableShards == null || availableShards.Count <= 0)
        throw new ArgumentException("Null or empty list is an invalid value of.", nameof (availableShards));
      if (availableShards.Count < this.MaxNumberOfShardsToUse)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Available Shards can not be less than Max Shards. Available Shards : {0}, Max Shards: {1}", (object) availableShards.Count, (object) this.MaxNumberOfShardsToUse)));
      if (indexingUnitsWithSizes == null || indexingUnitsWithSizes.Count <= 0)
        return (IList<ShardAssignmentDetails>) new List<ShardAssignmentDetails>();
      HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitSet = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      bool useReservedSpace = true;
      foreach (IndexingUnitWithSize indexingUnitsWithSiz in (IEnumerable<IndexingUnitWithSize>) indexingUnitsWithSizes)
      {
        indexingUnitSet.Add(indexingUnitsWithSiz.IndexingUnit);
        if (indexingUnitSet.Count > 1)
        {
          useReservedSpace = false;
          break;
        }
      }
      IList<ShardAssignmentDetails> shardAssignmentDetails = (IList<ShardAssignmentDetails>) new List<ShardAssignmentDetails>();
      availableShards.ForEach<ShardAssignmentDetails>((Action<ShardAssignmentDetails>) (x => shardAssignmentDetails.Add(new ShardAssignmentDetails(x))));
      return this.ProvisionShards(indexingExecutionContext, shardAssignmentDetails, indexingUnitsWithSizes, useReservedSpace);
    }

    public ShardAssignmentDetails ProvisionShard(
      IndexingExecutionContext indexingExecutionContext,
      List<ShardAssignmentDetails> availableShards,
      IndexingUnitWithSize indexingUnitWithSize)
    {
      if (indexingUnitWithSize == null)
        throw new ArgumentNullException(nameof (indexingUnitWithSize));
      return this.ProvisionShards(indexingExecutionContext, (IList<ShardAssignmentDetails>) availableShards, (IList<IndexingUnitWithSize>) new List<IndexingUnitWithSize>()
      {
        indexingUnitWithSize
      }).First<ShardAssignmentDetails>();
    }

    internal virtual ShardAssignmentDetails AssignBestFitShardIfPossible(
      IndexingExecutionContext indexingExecutionContext,
      IList<ShardAssignmentDetails> availableShards,
      IList<IndexingUnitWithSize> indexingUnitsWithSizes,
      bool useReservedSpaceIfNeeded,
      out long currentEstimatedDocCountOfAllIndexingUnits,
      out long estimatedGrowthOfAllIndexingUnits)
    {
      ShardAssignmentDetails shardAssignmentDetails = (ShardAssignmentDetails) null;
      currentEstimatedDocCountOfAllIndexingUnits = 0L;
      estimatedGrowthOfAllIndexingUnits = 0L;
      HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitSet = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (IndexingUnitWithSize indexingUnitsWithSiz in (IEnumerable<IndexingUnitWithSize>) indexingUnitsWithSizes)
      {
        if (!indexingUnitSet.Add(indexingUnitsWithSiz.IndexingUnit))
        {
          currentEstimatedDocCountOfAllIndexingUnits = -1L;
          estimatedGrowthOfAllIndexingUnits = -1L;
          return (ShardAssignmentDetails) null;
        }
        currentEstimatedDocCountOfAllIndexingUnits += (long) indexingUnitsWithSiz.CurrentEstimatedDocumentCount;
        estimatedGrowthOfAllIndexingUnits += (long) indexingUnitsWithSiz.EstimatedGrowth;
      }
      int num1 = int.MaxValue;
      int num2 = int.MaxValue;
      int num3 = int.MaxValue;
      int num4 = int.MaxValue;
      ShardAssignmentDetails assignmentDetails1 = (ShardAssignmentDetails) null;
      ShardAssignmentDetails assignmentDetails2 = (ShardAssignmentDetails) null;
      ShardAssignmentDetails assignmentDetails3 = (ShardAssignmentDetails) null;
      ShardAssignmentDetails assignmentDetails4 = (ShardAssignmentDetails) null;
      long num5 = checked (currentEstimatedDocCountOfAllIndexingUnits + estimatedGrowthOfAllIndexingUnits);
      if (num5 < (long) int.MaxValue)
      {
        HashSet<int> intSet = new HashSet<int>();
        foreach (ShardAssignmentDetails availableShard in (IEnumerable<ShardAssignmentDetails>) availableShards)
        {
          if (availableShard.IndexingUnits != null)
          {
            intSet.Add(availableShard.ShardId);
            if ((long) availableShard.FreeSpaceAvailable >= num5 && availableShard.FreeSpaceAvailable < num1)
            {
              num1 = availableShard.FreeSpaceAvailable;
              assignmentDetails1 = availableShard;
            }
            if (useReservedSpaceIfNeeded)
            {
              int num6 = availableShard.ReservedSpaceForShardGrowth + availableShard.FreeSpaceAvailable;
              if ((long) num6 >= num5 && num6 < num2)
              {
                num2 = num6;
                assignmentDetails2 = availableShard;
              }
            }
          }
          if ((long) availableShard.FreeSpaceAvailable >= num5 && availableShard.FreeSpaceAvailable < num3)
          {
            num3 = availableShard.FreeSpaceAvailable;
            assignmentDetails3 = availableShard;
          }
          if (useReservedSpaceIfNeeded)
          {
            int num7 = availableShard.ReservedSpaceForShardGrowth + availableShard.FreeSpaceAvailable;
            if ((long) num7 >= num5 && num7 < num4)
            {
              num4 = num7;
              assignmentDetails4 = availableShard;
            }
          }
        }
        if (assignmentDetails1 != null)
        {
          shardAssignmentDetails = assignmentDetails1;
          this.AssignIndexingUnitsToShard(shardAssignmentDetails, indexingUnitsWithSizes, currentEstimatedDocCountOfAllIndexingUnits, estimatedGrowthOfAllIndexingUnits);
        }
        else if (assignmentDetails2 != null)
        {
          shardAssignmentDetails = assignmentDetails2;
          this.AssignIndexingUnitsToShard(shardAssignmentDetails, indexingUnitsWithSizes, currentEstimatedDocCountOfAllIndexingUnits, estimatedGrowthOfAllIndexingUnits);
        }
        else if (assignmentDetails3 != null)
        {
          shardAssignmentDetails = assignmentDetails3;
          this.AssignIndexingUnitsToShard(shardAssignmentDetails, indexingUnitsWithSizes, currentEstimatedDocCountOfAllIndexingUnits, estimatedGrowthOfAllIndexingUnits);
        }
        else if (assignmentDetails4 != null)
        {
          shardAssignmentDetails = assignmentDetails4;
          this.AssignIndexingUnitsToShard(shardAssignmentDetails, indexingUnitsWithSizes, currentEstimatedDocCountOfAllIndexingUnits, estimatedGrowthOfAllIndexingUnits);
        }
      }
      return shardAssignmentDetails;
    }

    internal virtual List<ShardAssignmentDetails> AssignMultipleShards(
      IndexingExecutionContext indexingExecutionContext,
      IList<ShardAssignmentDetails> availableShards,
      IList<IndexingUnitWithSize> indexingUnitsWithSizes,
      bool useReservedSpaceIfNeeded,
      out long currentEstimatedSizeOfAllIndexingUnits,
      out long estimatedGrowthOfAllIndexingUnits)
    {
      List<ShardAssignmentDetails> assignmentDetailsList = !useReservedSpaceIfNeeded ? availableShards.OrderByDescending<ShardAssignmentDetails, int>((Func<ShardAssignmentDetails, int>) (x => x.FreeSpaceAvailable)).ToList<ShardAssignmentDetails>() : availableShards.OrderByDescending<ShardAssignmentDetails, int>((Func<ShardAssignmentDetails, int>) (x => x.FreeSpaceAvailable + x.ReservedSpaceForShardGrowth)).ToList<ShardAssignmentDetails>();
      List<IndexingUnitWithSize> list = indexingUnitsWithSizes.OrderByDescending<IndexingUnitWithSize, int>((Func<IndexingUnitWithSize, int>) (x => x.TotalSize)).ToList<IndexingUnitWithSize>();
      int num = 0;
      List<ShardAssignmentDetails> source = new List<ShardAssignmentDetails>();
      currentEstimatedSizeOfAllIndexingUnits = estimatedGrowthOfAllIndexingUnits = 0L;
      foreach (IndexingUnitWithSize indexingUnitWithSize in list)
      {
        currentEstimatedSizeOfAllIndexingUnits += (long) indexingUnitWithSize.CurrentEstimatedDocumentCount;
        estimatedGrowthOfAllIndexingUnits += (long) indexingUnitWithSize.EstimatedGrowth;
        bool flag = false;
        foreach (ShardAssignmentDetails assignmentDetails in source)
        {
          if (this.CanAssign(assignmentDetails, indexingUnitWithSize, useReservedSpaceIfNeeded))
          {
            flag = true;
            this.AssignIndexingUnitToShard(assignmentDetails, indexingUnitWithSize);
            break;
          }
        }
        if (!flag)
        {
          if (source.Count < this.MaxNumberOfShardsToUse)
          {
            ShardAssignmentDetails shardAssignmentDetails = assignmentDetailsList[num++];
            this.AssignIndexingUnitToShard(shardAssignmentDetails, indexingUnitWithSize);
            source.Add(shardAssignmentDetails);
          }
          else
          {
            this.AssignIndexingUnitToShard(source.OrderByDescending<ShardAssignmentDetails, int>((Func<ShardAssignmentDetails, int>) (x => x.FreeSpaceAvailable)).First<ShardAssignmentDetails>(), indexingUnitWithSize);
            Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", "ShardAllocationNumberOfMaxShardsAllottedLimitMet", (object) true);
          }
        }
      }
      return source;
    }

    internal bool CanAssign(
      ShardAssignmentDetails shard,
      IndexingUnitWithSize indexingUnitWithSize,
      bool useReservedSpaceIfNeeded)
    {
      return (indexingUnitWithSize.TotalSize <= shard.FreeSpaceAvailable || useReservedSpaceIfNeeded && indexingUnitWithSize.TotalSize <= shard.FreeSpaceAvailable + shard.ReservedSpaceForShardGrowth) && shard.IndexingUnits != null && shard.IndexingUnits.All<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => !x.Equals((object) indexingUnitWithSize.IndexingUnit)));
    }

    internal void AssignIndexingUnitsToShard(
      ShardAssignmentDetails shardAssignmentDetails,
      IList<IndexingUnitWithSize> indexingUnitsWithSize,
      long currentEstimatedSizeOfAllIndexingUnits,
      long estimatedGrowthOfAllIndexingUnits)
    {
      if (shardAssignmentDetails.IndexingUnits == null)
        shardAssignmentDetails.IndexingUnits = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      int num = (int) (currentEstimatedSizeOfAllIndexingUnits + estimatedGrowthOfAllIndexingUnits);
      if (shardAssignmentDetails.FreeSpaceAvailable < num)
      {
        if (shardAssignmentDetails.FreeSpaceAvailable + shardAssignmentDetails.ReservedSpaceForShardGrowth > num)
        {
          int freeSpaceAvailable = shardAssignmentDetails.FreeSpaceAvailable;
          shardAssignmentDetails.ReservedSpaceForShardGrowth -= (int) (currentEstimatedSizeOfAllIndexingUnits + estimatedGrowthOfAllIndexingUnits - (long) freeSpaceAvailable);
        }
        else
          shardAssignmentDetails.ReservedSpaceForShardGrowth = 0;
      }
      shardAssignmentDetails.IndexingUnits.AddRange<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>(indexingUnitsWithSize.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)));
      shardAssignmentDetails.EstimatedDocCountGrowth += (int) estimatedGrowthOfAllIndexingUnits;
      shardAssignmentDetails.CurrentEstimatedDocumentCount += (int) currentEstimatedSizeOfAllIndexingUnits;
    }

    internal void AssignIndexingUnitToShard(
      ShardAssignmentDetails shardAssignmentDetails,
      IndexingUnitWithSize indexingUnitWithSize)
    {
      ShardAssignmentDetails shardAssignmentDetails1 = shardAssignmentDetails;
      List<IndexingUnitWithSize> indexingUnitsWithSize = new List<IndexingUnitWithSize>();
      indexingUnitsWithSize.Add(indexingUnitWithSize);
      long estimatedDocumentCount = (long) indexingUnitWithSize.CurrentEstimatedDocumentCount;
      long estimatedGrowth = (long) indexingUnitWithSize.EstimatedGrowth;
      this.AssignIndexingUnitsToShard(shardAssignmentDetails1, (IList<IndexingUnitWithSize>) indexingUnitsWithSize, estimatedDocumentCount, estimatedGrowth);
    }

    internal virtual IList<ShardAssignmentDetails> ProvisionShards(
      IndexingExecutionContext indexingExecutionContext,
      IList<ShardAssignmentDetails> availableShards,
      IList<IndexingUnitWithSize> indexingUnitsWithSize,
      bool useReservedSpace)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<ShardAssignmentDetails> assignmentDetailsList = new List<ShardAssignmentDetails>();
      long num;
      long estimatedGrowthOfAllIndexingUnits;
      ShardAssignmentDetails assignmentDetails = this.AssignBestFitShardIfPossible(indexingExecutionContext, availableShards, indexingUnitsWithSize, useReservedSpace, out num, out estimatedGrowthOfAllIndexingUnits);
      if (assignmentDetails != null)
        assignmentDetailsList.Add(assignmentDetails);
      else
        assignmentDetailsList.AddRange((IEnumerable<ShardAssignmentDetails>) this.AssignMultipleShards(indexingExecutionContext, availableShards, indexingUnitsWithSize, useReservedSpace, out num, out estimatedGrowthOfAllIndexingUnits));
      stopwatch.Stop();
      IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>();
      properties.Add("ShardAllocationCurrentEstimatedSize", (object) num);
      properties.Add("ShardAllocationReservedEstimatedSize", (object) estimatedGrowthOfAllIndexingUnits);
      properties.Add("ShardAllocationTotalEstimatedSize", (object) (num + estimatedGrowthOfAllIndexingUnits));
      properties.Add("ShardAllocationTotalIndexingUnits", (object) indexingUnitsWithSize.Count);
      properties.Add("ShardAllocationTotalShardsAllocated", (object) assignmentDetailsList.Count);
      properties.Add("ShardAllocationTotalTimeTakenInMillis", (object) stopwatch.ElapsedMilliseconds);
      Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", properties, true);
      return (IList<ShardAssignmentDetails>) assignmentDetailsList;
    }
  }
}
