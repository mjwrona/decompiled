// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper : 
    IBootstrapper<
    #nullable disable
    IUpstreamMetadataManager>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> localMetadataService;
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> upstreamMetadataServiceFactory;
    private readonly IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService;

    public static IPendingAggBootstrapper<IUpstreamMetadataManager> Pending { get; } = NuGetAggregationResolver.PendingBootstrapper.WithAggregation1<IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>().WithAggregation2<IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>().PendingAggBootstrapperFor<IUpstreamMetadataManager, IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>((Func<IVssRequestContext, IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>, IUpstreamMetadataManager>) ((requestContext, localMetadataService, upstreamVersionListService) => new NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper(requestContext, localMetadataService, upstreamVersionListService).Bootstrap()));

    public NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> localMetadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService,
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> upstreamMetadataServiceFactory = null)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamVersionListService = upstreamVersionListService;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory ?? new NuGetUpstreamMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
    }

    public IUpstreamMetadataManager Bootstrap() => new UpstreamMetadataManagerBootstrapper<VssNuGetPackageName, VssNuGetPackageIdentity, INuGetMetadataEntry>(this.requestContext, this.localMetadataService, this.GetUpstreamRefreshStrategy(), (IConverter<string, VssNuGetPackageName>) new ByFuncConverter<string, VssNuGetPackageName>((Func<string, VssNuGetPackageName>) (x => new VssNuGetPackageName(x))), (IConverter<IPackageIdentity, VssNuGetPackageIdentity>) new ByFuncConverter<IPackageIdentity, VssNuGetPackageIdentity>((Func<IPackageIdentity, VssNuGetPackageIdentity>) (x => !(x is VssNuGetPackageIdentity getPackageIdentity) ? new VssNuGetPackageIdentity(x.Name.DisplayName, x.Version.DisplayVersion) : getPackageIdentity)), new TelemetryBootstrapper(this.requestContext, NuGetTracePoints.Telemetry.TraceInfo).Bootstrap(), (IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>) new GetCacheUpstreamMetadataCiDataHandler(this.requestContext), new UpstreamStatusUpdateHandlerBootstrapper(this.requestContext).Bootstrap()).Bootstrap();

    public IUpstreamRefreshStrategy<VssNuGetPackageName, VssNuGetPackageIdentity, INuGetMetadataEntry> GetUpstreamRefreshStrategy()
    {
      IAggregationAccessorFactory aggregationAccessorFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, new NuGetMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      PackageUpstreamBehaviorServiceFacade upstreamBehaviorService = new PackageUpstreamBehaviorServiceFacade(this.requestContext);
      IOrganizationPolicies organizationPolicies = this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (IOrganizationPolicies) new OrganizationPolicyServiceFacade(this.requestContext) : (IOrganizationPolicies) new AlwaysUseDefaultsOrganizationPolicies();
      ITerrapinMetadataValidator<VssNuGetPackageName, VssNuGetPackageVersion> metadataValidator = new TerrapinMetadataValidatorBootstrapper<VssNuGetPackageName, VssNuGetPackageVersion>(this.requestContext, (IIdentityResolver) NuGetIdentityResolver.Instance).Bootstrap();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> metadataServiceFactory = this.upstreamMetadataServiceFactory;
      UpstreamEntryRetainingStrategyWithRegistryOverride<INuGetMetadataEntry> upstreamEntriesRetainingStrategyHandler = new UpstreamEntryRetainingStrategyWithRegistryOverride<INuGetMetadataEntry>((IRegistryService) registryFacade);
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> versionListService = this.upstreamVersionListService;
      IAggregationAccessorFactory writeAggregationAccessorsFactory = aggregationAccessorFactory;
      InParallelAndCacheSkippingCommitApplier skippingCommitApplier = new InParallelAndCacheSkippingCommitApplier(tracerFacade);
      ITracerService tracerService = tracerFacade;
      UpstreamStatusClassifier upstreamStatusClassifier = new UpstreamStatusClassifier();
      IFeatureFlagService featureFlagService = featureFlagFacade;
      IFrotocolLevelPackagingSetting<IReadOnlyCollection<string>> bannedCustomUpstreamHostsSetting = PackagingServerConstants.BannedCustomUpstreamHostsSetting.Bootstrap(this.requestContext);
      ShouldIncludeExternalVersionsHelper shouldIncludeExternalVersionsHelper = new ShouldIncludeExternalVersionsHelper(tracerFacade, featureFlagFacade, (IRegistryService) registryFacade, (IPackageUpstreamBehaviorFacade) upstreamBehaviorService, organizationPolicies, this.requestContext.GetExecutionEnvironmentFacade());
      ITerrapinMetadataValidator<VssNuGetPackageName, VssNuGetPackageVersion> terrapinValidator = metadataValidator;
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithEntriesSetting = UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(this.requestContext);
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithoutEntriesSetting = UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(this.requestContext);
      return (IUpstreamRefreshStrategy<VssNuGetPackageName, VssNuGetPackageIdentity, INuGetMetadataEntry>) new ByVersionUpstreamRefreshStrategy<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry, INuGetMetadataEntryWriteable>((ITimeProvider) defaultTimeProvider, metadataServiceFactory, (IAsyncHandler<MetadataDocument<INuGetMetadataEntry>, List<INuGetMetadataEntry>>) upstreamEntriesRetainingStrategyHandler, versionListService, writeAggregationAccessorsFactory, (IAggregationCommitApplier) skippingCommitApplier, tracerService, (IUpstreamPackageNameMetadataRefreshStrategy<VssNuGetPackageName, INuGetMetadataEntry>) null, (IUpstreamStatusClassifier) upstreamStatusClassifier, featureFlagService, false, bannedCustomUpstreamHostsSetting, (IShouldIncludeExternalVersionsHelper) shouldIncludeExternalVersionsHelper, terrapinValidator, metadataValidityPeriodWithEntriesSetting, metadataValidityPeriodWithoutEntriesSetting);
    }
  }
}
