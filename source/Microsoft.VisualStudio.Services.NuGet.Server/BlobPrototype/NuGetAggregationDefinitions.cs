// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetAggregationDefinitions
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetAggregationDefinitions
  {
    public static readonly AggregationDefinition NuGetPackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageMetadata",
      Protocol = (IProtocol) Protocol.NuGet,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };
    public static readonly AggregationDefinition NuGetFeedIndexAggregationDefinition = new AggregationDefinition()
    {
      Name = "FeedIndex",
      Protocol = (IProtocol) Protocol.NuGet,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition NuGetStorageIdCacheAggregationDefinition;
    public static readonly AggregationDefinition NuGetNamesAggregationDefinition_Removed;
    public static readonly AggregationDefinition NuGetPackageVersionCountsAggregationDefinition;
    public static readonly AggregationDefinition NuGetVersionListWithSizeAggregationDefinition;

    static NuGetAggregationDefinitions()
    {
      CacheAggregationDefinition aggregationDefinition = new CacheAggregationDefinition();
      aggregationDefinition.Name = "StorageIdCache";
      aggregationDefinition.Protocol = (IProtocol) Protocol.NuGet;
      aggregationDefinition.DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition
      };
      NuGetAggregationDefinitions.NuGetStorageIdCacheAggregationDefinition = (AggregationDefinition) aggregationDefinition;
      NuGetAggregationDefinitions.NuGetNamesAggregationDefinition_Removed = new AggregationDefinition()
      {
        Name = "PackageNames",
        Protocol = (IProtocol) Protocol.NuGet,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition
        }
      };
      NuGetAggregationDefinitions.NuGetPackageVersionCountsAggregationDefinition = new AggregationDefinition()
      {
        Name = "PackageVersionCounts",
        Protocol = (IProtocol) Protocol.NuGet,
        IsOptionalForOpportunisticAppTierApply = true,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition
        }
      };
      NuGetAggregationDefinitions.NuGetVersionListWithSizeAggregationDefinition = new AggregationDefinition()
      {
        Name = "VersionListWithSize",
        Protocol = (IProtocol) Protocol.NuGet,
        IsOptionalForOpportunisticAppTierApply = true,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition
        }
      };
    }
  }
}
