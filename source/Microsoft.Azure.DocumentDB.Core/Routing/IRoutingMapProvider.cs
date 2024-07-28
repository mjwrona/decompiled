// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.IRoutingMapProvider
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal interface IRoutingMapProvider
  {
    Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionResourceId,
      Range<string> range,
      bool forceRefresh = false);

    Task<PartitionKeyRange> TryGetPartitionKeyRangeByIdAsync(
      string collectionResourceId,
      string partitionKeyRangeId,
      bool forceRefresh = false);
  }
}
