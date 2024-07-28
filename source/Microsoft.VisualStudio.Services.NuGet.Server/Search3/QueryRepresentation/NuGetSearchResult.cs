// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.NuGetSearchResult
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class NuGetSearchResult
  {
    internal VssNuGetPackageIdentity PackageIdentity => this.Metadata.Identity;

    internal IStorageId PackageStorageId { get; }

    public NuGetPackageMetadata Metadata { get; }

    public DateTime Created { get; }

    public int DownloadCount { get; }

    public bool IsLatestVersion { get; }

    public bool IsAbsoluteLatestVersion { get; }

    public DateTime LastUpdated { get; }

    public DateTime Published { get; }

    public long PackageSize { get; }

    public DateTime? LastEdited { get; }

    public ImmutableList<NuGetSearchResultVersionSummary> AllMatchingVersionsOfPackage { get; }

    public NuGetSearchResult(
      IStorageId packageStorageId,
      DateTime created,
      int downloadCount,
      bool isLatestVersion,
      bool isAbsoluteLatestVersion,
      DateTime lastUpdated,
      DateTime published,
      long packageSize,
      DateTime? lastEdited,
      IEnumerable<NuGetSearchResultVersionSummary> allVersionsOfPackage,
      NuGetPackageMetadata metadata)
    {
      this.PackageStorageId = packageStorageId;
      this.Created = created;
      this.DownloadCount = downloadCount;
      this.IsLatestVersion = isLatestVersion;
      this.IsAbsoluteLatestVersion = isAbsoluteLatestVersion;
      this.LastUpdated = lastUpdated;
      this.Published = published;
      this.PackageSize = packageSize;
      this.LastEdited = lastEdited;
      this.Metadata = metadata;
      this.AllMatchingVersionsOfPackage = allVersionsOfPackage != null ? allVersionsOfPackage.ToImmutableList<NuGetSearchResultVersionSummary>() : (ImmutableList<NuGetSearchResultVersionSummary>) null;
    }
  }
}
