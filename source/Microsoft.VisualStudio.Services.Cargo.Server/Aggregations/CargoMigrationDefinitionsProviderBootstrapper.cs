// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.CargoMigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  [ExcludeFromCodeCoverage]
  public class CargoMigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;

    public CargoMigrationDefinitionsProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMigrationDefinitionsProvider Bootstrap() => new MigrationDefinitionsProviderBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new CargoMigrationDefinitionsProviderBootstrapper.CargoMigrationDefinitionsProvider()).Bootstrap();

    private class CargoMigrationDefinitionsProvider : MigrationDefinitionsProviderBase
    {
      private static readonly MigrationDefinition CargoBlobMetadataV1 = new MigrationDefinition()
      {
        Name = nameof (CargoBlobMetadataV1),
        CommitLogBaseline = "ItemStoreCommitLog",
        Protocol = (IProtocol) Protocol.Cargo,
        Aggregations = new List<IAggregation>()
        {
          (IAggregation) CargoPackageMetadataAggregation.V1,
          (IAggregation) CargoFeedIndexAggregation.V1,
          (IAggregation) CargoStorageIdCacheAggregation.CargoBlobMetadataStorageIdCache,
          (IAggregation) CargoUpstreamVersionListAggregation.V1,
          (IAggregation) CargoProblemPackagesAggregation.V1
        }
      };
      private static readonly List<MigrationDefinition> Definitions = new List<MigrationDefinition>()
      {
        CargoMigrationDefinitionsProviderBootstrapper.CargoMigrationDefinitionsProvider.CargoBlobMetadataV1
      };

      public override MigrationDefinition? GetDefaultMigration(IProtocol protocol, FeedId feedId) => protocol != Protocol.Cargo ? (MigrationDefinition) null : CargoMigrationDefinitionsProviderBootstrapper.CargoMigrationDefinitionsProvider.CargoBlobMetadataV1;

      public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) CargoMigrationDefinitionsProviderBootstrapper.CargoMigrationDefinitionsProvider.Definitions;
    }
  }
}
