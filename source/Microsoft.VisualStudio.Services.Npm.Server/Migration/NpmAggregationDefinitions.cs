// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Migration.NpmAggregationDefinitions
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Migration
{
  public class NpmAggregationDefinitions
  {
    public static readonly AggregationDefinition NpmPackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageMetadata",
      Protocol = (IProtocol) Protocol.npm,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[0]
    };
    public static readonly AggregationDefinition NpmItemStorePackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "ItemStorePackageMetadata",
      Protocol = (IProtocol) Protocol.npm,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[0]
    };
    public static readonly AggregationDefinition NpmFeedIndexAggregationDefinition = new AggregationDefinition()
    {
      Name = "FeedIndex",
      Protocol = (IProtocol) Protocol.npm,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[2]
      {
        NpmAggregationDefinitions.NpmPackageMetadataAggregationDefinition,
        NpmAggregationDefinitions.NpmItemStorePackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition NpmNamesAggregationDefinition = new AggregationDefinition()
    {
      Name = "PackageNames",
      Protocol = (IProtocol) Protocol.npm,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        NpmAggregationDefinitions.NpmPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition NpmStorageIdCacheAggregationDefinition;
    public static readonly AggregationDefinition NpmVersionListWithSizeAggregationDefinition;

    static NpmAggregationDefinitions()
    {
      CacheAggregationDefinition aggregationDefinition = new CacheAggregationDefinition();
      aggregationDefinition.Name = "StorageIdCache";
      aggregationDefinition.Protocol = (IProtocol) Protocol.npm;
      aggregationDefinition.DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[2]
      {
        NpmAggregationDefinitions.NpmPackageMetadataAggregationDefinition,
        NpmAggregationDefinitions.NpmItemStorePackageMetadataAggregationDefinition
      };
      NpmAggregationDefinitions.NpmStorageIdCacheAggregationDefinition = (AggregationDefinition) aggregationDefinition;
      NpmAggregationDefinitions.NpmVersionListWithSizeAggregationDefinition = new AggregationDefinition()
      {
        Name = "VersionListWithSize",
        Protocol = (IProtocol) Protocol.npm,
        IsOptionalForOpportunisticAppTierApply = true,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          NpmAggregationDefinitions.NpmPackageMetadataAggregationDefinition
        }
      };
    }
  }
}
