// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
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
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper : 
    IBootstrapper<
    #nullable disable
    IUpstreamMetadataManager>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> localMetadataService;
    private readonly IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService;

    public static IPendingAggBootstrapper<IUpstreamMetadataManager> Pending { get; } = PyPiAggregationResolver.PendingBootstrapper.WithAggregation1<IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>().WithAggregation2<IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>().PendingAggBootstrapperFor<IUpstreamMetadataManager, IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>((Func<IVssRequestContext, IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>, IUpstreamMetadataManager>) ((requestContext, localMetadataService, upstreamVersionListService) => new PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper(requestContext, localMetadataService, upstreamVersionListService).Bootstrap()));

    public PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> localMetadataService,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IUpstreamMetadataManager Bootstrap() => new UpstreamMetadataManagerBootstrapper<PyPiPackageName, PyPiPackageIdentity, IPyPiMetadataEntry>(this.requestContext, this.localMetadataService, this.GetUpstreamRefreshStrategy(), (IConverter<string, PyPiPackageName>) new ByFuncConverter<string, PyPiPackageName>((Func<string, PyPiPackageName>) (x => new PyPiPackageName(x))), (IConverter<IPackageIdentity, PyPiPackageIdentity>) new ByFuncConverter<IPackageIdentity, PyPiPackageIdentity>((Func<IPackageIdentity, PyPiPackageIdentity>) (x => !(x is PyPiPackageIdentity piPackageIdentity) ? new PyPiPackageIdentity(new PyPiPackageName(x.Name.DisplayName), PyPiPackageIngestionValidationUtils.ValidateAndParseVersion(x.Version.DisplayVersion)) : piPackageIdentity)), new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap(), (IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>) new GetCacheUpstreamMetadataCiDataHandler(this.requestContext), new UpstreamStatusUpdateHandlerBootstrapper(this.requestContext).Bootstrap()).Bootstrap();

    public IUpstreamRefreshStrategy<PyPiPackageName, PyPiPackageIdentity, IPyPiMetadataEntry> GetUpstreamRefreshStrategy()
    {
      IAggregationAccessorFactory writeAggregationAccessorsFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, new PyPiMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      IFactory<UpstreamSource, Task<IUpstreamPyPiClient>> upstreamClientFactory = new UpstreamPyPiClientFactoryBootstrapper(this.requestContext).Bootstrap();
      DefaultTimeProvider timeProvider = new DefaultTimeProvider();
      PackageUpstreamBehaviorServiceFacade upstreamBehaviorService = new PackageUpstreamBehaviorServiceFacade(this.requestContext);
      IOrganizationPolicies organizationPolicies = this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (IOrganizationPolicies) new OrganizationPolicyServiceFacade(this.requestContext) : (IOrganizationPolicies) new AlwaysUseDefaultsOrganizationPolicies();
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      ITerrapinMetadataValidator<PyPiPackageName, PyPiPackageVersion> terrapinValidator = new TerrapinMetadataValidatorBootstrapper<PyPiPackageName, PyPiPackageVersion>(this.requestContext, (IIdentityResolver) PyPiIdentityResolver.Instance).Bootstrap();
      return (IUpstreamRefreshStrategy<PyPiPackageName, PyPiPackageIdentity, IPyPiMetadataEntry>) new ByVersionUpstreamRefreshStrategy<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry, IPyPiMetadataEntryWritable>((ITimeProvider) timeProvider, ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry>>>) (async upstreamSource =>
      {
        UpstreamSource source = upstreamSource;
        return PackageFilteringUpstreamMetadataServiceDecorator.Create<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry>((IUpstreamMetadataService<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry>) new PyPiUpstreamOnlyMetadataStore(source, await upstreamClientFactory.Get(upstreamSource), (ITimeProvider) timeProvider, tracerService), (IConverter<PyPiPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap(), PyPiIdentityResolver.Instance.IdentityFuser);
      })), this.requestContext.IsFeatureEnabledWithLogging("Packaging.PyPI.RetainUpstreamEntriesForTerrapin") ? (IAsyncHandler<MetadataDocument<IPyPiMetadataEntry>, List<IPyPiMetadataEntry>>) new UpstreamEntryRetainingStrategyWithRegistryOverride<IPyPiMetadataEntry>((IRegistryService) registryFacade) : (IAsyncHandler<MetadataDocument<IPyPiMetadataEntry>, List<IPyPiMetadataEntry>>) new RetainNothingUpstreamEntriesRetainingStrategyHandler<IPyPiMetadataEntry>(), this.upstreamVersionListService, writeAggregationAccessorsFactory, (IAggregationCommitApplier) new InParallelAndCacheSkippingCommitApplier(this.requestContext.GetTracerFacade()), tracerService, (IUpstreamPackageNameMetadataRefreshStrategy<PyPiPackageName, IPyPiMetadataEntry>) null, (IUpstreamStatusClassifier) new UpstreamStatusClassifier(), featureFlagFacade, true, PackagingServerConstants.BannedCustomUpstreamHostsSetting.Bootstrap(this.requestContext), (IShouldIncludeExternalVersionsHelper) new ShouldIncludeExternalVersionsHelper(tracerService, featureFlagFacade, (IRegistryService) this.requestContext.GetRegistryFacade(), (IPackageUpstreamBehaviorFacade) upstreamBehaviorService, organizationPolicies, this.requestContext.GetExecutionEnvironmentFacade()), terrapinValidator, UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(this.requestContext), UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(this.requestContext));
    }
  }
}
