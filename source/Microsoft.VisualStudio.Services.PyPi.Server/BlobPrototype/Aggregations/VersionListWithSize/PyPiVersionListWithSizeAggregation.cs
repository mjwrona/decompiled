// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.Aggregations.VersionListWithSize.PyPiVersionListWithSizeAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class PyPiVersionListWithSizeAggregation : 
    IAggregation<PyPiVersionListWithSizeAggregation, IPyPiVersionListWithSizeAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly PyPiVersionListWithSizeAggregation V1 = new PyPiVersionListWithSizeAggregation(nameof (V1));

    private PyPiVersionListWithSizeAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = AggregationDefinitions.PyPiVersionListWithSizeAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext, (IItemStoreBlobEncodingStrategy) new Base64ItemStoreBlobEncodingStrategy()).Bootstrap();
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      PackagingTracesFacade packagingTracesFacade = new PackagingTracesFacade(requestContext);
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      IEqualityComparer<string> ordinal = (IEqualityComparer<string>) StringComparer.Ordinal;
      return (IAggregationAccessor) new PyPiVersionListWithSizeAggregationAccessor((IAggregation) this, (IVersionListWithSizeFileProvider) new VersionListWithSizeFileProvider((IAggregation) this, blobServiceFactory, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, environmentFacade, (IPackagingTraces) packagingTracesFacade, tracerFacade, (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsWithSizeImplMetricsRecorder((IPackagingTraces) packagingTracesFacade), featureFlagFacade, (IConverter<string, IPackageName>) PyPiIdentityResolver.Instance.NameResolver, (IConverter<string, IPackageVersion>) PyPiIdentityResolver.Instance.VersionResolver, ordinal), tracerFacade, (IRetryCountProvider) new VersionListWithSizeRetryCountProvider(environmentFacade, (IRegistryService) requestContext.GetRegistryFacade(), (IProtocol) Protocol.PyPi), featureFlagFacade, requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext));
    }
  }
}
