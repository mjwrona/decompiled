// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
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
namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper : 
    IBootstrapper<
    #nullable disable
    IUpstreamMetadataManager>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> localMetadataService;
    private readonly IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService;

    public static IPendingAggBootstrapper<IUpstreamMetadataManager> Pending { get; } = MavenAggregationResolver.PendingBootstrapper.WithAggregation1<IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>().WithAggregation2<IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>().PendingAggBootstrapperFor<IUpstreamMetadataManager, IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>((Func<IVssRequestContext, IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>, IUpstreamMetadataManager>) ((requestContext, localMetadataService, upstreamVersionListService) => new MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper(requestContext, localMetadataService, upstreamVersionListService).Bootstrap()));

    public MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> localMetadataService,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IUpstreamMetadataManager Bootstrap() => new UpstreamMetadataManagerBootstrapper<MavenPackageName, MavenPackageIdentity, IMavenMetadataEntry>(this.requestContext, this.localMetadataService, this.GetUpstreamRefreshStrategy(), (IConverter<string, MavenPackageName>) new ByFuncConverter<string, MavenPackageName>((Func<string, MavenPackageName>) (x => new MavenPackageName(x))), (IConverter<IPackageIdentity, MavenPackageIdentity>) new ByFuncConverter<IPackageIdentity, MavenPackageIdentity>((Func<IPackageIdentity, MavenPackageIdentity>) (x => !(x is MavenPackageIdentity mavenPackageIdentity) ? new MavenPackageIdentity(new MavenPackageName(x.Name.DisplayName), new MavenPackageVersion(x.Version.DisplayVersion)) : mavenPackageIdentity)), new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap(), (IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>) new GetCacheUpstreamMetadataCiDataHandler(this.requestContext), new UpstreamStatusUpdateHandlerBootstrapper(this.requestContext).Bootstrap()).Bootstrap();

    public IUpstreamRefreshStrategy<MavenPackageName, MavenPackageIdentity, IMavenMetadataEntry> GetUpstreamRefreshStrategy()
    {
      IAggregationAccessorFactory writeAggregationAccessorsFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, new MavenMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      IFactory<UpstreamSource, Task<IUpstreamMavenClient>> upstreamClientFactory = new UpstreamMavenClientFactoryBootstrapper(this.requestContext).Bootstrap();
      DefaultTimeProvider timeProvider = new DefaultTimeProvider();
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>>> upstreamMetadataServiceFactory = ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>>>) (async upstreamSource =>
      {
        UpstreamSource source = upstreamSource;
        return PackageFilteringUpstreamMetadataServiceDecorator.Create<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>((IUpstreamMetadataService<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>) new MavenUpstreamOnlyMetadataStore(source, await upstreamClientFactory.Get(upstreamSource), (ITimeProvider) timeProvider), (IConverter<MavenPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap(), MavenIdentityResolver.Instance.IdentityFuser);
      }));
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      PackageUpstreamBehaviorServiceFacade upstreamBehaviorService = new PackageUpstreamBehaviorServiceFacade(this.requestContext);
      IOrganizationPolicies organizationPolicies = this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (IOrganizationPolicies) new OrganizationPolicyServiceFacade(this.requestContext) : (IOrganizationPolicies) new AlwaysUseDefaultsOrganizationPolicies();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITerrapinMetadataValidator<MavenPackageName, MavenPackageVersion> terrapinValidator = new TerrapinMetadataValidatorBootstrapper<MavenPackageName, MavenPackageVersion>(this.requestContext, (IIdentityResolver) MavenIdentityResolver.Instance).Bootstrap();
      return (IUpstreamRefreshStrategy<MavenPackageName, MavenPackageIdentity, IMavenMetadataEntry>) new ByVersionUpstreamRefreshStrategy<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry, IMavenMetadataEntryWritable>((ITimeProvider) timeProvider, upstreamMetadataServiceFactory, (IAsyncHandler<MetadataDocument<IMavenMetadataEntry>, List<IMavenMetadataEntry>>) new UpstreamEntryRetainingStrategyWithRegistryOverride<IMavenMetadataEntry>((IRegistryService) registryFacade), this.upstreamVersionListService, writeAggregationAccessorsFactory, (IAggregationCommitApplier) new InParallelAndCacheSkippingCommitApplier(tracerFacade), tracerFacade, (IUpstreamPackageNameMetadataRefreshStrategy<MavenPackageName, IMavenMetadataEntry>) null, (IUpstreamStatusClassifier) new UpstreamStatusClassifier(), featureFlagFacade, false, PackagingServerConstants.BannedCustomUpstreamHostsSetting.Bootstrap(this.requestContext), (IShouldIncludeExternalVersionsHelper) new ShouldIncludeExternalVersionsHelper(tracerFacade, featureFlagFacade, (IRegistryService) registryFacade, (IPackageUpstreamBehaviorFacade) upstreamBehaviorService, organizationPolicies, this.requestContext.GetExecutionEnvironmentFacade()), terrapinValidator, UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(this.requestContext), UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(this.requestContext));
    }
  }
}
