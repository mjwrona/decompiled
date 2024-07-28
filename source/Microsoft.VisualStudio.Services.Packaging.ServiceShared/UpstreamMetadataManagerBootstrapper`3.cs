// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataManagerBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataManagerBootstrapper<TPackageName, TPackageIdentity, TMetadataEntry> : 
    IBootstrapper<IUpstreamMetadataManager>
    where TPackageName : class, IPackageName
    where TPackageIdentity : class, IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> localMetadataService;
    private readonly IUpstreamRefreshStrategy<TPackageName, TPackageIdentity, TMetadataEntry> upstreamRefreshStrategy;
    private readonly IConverter<string, TPackageName> stringToPackageNameConverter;
    private readonly ITelemetryService telemetryService;
    private readonly IConverter<IPackageIdentity, TPackageIdentity> packageIdentityConverter;
    private readonly IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData> ciDataProducer;
    private readonly IUpstreamStatusHandler upstreamStatusResultHandler;

    public UpstreamMetadataManagerBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> localMetadataService,
      IUpstreamRefreshStrategy<TPackageName, TPackageIdentity, TMetadataEntry> upstreamRefreshStrategy,
      IConverter<string, TPackageName> stringToPackageNameConverter,
      IConverter<IPackageIdentity, TPackageIdentity> packageIdentityConverter,
      ITelemetryService telemetryService,
      IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData> ciDataProducer,
      IUpstreamStatusHandler upstreamStatusResultHandler)
    {
      this.requestContext = requestContext;
      this.localMetadataService = localMetadataService;
      this.upstreamRefreshStrategy = upstreamRefreshStrategy;
      this.stringToPackageNameConverter = stringToPackageNameConverter;
      this.telemetryService = telemetryService;
      this.ciDataProducer = ciDataProducer;
      this.upstreamStatusResultHandler = upstreamStatusResultHandler;
      this.packageIdentityConverter = packageIdentityConverter;
    }

    public IUpstreamMetadataManager Bootstrap()
    {
      IUpstreamsConfigurationHasher upstreamConfigurationHasher = UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext);
      RegistryServiceFacade registryServiceFacade = new RegistryServiceFacade(this.requestContext);
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      UpstreamPackagesToRefreshInformationConverter upstreamRefreshDataToPackageListConverter = new UpstreamPackagesToRefreshInformationConverter((IConverter<string, IPackageName>) this.stringToPackageNameConverter, new UpstreamMetadataRefreshJobSplittingConfiguration(PackagingServerConstants.MaxJobCount.Bootstrap(this.requestContext), PackagingServerConstants.HigherLimitOfWork.Bootstrap(this.requestContext)), (IRegistryService) registryServiceFacade);
      return (IUpstreamMetadataManager) new UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>(new UpstreamMetadataCacheInfoStoreFactoryBootstrapper(this.requestContext, (IConverter<string, IPackageName>) this.stringToPackageNameConverter).Bootstrap(), this.localMetadataService, this.upstreamRefreshStrategy, this.stringToPackageNameConverter, this.packageIdentityConverter, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(featureFlagFacade), upstreamConfigurationHasher, this.telemetryService, this.ciDataProducer, tracerFacade, (IRegistryService) registryServiceFacade, (IPackagingTraces) new PackagingTracesFacade(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), this.upstreamStatusResultHandler, featureFlagFacade, (IInPlaceSorter<TPackageName>) new ShuffleIfFeatureFlagOnInPlaceSorter<TPackageName>(featureFlagFacade), environmentFacade, (ITimeProvider) new DefaultTimeProvider(), (IUpstreamPackagesToRefreshInformationConverter) upstreamRefreshDataToPackageListConverter, FeedViewVisibilityValidatingHandler.Bootstrap(this.requestContext), (IUpstreamStatusClassifier) new UpstreamStatusClassifier(), UpstreamMetadata.AddToRefreshListOnlyIfAnyVersionsPresent.Bootstrap(this.requestContext));
    }
  }
}
