// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.ThrowIfDeletedDelegatingMetadataDocumentStore`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class ThrowIfDeletedDelegatingMetadataDocumentStore<TPackageId, TEntry> : 
    IReadMetadataDocumentService<
    #nullable disable
    TPackageId, TEntry>,
    IReadMetadataService<
    #nullable enable
    TPackageId, TEntry>,
    IReadSingleVersionMetadataService<TPackageId, TEntry>,
    IReadMetadataDocumentService
    where TPackageId : 
    #nullable disable
    IPackageIdentity
    where TEntry : class, IMetadataEntry<TPackageId>
  {
    private readonly IReadMetadataDocumentService<TPackageId, TEntry> fallbackStore;

    public ThrowIfDeletedDelegatingMetadataDocumentStore(
      IReadMetadataDocumentService<TPackageId, TEntry> fallbackStore)
    {
      this.fallbackStore = fallbackStore;
    }

    public async Task<List<TEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TEntry> packageNameQueryRequest)
    {
      return await this.fallbackStore.GetPackageVersionStatesAsync(packageNameQueryRequest);
    }

    public async Task<TEntry> GetPackageVersionStateAsync(IPackageRequest<TPackageId> packageRequest)
    {
      TEntry versionStateAsync = await this.fallbackStore.GetPackageVersionStateAsync(packageRequest);
      return (object) versionStateAsync == null || !versionStateAsync.IsDeleted() ? versionStateAsync : throw new PackageNotFoundException(Resources.Error_PackageVersionNotFound((object) packageRequest.PackageId.Name.DisplayName, (object) packageRequest.PackageId.Version.DisplayVersion, (object) packageRequest.Feed.FullyQualifiedName));
    }

    public async Task<MetadataDocument<TEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TEntry> packageNameRequest)
    {
      MetadataDocument<TEntry> statesDocumentAsync = await this.fallbackStore.GetPackageVersionStatesDocumentAsync(packageNameRequest);
      if (statesDocumentAsync == null || !this.IsVersionLevelRequest(packageNameRequest))
        return statesDocumentAsync;
      List<TEntry> entries = statesDocumentAsync.Entries;
      if ((entries != null ? (((DateTime?) entries.FirstOrDefault<TEntry>()?.DeletedDate).HasValue ? 1 : 0) : 0) != 0)
        throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) packageNameRequest.RequestData.PackageName.DisplayName, (object) packageNameRequest.RequestData.Feed.FullyQualifiedId));
      return statesDocumentAsync;
    }

    private bool IsVersionLevelRequest(PackageNameQuery<TEntry> packageNameRequest) => packageNameRequest.Options?.VersionLower != null && packageNameRequest.Options.VersionLower.Equals((object) packageNameRequest.Options.VersionUpper);

    public async Task<MetadataDocument<IMetadataEntry>> GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(
      IPackageNameRequest packageNameRequest)
    {
      return await this.fallbackStore.GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(packageNameRequest);
    }
  }
}
