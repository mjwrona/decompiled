// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Upstreams.CargoInternalUpstreamsController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Client.Internal;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Upstreams
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Cargo")]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "internalCargoUpstreams", ResourceVersion = 1)]
  public class CargoInternalUpstreamsController : CargoApiController
  {
    [HttpGet]
    [ClientLocationId("3501397D-87AB-4363-BF81-71238D31E4F9")]
    [ClientResponseType(typeof (IReadOnlyList<CargoInternalUpstreamMetadata>), null, null)]
    public async Task<IReadOnlyList<CargoInternalUpstreamMetadata>> GetLimitedMetadataList(
      string feedId,
      string packageName,
      Guid aadTenantId)
    {
      CargoInternalUpstreamsController upstreamsController = this;
      IPackageNameRequest<CargoPackageName> packageNameRequest = upstreamsController.GetPackageNameRequest(feedId, packageName);
      new UpstreamVerificationHelperBootstrapper(upstreamsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsController.TfsRequestContext, packageNameRequest.Feed, aadTenantId);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IReadOnlyList<CargoInternalUpstreamMetadata> list = (IReadOnlyList<CargoInternalUpstreamMetadata>) (await (await upstreamsController.GetMetadataService((IFeedRequest) packageNameRequest)).GetPackageVersionStatesAsync(new PackageNameQuery<ICargoMetadataEntry>((IPackageNameRequest) packageNameRequest))).Where<ICargoMetadataEntry>((Func<ICargoMetadataEntry, bool>) (packageMetadata => packageMetadata != null && !packageMetadata.IsDeleted())).Select<ICargoMetadataEntry, CargoInternalUpstreamMetadata>(CargoInternalUpstreamsController.\u003C\u003EO.\u003C0\u003E__ToApiResult ?? (CargoInternalUpstreamsController.\u003C\u003EO.\u003C0\u003E__ToApiResult = new Func<ICargoMetadataEntry, CargoInternalUpstreamMetadata>(CargoInternalUpstreamsController.ToApiResult))).ToList<CargoInternalUpstreamMetadata>();
      packageNameRequest = (IPackageNameRequest<CargoPackageName>) null;
      return list;
    }

    [HttpGet]
    [ClientLocationId("9337CC4A-C6EB-4642-B5DD-5FAFF29655CD")]
    [ClientResponseType(typeof (CargoInternalUpstreamMetadata), null, null)]
    public async Task<CargoInternalUpstreamMetadata> GetUpstreamMetadata(
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId)
    {
      CargoInternalUpstreamsController upstreamsController = this;
      IPackageRequest<CargoPackageIdentity> packageRequest = upstreamsController.GetPackageRequest(feedId, packageName, packageVersion);
      new UpstreamVerificationHelperBootstrapper(upstreamsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsController.TfsRequestContext, packageRequest.Feed, aadTenantId);
      ICargoMetadataEntry versionStateAsync = await (await upstreamsController.GetMetadataService((IFeedRequest) packageRequest)).GetPackageVersionStateAsync(packageRequest);
      CargoInternalUpstreamMetadata upstreamMetadata = versionStateAsync != null && !versionStateAsync.IsDeleted() ? CargoInternalUpstreamsController.ToApiResult(versionStateAsync) : throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_PackageVersionNotFound((object) packageName, (object) packageVersion));
      packageRequest = (IPackageRequest<CargoPackageIdentity>) null;
      return upstreamMetadata;
    }

    private async Task<ICargoPackageMetadataAggregationAccessor> GetMetadataService(
      IFeedRequest feedRequest)
    {
      return await CargoAggregationResolver.Bootstrap(this.TfsRequestContext).FactoryFor<ICargoPackageMetadataAggregationAccessor>().Get(feedRequest);
    }

    private static CargoInternalUpstreamMetadata ToApiResult(ICargoMetadataEntry package)
    {
      LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> publishManifest = package.Metadata.Serialized.PublishManifest;
      DeflateCompressibleBytes compressibleBytes = publishManifest != null ? publishManifest.Serialized : package.Metadata.Value.SynthesizePublishManifestFromIndexProperties().SerializeToDeflateCompressibleBytes();
      return new CargoInternalUpstreamMetadata()
      {
        SourceChain = package.SourceChain,
        Sha256 = package.Hashes.Single<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == HashType.SHA256)).Value.ToLowerInvariant(),
        Version = package.PackageIdentity.Version.DisplayVersion,
        IngestionManifestBytes = compressibleBytes.AsDeflatedBase64String(),
        PublishManifestIsReal = publishManifest != null,
        IndexRowBytes = CargoIndexVersionRow.FromMetadataEntry(package).SerializeToDeflateCompressibleBytes().AsDeflatedBase64String()
      };
    }
  }
}
