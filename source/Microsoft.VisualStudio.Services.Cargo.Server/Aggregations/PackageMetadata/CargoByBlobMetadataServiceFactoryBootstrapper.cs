// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoByBlobMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  [ExcludeFromCodeCoverage]
  public class CargoByBlobMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoByBlobMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      CargoMetadataDocumentSerializerFactory serializerFactory = new CargoMetadataDocumentSerializerFactory((ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>((Func<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>) (containerAddress => (IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) new UpstreamFilteringMetadataService<CargoPackageIdentity, ICargoMetadataEntry>((IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) new MetadataByBlobService<CargoPackageIdentity, ICargoMetadataEntry>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<ICargoMetadataEntry>, ISerializer<MetadataDocument<ICargoMetadataEntry>>>) serializerFactory, (IMetadataEntryOpApplierFactory<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new CargoMetadataEntryOpApplierFactory(), (IMetadataDocumentOpApplierFactory<MetadataDocument<ICargoMetadataEntry>>) new CargoMetadataDocumentOpApplierFactory(tracerService), (IConverter<IPackageNameRequest, Locator>) new ByFuncConverter<IPackageNameRequest, Locator>((Func<IPackageNameRequest, Locator>) (req => new Locator(new string[2]
      {
        req.Feed.Id.ToString(),
        req.PackageName.NormalizedName + ".txt"
      }))), (IComparer<IPackageVersion>) new ReverseVersionComparer<CargoPackageVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<ICargoMetadataEntry>) new LocalFilesCannotDecreaseMetadataChangeValidator<ICargoMetadataEntry>()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), this.requestContext.GetExecutionEnvironmentFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
    }
  }
}
