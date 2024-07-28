// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.AggregationDefinitions
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class AggregationDefinitions
  {
    public static readonly AggregationDefinition MavenPackageMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "MD",
      Protocol = (IProtocol) Protocol.Maven,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };
    public static readonly AggregationDefinition MavenFeedIndexAggregationDefinition = new AggregationDefinition()
    {
      Name = "FeedIndex",
      Protocol = (IProtocol) Protocol.Maven,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.MavenPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition MavenPluginMetadataAggregationDefinition = new AggregationDefinition()
    {
      Name = "PluginMetadata",
      Protocol = (IProtocol) Protocol.Maven,
      DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.MavenPackageMetadataAggregationDefinition
      }
    };
    public static readonly AggregationDefinition MavenStorageIdCacheAggregationDefinition;
    public static readonly AggregationDefinition MavenVersionListWithSizeAggregationDefinition;

    static AggregationDefinitions()
    {
      CacheAggregationDefinition aggregationDefinition = new CacheAggregationDefinition();
      aggregationDefinition.Name = "StorageIdCache";
      aggregationDefinition.Protocol = (IProtocol) Protocol.Maven;
      aggregationDefinition.DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
      {
        AggregationDefinitions.MavenPackageMetadataAggregationDefinition
      };
      AggregationDefinitions.MavenStorageIdCacheAggregationDefinition = (AggregationDefinition) aggregationDefinition;
      AggregationDefinitions.MavenVersionListWithSizeAggregationDefinition = new AggregationDefinition()
      {
        Name = "VersionListWithSize",
        Protocol = (IProtocol) Protocol.Maven,
        IsOptionalForOpportunisticAppTierApply = true,
        DependsOn = (IEnumerable<AggregationDefinition>) new AggregationDefinition[1]
        {
          AggregationDefinitions.MavenPackageMetadataAggregationDefinition
        }
      };
    }
  }
}
