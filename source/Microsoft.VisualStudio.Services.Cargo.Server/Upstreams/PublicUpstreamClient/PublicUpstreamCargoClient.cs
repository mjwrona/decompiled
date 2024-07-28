// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.PublicUpstreamClient.PublicUpstreamCargoClient
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.PublicUpstreamClient
{
  public class PublicUpstreamCargoClient : IUpstreamCargoClient
  {
    private readonly Uri packageSourceUri;
    private readonly IHttpClient httpClient;
    private readonly ICargoDashFixer dashFixer;

    public PublicUpstreamCargoClient(
      Uri packageSourceUri,
      IHttpClient httpClient,
      ICargoDashFixer dashFixer)
    {
      this.packageSourceUri = packageSourceUri;
      this.httpClient = httpClient;
      this.dashFixer = dashFixer;
    }

    public async Task<IReadOnlyList<LimitedCargoMetadata>> GetLimitedMetadataList(
      CargoPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return await this.dashFixer.TryMultipleDashCombinations<IReadOnlyList<LimitedCargoMetadata>>(packageName, (Func<CargoPackageName, Task<IReadOnlyList<LimitedCargoMetadata>>>) (async crateNameToUse => await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<IReadOnlyList<LimitedCargoMetadata>>(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageName), this.ConstructPackageMetadataJsonEndpointUri(crateNameToUse), HttpCompletionOption.ResponseContentRead, PublicUpstreamCargoClient.\u003C\u003EO.\u003C0\u003E__GetLimitedMetadata ?? (PublicUpstreamCargoClient.\u003C\u003EO.\u003C0\u003E__GetLimitedMetadata = new Func<HttpResponseMessage, Task<IReadOnlyList<LimitedCargoMetadata>>>(PublicUpstreamCargoClient.GetLimitedMetadata)))));
    }

    public async Task<Stream> GetPackageContentStreamAsync(CargoPackageIdentity packageIdentity) => await this.dashFixer.TryMultipleDashCombinations<Stream>(packageIdentity.Name, (Func<CargoPackageName, Task<Stream>>) (async crateNameToUse =>
    {
      Uri crateDownloadUri = await this.GetCrateDownloadUri(crateNameToUse, packageIdentity.Version);
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingAsync(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), crateDownloadUri, HttpCompletionOption.ResponseHeadersRead);
    }));

    public async Task<CargoUpstreamMetadata> GetUpstreamMetadata(
      CargoPackageIdentity packageIdentity)
    {
      Func<HttpResponseMessage, Task<CargoUpstreamMetadata>> func;
      return await this.dashFixer.TryMultipleDashCombinations<CargoUpstreamMetadata>(packageIdentity.Name, (Func<CargoPackageName, Task<CargoUpstreamMetadata>>) (async crateNameToUse => await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<CargoUpstreamMetadata>(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), this.ConstructPackageMetadataJsonEndpointUri(crateNameToUse), HttpCompletionOption.ResponseContentRead, func ?? (func = (Func<HttpResponseMessage, Task<CargoUpstreamMetadata>>) (async response => await this.GetPackageMetadata(response, packageIdentity))))));
    }

    private Uri ConstructPackageMetadataJsonEndpointUri(CargoPackageName cargoPackageName)
    {
      string lowerInvariant = cargoPackageName.DisplayName.ToLowerInvariant();
      return new Uri(this.packageSourceUri, this.GeneratePackageDirectory(lowerInvariant) + "/" + lowerInvariant);
    }

    private string GeneratePackageDirectory(string packageName)
    {
      string packageDirectory;
      switch (packageName.Length)
      {
        case 1:
          packageDirectory = "1";
          break;
        case 2:
          packageDirectory = "2";
          break;
        case 3:
          packageDirectory = string.Format("3/{0}", (object) packageName[0]);
          break;
        default:
          packageDirectory = packageName.Substring(0, 2) + "/" + packageName.Substring(2, 2);
          break;
      }
      return packageDirectory;
    }

    private async Task<CargoUpstreamMetadata> GetPackageMetadata(
      HttpResponseMessage httpResponse,
      CargoPackageIdentity packageIdentity)
    {
      return new CargoUpstreamMetadata(((await PublicUpstreamCargoClient.GetLimitedMetadata(httpResponse)).FirstOrDefault<LimitedCargoMetadata>((Func<LimitedCargoMetadata, bool>) (p => p.PackageVersion.Version == packageIdentity.Version)) ?? throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.DisplayName, (object) packageIdentity.Version.DisplayVersion))).IndexRow, (LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null, (IEnumerable<UpstreamSourceInfo>) ImmutableArray<UpstreamSourceInfo>.Empty, (IStorageId) null);
    }

    private static async Task<IReadOnlyList<LimitedCargoMetadata>> GetLimitedMetadata(
      HttpResponseMessage httpResponse)
    {
      IReadOnlyCollection<byte[]> nonEmptyLines = SplitToNonEmptyLines((ReadOnlySpan<byte>) await httpResponse.Content.ReadAsByteArrayAsync());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>> lazySerDesValues = nonEmptyLines.Select<byte[], DeflateCompressibleBytes>(PublicUpstreamCargoClient.\u003C\u003EO.\u003C1\u003E__FromUncompressedBytes ?? (PublicUpstreamCargoClient.\u003C\u003EO.\u003C1\u003E__FromUncompressedBytes = new Func<byte[], DeflateCompressibleBytes>(DeflateCompressibleBytes.FromUncompressedBytes))).Select<DeflateCompressibleBytes, LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>>(PublicUpstreamCargoClient.\u003C\u003EO.\u003C2\u003E__LazyDeserialize ?? (PublicUpstreamCargoClient.\u003C\u003EO.\u003C2\u003E__LazyDeserialize = new Func<DeflateCompressibleBytes, LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>>(CargoIndexVersionRow.LazyDeserialize)));
      List<LimitedCargoMetadata> limitedMetadata = new List<LimitedCargoMetadata>(nonEmptyLines.Count);
      foreach (LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> IndexRow in lazySerDesValues)
      {
        if (IndexRow.Value.PackageName != null)
        {
          if (IndexRow.Value.Version != null)
          {
            CargoPackageVersion version;
            try
            {
              version = CargoPackageVersionParser.Parse(IndexRow.Value.Version);
            }
            catch (InvalidVersionException ex)
            {
              continue;
            }
            limitedMetadata.Add(new LimitedCargoMetadata(VersionWithSourceChain<CargoPackageVersion>.FromExternalSource(version), IndexRow, (IImmutableList<HashAndType>) ImmutableArray.Create<HashAndType>(new HashAndType()
            {
              HashType = HashType.SHA256,
              Value = IndexRow.Value.Sha256
            })));
          }
        }
      }
      return (IReadOnlyList<LimitedCargoMetadata>) limitedMetadata;

      static IReadOnlyCollection<byte[]> SplitToNonEmptyLines(ReadOnlySpan<byte> span)
      {
        List<byte[]> nonEmptyLines = new List<byte[]>();
        int length;
        for (; !span.IsEmpty; span = span.Slice(length + 1))
        {
          length = span.IndexOfAny<byte>((byte) 10, (byte) 13);
          if (length < 0)
            length = span.Length;
          ReadOnlySpan<byte> readOnlySpan = span.Slice(0, length);
          if (!readOnlySpan.IsEmpty)
            nonEmptyLines.Add(readOnlySpan.ToArray());
          if (length >= span.Length)
            break;
        }
        return (IReadOnlyCollection<byte[]>) nonEmptyLines;
      }
    }

    private async Task<Uri> GetCrateDownloadUri(CargoPackageName name, CargoPackageVersion version)
    {
      Uri indexConfig = new Uri(this.packageSourceUri, "config.json");
      Uri crateDownloadUri;
      try
      {
        Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(await (await this.httpClient.GetAsync(indexConfig, HttpCompletionOption.ResponseContentRead)).Content.ReadAsStringAsync());
        string uriString1 = dictionary["dl"];
        if (uriString1.Contains("{sha256-checksum}"))
          throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_UpstreamFailure((object) uriString1, (object) "{sha256-checksum} marker is not supported"), new Uri(uriString1));
        string packageDirectory = this.GeneratePackageDirectory(name.DisplayName);
        string uriString2 = uriString1.Replace("{crate}", name.DisplayName).Replace("{version}", version.NormalizedVersion).Replace("{prefix}", packageDirectory).Replace("{lowerprefix}", packageDirectory.ToLowerInvariant());
        if (uriString2.Equals(dictionary["dl"]))
          uriString2 = uriString2 + "/" + name.DisplayName + "/" + version.DisplayVersion + "/download";
        crateDownloadUri = new Uri(uriString2);
      }
      catch (Exception ex)
      {
        throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_UpstreamFailure((object) indexConfig.AbsoluteUri, (object) ex.Message), ex, indexConfig);
      }
      indexConfig = (Uri) null;
      return crateDownloadUri;
    }
  }
}
