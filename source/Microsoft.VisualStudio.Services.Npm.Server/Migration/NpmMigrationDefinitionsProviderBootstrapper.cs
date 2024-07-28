// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Migration.NpmMigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.ItemStoreMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Migration
{
  public class NpmMigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;

    public NpmMigrationDefinitionsProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMigrationDefinitionsProvider Bootstrap() => new MigrationDefinitionsProviderBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider()).Bootstrap();

    private class NpmMigrationDefinitionsProvider : MigrationDefinitionsProviderBase
    {
      private static readonly MigrationDefinition itemStoreMetadataMigration = new MigrationDefinition()
      {
        Name = "NpmItemStoreMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.npm,
        IsDeprecated = true,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NpmItemStoreMetadataAggregation.V1_Removed,
          (IAggregation) NpmFeedIndexAggregation.V1,
          (IAggregation) NpmStorageIdCacheAggregation.NpmBlobMetadataStorageIdCache,
          (IAggregation) NpmUpstreamVersionListAggregation.V1
        }
      };
      private static readonly MigrationDefinition blobStoreMetadataMigration = new MigrationDefinition()
      {
        Name = "NpmBlobMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.npm,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NpmPackageMetadataAggregation.V1,
          (IAggregation) NpmFeedIndexAggregation.V1,
          (IAggregation) NpmStorageIdCacheAggregation.NpmBlobMetadataStorageIdCache,
          (IAggregation) NpmNamesAggregation.V1,
          (IAggregation) NpmUpstreamVersionListAggregation.V1,
          (IAggregation) NpmProblemPackagesAggregation.V1
        }
      };
      private static readonly MigrationDefinition NpmBlobMetadataMigrationV2Definition = new MigrationDefinition()
      {
        Name = "NpmBlobMetadataV2",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.npm,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NpmPackageMetadataAggregation.V1,
          (IAggregation) NpmFeedIndexAggregation.V1,
          (IAggregation) NpmStorageIdCacheAggregation.NpmBlobMetadataStorageIdCache,
          (IAggregation) NpmNamesAggregation.V1,
          (IAggregation) NpmUpstreamVersionListAggregation.V1,
          (IAggregation) NpmVersionListWithSizeAggregation.V1,
          (IAggregation) NpmProblemPackagesAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly List<MigrationDefinition> Definitions = new List<MigrationDefinition>()
      {
        NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider.itemStoreMetadataMigration,
        NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider.blobStoreMetadataMigration,
        NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider.NpmBlobMetadataMigrationV2Definition
      };

      public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId) => protocol != Protocol.npm ? (MigrationDefinition) null : NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider.blobStoreMetadataMigration;

      public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) NpmMigrationDefinitionsProviderBootstrapper.NpmMigrationDefinitionsProvider.Definitions;
    }
  }
}
