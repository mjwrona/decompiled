// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPublicRepositoryClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Google.Protobuf;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public class PyPiPublicRepositoryClient : IUpstreamPyPiClient
  {
    private readonly CollectionId downstreamCollectionId;
    private readonly IPyPiPublicRepository repository;
    private readonly IPublicRepositoryInterestTracker<PyPiPackageName> interestTracker;
    private readonly WellKnownUpstreamSource wellKnownSource;

    public PyPiPublicRepositoryClient(
      CollectionId downstreamCollectionId,
      IPyPiPublicRepository repository,
      IPublicRepositoryInterestTracker<PyPiPackageName> interestTracker,
      WellKnownUpstreamSource wellKnownSource)
    {
      this.downstreamCollectionId = downstreamCollectionId;
      this.repository = repository;
      this.interestTracker = interestTracker;
      this.wellKnownSource = wellKnownSource;
    }

    public async Task<Stream> GetFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      await this.interestTracker.RegisterInterestAsync(this.downstreamCollectionId, downstreamFeedRequest, packageIdentity.Name, this.wellKnownSource);
      return await this.repository.GetFileAsync(packageIdentity, filePath);
    }

    public async Task<Stream?> GetGpgSignatureForFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      return await this.repository.GetGpgSignatureForFileAsync(packageIdentity, filePath);
    }

    public async Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions)
    {
      HashSet<IPackageVersion> requestedSet = ((IEnumerable<IPackageVersion>) versions).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      return (await this.repository.GetPackageMetadataAsync(packageName)).Versions.Where<PyPiPubCacheVersionLevelInfo>((Func<PyPiPubCacheVersionLevelInfo, bool>) (x => requestedSet.Contains((IPackageVersion) x.Identity.Version))).Select<PyPiPubCacheVersionLevelInfo, LimitedPyPiMetadata>(new Func<PyPiPubCacheVersionLevelInfo, LimitedPyPiMetadata>(ConvertVersion));

      static LimitedPyPiMetadata ConvertVersion(PyPiPubCacheVersionLevelInfo versionInfo) => new LimitedPyPiMetadata(versionInfo.Identity, DetectRequiresPython(versionInfo), ((IEnumerable<IUnstoredPyPiPackageFile>) versionInfo.Files.Select<PyPiPubCachePackageVersionFileLevelInfo, PyPiPackageFile>(new Func<PyPiPubCachePackageVersionFileLevelInfo, PyPiPackageFile>(ConvertFile))).ToImmutableArray<IUnstoredPyPiPackageFile>());

      static PyPiPackageFile ConvertFile(PyPiPubCachePackageVersionFileLevelInfo fileInfo) => new PyPiPackageFile(fileInfo.Filename, (IStorageId) null, (IReadOnlyCollection<HashAndType>) ExtractHashes((IReadOnlyDictionary<string, ByteString>) fileInfo.Hashes), checked ((long) fileInfo.Size), fileInfo.UploadTime.ToDateTime(), ConvertDistType(fileInfo.DistType));

      static string? DetectRequiresPython(PyPiPubCacheVersionLevelInfo versionInfo) => versionInfo.Files.Select<PyPiPubCachePackageVersionFileLevelInfo, string>((Func<PyPiPubCachePackageVersionFileLevelInfo, string>) (x => x.RequiresPython)).FirstOrDefault<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x)));

      static ImmutableArray<HashAndType> ExtractHashes(
        IReadOnlyDictionary<string, ByteString> hashes)
      {
        ByteString byteString;
        if (!hashes.TryGetValue("sha256", out byteString))
          return ImmutableArray<HashAndType>.Empty;
        return ImmutableArray.Create<HashAndType>(new HashAndType()
        {
          HashType = HashType.SHA256,
          Value = byteString.ToByteArray().ToHexString()
        });
      }

      static PyPiDistType ConvertDistType(string distTypeString)
      {
        PyPiDistType result;
        return !Enum.TryParse<PyPiDistType>(distTypeString, true, out result) ? PyPiDistType.Unknown : result;
      }
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName packageName)
    {
      await this.interestTracker.RegisterInterestAsync(this.downstreamCollectionId, downstreamFeedRequest, packageName, this.wellKnownSource);
      return (IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>) (await this.repository.GetPackageMetadataAsync(packageName)).Versions.Select<PyPiPubCacheVersionLevelInfo, VersionWithSourceChain<PyPiPackageVersion>>((Func<PyPiPubCacheVersionLevelInfo, VersionWithSourceChain<PyPiPackageVersion>>) (x => VersionWithSourceChain.FromExternalSource<PyPiPackageVersion>(x.Identity.Version))).ToImmutableArray<VersionWithSourceChain<PyPiPackageVersion>>();
    }

    public async Task<PyPiUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string requestFilePath)
    {
      return await this.repository.GetUpstreamMetadataAsync(packageIdentity, requestFilePath);
    }
  }
}
