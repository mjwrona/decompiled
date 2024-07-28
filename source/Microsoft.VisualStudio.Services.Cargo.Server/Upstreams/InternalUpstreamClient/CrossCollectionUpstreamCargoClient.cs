// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.InternalUpstreamClient.CrossCollectionUpstreamCargoClient
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Client.Internal;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.InternalUpstreamClient
{
  public class CrossCollectionUpstreamCargoClient : IUpstreamCargoClient
  {
    private readonly string feedId;
    private readonly Guid aadTenantId;
    private readonly string projectId;
    private readonly InternalCargoHttpClient cargoHttpClient;

    public CrossCollectionUpstreamCargoClient(
      Guid? projectId,
      string feedId,
      Guid aadTenantId,
      InternalCargoHttpClient cargoHttpClient)
    {
      this.feedId = feedId;
      this.aadTenantId = aadTenantId;
      this.projectId = projectId?.ToString() ?? "";
      this.cargoHttpClient = cargoHttpClient;
    }

    public async Task<IReadOnlyList<LimitedCargoMetadata>> GetLimitedMetadataList(
      CargoPackageName packageName)
    {
      return (IReadOnlyList<LimitedCargoMetadata>) (await this.cargoHttpClient.GetLimitedMetadataListAsync(this.projectId, this.feedId, packageName.DisplayName, this.aadTenantId)).Select<CargoInternalUpstreamMetadata, LimitedCargoMetadata>((Func<CargoInternalUpstreamMetadata, LimitedCargoMetadata>) (x =>
      {
        CargoPackageIdentity packageIdentity = new CargoPackageIdentity(packageName, CargoPackageVersionParser.Parse(x.Version));
        ImmutableList<HashAndType> Hashes = ImmutableList.Create<HashAndType>(new HashAndType()
        {
          HashType = HashType.SHA256,
          Value = x.Sha256
        });
        return new LimitedCargoMetadata(VersionWithSourceChain.FromInternalSource<CargoPackageVersion>(packageIdentity.Version, x.SourceChain), CrossCollectionUpstreamCargoClient.GetIndexRow(x, packageIdentity), (IImmutableList<HashAndType>) Hashes);
      })).ToList<LimitedCargoMetadata>();
    }

    public async Task<Stream> GetPackageContentStreamAsync(CargoPackageIdentity packageIdentity) => await this.cargoHttpClient.GetPackageContentStreamAsync(this.projectId, this.feedId, packageIdentity.Name.DisplayName, packageIdentity.Version.DisplayVersion, this.aadTenantId, (object) null, new CancellationToken());

    public async Task<CargoUpstreamMetadata> GetUpstreamMetadata(
      CargoPackageIdentity packageIdentity)
    {
      CargoInternalUpstreamMetadata upstreamMetadataAsync = await this.cargoHttpClient.GetUpstreamMetadataAsync(this.projectId, this.feedId, packageIdentity.Name.DisplayName, packageIdentity.Version.DisplayVersion, this.aadTenantId);
      if (upstreamMetadataAsync == null || upstreamMetadataAsync.IngestionManifestBytes == null)
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.DisplayName, (object) packageIdentity.Version.DisplayVersion));
      return new CargoUpstreamMetadata(CrossCollectionUpstreamCargoClient.GetIndexRow(upstreamMetadataAsync, packageIdentity), upstreamMetadataAsync.PublishManifestIsReal ? CargoPublishManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(upstreamMetadataAsync.IngestionManifestBytes)) : (LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null, upstreamMetadataAsync.SourceChain, (IStorageId) null);
    }

    private static LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> GetIndexRow(
      CargoInternalUpstreamMetadata apiResponse,
      CargoPackageIdentity packageIdentity)
    {
      return apiResponse.IndexRowBytes == null ? CargoIndexVersionRow.FromCargoPackageMetadata(CargoPackageMetadata.FromPublishManifest(CargoPublishManifest.Deserialize(DeflateCompressibleBytes.FromDeflatedBase64String(apiResponse.IngestionManifestBytes))), packageIdentity, apiResponse.Sha256, false).LazySerialize() : CargoIndexVersionRow.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(apiResponse.IndexRowBytes));
    }
  }
}
