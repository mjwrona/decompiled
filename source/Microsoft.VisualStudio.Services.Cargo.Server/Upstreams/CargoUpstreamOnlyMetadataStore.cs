// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoUpstreamOnlyMetadataStore
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoUpstreamOnlyMetadataStore : 
    IUpstreamMetadataService<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, ICargoMetadataEntry>
  {
    private readonly UpstreamSource upstreamSource;
    private readonly IUpstreamCargoClient upstreamCargoClient;
    private readonly ITimeProvider timeProvider;
    private readonly IFactory<CargoPackageName, Task<ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata>>> limitedMetadata;

    public CargoUpstreamOnlyMetadataStore(
      UpstreamSource source,
      IUpstreamCargoClient upstreamCargoClient,
      ITimeProvider timeProvider)
    {
      this.upstreamSource = source;
      this.upstreamCargoClient = upstreamCargoClient;
      this.timeProvider = timeProvider;
      this.limitedMetadata = ByFuncInputFactory.For<CargoPackageName, Task<ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata>>>((Func<CargoPackageName, Task<ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata>>>) (async name => await this.GetLimitedMetadataDictionaryFromUpstream(name))).SingleElementCache<CargoPackageName, Task<ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata>>>();
    }

    public async Task<IEnumerable<ICargoMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      CargoPackageName packageName,
      IEnumerable<ICargoMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata> immutableDictionary = await this.limitedMetadata.Get(packageName);
      List<ICargoMetadataEntry> cargoMetadataEntryList = new List<ICargoMetadataEntry>();
      foreach (ICargoMetadataEntry entry in entries)
      {
        if (!entry.PackageIdentity.Name.Equals(packageName))
          throw new InvalidOperationException("UpdateEntriesWithTransientStateAsync can only handle entries from a single package name");
        if (entry.IsLocal)
          throw new InvalidOperationException("Do not send local entries to UpdateEntriesWithTransientStateAsync");
        LimitedCargoMetadata limitedCargoMetadata;
        if (immutableDictionary.TryGetValue(entry.PackageIdentity.Version, out limitedCargoMetadata))
        {
          ICargoMetadataEntryWriteable writeable = entry.CreateWriteable(fakeCommitHeader);
          bool flag = false;
          bool yanked1 = limitedCargoMetadata.IndexRow.Value.Yanked;
          bool yanked2 = entry.Yanked;
          if (yanked1 != yanked2)
          {
            writeable.Yanked = yanked1;
            flag = true;
          }
          if (flag)
          {
            writeable.Metadata = UpdateEntryMetadata(entry.Metadata, limitedCargoMetadata.IndexRow);
            cargoMetadataEntryList.Add((ICargoMetadataEntry) writeable);
          }
        }
      }
      return (IEnumerable<ICargoMetadataEntry>) cargoMetadataEntryList;

      static LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> UpdateEntryMetadata(
        LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata>? entryMetadata,
        LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> newIndexRow)
      {
        CargoRawPackageMetadata raw;
        if (entryMetadata != null)
          raw = entryMetadata.Serialized with
          {
            UpstreamIndexRow = newIndexRow
          };
        else
          raw = new CargoRawPackageMetadata((LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null, newIndexRow, (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null, (string) null, (string) null);
        return CargoPackageMetadata.LazyDeserialize(raw);
      }
    }

    public Task<object?> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      CargoPackageName name)
    {
      return Task.FromResult<object>((object) null);
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<CargoPackageVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      CargoPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<CargoPackageVersion>>) (await this.limitedMetadata.Get(packageName)).Select<KeyValuePair<CargoPackageVersion, LimitedCargoMetadata>, VersionWithSourceChain<CargoPackageVersion>>((Func<KeyValuePair<CargoPackageVersion, LimitedCargoMetadata>, VersionWithSourceChain<CargoPackageVersion>>) (x => x.Value.PackageVersion)).Where<VersionWithSourceChain<CargoPackageVersion>>((Func<VersionWithSourceChain<CargoPackageVersion>, bool>) (x => x != null)).ToList<VersionWithSourceChain<CargoPackageVersion>>();
    }

    public async Task<IEnumerable<ICargoMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      CargoPackageName packageName,
      IEnumerable<CargoPackageVersion> versions)
    {
      CargoUpstreamOnlyMetadataStore onlyMetadataStore = this;
      ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata> immutableDictionary = await onlyMetadataStore.limitedMetadata.Get(packageName);
      List<LimitedCargoMetadata> source = new List<LimitedCargoMetadata>();
      foreach (CargoPackageVersion version in versions)
      {
        LimitedCargoMetadata limitedCargoMetadata;
        if (immutableDictionary.TryGetValue(version, out limitedCargoMetadata))
          source.Add(limitedCargoMetadata);
      }
      // ISSUE: reference to a compiler-generated method
      return source.Select<LimitedCargoMetadata, ICargoMetadataEntry>(new Func<LimitedCargoMetadata, ICargoMetadataEntry>(onlyMetadataStore.\u003CGetPackageVersionStatesAsync\u003Eb__8_0));
    }

    private ICargoMetadataEntry CreateCachedUpstreamMetadataEntry(
      LimitedCargoMetadata version,
      DateTime now)
    {
      PackagingCommitId empty1 = PackagingCommitId.Empty;
      DateTime createdDate = now;
      DateTime modifiedDate = now;
      Guid empty2 = Guid.Empty;
      Guid empty3 = Guid.Empty;
      UpstreamStorageId packageStorageId = new UpstreamStorageId(UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(this.upstreamSource));
      LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> metadata = CargoPackageMetadata.LazyDeserialize(new CargoRawPackageMetadata((LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null, version.IndexRow, (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null, (string) null, (string) null));
      // ISSUE: variable of a boxed type
      __Boxed<ImmutableArray<UpstreamSourceInfo>> empty4 = (ValueType) ImmutableArray<UpstreamSourceInfo>.Empty;
      IImmutableList<HashAndType> hashes = version.Hashes;
      bool yanked = version.IndexRow.Value.Yanked;
      // ISSUE: variable of a boxed type
      __Boxed<ImmutableArray<InnerFileReference>> empty5 = (ValueType) ImmutableArray<InnerFileReference>.Empty;
      int num = yanked ? 1 : 0;
      return (ICargoMetadataEntry) new CargoMetadataEntry(empty1, createdDate, modifiedDate, empty2, empty3, (IStorageId) packageStorageId, 0L, metadata, (IEnumerable<UpstreamSourceInfo>) empty4, (IEnumerable<HashAndType>) hashes, (IEnumerable<InnerFileReference>) empty5, num != 0);
    }

    private async Task<ImmutableDictionary<CargoPackageVersion, LimitedCargoMetadata>> GetLimitedMetadataDictionaryFromUpstream(
      CargoPackageName packageName)
    {
      return (await this.upstreamCargoClient.GetLimitedMetadataList(packageName)).ToImmutableDictionary<LimitedCargoMetadata, CargoPackageVersion>((Func<LimitedCargoMetadata, CargoPackageVersion>) (x => x.PackageVersion.Version));
    }
  }
}
