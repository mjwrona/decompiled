// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Migration.NuGetMigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.Removed;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Migration
{
  public class NuGetMigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetMigrationDefinitionsProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMigrationDefinitionsProvider Bootstrap() => new MigrationDefinitionsProviderBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider()).Bootstrap();

    private class NuGetMigrationDefinitionsProvider : MigrationDefinitionsProviderBase
    {
      private static readonly MigrationDefinition itemStoreMetadataMigrationDefinition = new MigrationDefinition()
      {
        Name = "NuGetItemStoreMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition blobMetadataMigrationDefinition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V1,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition blobMetadataMigrationV2Definition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadataV2",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V2,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition blobMetadataMigrationV3Definition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadataV3",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V3,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition blobMetadataMigrationV4Definition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadataV4",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V3,
          NuGetNamesAggregation_Removed.V1_Removed,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition blobMetadataMigrationV5Definition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadataV5",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V3,
          (IAggregation) NuGetPackageVersionCountsAggregation.V1,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1,
          (IAggregation) NuGetUpstreamVersionListAggregation.V1,
          (IAggregation) NuGetProblemPackagesAggregation.V1
        }
      };
      private static readonly MigrationDefinition blobMetadataMigrationV6Definition = new MigrationDefinition()
      {
        Name = "NuGetBlobMetadataV6",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.NuGet,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) NuGetPackageMetadataAggregation.V3,
          (IAggregation) NuGetPackageVersionCountsAggregation.V1,
          (IAggregation) NuGetStorageIdCacheAggregation.NuGetBlobMetadataStorageIdCache,
          (IAggregation) NuGetFeedIndexAggregation.V1,
          (IAggregation) NuGetUpstreamVersionListAggregation.V1,
          (IAggregation) NuGetVersionListWithSizeAggregation.V1,
          (IAggregation) NuGetProblemPackagesAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly List<MigrationDefinition> Definitions = new List<MigrationDefinition>()
      {
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.itemStoreMetadataMigrationDefinition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationDefinition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV2Definition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV3Definition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV4Definition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV5Definition,
        NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV6Definition
      };

      public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId) => protocol == Protocol.NuGet ? NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.blobMetadataMigrationV5Definition : (MigrationDefinition) null;

      public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) NuGetMigrationDefinitionsProviderBootstrapper.NuGetMigrationDefinitionsProvider.Definitions;
    }
  }
}
