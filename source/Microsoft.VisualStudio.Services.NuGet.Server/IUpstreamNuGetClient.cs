// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.IUpstreamNuGetClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public interface IUpstreamNuGetClient : IUpstreamPackageNamesClient
  {
    Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity);

    Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions);

    Task<NuGetUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity);

    Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName);

    Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> GetPackageNames();

    Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint);

    Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions);
  }
}
