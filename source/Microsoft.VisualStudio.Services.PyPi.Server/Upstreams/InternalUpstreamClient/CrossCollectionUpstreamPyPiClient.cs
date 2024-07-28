// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient.CrossCollectionUpstreamPyPiClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient
{
  public class CrossCollectionUpstreamPyPiClient : IUpstreamPyPiClient
  {
    private readonly string feedId;
    private readonly Guid aadTenantId;
    private readonly string projectId;
    private readonly InternalPyPiHttpClient pypiHttpClient;

    public CrossCollectionUpstreamPyPiClient(
      Guid? projectId,
      string feedId,
      Guid aadTenantId,
      InternalPyPiHttpClient pypiHttpClient)
    {
      this.feedId = feedId;
      this.aadTenantId = aadTenantId;
      this.projectId = projectId?.ToString();
      this.pypiHttpClient = pypiHttpClient;
    }

    public Task<Stream> GetFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      return this.pypiHttpClient.GetFileInternalAsync(this.projectId, this.feedId, packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion, filePath, this.aadTenantId, (object) null, new CancellationToken());
    }

    public async Task<Stream> GetGpgSignatureForFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      PyPiInternalUpstreamMetadata upstreamMetadata = await this.GetInternalUpstreamMetadata(packageIdentity, filePath);
      return !string.IsNullOrWhiteSpace(upstreamMetadata.Base64ZippedGpgSignature) ? (Stream) new MemoryStream(DeflateCompressibleBytes.FromDeflatedBase64String(upstreamMetadata.Base64ZippedGpgSignature).AsUncompressedBytes(), false) : (Stream) null;
    }

    public async Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions)
    {
      return (IEnumerable<LimitedPyPiMetadata>) new LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter().Convert(await this.pypiHttpClient.GetLimitedMetadataAsync(versions.Select<PyPiPackageVersion, string>((Func<PyPiPackageVersion, string>) (x => x.NormalizedVersion)), this.projectId, this.feedId, packageName.NormalizedName, this.aadTenantId));
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      IFeedRequest _,
      PyPiPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>) (await this.pypiHttpClient.GetPackageVersionsExposedToDownstreamsAsync(this.projectId, this.feedId, packageName.NormalizedName, this.aadTenantId)).VersionInfo.Select<RawVersionWithSourceChain, VersionWithSourceChain<PyPiPackageVersion>>((Func<RawVersionWithSourceChain, VersionWithSourceChain<PyPiPackageVersion>>) (x => VersionWithSourceChain.FromInternalSource<PyPiPackageVersion>(PyPiPackageVersionParser.Parse(x.NormalizedVersion), x.SourceChain))).ToList<VersionWithSourceChain<PyPiPackageVersion>>();
    }

    public async Task<PyPiUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string fileName)
    {
      PyPiInternalUpstreamMetadata upstreamMetadata = await this.GetInternalUpstreamMetadata(packageIdentity, fileName);
      return new PyPiUpstreamMetadata(upstreamMetadata.RawFileMetadata, upstreamMetadata.SourceChain.ToImmutableArray<UpstreamSourceInfo>());
    }

    private Task<PyPiInternalUpstreamMetadata> GetInternalUpstreamMetadata(
      PyPiPackageIdentity packageIdentity,
      string fileName)
    {
      return this.pypiHttpClient.GetUpstreamMetadataAsync(this.projectId, this.feedId, packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion, fileName, this.aadTenantId);
    }
  }
}
