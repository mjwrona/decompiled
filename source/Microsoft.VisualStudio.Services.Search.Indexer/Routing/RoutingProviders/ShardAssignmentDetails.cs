// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.ShardAssignmentDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class ShardAssignmentDetails
  {
    internal ShardAssignmentDetails(
      int shardId,
      int currentEstimatedDocumentCount,
      int estimatedDocCountGrowth,
      int reservedSpaceForShardGrowth,
      int maxShardSize,
      HashSet<IndexingUnit> indexingUnits)
    {
      this.ShardId = shardId;
      this.CurrentEstimatedDocumentCount = currentEstimatedDocumentCount;
      this.EstimatedDocCountGrowth = estimatedDocCountGrowth;
      this.ReservedSpaceForShardGrowth = reservedSpaceForShardGrowth;
      this.MaxShardSize = maxShardSize;
      this.IndexingUnits = indexingUnits;
    }

    internal ShardAssignmentDetails(ShardAssignmentDetails shardAssignmentDetails)
      : this(shardAssignmentDetails.ShardId, shardAssignmentDetails.CurrentEstimatedDocumentCount, shardAssignmentDetails.EstimatedDocCountGrowth, shardAssignmentDetails.ReservedSpaceForShardGrowth, shardAssignmentDetails.MaxShardSize, shardAssignmentDetails.IndexingUnits)
    {
    }

    public int ShardId { get; set; }

    public int CurrentEstimatedDocumentCount { get; set; }

    public int EstimatedDocCountGrowth { get; set; }

    public int ReservedSpaceForShardGrowth { get; set; }

    public int FreeSpaceAvailable => this.MaxShardSize - (this.CurrentEstimatedDocumentCount + this.EstimatedDocCountGrowth + this.ReservedSpaceForShardGrowth);

    public int MaxShardSize { get; }

    public HashSet<IndexingUnit> IndexingUnits { get; set; }

    public override int GetHashCode() => this.ShardId.GetHashCode();

    public override string ToString() => this.ShardId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
