// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper : 
    IBootstrapper<
    #nullable disable
    IUpstreamMetadataManager>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> localMetadataService;
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory;
    private readonly IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService;

    public static IPendingAggBootstrapper<IUpstreamMetadataManager> Pending { get; } = NpmAggregationResolver.PendingBootstrapperInstance.WithAggregation1<IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>().WithAggregation2<IUpstreamVersionListService<NpmPackageName, SemanticVersion>>().PendingAggBootstrapperFor<IUpstreamMetadataManager, IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>((Func<IVssRequestContext, IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>, IUpstreamVersionListService<NpmPackageName, SemanticVersion>, IUpstreamMetadataManager>) ((requestContext, localMetadataService, upstreamVersionListService) => new NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper(requestContext, localMetadataService, upstreamVersionListService).Bootstrap()));

    public NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> localMetadataService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService,
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory = null)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamVersionListService = upstreamVersionListService;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory ?? new NpmUpstreamMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
    }

    public IUpstreamMetadataManager Bootstrap() => new UpstreamMetadataManagerBootstrapper<NpmPackageName, NpmPackageIdentity, INpmMetadataEntry>(this.requestContext, this.localMetadataService, this.GetUpstreamRefreshStrategy(), (IConverter<string, NpmPackageName>) new ByFuncConverter<string, NpmPackageName>((Func<string, NpmPackageName>) (x => new NpmPackageName(x))), (IConverter<IPackageIdentity, NpmPackageIdentity>) new ByFuncConverter<IPackageIdentity, NpmPackageIdentity>((Func<IPackageIdentity, NpmPackageIdentity>) (x => !(x is NpmPackageIdentity npmPackageIdentity) ? new NpmPackageIdentity(x.Name.DisplayName, SemanticVersion.Parse(x.Version.NormalizedVersion)) : npmPackageIdentity)), new TelemetryBootstrapper(this.requestContext, NpmTracePoints.Telemetry.TraceInfo).Bootstrap(), (IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData>) new GetCacheUpstreamMetadataCiDataHandler(this.requestContext), new UpstreamStatusUpdateHandlerBootstrapper(this.requestContext).Bootstrap()).Bootstrap();

    public IUpstreamRefreshStrategy<NpmPackageName, NpmPackageIdentity, INpmMetadataEntry> GetUpstreamRefreshStrategy()
    {
      IAggregationAccessorFactory aggregationAccessorFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, new NpmMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      PackageUpstreamBehaviorServiceFacade upstreamBehaviorService = new PackageUpstreamBehaviorServiceFacade(this.requestContext);
      IOrganizationPolicies organizationPolicies = this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (IOrganizationPolicies) new OrganizationPolicyServiceFacade(this.requestContext) : (IOrganizationPolicies) new AlwaysUseDefaultsOrganizationPolicies();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITerrapinMetadataValidator<NpmPackageName, SemanticVersion> metadataValidator = new TerrapinMetadataValidatorBootstrapper<NpmPackageName, SemanticVersion>(this.requestContext, (IIdentityResolver) NpmIdentityResolver.Instance).Bootstrap();
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> metadataServiceFactory = this.upstreamMetadataServiceFactory;
      UpstreamEntryRetainingStrategyWithRegistryOverride<INpmMetadataEntry> upstreamEntriesRetainingStrategyHandler = new UpstreamEntryRetainingStrategyWithRegistryOverride<INpmMetadataEntry>((IRegistryService) registryFacade);
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> versionListService = this.upstreamVersionListService;
      IAggregationAccessorFactory writeAggregationAccessorsFactory = aggregationAccessorFactory;
      InParallelAndCacheSkippingCommitApplier skippingCommitApplier = new InParallelAndCacheSkippingCommitApplier(tracerFacade);
      ITracerService tracerService = tracerFacade;
      NpmPackageNameMetadataRefreshStrategy packageNameRefreshStrategy = new NpmPackageNameMetadataRefreshStrategy(this.upstreamMetadataServiceFactory, tracerFacade);
      UpstreamStatusClassifier upstreamStatusClassifier = new UpstreamStatusClassifier();
      IFeatureFlagService featureFlagService = featureFlagFacade;
      IFrotocolLevelPackagingSetting<IReadOnlyCollection<string>> bannedCustomUpstreamHostsSetting = PackagingServerConstants.BannedCustomUpstreamHostsSetting.Bootstrap(this.requestContext);
      ShouldIncludeExternalVersionsHelper shouldIncludeExternalVersionsHelper = new ShouldIncludeExternalVersionsHelper(tracerFacade, featureFlagFacade, (IRegistryService) registryFacade, (IPackageUpstreamBehaviorFacade) upstreamBehaviorService, organizationPolicies, this.requestContext.GetExecutionEnvironmentFacade());
      ITerrapinMetadataValidator<NpmPackageName, SemanticVersion> terrapinValidator = metadataValidator;
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithEntriesSetting = Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(this.requestContext);
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithoutEntriesSetting = Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(this.requestContext);
      return (IUpstreamRefreshStrategy<NpmPackageName, NpmPackageIdentity, INpmMetadataEntry>) new ByVersionUpstreamRefreshStrategy<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry, INpmMetadataEntryWriteable>((ITimeProvider) defaultTimeProvider, metadataServiceFactory, (IAsyncHandler<MetadataDocument<INpmMetadataEntry>, List<INpmMetadataEntry>>) upstreamEntriesRetainingStrategyHandler, versionListService, writeAggregationAccessorsFactory, (IAggregationCommitApplier) skippingCommitApplier, tracerService, (IUpstreamPackageNameMetadataRefreshStrategy<NpmPackageName, INpmMetadataEntry>) packageNameRefreshStrategy, (IUpstreamStatusClassifier) upstreamStatusClassifier, featureFlagService, false, bannedCustomUpstreamHostsSetting, (IShouldIncludeExternalVersionsHelper) shouldIncludeExternalVersionsHelper, terrapinValidator, metadataValidityPeriodWithEntriesSetting, metadataValidityPeriodWithoutEntriesSetting);
    }
  }
}
