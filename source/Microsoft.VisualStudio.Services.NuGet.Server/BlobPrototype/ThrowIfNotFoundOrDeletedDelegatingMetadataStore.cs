// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ThrowIfNotFoundOrDeletedDelegatingMetadataStore
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ThrowIfNotFoundOrDeletedDelegatingMetadataStore : 
    IReadMetadataService<
    #nullable disable
    VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IReadSingleVersionMetadataService<
    #nullable enable
    VssNuGetPackageIdentity, INuGetMetadataEntry>
  {
    private readonly 
    #nullable disable
    IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> fallbackStore;

    public ThrowIfNotFoundOrDeletedDelegatingMetadataStore(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> fallbackStore)
    {
      this.fallbackStore = fallbackStore;
    }

    public async Task<List<INuGetMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<INuGetMetadataEntry> packageNameQueryRequest)
    {
      return await this.fallbackStore.GetPackageVersionStatesAsync(packageNameQueryRequest);
    }

    public async Task<INuGetMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<VssNuGetPackageIdentity> packageRequest)
    {
      INuGetMetadataEntry versionStateAsync = await this.fallbackStore.GetPackageVersionStateAsync(packageRequest);
      return versionStateAsync != null && !versionStateAsync.IsDeleted() ? versionStateAsync : throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageIdentity) packageRequest.PackageId, packageRequest.Feed);
    }
  }
}
