// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Migration.PyPiMigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.Aggregations.VersionListWithSize;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Migration
{
  public class PyPiMigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiMigrationDefinitionsProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMigrationDefinitionsProvider Bootstrap() => new MigrationDefinitionsProviderBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider()).Bootstrap();

    private class PyPiMigrationDefinitionsProvider : MigrationDefinitionsProviderBase
    {
      private static readonly MigrationDefinition DataImportBaselineMigration = new MigrationDefinition()
      {
        Name = "PyPiBlobMetadata",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.PyPi,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) PyPiFeedIndexAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly MigrationDefinition FirstReleasedPyPiMetadataMigration = new MigrationDefinition()
      {
        Name = "PyPiBlobMetadataV2",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.PyPi,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) PyPiMetadataAggregation.V1,
          (IAggregation) PyPiFeedIndexAggregation.V1,
          (IAggregation) PyPiPackageVersionMetadataAggregation.V2,
          (IAggregation) PyPiStorageIdCacheAggregation.PyPiBlobMetadataStorageIdCache,
          (IAggregation) PyPiUpstreamVersionListAggregation.V1,
          (IAggregation) PyPiProblemPackagesAggregation.V1
        }
      };
      private static readonly MigrationDefinition PyPiBlobMetadataMigrationV3Definition = new MigrationDefinition()
      {
        Name = "PyPiBlobMetadataV3",
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.PyPi,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) PyPiMetadataAggregation.V1,
          (IAggregation) PyPiFeedIndexAggregation.V1,
          (IAggregation) PyPiPackageVersionMetadataAggregation.V2,
          (IAggregation) PyPiStorageIdCacheAggregation.PyPiBlobMetadataStorageIdCache,
          (IAggregation) PyPiUpstreamVersionListAggregation.V1,
          (IAggregation) PyPiVersionListWithSizeAggregation.V1,
          (IAggregation) PyPiProblemPackagesAggregation.V1
        },
        IsDeprecated = true
      };
      private static readonly List<MigrationDefinition> Definitions = new List<MigrationDefinition>()
      {
        PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider.DataImportBaselineMigration,
        PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider.FirstReleasedPyPiMetadataMigration,
        PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider.PyPiBlobMetadataMigrationV3Definition
      };

      public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId) => protocol != Protocol.PyPi ? (MigrationDefinition) null : PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider.FirstReleasedPyPiMetadataMigration;

      public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) PyPiMigrationDefinitionsProviderBootstrapper.PyPiMigrationDefinitionsProvider.Definitions;
    }
  }
}
