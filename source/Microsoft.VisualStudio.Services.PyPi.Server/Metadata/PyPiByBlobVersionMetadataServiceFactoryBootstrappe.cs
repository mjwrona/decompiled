// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiByBlobVersionMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Metadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiByBlobVersionMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiByBlobVersionMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      PyPiVersionMetadataDocumentSerializerFactory serializerFactory = new PyPiVersionMetadataDocumentSerializerFactory((ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>((Func<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>) (containerAddress => (IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>) new VersionMetadataByBlobService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<IPyPiMetadataEntryWithRawMetadata>, ISerializer<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>>) serializerFactory, (IMetadataEntryOpApplierFactory<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new PyPiVersionMetadataOpApplierFactory(), (IMetadataDocumentOpApplierFactory<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new PyPiVersionMetadataDocumentApplierFactory(), (IConverter<IPackageRequest<PyPiPackageIdentity>, Locator>) new ByFuncConverter<IPackageRequest<IPackageIdentity>, Locator>((Func<IPackageRequest<IPackageIdentity>, Locator>) (req => new Locator(new string[3]
      {
        req.Feed.Id.ToString(),
        req.PackageId.Name.NormalizedName ?? "",
        req.PackageId.Version.NormalizedVersion + ".txt"
      }))), (IComparer<IPackageVersion>) new ReverseVersionComparer<PyPiPackageVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<IPyPiMetadataEntryWithRawMetadata>) new LocalFilesCannotDecreaseMetadataChangeValidator<IPyPiMetadataEntryWithRawMetadata>())));
    }
  }
}
