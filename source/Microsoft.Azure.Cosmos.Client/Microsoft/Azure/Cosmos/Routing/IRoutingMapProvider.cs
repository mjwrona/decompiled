// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.IRoutingMapProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal interface IRoutingMapProvider
  {
    Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionResourceId,
      Range<string> range,
      ITrace trace,
      bool forceRefresh = false);

    Task<PartitionKeyRange> TryGetPartitionKeyRangeByIdAsync(
      string collectionResourceId,
      string partitionKeyRangeId,
      ITrace trace,
      bool forceRefresh = false);
  }
}
