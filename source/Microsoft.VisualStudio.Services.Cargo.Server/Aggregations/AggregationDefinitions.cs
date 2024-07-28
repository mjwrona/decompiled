// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.AggregationDefinitions
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  [ExcludeFromCodeCoverage]
  public static class AggregationDefinitions
  {
    public static readonly AggregationDefinition CargoPackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageMetadata",
      Protocol = (IProtocol) Protocol.Cargo,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };
    public static readonly AggregationDefinition CargoFeedIndexAggregationDefinition = new AggregationDefinition()
    {
      Name = "FeedIndex",
      Protocol = (IProtocol) Protocol.Cargo,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.CargoPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition CargoStorageIdCacheAggregationDefinition;

    static AggregationDefinitions()
    {
      CacheAggregationDefinition aggregationDefinition = new CacheAggregationDefinition();
      aggregationDefinition.Name = "StorageIdCache";
      aggregationDefinition.Protocol = (IProtocol) Protocol.Cargo;
      aggregationDefinition.DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.CargoPackageMetadataAggregationDefinition
      };
      AggregationDefinitions.CargoStorageIdCacheAggregationDefinition = (AggregationDefinition) aggregationDefinition;
    }
  }
}
