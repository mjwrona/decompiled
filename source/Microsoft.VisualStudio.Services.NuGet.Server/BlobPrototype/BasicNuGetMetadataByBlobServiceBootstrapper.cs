// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.BasicNuGetMetadataByBlobServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGet;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class BasicNuGetMetadataByBlobServiceBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>>
  {
    private readonly IVssRequestContext requestContext;

    public BasicNuGetMetadataByBlobServiceBootstrapper(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      requestContext.CheckProjectCollectionRequestContext();
    }

    public IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      NuGetJsonSerializerFactory serializerFactory = new NuGetJsonSerializerFactory(this.requestContext.GetFeatureFlagFacade(), (ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>((Func<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>) (containerAddress => (IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new UpstreamFilteringMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>((IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new MetadataByBlobService<VssNuGetPackageIdentity, INuGetMetadataEntry>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<INuGetMetadataEntry>, ISerializer<MetadataDocument<INuGetMetadataEntry>>>) serializerFactory, (IMetadataEntryOpApplierFactory<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new NuGetMetadataOpApplierFactory(), (IMetadataDocumentOpApplierFactory<MetadataDocument<INuGetMetadataEntry>>) new NuGetMetadataDocumentOpApplierFactory(tracerService), (IConverter<IPackageNameRequest, Locator>) new ByFuncConverter<IPackageNameRequest, Locator>((Func<IPackageNameRequest, Locator>) (req => new Locator(new string[2]
      {
        req.Feed.Id.ToString(),
        req.PackageName.NormalizedName + ".txt"
      }))), (IComparer<IPackageVersion>) new ReverseVersionComparer<VssNuGetPackageVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<INuGetMetadataEntry>) new LocalEntriesCannotDecreaseMetadataChangeValidator<INuGetMetadataEntry>()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
    }
  }
}
