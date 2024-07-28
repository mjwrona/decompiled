// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.NuGetPackageVersionCountsAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class NuGetPackageVersionCountsAggregation : 
    IAggregation<NuGetPackageVersionCountsAggregation, INuGetPackageVersionCountsAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NuGetPackageVersionCountsAggregation V1 = new NuGetPackageVersionCountsAggregation(nameof (V1));

    private NuGetPackageVersionCountsAggregation(string versionName) => this.VersionName = versionName;

    public AggregationDefinition Definition { get; } = NuGetAggregationDefinitions.NuGetPackageVersionCountsAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext, (IItemStoreBlobEncodingStrategy) new Base64ItemStoreBlobEncodingStrategy()).Bootstrap();
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      PackagingTracesFacade packagingTracesFacade = new PackagingTracesFacade(requestContext);
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      return (IAggregationAccessor) new NuGetPackageVersionCountsAggregationAccessor((IAggregation) this, (IConverter<IFeedRequest, Guid>) new LocalViewFoldingRequestToViewConverter(), (IVersionListsFileProvider) new VersionListsFileProvider((IAggregation) this, blobServiceFactory, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, environmentFacade, (IPackagingTraces) packagingTracesFacade, tracerFacade, (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsImplementationMetricsRecorder((IPackagingTraces) packagingTracesFacade), featureFlagFacade), tracerFacade, (IRetryCountProvider) new NuGetPackageVersionCountsRetryCountProvider(environmentFacade, (IRegistryService) requestContext.GetRegistryFacade()), featureFlagFacade, (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext), (IVersionCountsFromFileProvider) new VersionCountsFromFileProvider((INuGetPackageMetadataSearchVersionFilteringStrategy) new NuGetPackageMetadataSearchVersionFilteringStrategy(), (INuGetLatestVersionsFinder) new NuGetLatestVersionsFinder()));
    }
  }
}
