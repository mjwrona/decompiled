// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageNameCachingUpstreamService`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class PackageNameCachingUpstreamService<TName, TVer, TId, TEntry> : 
    IUpstreamMetadataService<
    #nullable disable
    TName, TVer, TId, TEntry>
    where TName : IPackageName
    where TVer : IPackageVersion
    where TId : IPackageIdentity<TName, TVer>
  {
    private readonly IPackageNameCacheService<TName> packageNameCacheService;
    private readonly IUpstreamMetadataService<TName, TVer, TId, TEntry> fallbackUpstreamService;

    public PackageNameCachingUpstreamService(
      IPackageNameCacheService<TName> packageNameCacheService,
      IUpstreamMetadataService<TName, TVer, TId, TEntry> fallbackUpstreamService)
    {
      this.packageNameCacheService = packageNameCacheService;
      this.fallbackUpstreamService = fallbackUpstreamService;
    }

    public async Task<IEnumerable<TEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      TName packageName,
      IEnumerable<TEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      return await this.fallbackUpstreamService.UpdateEntriesWithTransientStateAsync(downstreamFeedRequest, packageName, entries, fakeCommitHeader);
    }

    public async Task<object> GetPackageNameMetadata(IFeedRequest downstreamFeedRequest, TName name) => await this.fallbackUpstreamService.GetPackageNameMetadata(downstreamFeedRequest, name);

    public async Task<IReadOnlyList<VersionWithSourceChain<TVer>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      TName packageName)
    {
      if (!await this.packageNameCacheService.Has(packageName))
        throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) packageName.NormalizedName, (object) this.packageNameCacheService.GetSource().GetFullyQualifiedFeedId()));
      return await this.fallbackUpstreamService.GetPackageVersionsAsync(downstreamFeedRequest, packageName);
    }

    public Task<IEnumerable<TEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      TName name,
      IEnumerable<TVer> versions)
    {
      return this.fallbackUpstreamService.GetPackageVersionStatesAsync(downstreamFeedRequest, name, versions);
    }
  }
}
