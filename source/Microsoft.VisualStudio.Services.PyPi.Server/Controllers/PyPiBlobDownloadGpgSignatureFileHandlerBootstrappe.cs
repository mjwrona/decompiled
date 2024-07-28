// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiBlobDownloadGpgSignatureFileHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  public class PyPiBlobDownloadGpgSignatureFileHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage, IPyPiVersionMetadataAggregationAccessor, IPyPiStorageIdCacheAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiBlobDownloadGpgSignatureFileHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageFileRequest, HttpResponseMessage> Bootstrap(
      IPyPiVersionMetadataAggregationAccessor metadataService,
      IPyPiStorageIdCacheAggregationAccessor cacheService)
    {
      return (IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>) new RawPackageFileRequestConverter<PyPiPackageIdentity>(new PyPiRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<IRawPackageFileRequest, PackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage>((IAsyncHandler<PackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage>) new PyPiDownloadGpgSignatureHandler((IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>) new ThrowIfDeletedDelegatingMetadataDocumentStore<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>((IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>) metadataService)));
    }
  }
}
