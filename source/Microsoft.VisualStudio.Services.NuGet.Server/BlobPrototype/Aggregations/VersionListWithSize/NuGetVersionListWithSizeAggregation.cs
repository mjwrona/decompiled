// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.VersionListWithSize.NuGetVersionListWithSizeAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class NuGetVersionListWithSizeAggregation : 
    IAggregation<NuGetVersionListWithSizeAggregation, INuGetVersionListWithSizeAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NuGetVersionListWithSizeAggregation V1 = new NuGetVersionListWithSizeAggregation(nameof (V1));

    private NuGetVersionListWithSizeAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NuGetAggregationDefinitions.NuGetVersionListWithSizeAggregationDefinition;

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
      IEqualityComparer<string> ordinalIgnoreCase = (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase;
      return (IAggregationAccessor) new NuGetVersionListWithSizeAggregationAccessor((IAggregation) this, (IVersionListWithSizeFileProvider) new VersionListWithSizeFileProvider((IAggregation) this, blobServiceFactory, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, environmentFacade, (IPackagingTraces) packagingTracesFacade, tracerFacade, (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsWithSizeImplMetricsRecorder((IPackagingTraces) packagingTracesFacade), featureFlagFacade, (IConverter<string, IPackageName>) NuGetIdentityResolver.Instance.NameResolver, (IConverter<string, IPackageVersion>) NuGetIdentityResolver.Instance.VersionResolver, ordinalIgnoreCase), tracerFacade, (IRetryCountProvider) new VersionListWithSizeRetryCountProvider(environmentFacade, (IRegistryService) requestContext.GetRegistryFacade(), (IProtocol) Protocol.NuGet), featureFlagFacade, requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext));
    }
  }
}
