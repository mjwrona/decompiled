// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper : 
    IBootstrapper<IUpstreamMetadataManager>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> localMetadataService;
    private readonly IUpstreamVersionListService<CargoPackageName, CargoPackageVersion> upstreamVersionListService;

    public static IPendingAggBootstrapper<IUpstreamMetadataManager> Pending { get; } = CargoAggregationResolver.PendingBootstrapper.WithAggregation1<IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>().WithAggregation2<IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>().PendingAggBootstrapperFor<IUpstreamMetadataManager, IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>((Func<IVssRequestContext, IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IUpstreamMetadataManager>) ((requestContext, localMetadataService, upstreamVersionListService) => new CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper(requestContext, localMetadataService, upstreamVersionListService).Bootstrap()));

    public CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> localMetadataService,
      IUpstreamVersionListService<CargoPackageName, CargoPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IUpstreamMetadataManager Bootstrap() => new UpstreamMetadataManagerBootstrapper<CargoPackageName, CargoPackageIdentity, ICargoMetadataEntry>(this.requestContext, this.localMetadataService, this.GetUpstreamRefreshStrategy(), CargoIdentityResolver.Instance.NameResolver, CargoIdentityResolver.Instance.IdentityDowncaster, new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap(), (IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>) new GetCacheUpstreamMetadataCiDataHandler(this.requestContext), new UpstreamStatusUpdateHandlerBootstrapper(this.requestContext).Bootstrap()).Bootstrap();

    public IUpstreamRefreshStrategy<CargoPackageName, CargoPackageIdentity, ICargoMetadataEntry> GetUpstreamRefreshStrategy()
    {
      IAggregationAccessorFactory writeAggregationAccessorsFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, new CargoMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      IFactory<UpstreamSource, Task<IUpstreamCargoClient>> upstreamClientFactory = new UpstreamCargoClientFactoryBootstrapper(this.requestContext).Bootstrap();
      DefaultTimeProvider timeProvider = new DefaultTimeProvider();
      PackageUpstreamBehaviorServiceFacade upstreamBehaviorService = new PackageUpstreamBehaviorServiceFacade(this.requestContext);
      IOrganizationPolicies organizationPolicies = this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (IOrganizationPolicies) new OrganizationPolicyServiceFacade(this.requestContext) : (IOrganizationPolicies) new AlwaysUseDefaultsOrganizationPolicies();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITerrapinMetadataValidator<CargoPackageName, CargoPackageVersion> terrapinValidator = new TerrapinMetadataValidatorBootstrapper<CargoPackageName, CargoPackageVersion>(this.requestContext, (IIdentityResolver) CargoIdentityResolver.Instance).Bootstrap();
      return (IUpstreamRefreshStrategy<CargoPackageName, CargoPackageIdentity, ICargoMetadataEntry>) new ByVersionUpstreamRefreshStrategy<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry, ICargoMetadataEntryWriteable>((ITimeProvider) timeProvider, ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry>>>) (async upstreamSource =>
      {
        UpstreamSource source = upstreamSource;
        return PackageFilteringUpstreamMetadataServiceDecorator.Create<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry>((IUpstreamMetadataService<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry>) new CargoUpstreamOnlyMetadataStore(source, await upstreamClientFactory.Get(upstreamSource), (ITimeProvider) timeProvider), (IConverter<CargoPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap(), CargoIdentityResolver.Instance.IdentityFuser);
      })), (IAsyncHandler<MetadataDocument<ICargoMetadataEntry>, List<ICargoMetadataEntry>>) new UpstreamEntryRetainingStrategyWithRegistryOverride<ICargoMetadataEntry>((IRegistryService) this.requestContext.GetRegistryFacade()), this.upstreamVersionListService, writeAggregationAccessorsFactory, (IAggregationCommitApplier) new InParallelAndCacheSkippingCommitApplier(tracerFacade), tracerFacade, (IUpstreamPackageNameMetadataRefreshStrategy<CargoPackageName, ICargoMetadataEntry>) null, (IUpstreamStatusClassifier) new UpstreamStatusClassifier(), featureFlagFacade, false, PackagingServerConstants.BannedCustomUpstreamHostsSetting.Bootstrap(this.requestContext), (IShouldIncludeExternalVersionsHelper) new ShouldIncludeExternalVersionsHelper(tracerFacade, featureFlagFacade, (IRegistryService) this.requestContext.GetRegistryFacade(), (IPackageUpstreamBehaviorFacade) upstreamBehaviorService, organizationPolicies, this.requestContext.GetExecutionEnvironmentFacade()), terrapinValidator, UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(this.requestContext), UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(this.requestContext));
    }
  }
}
