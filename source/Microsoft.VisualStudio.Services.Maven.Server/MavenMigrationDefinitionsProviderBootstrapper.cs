// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenMigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.BlobPrototype.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenMigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;

    public MavenMigrationDefinitionsProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMigrationDefinitionsProvider Bootstrap() => new MigrationDefinitionsProviderBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider()).Bootstrap();

    private class MavenMigrationDefinitionsProvider : MigrationDefinitionsProviderBase
    {
      private static readonly MigrationDefinition itemStoreMetadataMigration = new MigrationDefinition()
      {
        Name = "MavenItemStoreMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.Maven,
        IsDeprecated = true,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) MavenMetadataAggregation.ItemStoreV1_Removed,
          (IAggregation) MavenFeedIndexAggregation.V1,
          (IAggregation) MavenPluginMetadataAggregation.ByItemStore,
          (IAggregation) MavenStorageIdCacheAggregation.MavenBlobMetadataStorageIdCache,
          (IAggregation) MavenUpstreamVersionListAggregation.V1
        }
      };
      private static readonly MigrationDefinition blobStoreMetadataMigration = new MigrationDefinition()
      {
        Name = "MavenBlobStoreMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.Maven,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) MavenMetadataAggregation.BlobStoreV1,
          (IAggregation) MavenFeedIndexAggregation.V1,
          (IAggregation) MavenPluginMetadataAggregation.ByItemStore,
          (IAggregation) MavenStorageIdCacheAggregation.MavenBlobMetadataStorageIdCache,
          (IAggregation) MavenUpstreamVersionListAggregation.V1,
          (IAggregation) MavenProblemPackagesAggregation.V1
        }
      };
      private static readonly MigrationDefinition MavenBlobMetadataMigrationV2Definition = new MigrationDefinition()
      {
        Name = "MavenBlobMetadataV2",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.Maven,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) MavenMetadataAggregation.BlobStoreV1,
          (IAggregation) MavenFeedIndexAggregation.V1,
          (IAggregation) MavenPluginMetadataAggregation.ByItemStore,
          (IAggregation) MavenStorageIdCacheAggregation.MavenBlobMetadataStorageIdCache,
          (IAggregation) MavenUpstreamVersionListAggregation.V1,
          (IAggregation) MavenVersionListWithSizeAggregation.V1,
          (IAggregation) MavenProblemPackagesAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly List<MigrationDefinition> Definitions = new List<MigrationDefinition>()
      {
        MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider.itemStoreMetadataMigration,
        MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider.blobStoreMetadataMigration,
        MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider.MavenBlobMetadataMigrationV2Definition
      };

      public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId) => protocol != Protocol.Maven ? (MigrationDefinition) null : MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider.blobStoreMetadataMigration;

      public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) MavenMigrationDefinitionsProviderBootstrapper.MavenMigrationDefinitionsProvider.Definitions;
    }
  }
}
