// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiByBlobMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiByBlobMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiByBlobMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      PyPiMetadataDocumentSerializerFactory serializerFactory = new PyPiMetadataDocumentSerializerFactory((ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>((Func<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>) (containerAddress => (IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) new UpstreamFileFilteringMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>((IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) new MetadataByBlobService<PyPiPackageIdentity, IPyPiMetadataEntry>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<IPyPiMetadataEntry>, ISerializer<MetadataDocument<IPyPiMetadataEntry>>>) serializerFactory, (IMetadataEntryOpApplierFactory<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new PyPiMetadataOpApplierFactory(), (IMetadataDocumentOpApplierFactory<MetadataDocument<IPyPiMetadataEntry>>) new PyPiMetadataDocumentOpApplierFactory(), (IConverter<IPackageNameRequest, Locator>) new ByFuncConverter<IPackageNameRequest, Locator>((Func<IPackageNameRequest, Locator>) (req => new Locator(new string[2]
      {
        req.Feed.Id.ToString(),
        req.PackageName.NormalizedName + ".txt"
      }))), (IComparer<IPackageVersion>) new ReverseVersionComparer<PyPiPackageVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<IPyPiMetadataEntry>) new LocalFilesCannotDecreaseMetadataChangeValidator<IPyPiMetadataEntry>()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
    }
  }
}
