// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Migration.AggregationDefinitions
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Migration
{
  public class AggregationDefinitions
  {
    public static readonly AggregationDefinition PyPiPackageVersionMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageVersionMetadata",
      Protocol = (IProtocol) Protocol.PyPi,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };
    public static readonly AggregationDefinition PyPiPackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageMetadata",
      Protocol = (IProtocol) Protocol.PyPi,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.PyPiPackageVersionMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition PyPiFeedIndexAggregationDefinition = new AggregationDefinition()
    {
      Name = "FeedIndex",
      Protocol = (IProtocol) Protocol.PyPi,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.PyPiPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition PyPiStorageIdCacheAggregationDefinition;
    public static readonly AggregationDefinition PyPiVersionListWithSizeAggregationDefinition;

    static AggregationDefinitions()
    {
      CacheAggregationDefinition aggregationDefinition = new CacheAggregationDefinition();
      aggregationDefinition.Name = "StorageIdCache";
      aggregationDefinition.Protocol = (IProtocol) Protocol.PyPi;
      aggregationDefinition.DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.PyPiPackageMetadataAggregationDefinition
      };
      AggregationDefinitions.PyPiStorageIdCacheAggregationDefinition = (AggregationDefinition) aggregationDefinition;
      AggregationDefinitions.PyPiVersionListWithSizeAggregationDefinition = new AggregationDefinition()
      {
        Name = "VersionListWithSize",
        Protocol = (IProtocol) Protocol.PyPi,
        IsOptionalForOpportunisticAppTierApply = true,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          AggregationDefinitions.PyPiPackageMetadataAggregationDefinition
        }
      };
    }
  }
}
